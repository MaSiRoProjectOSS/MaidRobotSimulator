/**
 * @file WaistDownUnitController.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Control wasit down unit.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

/* Comment below if you don't need debug texts. */
#define SHOW_STATUS_FOR_DEBUG

using UniHumanoid;
using UnityEngine;

namespace MaidRobotSimulator.MaidRobotCafe
{
    public class WaistDownUnitController
    {
        /*********************************************************
        * Private variables
        *********************************************************/
        private GameObject _Robot_GameObject;

        private Vector3 _position = CommonParameter.POSITION_INIT;    /*!< robot position */
        private Vector3 _euler_angle = CommonParameter.EULER_INIT;    /*!< robot euler angle */
        private Quaternion _rotation = CommonParameter.ROTATION_INIT; /*!< robot quaternion */

        private Vector3 _reference_translational_velocity_unity_axis =
            CommonParameter.TRANSLATIONAL_VELOCITY_INIT; /*!< velocity reference */
        private Vector3 _reference_angular_velocity_unity_axis =
            CommonParameter.ANGULAR_VELOCITY_INIT; /*!< angular velocity reference */

        private Vector3 _hip_position;
        private Quaternion _hip_rotation;
        private Quaternion _spine_rotation;
        private Quaternion _chest_rotation;
        private Quaternion _upper_chest_rotation;
        private Quaternion _right_shoulder_rotation;
        private Quaternion _left_shoulder_rotation;

        public WaistDownUnitController()
        {
            this._Robot_GameObject = GameObject.Find(CommonParameter.ROBOT_NAME);

            this._hip_position = this._Robot_GameObject.transform.position + CommonParameter.HIP_POSITION_INIT;

            this._hip_rotation = CommonParameter.HIP_ROTATION_INIT;
            this._spine_rotation = CommonParameter.SPINE_ROTATION_INIT;
            this._chest_rotation = CommonParameter.CHEST_ROTATION_INIT;
            this._upper_chest_rotation = CommonParameter.UPPER_CHEST_ROTATION_INIT;
            this._right_shoulder_rotation = CommonParameter.RIGHT_SHOULDER_ROTATION_INIT;
            this._left_shoulder_rotation = CommonParameter.LEFT_SHOULDER_ROTATION_INIT;
    }

        public Vector3 get_current_position()
        {
            return this._position;
        }
        public Quaternion get_current_rotation()
        {
            return this._rotation;
        }

        public Vector3 get_current_euler_angle()
        {
            return this._euler_angle;
        }

        public Vector3 get_reference_translational_velocity_unity_axis()
        {
            return this._reference_translational_velocity_unity_axis;
        }

        public float get_current_velocity_magnitude()
        {
            return this._reference_translational_velocity_unity_axis.magnitude;
        }

        public Vector3 get_reference_angular_velocity_unity_axis()
        {
            return this._reference_angular_velocity_unity_axis;
        }

        public void set_reference_translational_velocity_unity_axis(Vector3 velocity)
        {
            this._reference_translational_velocity_unity_axis.x = velocity.x;
            this._reference_translational_velocity_unity_axis.y = velocity.y;
            this._reference_translational_velocity_unity_axis.z = velocity.z;
        }

        public void set_reference_angular_velocity_unity_axis(Vector3 angular_velocity)
        {
            this._reference_angular_velocity_unity_axis.x = angular_velocity.x;
            this._reference_angular_velocity_unity_axis.y = angular_velocity.y;
            this._reference_angular_velocity_unity_axis.z = angular_velocity.z;
        }

        public Vector3 get_hip_position()
        {
            return this._hip_position;
        }

        public Quaternion get_hip_rotation()
        {
            return this._hip_rotation;
        }

        public Quaternion get_spine_rotation()
        {
            return this._spine_rotation;
        }

        public Quaternion get_chest_rotation()
        {
            return this._chest_rotation;
        }

        public Quaternion get_upper_chest_rotation()
        {
            return this._upper_chest_rotation;
        }

        public Quaternion get_right_shoulder_rotation()
        {
            return this._right_shoulder_rotation;
        }

        public Quaternion get_left_shoulder_rotation()
        {
            return this._left_shoulder_rotation;
        }


        public void update_orientation(Vector3 position, Quaternion rotation)
        {
            CommonTransform.update_orientation(position, rotation,
                ref this._position, ref this._rotation, ref this._euler_angle);
        }

        public void move_robot_position_rotation(float delta_time,
            ref GameObject Robot)
        {
            Vector3 d_pos = this._reference_translational_velocity_unity_axis * delta_time;
            Robot.transform.Translate(d_pos, Space.Self);

            Quaternion delta_euler_angle = Quaternion.Euler(
                this._reference_angular_velocity_unity_axis * Mathf.Rad2Deg * delta_time);
            Robot.transform.rotation = Robot.transform.rotation * delta_euler_angle;

            CommonTransform.update_orientation(Robot.transform.position, Robot.transform.rotation,
                ref this._position, ref this._rotation, ref this._euler_angle);
        }

        public void draw_hip_rotation(ref GameObject robot_GameObject, Quaternion robot_rotation_unity_axis)
        {
            Humanoid humanoid_component = robot_GameObject.GetComponent<Humanoid>();
            CommonTransform.draw_hip_rotation(this._hip_rotation, robot_rotation_unity_axis, ref humanoid_component);
        }

        public void update_hip_position()
        {
            this._hip_position = this._Robot_GameObject.transform.position + CommonParameter.HIP_POSITION_INIT;
        }

        public void calculate_wasit_down_absolute_bone_transform(
            Vector3 hip_base_position, ref SystemStructure.ST_WAIST_DOWN_ABSOLUTE_TRANSFORM waist_down_absolute_transform)
        {
            Quaternion hip_absolute_rotation = this.get_hip_rotation();

            Vector3 hip_point_position = hip_base_position
                + hip_absolute_rotation * (CommonParameter.ROBOT_BONES_LENGTH.hips * CommonParameter.WAIST_BONE_DIRECTION_VECTOR);

            Vector3 spine_base_position = hip_point_position + CommonParameter.ROBOT_BONES_OFFSET.hip_to_spine;
            Quaternion spine_absolute_rotation = hip_absolute_rotation * this.get_spine_rotation();

            Vector3 spine_point_position = spine_base_position
                + spine_absolute_rotation * (CommonParameter.ROBOT_BONES_LENGTH.spine * CommonParameter.WAIST_BONE_DIRECTION_VECTOR);

            Vector3 chest_base_position = spine_point_position + CommonParameter.ROBOT_BONES_OFFSET.spine_to_chest;
            Quaternion chest_absolute_rotation = spine_absolute_rotation * this.get_chest_rotation();

            Vector3 chest_point_position = chest_base_position
                + chest_absolute_rotation * (CommonParameter.ROBOT_BONES_LENGTH.chest * CommonParameter.WAIST_BONE_DIRECTION_VECTOR);

            Vector3 upper_chest_base_position = chest_point_position + CommonParameter.ROBOT_BONES_OFFSET.chest_to_upper_chest;
            Quaternion upper_chest_absolute_rotation = chest_absolute_rotation * this.get_upper_chest_rotation();

            Vector3 upper_chest_point_position = upper_chest_base_position
                + upper_chest_absolute_rotation * (CommonParameter.ROBOT_BONES_LENGTH.upper_chest * CommonParameter.WAIST_BONE_DIRECTION_VECTOR);

            Vector3 right_shoulder_position = upper_chest_point_position + CommonParameter.ROBOT_BONES_OFFSET.upper_chest_to_right_shoulder;
            Vector3 left_shoulder_position = upper_chest_point_position + CommonParameter.ROBOT_BONES_OFFSET.upper_chest_to_left_shoulder;

            Quaternion right_shoulder_absolute_rotation = upper_chest_absolute_rotation * this.get_right_shoulder_rotation();
            Quaternion left_shoulder_absolute_rotation = upper_chest_absolute_rotation * this.get_left_shoulder_rotation();


            waist_down_absolute_transform.hip_position = hip_base_position;
            waist_down_absolute_transform.hip_rotation = hip_absolute_rotation;
            waist_down_absolute_transform.spine_position = spine_base_position;
            waist_down_absolute_transform.spine_rotation = spine_absolute_rotation;
            waist_down_absolute_transform.chest_position = chest_base_position;
            waist_down_absolute_transform.chest_rotation = chest_absolute_rotation;
            waist_down_absolute_transform.upper_chest_position = upper_chest_base_position;
            waist_down_absolute_transform.upper_chest_rotation = upper_chest_absolute_rotation;
            waist_down_absolute_transform.right_shoulder_position = right_shoulder_position;
            waist_down_absolute_transform.left_shoulder_position = left_shoulder_position;
            waist_down_absolute_transform.right_shoulder_rotation = right_shoulder_absolute_rotation;
            waist_down_absolute_transform.left_shoulder_rotation = left_shoulder_absolute_rotation;
        }

        /*********************************************************
        * Destructor
        *********************************************************/
        ~WaistDownUnitController()
        {

        }
    }

}
