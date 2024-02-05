/**
 * @file EyeController.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Control eye rotation.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

using UnityEngine;

namespace MaidRobotSimulator.MaidRobotCafe
{
    public class EyeController
    {
        private SystemStructure.HEAD_UNIT_MODE _head_unit_mode = SystemStructure.HEAD_UNIT_MODE.LOOK_FORWARD;

        private Quaternion _right_eye_rotation_input = Quaternion.identity;
        private Quaternion _left_eye_rotation_input = Quaternion.identity;
        private Quaternion _right_eye_rotation_output = Quaternion.identity;
        private Quaternion _left_eye_rotation_output = Quaternion.identity;

        private CommonRateManager<float> _right_eye_rotation_average_manager;
        private CommonRateManager<float> _left_eye_rotation_average_manager;
        public EyeController()
        {
            float[] initial_rotation = new float[CommonParameter.QUATERNION_DIMENSTION_NUM];
            CommonTransform.quaternion_to_array(Quaternion.identity, ref initial_rotation);

            this._right_eye_rotation_average_manager =
                new CommonRateManager<float>(CommonParameter.EYE_FACE_TRACK_AVERAGE_BUFFER_LENGTH,
                initial_rotation);
            this._left_eye_rotation_average_manager =
                new CommonRateManager<float>(CommonParameter.EYE_FACE_TRACK_AVERAGE_BUFFER_LENGTH,
                initial_rotation);
        }

        /*********************************************************
         * Public functions
         *********************************************************/
        public void set_eye_rotation_input(MessageStructure.ST_MRS_EYE eye_reference)
        {
            float pitch_angle_right = CommonParameter.POSE_TO_EYE_ANGLE_PITCH_FACTOR
                * (eye_reference.right_z - CommonParameter.POSE_TO_EYE_ANGLE_CENTER_Z);
            float yaw_angle_right = CommonParameter.POSE_TO_EYE_ANGLE_YAW_FACTOR
                * (eye_reference.right_y - CommonParameter.POSE_TO_EYE_ANGLE_CENTER_Y);
            float pitch_angle_left = CommonParameter.POSE_TO_EYE_ANGLE_PITCH_FACTOR
                * (eye_reference.left_z - CommonParameter.POSE_TO_EYE_ANGLE_CENTER_Z);
            float yaw_angle_left = CommonParameter.POSE_TO_EYE_ANGLE_YAW_FACTOR
                * (eye_reference.left_y - CommonParameter.POSE_TO_EYE_ANGLE_CENTER_Y);

            pitch_angle_right = Mathf.Clamp(pitch_angle_right,
                CommonParameter.EYE_FACE_TRACK_PITCH_ANGLE_DEGREE_MIN, CommonParameter.EYE_FACE_TRACK_PITCH_ANGLE_DEGREE_MAX);
            yaw_angle_right = Mathf.Clamp(yaw_angle_right,
                CommonParameter.EYE_FACE_TRACK_YAW_ANGLE_DEGREE_MIN, CommonParameter.EYE_FACE_TRACK_YAW_ANGLE_DEGREE_MAX);
            pitch_angle_left = Mathf.Clamp(pitch_angle_left,
                CommonParameter.EYE_FACE_TRACK_PITCH_ANGLE_DEGREE_MIN, CommonParameter.EYE_FACE_TRACK_PITCH_ANGLE_DEGREE_MAX);
            yaw_angle_left = Mathf.Clamp(yaw_angle_left,
                CommonParameter.EYE_FACE_TRACK_YAW_ANGLE_DEGREE_MIN, CommonParameter.EYE_FACE_TRACK_YAW_ANGLE_DEGREE_MAX);

            this._right_eye_rotation_input = Quaternion.Euler(0.0f, pitch_angle_right, yaw_angle_right);
            this._left_eye_rotation_input = Quaternion.Euler(0.0f, pitch_angle_left, yaw_angle_left);
        }

        public void get_output_eye_rotation(
            ref Quaternion right_eye_rotation, ref Quaternion left_eye_rotation)
        {
            right_eye_rotation = this._right_eye_rotation_output;
            left_eye_rotation = this._left_eye_rotation_output;
        }

        public void set_head_unit_mode(SystemStructure.HEAD_UNIT_MODE head_unit_mode)
        {
            this._head_unit_mode = head_unit_mode;
        }

        public void calculate()
        {
            if (SystemStructure.HEAD_UNIT_MODE.FACE_TRACKING == this._head_unit_mode)
            {
                float[] rotation_input = new float[CommonParameter.QUATERNION_DIMENSTION_NUM];

                CommonTransform.quaternion_to_array(this._right_eye_rotation_input, ref rotation_input);
                this._right_eye_rotation_average_manager.set_moving_average_value(rotation_input);
                CommonTransform.quaternion_to_array(this._left_eye_rotation_input, ref rotation_input);
                this._left_eye_rotation_average_manager.set_moving_average_value(rotation_input);

                CommonTransform.array_to_quaternion(
                    this._right_eye_rotation_average_manager.get_moving_average_values(), ref this._right_eye_rotation_output);
                this._right_eye_rotation_output.Normalize();
                CommonTransform.array_to_quaternion(
                    this._left_eye_rotation_average_manager.get_moving_average_values(), ref this._left_eye_rotation_output);
                this._left_eye_rotation_output.Normalize();
            }
            else if (SystemStructure.HEAD_UNIT_MODE.LOOK_FORWARD == this._head_unit_mode)
            {
                this._right_eye_rotation_output = Quaternion.identity;
                this._left_eye_rotation_output = Quaternion.identity;
            }
            else
            {
                /* Do Nothing. */
            }
        }

        /*********************************************************
         * Private functions
         *********************************************************/


        /*********************************************************
        * Destructor
        *********************************************************/
        ~EyeController()
        {
        }
    }
}
