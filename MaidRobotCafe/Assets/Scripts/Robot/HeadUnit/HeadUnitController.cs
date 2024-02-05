/**
 * @file HeadUnitController.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Control head unit.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

/* Comment below if you don't need debug camera image. */
// #define LOG_CAMERA_IMAGE_FOR_DEBUG

using System.IO;
using System.Collections;
using UnityEngine;
using UniHumanoid;


namespace MaidRobotSimulator.MaidRobotCafe
{
    public class HeadUnitController
    {
        private GameObject _face_GameObject;
        private SkinnedMeshRenderer _face_skinned_mesh_renderer;

        private NeckController _neck_controller;
        private EyeController _eye_controller;

        private float _take_picture_interval = CommonParameter.EYE_CAMERA_IMAGE_TAKE_INTERVAL;
        private Camera _right_eye_camera_object;
        private Camera _left_eye_camera_object;
        private RenderTexture _right_eye_render_texture;
        private RenderTexture _left_eye_render_texture;

        private CommonImageProcessor _right_eye_image_processor;
        private CommonImageProcessor _left_eye_image_processor;

        private Quaternion _neck_rotation;
        private Quaternion _head_rotation;
        private Quaternion _right_eye_rotation;
        private Quaternion _left_eye_rotation;

        private int _picture_count = 0;

        private float _elapsed_time_after_last_take = Mathf.Infinity; /*!< elapsed time after last take picture */

        private MessageStructure.ST_SENSOR_MSGS_IMAGE _right_eye_camera_image;
        private MessageStructure.ST_SENSOR_MSGS_IMAGE _left_eye_camera_image;

        private int _right_eye_blend_shape_index = 0;
        private int _left_eye_blend_shape_index = 0;
        private float _right_eye_blend_shape_weight = 0;
        private float _left_eye_blend_shape_weight = 0;

        private CommonStateMachine<SystemStructure.EYE_BLINK_MODE> _eye_blink_state_machine
            = new CommonStateMachine<SystemStructure.EYE_BLINK_MODE>(SystemStructure.EYE_BLINK_MODE.NOT_BLINKING);

        private SystemStructure.ST_EULER_ANGLE _neck_angle =
            new SystemStructure.ST_EULER_ANGLE(0.0f, 0.0f, 0.0f);

        private SystemStructure.ST_EULER_ANGLE _neck_angle_reference =
            new SystemStructure.ST_EULER_ANGLE(0.0f, 0.0f, 0.0f);

        private Quaternion _right_eye_rotation_reference = Quaternion.identity;
        private Quaternion _left_eye_rotation_reference = Quaternion.identity;

#if LOG_CAMERA_IMAGE_FOR_DEBUG
        private string _project_path = "";
#endif

        public HeadUnitController()
        {
            GameObject robot_GameObject = GameObject.Find(CommonParameter.ROBOT_NAME); 
            Transform face_transform = robot_GameObject.transform.Find(CommonParameter.FACE_NAME);
            this._face_GameObject = face_transform.gameObject;
            this._face_skinned_mesh_renderer = this._face_GameObject.GetComponent<SkinnedMeshRenderer>();
            this._initialize_blink_blend_shape();

            GameObject RightEyeCamera_GameObject = GameObject.Find(CommonParameter.RIGHT_EYE_CAMERA_NAME);
            this._right_eye_camera_object = RightEyeCamera_GameObject.GetComponent<Camera>();
            this._right_eye_render_texture = this._right_eye_camera_object.targetTexture;

            GameObject LeftEyeCamera_GameObject = GameObject.Find(CommonParameter.LEFT_EYE_CAMERA_NAME);
            this._left_eye_camera_object = LeftEyeCamera_GameObject.GetComponent<Camera>();
            this._left_eye_render_texture = this._left_eye_camera_object.targetTexture;

            this._right_eye_image_processor = new CommonImageProcessor(
                this._right_eye_render_texture, this._right_eye_camera_object,
                CommonParameter.OUTPUT_RIGHT_EYE_CAMERA_IMAGE_NAME);
            this._left_eye_image_processor = new CommonImageProcessor(
                this._left_eye_render_texture, this._left_eye_camera_object,
                CommonParameter.OUTPUT_LEFT_EYE_CAMERA_IMAGE_NAME);

            this._eye_blink_state_machine.set_elapsed_time_after_mode_changed(0.0f);

            this._neck_controller = new NeckController();
            this._eye_controller = new EyeController();

            this._set_neck_eye_mode(SystemStructure.HEAD_UNIT_MODE.LOOK_FORWARD);

            this._neck_rotation = CommonParameter.NECK_ROTATION_INIT;
            this._head_rotation = CommonParameter.HEAD_ROTATION_INIT;
            this._right_eye_rotation = CommonParameter.RIGHT_EYE_ROTATION_INIT;
            this._left_eye_rotation = CommonParameter.LEFT_EYE_ROTATION_INIT;

#if LOG_CAMERA_IMAGE_FOR_DEBUG
           string assets_path = Application.dataPath;
           this._project_path = System.IO.Path.GetDirectoryName(assets_path);

           string log_folder_path = Path.Combine(this._project_path, CommonParameter.EYE_CAMERA_LOG_FOLDER);
           if (Directory.Exists(log_folder_path))
           {
               Directory.Delete(log_folder_path, true);
           }
           Directory.CreateDirectory(log_folder_path);
#endif
        }

        /*********************************************************
         * Public functions
         *********************************************************/
        public Quaternion get_neck_rotation()
        {
            return this._neck_rotation;
        }

        public Quaternion get_head_rotation()
        {
            return this._head_rotation;
        }

        public Quaternion get_right_eye_rotation()
        {
            return this._right_eye_rotation;
        }

        public Quaternion get_left_eye_rotation()
        {
            return this._left_eye_rotation;
        }

        public MessageStructure.ST_SENSOR_MSGS_IMAGE get_current_right_eye_image()
        {
            return this._right_eye_camera_image;
        }

        public MessageStructure.ST_SENSOR_MSGS_IMAGE get_current_left_eye_image()
        {
            return this._left_eye_camera_image;
        }

        public void set_head_rotation(Quaternion head_rotation)
        {
            this._head_rotation = head_rotation;
        }

        public void set_right_eye_rotation(Quaternion right_eye_rotation)
        {
            this._right_eye_rotation = right_eye_rotation;
        }

        public void set_left_eye_rotation(Quaternion left_eye_rotation)
        {
            this._left_eye_rotation = left_eye_rotation;
        }

        public void set_right_eye_blend_shape_weight(float weight)
        {
            if (weight > CommonParameter.EYE_BLINK_WEIGHT_MAX)
            {
                weight = CommonParameter.EYE_BLINK_WEIGHT_MAX;
            }
            else if (weight < CommonParameter.EYE_BLINK_WEIGHT_MIN)
            {
                weight = CommonParameter.EYE_BLINK_WEIGHT_MIN;
            }

            this._right_eye_blend_shape_weight = weight;
        }

        public void set_left_eye_blend_shape_weight(float weight)
        {
            if (weight > CommonParameter.EYE_BLINK_WEIGHT_MAX)
            {
                weight = CommonParameter.EYE_BLINK_WEIGHT_MAX;
            }
            else if (weight < CommonParameter.EYE_BLINK_WEIGHT_MIN)
            {
                weight = CommonParameter.EYE_BLINK_WEIGHT_MIN;
            }

            this._left_eye_blend_shape_weight = weight;
        }

        public SystemStructure.EYE_BLINK_MODE get_eye_blink_mode()
        {
            return this._eye_blink_state_machine.get_mode();
        }

        public void set_head_status(MessageStructure.ST_HEAD_STATUS head_status)
        {
            if(true == head_status.human_detected)
            {
                this._set_neck_eye_mode(SystemStructure.HEAD_UNIT_MODE.FACE_TRACKING);
            }
            else
            {
                this._set_neck_eye_mode(SystemStructure.HEAD_UNIT_MODE.LOOK_FORWARD);
            }
        }

        public void take_images_from_eyes(float delta_time)
        {
            if (this._elapsed_time_after_last_take >= this._take_picture_interval)
            {
                Texture2D right_eye_screenshot = this._right_eye_image_processor.take_screenshot_from_camera(
                    this._right_eye_render_texture,
                    this._right_eye_camera_object);
                Texture2D left_eye_screenshot = this._left_eye_image_processor.take_screenshot_from_camera(
                    this._left_eye_render_texture,
                    this._left_eye_camera_object);

                /* Get raw image data */
                this._right_eye_camera_image = this._right_eye_image_processor.input_data_to_image_structure(
                    this._right_eye_render_texture, right_eye_screenshot);
                this._left_eye_camera_image = this._left_eye_image_processor.input_data_to_image_structure(
                    this._left_eye_render_texture, left_eye_screenshot);

#if LOG_CAMERA_IMAGE_FOR_DEBUG
                byte[] right_eye_picture_bytes = right_eye_screenshot.EncodeToPNG();
                byte[] left_eye_picture_bytes = left_eye_screenshot.EncodeToPNG();

                string right_eye_save_path = Path.Combine(this._project_path, CommonParameter.EYE_CAMERA_LOG_FOLDER
                                        + "/" + CommonParameter.EYE_CAMERA_LOG_RIGHT_FILE_NAME
                                        + this._picture_count.ToString() + ".png");
                string left_eye_save_path = Path.Combine(this._project_path, CommonParameter.EYE_CAMERA_LOG_FOLDER
                                        + "/" + CommonParameter.EYE_CAMERA_LOG_LEFT_FILE_NAME
                                        + this._picture_count.ToString() + ".png");

                File.WriteAllBytes(right_eye_save_path, right_eye_picture_bytes);
                File.WriteAllBytes(left_eye_save_path, left_eye_picture_bytes);
#endif

                /* These are needed for relasing Unity memory allocation. */
                MonoBehaviour.Destroy(right_eye_screenshot);
                MonoBehaviour.Destroy(left_eye_screenshot);

                this._picture_count += 1;

                this._elapsed_time_after_last_take = 0.0f;
            }
            else
            {
                this._elapsed_time_after_last_take += delta_time;
            }
        }

        public void update_head_and_eyes_rotation(float delta_time,
            Quaternion neck_rotation_reference,
            MessageStructure.ST_MRS_EYE eye_reference)
        {
            /* Neck angle */
            this._neck_angle_reference = this._calculate_neck_angle(neck_rotation_reference);

            this._neck_controller.set_neck_input_angle(this._neck_angle_reference);
            this._neck_controller.calculate(delta_time);
            this._neck_angle = this._neck_controller.get_output_neck_face_track_angle();

            Quaternion neck_rotation = Quaternion.Euler(this._neck_angle.roll, this._neck_angle.pitch, this._neck_angle.yaw);

            this._head_rotation = CommonParameter.HEAD_ROTATION_INIT * neck_rotation;

            /* Eye angle */
            this._eye_controller.set_eye_rotation_input(eye_reference);
            this._eye_controller.calculate();
            this._eye_controller.get_output_eye_rotation(
                ref this._right_eye_rotation_reference, ref this._left_eye_rotation_reference);

            this._right_eye_rotation = CommonParameter.RIGHT_EYE_ROTATION_INIT * this._right_eye_rotation_reference;
            this._left_eye_rotation = CommonParameter.LEFT_EYE_ROTATION_INIT * this._left_eye_rotation_reference;
        }

        public void update_eyes_blink(float delta_time)
        {
            this._update_eyes_blink_without_outside_controller(delta_time);
        }

        public void draw_head_rotation(ref GameObject robot_GameObject)
        {
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.Head,
                this._head_rotation, ref robot_GameObject);
        }

        public void draw_eyes_rotation(ref GameObject robot_GameObject)
        {
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.RightEye,
                this._right_eye_rotation, ref robot_GameObject);
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.LeftEye,
                this._left_eye_rotation, ref robot_GameObject);

            this._draw_eyes_blink();
        }

        public float get_right_eye_image_process_rate()
        {
            return this._right_eye_image_processor.get_image_process_rate();
        }

        public float get_left_eye_image_process_rate()
        {
            return this._left_eye_image_processor.get_image_process_rate();
        }

        public SystemStructure.ST_EULER_ANGLE get_neck_angle()
        {
            return this._neck_angle;
            //return this._neck_angle_reference;
        }

        public SystemStructure.NECK_POSE_MODE get_neck_pose_mode()
        {
            return this._neck_controller.get_neck_pose_mode();
        }

        public Vector3 get_right_eye_reference_angle()
        {
            return this._right_eye_rotation_reference.eulerAngles;
        }

        public Vector3 get_left_eye_reference_angle()
        {
            return this._left_eye_rotation_reference.eulerAngles;
        }

        /*********************************************************
         * Coroutine functions
         *********************************************************/
        public IEnumerator excite_blink_event_randomly()
        {
            while (true)
            {
                if (SystemStructure.EYE_BLINK_MODE.NOT_BLINKING == this._eye_blink_state_machine.get_mode())
                {
                    float seed = Random.Range(
                        CommonParameter.EYE_BLINK_TIMING_RANDOM_SEED_MIN,
                        CommonParameter.EYE_BLINK_TIMING_RANDOM_SEED_MAX);

                    if (seed > CommonParameter.EYE_RANDOM_BLINKING_THRESHOLD)
                    {
                        this._eye_blink_state_machine.update_mode(SystemStructure.EYE_BLINK_MODE.BLINKING);
                    }
                }

                yield return new WaitForSeconds(CommonParameter.EYE_BLINKING_TIME_INTERVAL);
            }
        }

        /*********************************************************
         * Private functions
         *********************************************************/
        private void _initialize_blink_blend_shape()
        {
            int blend_shape_count = this._face_skinned_mesh_renderer.sharedMesh.blendShapeCount;

            for (int i = 0; i < blend_shape_count; i++)
            {
                string blend_shape_name = this._face_skinned_mesh_renderer.sharedMesh.GetBlendShapeName(i);

                if (string.Equals(blend_shape_name, CommonParameter.RIGHT_EYE_BLEND_SHAPE_NAME))
                {
                    this._right_eye_blend_shape_index = i;
                }
                if (string.Equals(blend_shape_name, CommonParameter.LEFT_EYE_BLEND_SHAPE_NAME))
                {
                    this._left_eye_blend_shape_index = i;
                }
            }

            this._right_eye_blend_shape_weight = 
                this._face_skinned_mesh_renderer.GetBlendShapeWeight(this._right_eye_blend_shape_index);
            this._right_eye_blend_shape_weight =
                this._face_skinned_mesh_renderer.GetBlendShapeWeight(this._left_eye_blend_shape_index);
        }

        private void _draw_eyes_blink()
        {
            this._face_skinned_mesh_renderer.SetBlendShapeWeight(
                this._right_eye_blend_shape_index, this._right_eye_blend_shape_weight);
            this._face_skinned_mesh_renderer.SetBlendShapeWeight(
                this._left_eye_blend_shape_index, this._left_eye_blend_shape_weight);
        }

        private void _update_eyes_blink_without_outside_controller(float delta_time)
        {
            if (SystemStructure.EYE_BLINK_MODE.BLINKING == this._eye_blink_state_machine.get_mode())
            {
                this._eye_blink_state_machine.set_elapsed_time_after_mode_changed(
                    this._eye_blink_state_machine.get_elapsed_time_after_mode_changed() + delta_time);

                float blink_weight = CommonParameter.EYE_BLINK_WEIGHT_MAX
                    - Mathf.Abs(CommonParameter.EYE_BLINK_WEIGHT_MAX - CommonParameter.EYE_BLINK_FETCH_LENGTH
                        * (this._eye_blink_state_machine.get_elapsed_time_after_mode_changed()
                            / CommonParameter.EYE_BLINKING_TIME_INTERVAL));

                this.set_right_eye_blend_shape_weight(blink_weight);
                this.set_left_eye_blend_shape_weight(blink_weight);

                if (this._eye_blink_state_machine.get_elapsed_time_after_mode_changed()
                        > CommonParameter.EYE_BLINKING_TIME_INTERVAL)
                {
                    this._eye_blink_state_machine.set_elapsed_time_after_mode_changed(0.0f);
                    this._eye_blink_state_machine.update_mode(SystemStructure.EYE_BLINK_MODE.NOT_BLINKING);
                }
            }
            else
            {
                this.set_right_eye_blend_shape_weight(0.0f);
                this.set_left_eye_blend_shape_weight(0.0f);
            }
        }

        private SystemStructure.ST_EULER_ANGLE _calculate_neck_angle(Quaternion neck_rotation)
        {
            SystemStructure.ST_EULER_ANGLE neck_angle_out = new SystemStructure.ST_EULER_ANGLE(0.0f, 0.0f, 0.0f);

            neck_angle_out.yaw = (neck_rotation.x - CommonParameter.POSE_TO_NECK_ANGLE_CENTER)
                * CommonParameter.POSE_TO_NECK_ANGLE_YAW_FACTOR;
            neck_angle_out.pitch = (neck_rotation.y - CommonParameter.POSE_TO_NECK_ANGLE_CENTER)
                * CommonParameter.POSE_TO_NECK_ANGLE_PITCH_FACTOR;
            neck_angle_out.roll = neck_rotation.z * CommonParameter.POSE_TO_NECK_ANGLE_ROLL_FACTOR;

            return neck_angle_out;
        }

        private void _set_neck_eye_mode(SystemStructure.HEAD_UNIT_MODE mode)
        {
            this._neck_controller.set_head_unit_mode(mode);
            this._eye_controller.set_head_unit_mode(mode);
        }

        /*********************************************************
        * Destructor
        *********************************************************/
        ~HeadUnitController()
        {
            this._right_eye_image_processor.Dispose();
            this._left_eye_image_processor.Dispose();
        }
    }
}
