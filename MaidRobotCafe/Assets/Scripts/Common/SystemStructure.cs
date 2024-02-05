/**
 * @file SystemStructure.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Define system data structure for defining parameters and enumurated values.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

using System;
using UnityEngine;

namespace MaidRobotSimulator.MaidRobotCafe
{
    public class SystemStructure
    {
        public static int SPACE_DIMENSION_NUM = 3;
        public static int QUATERNION_DIMENSTION_NUM = 4;

        public enum SCENE_MODE
        {
            CAFE,
            ROOM,
            EVENT
        }

        public enum ROBOT_MODE
        {
            CATERING,
            HAND_HOLDING
        }

        public enum PLAYER_MODE
        {
            FREE,
            HAND_HOLDING
        }

        public enum INPUT_MODE
        {
            COMMUNICATION,
            KEYBOARD
        }

        public enum COMMUNICATION_MODE
        {
            ROS,
            UDP
        }

        public enum SYNC_MODE
        {
            SYNC,
            ASYNC
        }
        public enum OBJECTS_STATE
        {
            INACTIVATE,
            STAY,
            ON_TRAY
        }

        public enum ROBOT_CARRYING_STATE
        {
            NONE,
            CARRY_TEACUP
        }

        public enum ROBOT_HAND_HOLDING_SIDE
        {
            RIGHT,
            LEFT
        }

        public enum HEAD_UNIT_MODE
        {
            UNKNOWN,
            LOOK_FORWARD,
            I_AGREE,
            QUESTION,
            STARE_AT_THE_OTHER_PERSON,
            FOLLOW_YOUR_HAND,
            FACE_TRACKING
        }

        public enum NECK_POSE_MODE
        {
            RANDOM_MOVE,
            FACE_TRACK,
            COMMAND_MOVING,
            ARM_TRACK,
        }

        public enum NECK_POSE_COMMAND
        {
            STAY,
            NOD,
            INCLINE
        }

        public enum POSE_SEQUENCE
        {
            FIRST,
            SECOND,
            THIRD
        }

        public enum EYE_BLINK_MODE
        {
            NOT_BLINKING,
            BLINKING
        }

        public enum CARRY_OBJECTS
        {
            TEACUP_1
        }

        public enum CAMERA_MODE
        {
            FAR,
            NEAR
        }

        public enum  PLAYER_CAMERA_MODE
        {
            NORMAL,
            FOCUS_TO_PLAYER_VIEW
        }

        public enum START_MENU_MODE
        {
            CLOSE,
            OPEN
        }

        public enum SEND_MODE
        {
            WAIT,
            SEND
        }

        public enum UDP_ERROR_KIND
        {
            NONE,
            MESSAGE_LENGTH,
            SOCKET_EXCEPTION
        }

        public enum ROS_ERROR_KIND
        {
            NONE,
            CONNECTION
        }

        public struct ST_EULER_ANGLE
        {
            public float roll;
            public float pitch;
            public float yaw;
            public int length;
            public ST_EULER_ANGLE(float roll_in, float pitch_in, float yaw_in)
            {
                this.roll = roll_in;
                this.pitch = pitch_in;
                this.yaw = yaw_in;

                length = 3;
            }

            public int get_length()
            {
                return this.length;
            }
        }

        public struct ST_8_DIRECTION_MOVE
        {
            public bool forward;
            public bool backward;
            public bool right;
            public bool left;
            public bool up;
            public bool down;
            public bool turn_right;
            public bool turn_left;

            public ST_8_DIRECTION_MOVE(
                bool forward_in,
                bool backward_in,
                bool right_in,
                bool left_in,
                bool up_in,
                bool down_in,
                bool turn_right_in,
                bool turn_left_in)
            {
                this.forward = forward_in;
                this.backward = backward_in;
                this.right = right_in;
                this.left = left_in;
                this.up = up_in;
                this.down = down_in;
                this.turn_right = turn_right_in;
                this.turn_left = turn_left_in;
            }
        }

        public struct ST_FIRST_PERSON_DIRECTION_VELOCITY
        {
            public float forward_backward;
            public float left_right;
            public float up_down;
            public float look_up_down;
            public float turn_left_right;

            public ST_FIRST_PERSON_DIRECTION_VELOCITY(
                float forward_backward,
                float left_right,
                float up_down,
                float look_up_down,
                float turn_left_right)
            {
                this.forward_backward = forward_backward;
                this.left_right = left_right;
                this.up_down = up_down;
                this.look_up_down = look_up_down;
                this.turn_left_right = turn_left_right;
            }
        }

        public struct ST_MOVE_ROBOT_REFERENCE
        {
            public float forward_velocity;
            public float yaw_angular_velocity;
            public ST_MOVE_ROBOT_REFERENCE(float forward_velocity_in, float yaw_angular_velocity_in)
            {
                this.forward_velocity = forward_velocity_in;
                this.yaw_angular_velocity = yaw_angular_velocity_in;
            }
        }

        public struct ST_ROBOT_SIMULATION_INFORMATION
        {
            public float[] position;
            public float[] rotation;
            public ST_ROBOT_SIMULATION_INFORMATION(float[] position_in, float[] rotation_in)
            {
                this.position = new float[SPACE_DIMENSION_NUM];
                this.rotation = new float[SPACE_DIMENSION_NUM];
                Array.Copy(this.position, position_in, this.position.Length);
                Array.Copy(this.rotation, rotation_in, this.rotation.Length);
            }
        }

        public struct ST_CARRY_OBJECTS_NAME_AND_ID
        {
            public string NAME;
            public CARRY_OBJECTS ID;
            public ST_CARRY_OBJECTS_NAME_AND_ID(string name, CARRY_OBJECTS id)
            {
                this.NAME = name;
                this.ID = id;
            }
        }

        public enum CARRY_OBJECT_PLACE
        {
            ON_FLASKET,
            FRONT_OF_CHAIR_1,
            FRONT_OF_CHAIR_2,
            FRONT_OF_CHAIR_3,
            FRONT_OF_CHAIR_4,
            FRONT_OF_CHAIR_5,
            FRONT_OF_CHAIR_6,
            FRONT_OF_CHAIR_7,
            FRONT_OF_CHAIR_8,
            FRONT_OF_CHAIR_9,
            FRONT_OF_CHAIR_10,
            FRONT_OF_CHAIR_11,
            FRONT_OF_CHAIR_12,
            FRONT_OF_CHAIR_13,
            FRONT_OF_CHAIR_14
        }

        public struct ST_CARRY_OBJECT_POSITION_AND_ID
        {
            public Vector3 POSITION;
            public CARRY_OBJECT_PLACE ID;
            public ST_CARRY_OBJECT_POSITION_AND_ID(Vector3 position, CARRY_OBJECT_PLACE name)
            {
                this.POSITION = position;
                this.ID = name;
            }
        }

        public struct ST_HUMANOID_BONES_LENGTH
        {
            public float hips;
            public float spine;
            public float chest;
            public float upper_chest;
            public float neck;
            public float head;
            public float eye;
            public float shoulder;
            public float upper_arm;
            public float lower_arm;
            public float hand;
            public float thumb_proximal;
            public float thumb_intermediate;
            public float thumb_distal;
            public float index_proximal;
            public float index_intermediate;
            public float index_distal;
            public float middle_proximal;
            public float middle_intermediate;
            public float middle_distal;
            public float ring_proximal;
            public float ring_intermediate;
            public float ring_distal;
            public float little_proximal;
            public float little_intermediate;
            public float little_distal;
            public float upper_leg;
            public float lower_leg;
            public float foot;
            public float toe;
            public ST_HUMANOID_BONES_LENGTH(
                float hips_in,
                float sprine_in,
                float chest_in,
                float upper_chest_in,
                float neck_in,
                float head_in,
                float eye_in,
                float shoulder_in,
                float upper_arm_in,
                float lower_arm_in,
                float hand_in,
                float thumb_proximal_in,
                float thumb_intermediate_in,
                float thumb_distal_in,
                float index_proximal_in,
                float index_intermediate_in,
                float index_distal_in,
                float middle_proximal_in,
                float middle_intermediate_in,
                float middle_distal_in,
                float ring_proximal_in,
                float ring_intermediate_in,
                float ring_distal_in,
                float little_proximal_in,
                float little_intermediate_in,
                float little_distal_in,
                float upper_leg_in,
                float lower_leg_in,
                float foot_in,
                float toe_in
                )
            {
                this.hips = hips_in;
                this.spine = sprine_in;
                this.chest = chest_in;
                this.upper_chest = upper_chest_in;
                this.neck = neck_in;
                this.head = head_in;
                this.eye = eye_in;
                this.shoulder = shoulder_in;
                this.upper_arm = upper_arm_in;
                this.lower_arm = lower_arm_in;
                this.hand = hand_in;
                this.thumb_proximal = thumb_proximal_in;
                this.thumb_intermediate = thumb_intermediate_in;
                this.thumb_distal = thumb_distal_in;
                this.index_proximal = index_proximal_in;
                this.index_intermediate = index_intermediate_in;
                this.index_distal = index_distal_in;
                this.middle_proximal = middle_proximal_in;
                this.middle_intermediate = middle_intermediate_in;
                this.middle_distal = middle_distal_in;
                this.ring_proximal = ring_proximal_in;
                this.ring_intermediate = ring_intermediate_in;
                this.ring_distal = ring_distal_in;
                this.little_proximal = little_proximal_in;
                this.little_intermediate = little_intermediate_in;
                this.little_distal = little_distal_in;
                this.upper_leg = upper_leg_in;
                this.lower_leg = lower_leg_in;
                this.foot = foot_in;
                this.toe = toe_in;
            }
        }

        public struct ST_HUMANOID_BONES_OFFSET
        {
            public Vector3 hip_to_spine;
            public Vector3 spine_to_chest;
            public Vector3 chest_to_upper_chest;
            public Vector3 upper_chest_to_neck;
            public Vector3 head_to_right_eye;
            public Vector3 head_to_left_eye;
            public Vector3 upper_chest_to_right_shoulder;
            public Vector3 upper_chest_to_left_shoulder;
            public ST_HUMANOID_BONES_OFFSET(
                Vector3 hip_to_spine_in,
                Vector3 spine_to_chest_in,
                Vector3 chest_to_upper_chest_in,
                Vector3 upper_chest_to_neck_in,
                Vector3 head_to_right_eye_in,
                Vector3 head_to_left_eye_in,
                Vector3 upper_chest_to_right_shoulder_in,
                Vector3 upper_chest_to_left_shoulder_in
                )
            {
                this.hip_to_spine = hip_to_spine_in;
                this.spine_to_chest = spine_to_chest_in;
                this.chest_to_upper_chest = chest_to_upper_chest_in;
                this.upper_chest_to_neck = upper_chest_to_neck_in;
                this.head_to_right_eye = head_to_right_eye_in;
                this.head_to_left_eye = head_to_left_eye_in;
                this.upper_chest_to_right_shoulder = upper_chest_to_right_shoulder_in;
                this.upper_chest_to_left_shoulder = upper_chest_to_left_shoulder_in;
            }
        }

        public struct ST_WAIST_DOWN_ABSOLUTE_TRANSFORM
        {
            public Vector3 hip_position;
            public Quaternion hip_rotation;
            public Vector3 spine_position;
            public Quaternion spine_rotation;
            public Vector3 chest_position;
            public Quaternion chest_rotation;
            public Vector3 upper_chest_position;
            public Quaternion upper_chest_rotation;
            public Vector3 right_shoulder_position;
            public Quaternion right_shoulder_rotation;
            public Vector3 left_shoulder_position;
            public Quaternion left_shoulder_rotation;
            public ST_WAIST_DOWN_ABSOLUTE_TRANSFORM(
                Vector3 hip_position_in,
                Quaternion hip_rotation_in,
                Vector3 spine_position_in,
                Quaternion spine_rotation_in,
                Vector3 chest_position_in,
                Quaternion chest_rotation_in,
                Vector3 upper_chest_position_in,
                Quaternion upper_chest_rotation_in,
                Vector3 right_shoulder_position_in,
                Quaternion right_shoulder_rotation_in,
                Vector3 left_shoulder_position_in,
                Quaternion left_shoulder_rotation_in
                )
            {
                this.hip_position = hip_position_in;
                this.hip_rotation = hip_rotation_in;
                this.spine_position = spine_position_in;
                this.spine_rotation = spine_rotation_in;
                this.chest_position = chest_position_in;
                this.chest_rotation = chest_rotation_in;
                this.upper_chest_position = upper_chest_position_in;
                this.upper_chest_rotation = upper_chest_rotation_in;
                this.right_shoulder_position = right_shoulder_position_in;
                this.right_shoulder_rotation = right_shoulder_rotation_in;
                this.left_shoulder_position = left_shoulder_position_in;
                this.left_shoulder_rotation = left_shoulder_rotation_in;
            }
        }

        public struct ST_HUMANOID_ARM_ROTATION
        {
            public Quaternion upper_arm;
            public Quaternion lower_arm;
            public Quaternion hand;
            public Quaternion thumb_proximal;
            public Quaternion thumb_intermediate;
            public Quaternion thumb_distal;
            public Quaternion index_proximal;
            public Quaternion index_intermediate;
            public Quaternion index_distal;
            public Quaternion middle_proximal;
            public Quaternion middle_intermediate;
            public Quaternion middle_distal;
            public Quaternion ring_proximal;
            public Quaternion ring_intermediate;
            public Quaternion ring_distal;
            public Quaternion little_proximal;
            public Quaternion little_intermediate;
            public Quaternion little_distal;
            public ST_HUMANOID_ARM_ROTATION(
                Quaternion upper_arm_in,
                Quaternion lower_arm_in,
                Quaternion hand_in,
                Quaternion thumb_proximal_in,
                Quaternion thumb_intermediate_in,
                Quaternion thumb_distal_in,
                Quaternion index_proximal_in,
                Quaternion index_intermediate_in,
                Quaternion index_distal_in,
                Quaternion middle_proximal_in,
                Quaternion middle_intermediate_in,
                Quaternion middle_distal_in,
                Quaternion ring_proximal_in,
                Quaternion ring_intermediate_in,
                Quaternion ring_distal_in,
                Quaternion little_proximal_in,
                Quaternion little_intermediate_in,
                Quaternion little_distal_in
                )
            {
                this.upper_arm           = upper_arm_in;
                this.lower_arm           = lower_arm_in;
                this.hand                = hand_in;
                this.thumb_proximal      = thumb_proximal_in;
                this.thumb_intermediate  = thumb_intermediate_in;
                this.thumb_distal        = thumb_distal_in;
                this.index_proximal      = index_proximal_in;
                this.index_intermediate  = index_intermediate_in;
                this.index_distal        = index_distal_in;
                this.middle_proximal     = middle_proximal_in;
                this.middle_intermediate = middle_intermediate_in;
                this.middle_distal       = middle_distal_in;
                this.ring_proximal       = ring_proximal_in;
                this.ring_intermediate   = ring_intermediate_in;
                this.ring_distal         = ring_distal_in;
                this.little_proximal     = little_proximal_in;
                this.little_intermediate = little_intermediate_in;
                this.little_distal       = little_distal_in;
            }
        }

        public struct ST_HUMANOID_ARM_POSITION
        {
            public Vector3 upper_arm;
            public Vector3 lower_arm;
            public Vector3 hand;
            public Vector3 thumb_proximal;
            public Vector3 thumb_intermediate;
            public Vector3 thumb_distal;
            public Vector3 index_proximal;
            public Vector3 index_intermediate;
            public Vector3 index_distal;
            public Vector3 middle_proximal;
            public Vector3 middle_intermediate;
            public Vector3 middle_distal;
            public Vector3 ring_proximal;
            public Vector3 ring_intermediate;
            public Vector3 ring_distal;
            public Vector3 little_proximal;
            public Vector3 little_intermediate;
            public Vector3 little_distal;
            public ST_HUMANOID_ARM_POSITION(
                Vector3 upper_arm_in,
                Vector3 lower_arm_in,
                Vector3 hand_in,
                Vector3 thumb_proximal_in,
                Vector3 thumb_intermediate_in,
                Vector3 thumb_distal_in,
                Vector3 index_proximal_in,
                Vector3 index_intermediate_in,
                Vector3 index_distal_in,
                Vector3 middle_proximal_in,
                Vector3 middle_intermediate_in,
                Vector3 middle_distal_in,
                Vector3 ring_proximal_in,
                Vector3 ring_intermediate_in,
                Vector3 ring_distal_in,
                Vector3 little_proximal_in,
                Vector3 little_intermediate_in,
                Vector3 little_distal_in
                )
            {
                this.upper_arm = upper_arm_in;
                this.lower_arm = lower_arm_in;
                this.hand = hand_in;
                this.thumb_proximal = thumb_proximal_in;
                this.thumb_intermediate = thumb_intermediate_in;
                this.thumb_distal = thumb_distal_in;
                this.index_proximal = index_proximal_in;
                this.index_intermediate = index_intermediate_in;
                this.index_distal = index_distal_in;
                this.middle_proximal = middle_proximal_in;
                this.middle_intermediate = middle_intermediate_in;
                this.middle_distal = middle_distal_in;
                this.ring_proximal = ring_proximal_in;
                this.ring_intermediate = ring_intermediate_in;
                this.ring_distal = ring_distal_in;
                this.little_proximal = little_proximal_in;
                this.little_intermediate = little_intermediate_in;
                this.little_distal = little_distal_in;
            }
        }
    }
}
