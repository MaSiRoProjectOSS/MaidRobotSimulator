/**
 * @file InverseKinematicsManager.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Calculate inverse kinematics.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

using System;
using UnityEngine;
using UniHumanoid;

namespace MaidRobotSimulator.MaidRobotCafe
{
    public class InverseKinematicsManager
    {
        public struct ST_JOINT_ANGLE_LIMIT
        {
            public float roll_max;
            public float roll_min;
            public float pitch_max;
            public float pitch_min;
            public float yaw_max;
            public float yaw_min;

            public ST_JOINT_ANGLE_LIMIT(
                float roll_max_in,
                float roll_min_in,
                float pitch_max_in,
                float pitch_min_in,
                float yaw_max_in,
                float yaw_min_in
                )
            {
                this.roll_max = roll_max_in;
                this.roll_min = roll_min_in;
                this.pitch_max = pitch_max_in;
                this.pitch_min = pitch_min_in;
                this.yaw_max = yaw_max_in;
                this.yaw_min = yaw_min_in;
            }
        }

        /*********************************************************
        * Private variables
        *********************************************************/
        private GameObject _robot_GameObject;
        private RobotController _RobotController;

        private Animator _animator;

        private ArmUnitController _ArmUnitController;

        private Quaternion _upper_arm_rotation;
        private Quaternion _lower_arm_rotation;
        private Vector3 _hand_position;
        private ST_JOINT_ANGLE_LIMIT _upper_arm_limit;
        private ST_JOINT_ANGLE_LIMIT _lower_arm_limit;
        private SystemStructure.ROBOT_HAND_HOLDING_SIDE _hand_holding_side;

        public InverseKinematicsManager(
            SystemStructure.ROBOT_HAND_HOLDING_SIDE hand_holding_side,
            ArmUnitController arm_unit_controller)
        {
            this._hand_holding_side = hand_holding_side;
            this._ArmUnitController = arm_unit_controller;

            this._robot_GameObject = GameObject.Find(CommonParameter.ROBOT_NAME);
            this._RobotController = this._robot_GameObject.GetComponent<RobotController>();

            this._animator = this._robot_GameObject.GetComponent<Animator>();

            if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.RIGHT == this._hand_holding_side)
            {
                this._upper_arm_rotation = CommonParameter.RIGHT_UPPER_ARM_HAND_HOLDING_ROTATION_INIT;
                this._lower_arm_rotation = CommonParameter.RIGHT_LOWER_ARM_HAND_HOLDING_ROTATION_INIT;
                this._upper_arm_limit = new ST_JOINT_ANGLE_LIMIT(
                    CommonParameter.RIGHT_UPPER_ARM_ROLL_ANGLE_MAX,
                    CommonParameter.RIGHT_UPPER_ARM_ROLL_ANGLE_MIN,
                    CommonParameter.RIGHT_UPPER_ARM_PITCH_ANGLE_MAX,
                    CommonParameter.RIGHT_UPPER_ARM_PITCH_ANGLE_MIN,
                    CommonParameter.RIGHT_UPPER_ARM_YAW_ANGLE_MAX,
                    CommonParameter.RIGHT_UPPER_ARM_YAW_ANGLE_MIN
                    );
                this._lower_arm_limit = new ST_JOINT_ANGLE_LIMIT(
                    CommonParameter.RIGHT_LOWER_ARM_ROLL_ANGLE_MAX,
                    CommonParameter.RIGHT_LOWER_ARM_ROLL_ANGLE_MIN,
                    CommonParameter.RIGHT_LOWER_ARM_PITCH_ANGLE_MAX,
                    CommonParameter.RIGHT_LOWER_ARM_PITCH_ANGLE_MIN,
                    CommonParameter.RIGHT_LOWER_ARM_YAW_ANGLE_MAX,
                    CommonParameter.RIGHT_LOWER_ARM_YAW_ANGLE_MIN
                    );
            }
            else if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.LEFT == this._hand_holding_side)
            {
                this._upper_arm_rotation = CommonParameter.LEFT_UPPER_ARM_HAND_HOLDING_ROTATION_INIT;
                this._lower_arm_rotation = CommonParameter.LEFT_LOWER_ARM_HAND_HOLDING_ROTATION_INIT;
                this._upper_arm_limit = new ST_JOINT_ANGLE_LIMIT(
                    CommonParameter.LEFT_UPPER_ARM_ROLL_ANGLE_MAX,
                    CommonParameter.LEFT_UPPER_ARM_ROLL_ANGLE_MIN,
                    CommonParameter.LEFT_UPPER_ARM_PITCH_ANGLE_MAX,
                    CommonParameter.LEFT_UPPER_ARM_PITCH_ANGLE_MIN,
                    CommonParameter.LEFT_UPPER_ARM_YAW_ANGLE_MAX,
                    CommonParameter.LEFT_UPPER_ARM_YAW_ANGLE_MIN
                    );
                this._lower_arm_limit = new ST_JOINT_ANGLE_LIMIT(
                    CommonParameter.LEFT_LOWER_ARM_ROLL_ANGLE_MAX,
                    CommonParameter.LEFT_LOWER_ARM_ROLL_ANGLE_MIN,
                    CommonParameter.LEFT_LOWER_ARM_PITCH_ANGLE_MAX,
                    CommonParameter.LEFT_LOWER_ARM_PITCH_ANGLE_MIN,
                    CommonParameter.LEFT_LOWER_ARM_YAW_ANGLE_MAX,
                    CommonParameter.LEFT_LOWER_ARM_YAW_ANGLE_MIN
                    );
            }
            else
            {
                throw new Exception("Abnormal robot hand holding side");
            }
        }

        /*********************************************************
        * Public functions
        *********************************************************/
        public SystemStructure.ROBOT_HAND_HOLDING_SIDE get_hand_holding_side()
        {
            return this._hand_holding_side;
        }

        public Vector3 get_hand_position_for_IK()
        {
            Vector3 hand_position = Vector3.zero;

            if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.RIGHT == this._hand_holding_side)
            {
                hand_position = this._ArmUnitController.get_right_hand_absolute_position();
            }
            else if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.LEFT == this._hand_holding_side)
            {
                hand_position = this._ArmUnitController.get_left_hand_absolute_position();
            }
            else
            {
                throw new Exception("Abnormal robot hand holding side");
            }

            return hand_position;
        }

        public Vector3 get_upper_arm_position_for_IK()
        {
            Vector3 upper_arm_position = Vector3.zero;

            if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.RIGHT == this._hand_holding_side)
            {
                upper_arm_position = this._ArmUnitController.get_right_upper_arm_absolute_position();
            }
            else if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.LEFT == this._hand_holding_side)
            {
                upper_arm_position = this._ArmUnitController.get_left_upper_arm_absolute_position();
            }
            else
            {
                throw new Exception("Abnormal robot hand holding side");
            }

            return upper_arm_position;
        }

        public void calculate_IK_and_draw_arm_rotation(Vector3 IK_robot_hand_position_unity_axis
            ,WaistDownUnitController waist_down_unit_controller)
        {
            Vector3 hand_position_unity = Vector3.zero;

            this._calculate_not_holding_hand_position(waist_down_unit_controller, ref hand_position_unity);

            if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.RIGHT == this.get_hand_holding_side())
            {
                this._animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                this._animator.SetIKPosition(AvatarIKGoal.RightHand, IK_robot_hand_position_unity_axis);

                this._animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                this._animator.SetIKPosition(AvatarIKGoal.LeftHand, hand_position_unity);
            }
            else if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.LEFT == this.get_hand_holding_side())
            {
                CommonTransform.transform_position_from_robot_to_unity(
                    this._ArmUnitController.get_right_hand_absolute_position(), ref hand_position_unity);
                this._animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                this._animator.SetIKPosition(AvatarIKGoal.RightHand, hand_position_unity);

                this._animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                this._animator.SetIKPosition(AvatarIKGoal.LeftHand, IK_robot_hand_position_unity_axis);
            }
            else
            {
                throw new Exception("Abnormal hand holding side");
            }
        }

        public void read_arm_rotation_after_IK_calculation(
            ref Quaternion upper_arm_rotation_out, ref Quaternion lower_arm_rotation_out)
        {
            this._read_arm_rotation_from_robot();

            upper_arm_rotation_out = this._upper_arm_rotation;
            lower_arm_rotation_out = this._lower_arm_rotation;
        }

        /*********************************************************
        * Private functions
        *********************************************************/
        private void _read_arm_rotation_from_robot()
        {
            Humanoid humanoid_component = this._RobotController.GetComponent<Humanoid>();
            Quaternion upper_arm_rotation = Quaternion.identity;
            Quaternion lower_arm_rotation = Quaternion.identity;

            if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.RIGHT == this._hand_holding_side)
            {
                CommonTransform.transform_relative_rotation_from_unity_to_robot(
                    Quaternion.Inverse(humanoid_component.RightShoulder.rotation) * humanoid_component.RightUpperArm.rotation,
                    ref upper_arm_rotation);
                CommonTransform.transform_relative_rotation_from_unity_to_robot(
                    Quaternion.Inverse(humanoid_component.RightUpperArm.rotation) * humanoid_component.RightLowerArm.rotation,
                    ref lower_arm_rotation);
            }
            else if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.LEFT == this._hand_holding_side)
            {
                CommonTransform.transform_relative_rotation_from_unity_to_robot(
                    Quaternion.Inverse(humanoid_component.LeftShoulder.rotation) * humanoid_component.LeftUpperArm.rotation,
                    ref upper_arm_rotation);
                CommonTransform.transform_relative_rotation_from_unity_to_robot(
                    Quaternion.Inverse(humanoid_component.LeftUpperArm.rotation) * humanoid_component.LeftLowerArm.rotation,
                    ref lower_arm_rotation);
            }
            else
            {
                throw new Exception("Abnormal robot hand holding side");
            }


            this._upper_arm_rotation = upper_arm_rotation;
            this._lower_arm_rotation = lower_arm_rotation;
        }

        private void _calculate_not_holding_hand_position(
            WaistDownUnitController waist_down_unit_controller,
            ref Vector3 hand_position_unity)
        {
            Vector3 hand_position = Vector3.zero;

            if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.RIGHT == this._hand_holding_side)
            {
                hand_position = waist_down_unit_controller.get_current_position()
                    + Quaternion.Inverse(this._ArmUnitController.get_robot_rotation_at_initializing())
                      * waist_down_unit_controller.get_current_rotation()
                      * this._ArmUnitController.get_distance_from_robot_to_left_hand();
            }
            else if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.LEFT == this._hand_holding_side)
            {
                hand_position = waist_down_unit_controller.get_current_position()
                    + Quaternion.Inverse(this._ArmUnitController.get_robot_rotation_at_initializing())
                      * waist_down_unit_controller.get_current_rotation()
                      * this._ArmUnitController.get_distance_from_robot_to_right_hand();
            }
            else
            {
                throw new Exception("Abnormal robot hand holding side");
            }

            CommonTransform.transform_position_from_robot_to_unity(
                hand_position, ref hand_position_unity);
        }

        /*********************************************************
        * Destructor
        *********************************************************/
        ~InverseKinematicsManager()
        {

        }
    }
}
