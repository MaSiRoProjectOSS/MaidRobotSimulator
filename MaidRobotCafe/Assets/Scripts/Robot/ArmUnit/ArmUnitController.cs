/**
 * @file ArmUnitController.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Control wasit down unit.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

using System;
using UnityEngine;
using UniHumanoid;

namespace MaidRobotSimulator.MaidRobotCafe
{
    public class ArmUnitController
    {
        private GameObject _Robot_GameObject;
        private RobotController _robot_controller;

        private WaistDownUnitController _WaistDownUnitController;

        private EnvironmentController _EnvironmentController;
        private PlayerController _PlayerController;

        private InputManager _InputManager;

        private SystemStructure.ST_HUMANOID_ARM_ROTATION _right_arm_rotation =
            new SystemStructure.ST_HUMANOID_ARM_ROTATION(
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity);

        private SystemStructure.ST_HUMANOID_ARM_ROTATION _left_arm_rotation =
            new SystemStructure.ST_HUMANOID_ARM_ROTATION(
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity,
                Quaternion.identity);

        SystemStructure.ST_8_DIRECTION_MOVE _hand_direction_input = new SystemStructure.ST_8_DIRECTION_MOVE(
            false, false, false, false, false, false, false, false); /*!< direction input */

        Vector3 _right_upper_arm_absolute_position = Vector3.zero;
        Vector3 _left_upper_arm_absolute_position = Vector3.zero;
        Vector3 _right_upper_arm_relative_position = Vector3.zero;
        Vector3 _left_upper_arm_relative_position = Vector3.zero;
        Vector3 _right_hand_absolute_position = Vector3.zero;
        Vector3 _left_hand_absolute_position = Vector3.zero;
        Vector3 _right_hand_relative_position = Vector3.zero;
        Vector3 _left_hand_relative_position = Vector3.zero;


        Vector3 _distance_from_robot_to_right_hand = Vector3.zero;
        Vector3 _distance_from_robot_to_left_hand = Vector3.zero;
        Quaternion _robot_rotation_at_initializing = Quaternion.identity;

        private SystemStructure.ROBOT_HAND_HOLDING_SIDE _hand_holding_side = CommonParameter.HAND_HOLDING_SIDE_MODE_INIT;

        public ArmUnitController(WaistDownUnitController waist_down_unit_controller)
        {
            this._WaistDownUnitController = waist_down_unit_controller;
            this._InputManager = GameObject.Find(CommonParameter.INPUT_MANAGER_NAME).GetComponent<InputManager>();

            this._Robot_GameObject = GameObject.Find(CommonParameter.ROBOT_NAME);
            this._robot_controller = this._Robot_GameObject.GetComponent<RobotController>();

            this._EnvironmentController = GameObject.Find(CommonParameter.ENVIRONMENT_NAME).GetComponent<EnvironmentController>();

            GameObject player_GameObject = GameObject.Find(this._EnvironmentController.get_player_name());
            if (null != player_GameObject)
            {
                this._PlayerController = player_GameObject.GetComponent<PlayerController>();
            }

            this.initialize_arm_and_hand_angles();

            this.initialize_arm_position_for_hand_holding(this._WaistDownUnitController);
        }

        public Vector3 get_right_hand_absolute_position()
        {
            return this._right_hand_absolute_position;
        }

        public Vector3 get_left_hand_absolute_position()
        {
            return this._left_hand_absolute_position;
        }

        public Vector3 get_right_hand_relative_position()
        {
            return this._right_hand_relative_position;
        }

        public Vector3 get_left_hand_relative_position()
        {
            return this._left_hand_relative_position;
        }

        public Vector3 get_right_upper_arm_absolute_position()
        {
            return this._right_upper_arm_absolute_position;
        }

        public Vector3 get_left_upper_arm_absolute_position()
        {
            return this._left_upper_arm_absolute_position;
        }

        public Vector3 get_right_upper_arm_relative_position()
        {
            return this._right_upper_arm_relative_position;
        }

        public Vector3 get_left_upper_arm_relative_position()
        {
            return this._left_upper_arm_relative_position;
        }

        public Quaternion get_upper_arm_rotation_for_locomotion()
        {
            Quaternion upper_arm_rotation = Quaternion.identity;

            if(SystemStructure.ROBOT_HAND_HOLDING_SIDE.RIGHT == this._hand_holding_side)
            {
                upper_arm_rotation = this.get_right_upper_arm_rotation();
            }
            else if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.LEFT == this._hand_holding_side)
            {
                upper_arm_rotation = this.get_left_upper_arm_rotation();
            }
            else
            {
                throw new Exception("Abnormal hand holding side");
            }

            return upper_arm_rotation;
        }

        public Quaternion get_right_upper_arm_rotation()
        {
            return this._WaistDownUnitController.get_right_shoulder_rotation()
                * this._right_arm_rotation.upper_arm;
        }

        public Quaternion get_left_upper_arm_rotation()
        {
            return this._WaistDownUnitController.get_left_shoulder_rotation()
                * this._left_arm_rotation.upper_arm;
        }

        public Quaternion get_right_upper_arm_relative_rotation()
        {
            return this._right_arm_rotation.upper_arm;
        }

        public Quaternion get_left_upper_arm_relative_rotation()
        {
            return this._left_arm_rotation.upper_arm;
        }

        public Quaternion get_right_lower_arm_relative_rotation()
        {
            return this._right_arm_rotation.lower_arm;
        }

        public Quaternion get_left_lower_arm_relative_rotation()
        {
            return this._left_arm_rotation.lower_arm;
        }

        public SystemStructure.ROBOT_HAND_HOLDING_SIDE get_hand_holding_side()
        {
            return this._hand_holding_side;
        }

        public Vector3 get_distance_from_robot_to_right_hand()
        {
            return this._distance_from_robot_to_right_hand;
        }

        public Vector3 get_distance_from_robot_to_left_hand()
        {
            return this._distance_from_robot_to_left_hand;
        }

        public Quaternion get_robot_rotation_at_initializing()
        {
            return this._robot_rotation_at_initializing;
        }

        public Vector3 get_hand_position_for_hand_holding()
        {
            Vector3 hand_position = Vector3.zero;

            if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.RIGHT == this._hand_holding_side)
            {
                hand_position = this.get_right_hand_relative_position();
                hand_position.x -= this._right_upper_arm_relative_position.x;
                hand_position.y -= this._right_upper_arm_relative_position.y;
            }
            else if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.LEFT == this._hand_holding_side)
            {
                hand_position = this.get_left_hand_relative_position();
                hand_position.x -= this._left_upper_arm_relative_position.x;
                hand_position.y -= this._left_upper_arm_relative_position.y;
            }
            else
            {
                throw new Exception("Abnormal robot hand holding side");
            }

            return hand_position;
        }

        public Vector3 get_holding_hand_absolute_position()
        {
            Vector3 hand_position = Vector3.zero;

            if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.RIGHT == this._hand_holding_side)
            {
                hand_position = this.get_right_hand_absolute_position();
            }
            else if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.LEFT == this._hand_holding_side)
            {
                hand_position = this.get_left_hand_absolute_position();
            }
            else
            {
                throw new Exception("Abnormal robot hand holding side");
            }

            return hand_position;
        }

        public void draw_arm_rotation(ref GameObject robot_GameObject)
        {
            /* Shoulder -> Upper arm */
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.RightUpperArm,
                this._right_arm_rotation.upper_arm, ref robot_GameObject);
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.LeftUpperArm,
                this._left_arm_rotation.upper_arm, ref robot_GameObject);

            /* Upper arm -> Lower arm */
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.RightLowerArm,
                this._right_arm_rotation.lower_arm, ref robot_GameObject);
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.LeftLowerArm,
                this._left_arm_rotation.lower_arm, ref robot_GameObject);

            /* Lower arm -> Hand */
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.RightHand,
                this._right_arm_rotation.hand, ref robot_GameObject);
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.LeftHand,
                this._left_arm_rotation.hand, ref robot_GameObject);

            /* Hand -> Thumb */
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.RightThumbProximal,
                this._right_arm_rotation.thumb_proximal, ref robot_GameObject);
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.RightThumbIntermediate,
                this._right_arm_rotation.thumb_intermediate, ref robot_GameObject);
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.RightThumbDistal,
                this._right_arm_rotation.thumb_distal, ref robot_GameObject);

            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.LeftThumbProximal,
                this._left_arm_rotation.thumb_proximal, ref robot_GameObject);
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.LeftThumbIntermediate,
                this._left_arm_rotation.thumb_intermediate, ref robot_GameObject);
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.LeftThumbDistal,
                this._left_arm_rotation.thumb_distal, ref robot_GameObject);

            /* Hand -> Index */
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.RightIndexProximal,
                this._right_arm_rotation.index_proximal, ref robot_GameObject);
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.RightIndexIntermediate,
                this._right_arm_rotation.index_intermediate, ref robot_GameObject);
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.RightIndexDistal,
                this._right_arm_rotation.index_distal, ref robot_GameObject);

            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.LeftIndexProximal,
                this._left_arm_rotation.index_proximal, ref robot_GameObject);
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.LeftIndexIntermediate,
                this._left_arm_rotation.index_intermediate, ref robot_GameObject);
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.LeftIndexDistal,
                this._left_arm_rotation.index_distal, ref robot_GameObject);

            /* Hand -> Middle */
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.RightMiddleProximal,
                this._right_arm_rotation.middle_proximal, ref robot_GameObject);
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.RightMiddleIntermediate,
                this._right_arm_rotation.middle_intermediate, ref robot_GameObject);
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.RightMiddleDistal,
                this._right_arm_rotation.middle_distal, ref robot_GameObject);

            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.LeftMiddleProximal,
                this._left_arm_rotation.middle_proximal, ref robot_GameObject);
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.LeftMiddleIntermediate,
                this._left_arm_rotation.middle_intermediate, ref robot_GameObject);
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.LeftMiddleDistal,
                this._left_arm_rotation.middle_distal, ref robot_GameObject);

            /* Hand -> Ring */
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.RightRingProximal,
                this._right_arm_rotation.ring_proximal, ref robot_GameObject);
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.RightRingIntermediate,
                this._right_arm_rotation.ring_intermediate, ref robot_GameObject);
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.RightRingDistal,
                this._right_arm_rotation.ring_distal, ref robot_GameObject);

            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.LeftRingProximal,
                this._left_arm_rotation.ring_proximal, ref robot_GameObject);
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.LeftRingIntermediate,
                this._left_arm_rotation.ring_intermediate, ref robot_GameObject);
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.LeftRingDistal,
                this._left_arm_rotation.ring_distal, ref robot_GameObject);

            /* Hand -> Little */
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.RightLittleProximal,
                this._right_arm_rotation.little_proximal, ref robot_GameObject);
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.RightLittleIntermediate,
                this._right_arm_rotation.little_intermediate, ref robot_GameObject);
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.RightLittleDistal,
                this._right_arm_rotation.little_distal, ref robot_GameObject);

            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.LeftLittleProximal,
                this._left_arm_rotation.little_proximal, ref robot_GameObject);
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.LeftLittleIntermediate,
                this._left_arm_rotation.little_intermediate, ref robot_GameObject);
            CommonTransform.draw_robot_bone_rotation(HumanBodyBones.LeftLittleDistal,
                this._left_arm_rotation.little_distal, ref robot_GameObject);
        }

        public void initialize_arm_and_hand_angles()
        {
            if (SystemStructure.ROBOT_MODE.CATERING == this._robot_controller.get_robot_mode())
            {
                this._right_arm_rotation.upper_arm = CommonParameter.RIGHT_UPPER_ARM_CATERING_ROTATION_INIT;
                this._right_arm_rotation.lower_arm = CommonParameter.RIGHT_LOWER_ARM_CATERING_ROTATION_INIT;
                this._right_arm_rotation.hand = CommonParameter.RIGHT_HAND_CATERING_ROTATION_INIT;
                this._right_arm_rotation.thumb_proximal = CommonParameter.RIGHT_THUMB_PROXIMAL_CATERING_ROTATION_INIT;
                this._right_arm_rotation.thumb_intermediate = CommonParameter.RIGHT_THUMB_INTERMEDIATE_CATERING_ROTATION_INIT;
                this._right_arm_rotation.thumb_distal = CommonParameter.RIGHT_THUMB_DISTAL_CATERING_ROTATION_INIT;
                this._right_arm_rotation.index_proximal = CommonParameter.RIGHT_INDEX_PROXIMAL_CATERING_ROTATION_INIT;
                this._right_arm_rotation.index_intermediate = CommonParameter.RIGHT_INDEX_INTERMEDIATE_CATERING_ROTATION_INIT;
                this._right_arm_rotation.index_distal = CommonParameter.RIGHT_INDEX_DISTAL_CATERING_ROTATION_INIT;
                this._right_arm_rotation.middle_proximal = CommonParameter.RIGHT_MIDDLE_PROXIMAL_CATERING_ROTATION_INIT;
                this._right_arm_rotation.middle_intermediate = CommonParameter.RIGHT_MIDDLE_INTERMEDIATE_CATERING_ROTATION_INIT;
                this._right_arm_rotation.middle_distal = CommonParameter.RIGHT_MIDDLE_DISTAL_CATERING_ROTATION_INIT;
                this._right_arm_rotation.ring_proximal = CommonParameter.RIGHT_RING_PROXIMAL_CATERING_ROTATION_INIT;
                this._right_arm_rotation.ring_intermediate = CommonParameter.RIGHT_RING_INTERMEDIATE_CATERING_ROTATION_INIT;
                this._right_arm_rotation.ring_distal = CommonParameter.RIGHT_RING_DISTAL_CATERING_ROTATION_INIT;
                this._right_arm_rotation.little_proximal = CommonParameter.RIGHT_LITTLE_PROXIMAL_CATERING_ROTATION_INIT;
                this._right_arm_rotation.little_intermediate = CommonParameter.RIGHT_LITTLE_INTERMEDIATE_CATERING_ROTATION_INIT;
                this._right_arm_rotation.little_distal = CommonParameter.RIGHT_LITTLE_DISTAL_CATERING_ROTATION_INIT;

                this._left_arm_rotation.upper_arm = CommonParameter.LEFT_UPPER_ARM_CATERING_ROTATION_INIT;
                this._left_arm_rotation.lower_arm = CommonParameter.LEFT_LOWER_ARM_CATERING_ROTATION_INIT;
                this._left_arm_rotation.hand = CommonParameter.LEFT_HAND_CATERING_ROTATION_INIT;
                this._left_arm_rotation.thumb_proximal = CommonParameter.LEFT_THUMB_PROXIMAL_CATERING_ROTATION_INIT;
                this._left_arm_rotation.thumb_intermediate = CommonParameter.LEFT_THUMB_INTERMEDIATE_CATERING_ROTATION_INIT;
                this._left_arm_rotation.thumb_distal = CommonParameter.LEFT_THUMB_DISTAL_CATERING_ROTATION_INIT;
                this._left_arm_rotation.index_proximal = CommonParameter.LEFT_INDEX_PROXIMAL_CATERING_ROTATION_INIT;
                this._left_arm_rotation.index_intermediate = CommonParameter.LEFT_INDEX_INTERMEDIATE_CATERING_ROTATION_INIT;
                this._left_arm_rotation.index_distal = CommonParameter.LEFT_INDEX_DISTAL_CATERING_ROTATION_INIT;
                this._left_arm_rotation.middle_proximal = CommonParameter.LEFT_MIDDLE_PROXIMAL_CATERING_ROTATION_INIT;
                this._left_arm_rotation.middle_intermediate = CommonParameter.LEFT_MIDDLE_INTERMEDIATE_CATERING_ROTATION_INIT;
                this._left_arm_rotation.middle_distal = CommonParameter.LEFT_MIDDLE_DISTAL_CATERING_ROTATION_INIT;
                this._left_arm_rotation.ring_proximal = CommonParameter.LEFT_RING_PROXIMAL_CATERING_ROTATION_INIT;
                this._left_arm_rotation.ring_intermediate = CommonParameter.LEFT_RING_INTERMEDIATE_CATERING_ROTATION_INIT;
                this._left_arm_rotation.ring_distal = CommonParameter.LEFT_RING_DISTAL_CATERING_ROTATION_INIT;
                this._left_arm_rotation.little_proximal = CommonParameter.LEFT_LITTLE_PROXIMAL_CATERING_ROTATION_INIT;
                this._left_arm_rotation.little_intermediate = CommonParameter.LEFT_LITTLE_INTERMEDIATE_CATERING_ROTATION_INIT;
                this._left_arm_rotation.little_distal = CommonParameter.LEFT_LITTLE_DISTAL_CATERING_ROTATION_INIT;
            }
            else if (SystemStructure.ROBOT_MODE.HAND_HOLDING == this._robot_controller.get_robot_mode())
            {
                this._right_arm_rotation.upper_arm = CommonParameter.RIGHT_UPPER_ARM_HAND_HOLDING_ROTATION_INIT;
                this._right_arm_rotation.lower_arm = CommonParameter.RIGHT_LOWER_ARM_HAND_HOLDING_ROTATION_INIT;
                this._right_arm_rotation.hand = CommonParameter.RIGHT_HAND_HAND_HOLDING_ROTATION_INIT;
                this._right_arm_rotation.thumb_proximal = CommonParameter.RIGHT_THUMB_PROXIMAL_HAND_HOLDING_ROTATION_INIT;
                this._right_arm_rotation.thumb_intermediate = CommonParameter.RIGHT_THUMB_INTERMEDIATE_HAND_HOLDING_ROTATION_INIT;
                this._right_arm_rotation.thumb_distal = CommonParameter.RIGHT_THUMB_DISTAL_HAND_HOLDING_ROTATION_INIT;
                this._right_arm_rotation.index_proximal = CommonParameter.RIGHT_INDEX_PROXIMAL_HAND_HOLDING_ROTATION_INIT;
                this._right_arm_rotation.index_intermediate = CommonParameter.RIGHT_INDEX_INTERMEDIATE_HAND_HOLDING_ROTATION_INIT;
                this._right_arm_rotation.index_distal = CommonParameter.RIGHT_INDEX_DISTAL_HAND_HOLDING_ROTATION_INIT;
                this._right_arm_rotation.middle_proximal = CommonParameter.RIGHT_MIDDLE_PROXIMAL_HAND_HOLDING_ROTATION_INIT;
                this._right_arm_rotation.middle_intermediate = CommonParameter.RIGHT_MIDDLE_INTERMEDIATE_HAND_HOLDING_ROTATION_INIT;
                this._right_arm_rotation.middle_distal = CommonParameter.RIGHT_MIDDLE_DISTAL_HAND_HOLDING_ROTATION_INIT;
                this._right_arm_rotation.ring_proximal = CommonParameter.RIGHT_RING_PROXIMAL_HAND_HOLDING_ROTATION_INIT;
                this._right_arm_rotation.ring_intermediate = CommonParameter.RIGHT_RING_INTERMEDIATE_HAND_HOLDING_ROTATION_INIT;
                this._right_arm_rotation.ring_distal = CommonParameter.RIGHT_RING_DISTAL_HAND_HOLDING_ROTATION_INIT;
                this._right_arm_rotation.little_proximal = CommonParameter.RIGHT_LITTLE_PROXIMAL_HAND_HOLDING_ROTATION_INIT;
                this._right_arm_rotation.little_intermediate = CommonParameter.RIGHT_LITTLE_INTERMEDIATE_HAND_HOLDING_ROTATION_INIT;
                this._right_arm_rotation.little_distal = CommonParameter.RIGHT_LITTLE_DISTAL_HAND_HOLDING_ROTATION_INIT;

                this._left_arm_rotation.upper_arm = CommonParameter.LEFT_UPPER_ARM_HAND_HOLDING_ROTATION_INIT;
                this._left_arm_rotation.lower_arm = CommonParameter.LEFT_LOWER_ARM_HAND_HOLDING_ROTATION_INIT;
                this._left_arm_rotation.hand = CommonParameter.LEFT_HAND_HAND_HOLDING_ROTATION_INIT;
                this._left_arm_rotation.thumb_proximal = CommonParameter.LEFT_THUMB_PROXIMAL_HAND_HOLDING_ROTATION_INIT;
                this._left_arm_rotation.thumb_intermediate = CommonParameter.LEFT_THUMB_INTERMEDIATE_HAND_HOLDING_ROTATION_INIT;
                this._left_arm_rotation.thumb_distal = CommonParameter.LEFT_THUMB_DISTAL_HAND_HOLDING_ROTATION_INIT;
                this._left_arm_rotation.index_proximal = CommonParameter.LEFT_INDEX_PROXIMAL_HAND_HOLDING_ROTATION_INIT;
                this._left_arm_rotation.index_intermediate = CommonParameter.LEFT_INDEX_INTERMEDIATE_HAND_HOLDING_ROTATION_INIT;
                this._left_arm_rotation.index_distal = CommonParameter.LEFT_INDEX_DISTAL_HAND_HOLDING_ROTATION_INIT;
                this._left_arm_rotation.middle_proximal = CommonParameter.LEFT_MIDDLE_PROXIMAL_HAND_HOLDING_ROTATION_INIT;
                this._left_arm_rotation.middle_intermediate = CommonParameter.LEFT_MIDDLE_INTERMEDIATE_HAND_HOLDING_ROTATION_INIT;
                this._left_arm_rotation.middle_distal = CommonParameter.LEFT_MIDDLE_DISTAL_HAND_HOLDING_ROTATION_INIT;
                this._left_arm_rotation.ring_proximal = CommonParameter.LEFT_RING_PROXIMAL_HAND_HOLDING_ROTATION_INIT;
                this._left_arm_rotation.ring_intermediate = CommonParameter.LEFT_RING_INTERMEDIATE_HAND_HOLDING_ROTATION_INIT;
                this._left_arm_rotation.ring_distal = CommonParameter.LEFT_RING_DISTAL_HAND_HOLDING_ROTATION_INIT;
                this._left_arm_rotation.little_proximal = CommonParameter.LEFT_LITTLE_PROXIMAL_HAND_HOLDING_ROTATION_INIT;
                this._left_arm_rotation.little_intermediate = CommonParameter.LEFT_LITTLE_INTERMEDIATE_HAND_HOLDING_ROTATION_INIT;
                this._left_arm_rotation.little_distal = CommonParameter.LEFT_LITTLE_DISTAL_HAND_HOLDING_ROTATION_INIT;
            }
            else
            {
                throw new Exception("Abnormal robot mode");
            }
        }

        public void move_holding_hand(Humanoid humanoid_component, float delta_time)
        {
            this._get_hand_direction();

            this._update_hand_position_and_rotation(humanoid_component, delta_time);
        }

        public void initialize_arm_position_for_hand_holding(WaistDownUnitController waist_down_unit_controller)
        {
            SystemStructure.ST_WAIST_DOWN_ABSOLUTE_TRANSFORM waist_down_absolute_transform = new SystemStructure.ST_WAIST_DOWN_ABSOLUTE_TRANSFORM();
            waist_down_unit_controller.calculate_wasit_down_absolute_bone_transform(
                        waist_down_unit_controller.get_hip_position(), ref waist_down_absolute_transform);

            this._right_upper_arm_relative_position = waist_down_absolute_transform.right_shoulder_position
                + waist_down_unit_controller.get_right_shoulder_rotation() *
                (CommonParameter.ROBOT_BONES_LENGTH.shoulder * CommonParameter.RIGHT_SHOULDER_BONE_DIRECTION_VECTOR);
            this._left_upper_arm_relative_position = waist_down_absolute_transform.left_shoulder_position
                + waist_down_unit_controller.get_left_shoulder_rotation() *
                (CommonParameter.ROBOT_BONES_LENGTH.shoulder * CommonParameter.LEFT_SHOULDER_BONE_DIRECTION_VECTOR);

            Quaternion right_upper_arm_rotation_from_shoulder =
                waist_down_unit_controller.get_right_shoulder_rotation() * this._right_arm_rotation.upper_arm;
            Quaternion left_upper_arm_rotation_from_shoulder =
                waist_down_unit_controller.get_left_shoulder_rotation() * this._left_arm_rotation.upper_arm;

            Vector3 right_lower_arm_position = this._right_upper_arm_relative_position
                + right_upper_arm_rotation_from_shoulder *
                (CommonParameter.ROBOT_BONES_LENGTH.upper_arm * CommonParameter.RIGHT_SHOULDER_BONE_DIRECTION_VECTOR);
            Vector3 left_lower_arm_position = this._left_upper_arm_relative_position
                + left_upper_arm_rotation_from_shoulder *
                (CommonParameter.ROBOT_BONES_LENGTH.upper_arm * CommonParameter.LEFT_SHOULDER_BONE_DIRECTION_VECTOR);

            Quaternion right_lower_arm_rotation_from_shoulder =
                right_upper_arm_rotation_from_shoulder * this._right_arm_rotation.lower_arm;
            Quaternion left_lower_arm_rotation_from_shoulder =
                left_upper_arm_rotation_from_shoulder * this._left_arm_rotation.lower_arm;

            this._right_hand_relative_position = right_lower_arm_position
                + right_lower_arm_rotation_from_shoulder *
                (CommonParameter.ROBOT_BONES_LENGTH.lower_arm * CommonParameter.RIGHT_SHOULDER_BONE_DIRECTION_VECTOR);
            this._left_hand_relative_position = left_lower_arm_position
                + left_lower_arm_rotation_from_shoulder *
                (CommonParameter.ROBOT_BONES_LENGTH.lower_arm * CommonParameter.LEFT_SHOULDER_BONE_DIRECTION_VECTOR);


            this._right_upper_arm_absolute_position = 
                this._WaistDownUnitController.get_current_rotation() * this._right_upper_arm_relative_position
                + this._WaistDownUnitController.get_current_position();
            this._left_upper_arm_absolute_position = 
                this._WaistDownUnitController.get_current_rotation() * this._left_upper_arm_relative_position
                + this._WaistDownUnitController.get_current_position();

            this._distance_from_robot_to_right_hand =
                this._WaistDownUnitController.get_current_rotation() * this._right_hand_relative_position;
            this._distance_from_robot_to_left_hand =
                this._WaistDownUnitController.get_current_rotation() * this._left_hand_relative_position;

            this._right_hand_absolute_position =
                this._distance_from_robot_to_right_hand + this._WaistDownUnitController.get_current_position();
            this._left_hand_absolute_position =
                this._distance_from_robot_to_left_hand + this._WaistDownUnitController.get_current_position();

            this._robot_rotation_at_initializing = this._WaistDownUnitController.get_current_rotation();
        }

        public void update_hand_holding_arm_rotation(Quaternion upper_arm_rotation, Quaternion lower_arm_rotation)
        {
            if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.RIGHT == this._hand_holding_side)
            {
                this._right_arm_rotation.upper_arm = upper_arm_rotation;
                this._right_arm_rotation.lower_arm = lower_arm_rotation;
            }
            else if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.LEFT == this._hand_holding_side)
            {
                this._left_arm_rotation.upper_arm = upper_arm_rotation;
                this._left_arm_rotation.lower_arm = lower_arm_rotation;
            }
            else
            {
                throw new Exception("Abnormal hand holding side");
            }
        }

        public void get_hand_holding_arm_euler_angle(
            ref Vector3 upper_arm_angle, ref Vector3 lower_arm_angle)
        {
            Quaternion upper_arm_rotation = Quaternion.identity;
            Quaternion lower_arm_rotation = Quaternion.identity;

            if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.RIGHT == this._hand_holding_side)
            {
                upper_arm_rotation = this._right_arm_rotation.upper_arm;
                lower_arm_rotation = this._right_arm_rotation.lower_arm;
            }
            else if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.LEFT == this._hand_holding_side)
            {
                upper_arm_rotation = this._left_arm_rotation.upper_arm;
                lower_arm_rotation = this._left_arm_rotation.lower_arm;
            }
            else
            {
                throw new Exception("Abnormal hand holding side");
            }

            CommonTransform.transform_quaternion_to_euler_degree(
                upper_arm_rotation, ref upper_arm_angle);
            CommonTransform.transform_quaternion_to_euler_degree(
                lower_arm_rotation, ref lower_arm_angle);
        }

        /*********************************************************
         * Private functions
         *********************************************************/
        private void _get_hand_direction()
        {
            this._InputManager.get_hand_direction_input(ref this._hand_direction_input);
        }

        private void _update_hand_position_and_rotation(Humanoid humanoid_component, float delta_time)
        {
            Vector3 next_hand_position = Vector3.zero;
            Vector3 delta_position = Vector3.zero;

            /* get current position */
            if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.RIGHT == this._hand_holding_side)
            {
                next_hand_position = this._right_hand_absolute_position;
            }
            else if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.LEFT == this._hand_holding_side)
            {
                next_hand_position = this._left_hand_absolute_position;
            }
            else
            {
                throw new Exception("Abnormal hand holding side");
            }

            /* set speed */
            if (this._hand_direction_input.forward)
            {
                delta_position.x = -CommonParameter.HAND_MOTION_SPEED * delta_time;
            }
            if (this._hand_direction_input.backward)
            {
                delta_position.x = CommonParameter.HAND_MOTION_SPEED * delta_time;
            }
            if (this._hand_direction_input.right)
            {
                delta_position.y = CommonParameter.HAND_MOTION_SPEED * delta_time;
            }
            if (this._hand_direction_input.left)
            {
                delta_position.y = -CommonParameter.HAND_MOTION_SPEED * delta_time;
            }
            if (this._hand_direction_input.up)
            {
                delta_position.z = CommonParameter.HAND_MOTION_SPEED * delta_time;
            }
            if (this._hand_direction_input.down)
            {
                delta_position.z = -CommonParameter.HAND_MOTION_SPEED * delta_time;
            }

            /* set next position */
            next_hand_position += 
                this._WaistDownUnitController.get_current_rotation() * delta_position;

            /* add player hand position */
            if (null != this._PlayerController)
            {
                next_hand_position += this._PlayerController.get_hand_holding_position();
            }

            /* update current position */
            if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.RIGHT == this._hand_holding_side)
            {
                this._right_hand_absolute_position = next_hand_position;
                this._right_hand_relative_position = 
                    Quaternion.Inverse(this._WaistDownUnitController.get_current_rotation())
                    * (this._right_hand_absolute_position
                        - this._WaistDownUnitController.get_current_position());
            }
            else if (SystemStructure.ROBOT_HAND_HOLDING_SIDE.LEFT == this._hand_holding_side)
            {
                this._left_hand_absolute_position = next_hand_position;
                this._left_hand_relative_position =
                    Quaternion.Inverse(this._WaistDownUnitController.get_current_rotation())
                    * (this._left_hand_absolute_position
                        - this._WaistDownUnitController.get_current_position());
            }
            else
            {
                throw new Exception("Abnormal hand holding side");
            }
        }

        /*********************************************************
        * Destructor
        *********************************************************/
        ~ArmUnitController()
        {

        }
    }

}
