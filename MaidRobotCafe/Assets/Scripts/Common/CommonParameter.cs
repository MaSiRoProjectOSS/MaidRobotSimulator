/**
 * @file CommonParameter.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Common Parameter for Maid Robot Simulator.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

using System;
using UnityEditor.PackageManager;
using UnityEngine;

namespace MaidRobotSimulator.MaidRobotCafe
{
    public class CommonParameter
    {
        public static string ROBOT_NAME = "Misen";
        public static string KEYBOARD_INPUT_NAME = "KeyboardInput";
        public static string CARRY_OBJECTS_NAME = "CarryObjects";
        public static string COMMUNICATOR_NAME = "Communicator";
        public static string ANIMATOR_SPEED_NAME = "Speed";
        public static string RIGHT_EYE_CAMERA_NAME = "RightEyeCamera";
        public static string LEFT_EYE_CAMERA_NAME = "LeftEyeCamera";
        public static string TRAY_NAME = "Tray";

        public static string MOTION_SPEED_NAME = "MotionSpeed";

        public static SystemStructure.ROBOT_MODE INITIAL_ROBOT_MODE = SystemStructure.ROBOT_MODE.CATERING;
        public static float ROBOT_MODE_WAIT_TIME = 0.5f;

        public static SystemStructure.COMMUNICATION_MODE ROBOT_COMMUNICATION_MODE = SystemStructure.COMMUNICATION_MODE.ROS;
        public static SystemStructure.SYNC_MODE ROBOT_SYNC_MODE = SystemStructure.SYNC_MODE.ASYNC;
        public static SystemStructure.CAMERA_MODE CAMERA_MODE_INIT = SystemStructure.CAMERA_MODE.FAR;
        public static float CAMERA_MODE_WAIT_TIME = 0.5f;

        public static SystemStructure.ROBOT_HAND_HOLDING_SIDE HAND_HOLDING_SIDE_MODE_INIT = 
            SystemStructure.ROBOT_HAND_HOLDING_SIDE.RIGHT;

        public static SystemStructure.ST_CARRY_OBJECTS_NAME_AND_ID[] CARRY_OBJECTS_GAME_OBJECT_AND_ID = {
            new SystemStructure.ST_CARRY_OBJECTS_NAME_AND_ID("TEACUP_1", SystemStructure.CARRY_OBJECTS.TEACUP_1)
        };

        public static float ROBOT_POSITION_ROTATION_SEND_INTERVAL = 0.1f;
        public static float HAND_POSITION_SEND_INTERVAL = 0.1f;
        public static float EYE_CAMERA_IMAGE_SEND_INTERVAL = 0.1f; /* 10Hz */
        public static float EYE_CAMERA_IMAGE_TAKE_INTERVAL = EYE_CAMERA_IMAGE_SEND_INTERVAL / 2.0f;

        public static bool CAMERA_DEBUG_FREEZE_CAMERA_FUNCTION = false;
        public static bool CAMERA_DEBUG_DO_NOT_FLIP = false;

        public static Vector3 POSITION_INIT = Vector3.zero;
        public static Quaternion ROTATION_INIT = Quaternion.identity;
        public static Vector3 EULER_INIT = Vector3.zero;
        public static Vector3 TRANSLATIONAL_VELOCITY_INIT = Vector3.zero;
        public static Vector3 ANGULAR_VELOCITY_INIT = Vector3.zero;

        public static float TRANSLATIONAL_VELOCITY_LIMIT_MAX = 1.0f;
        public static float TRANSLATIONAL_VELOCITY_LIMIT_MIN = -1.0f;
        public static float ANGULAR_VELOCITY_LIMIT_MAX = 1.0f;
        public static float ANGULAR_VELOCITY_LIMIT_MIN = -1.0f;

        public static Quaternion AXIS_REMOVE_OFFSET_Q = Quaternion.Euler(0, -90, 0);
        public static Quaternion AXIS_UNITY_TO_ROBOT = Quaternion.Euler(90, 0, 0);
        public static Quaternion AXIS_ROBOT_TO_UNITY = Quaternion.Euler(-90, 0, 0);
        public static Vector3 AXIS_RIGHT_TO_LEFT = new Vector3(1, -1, 1);
        public static Vector3 AXIS_LEFT_TO_RIGHT = new Vector3(1, -1, 1);
        public static Vector3 INVERSE_ROTATION = new Vector3(-1, -1, -1);

        /* motion speed */
        public static float WALKING_MOTION_SPEED = 1.0f;
        public static float WALKING_MOTION_MPS_TO_ANIMATION_FACTOR = 3.0f;
        public static float WALKING_ROTATION_SPEED = 1.0f;
        public static float HAND_MOTION_SPEED = 0.2f;
        public static float HAND_ROTATION_SPEED = 0.2f;

        /* neck control */
        public static float NECK_INTERPOLATION_PERIOD = 1.0f;
        public static float POSE_TO_NECK_ANGLE_CENTER = 0.5f;
        public static float POSE_TO_NECK_ANGLE_ROLL_FACTOR = 1.0f;
        public static float POSE_TO_NECK_ANGLE_PITCH_FACTOR = 50.0f;
        public static float POSE_TO_NECK_ANGLE_YAW_FACTOR = 50.0f;

        /* eye control */
        public static float POSE_TO_EYE_ANGLE_CENTER_Y = 0.5f;
        public static float POSE_TO_EYE_ANGLE_CENTER_Z = -0.5f;
        public static float POSE_TO_EYE_ANGLE_PITCH_FACTOR = 1.0f * Mathf.Rad2Deg;
        public static float POSE_TO_EYE_ANGLE_YAW_FACTOR = -1.0f * Mathf.Rad2Deg;

        /* initial root (hip) position */
        public static Vector3 HIP_POSITION_INIT =
            new Vector3(-0.01366045f, 0.002265918f, 0.8865866f);

        /* bone direction vector */
        public static Vector3 WAIST_BONE_DIRECTION_VECTOR = 
            new Vector3(0.0f, 0.0f, 1.0f);

        public static Vector3 RIGHT_SHOULDER_BONE_DIRECTION_VECTOR =
            new Vector3(0.0f, -1.0f, 0.0f);

        public static Vector3 LEFT_SHOULDER_BONE_DIRECTION_VECTOR =
            new Vector3(0.0f, 1.0f, 0.0f);


        /* bones length */
        public static SystemStructure.ST_HUMANOID_BONES_LENGTH ROBOT_BONES_LENGTH =
            new SystemStructure.ST_HUMANOID_BONES_LENGTH(
                0.05323f,    /* hips */
                0.11218f,    /* spine */
                0.10863f,    /* chest */
                0.13752f,    /* upper_chest */
                0.073743f,   /* neck */
                0.10213f,    /* head */
                0.051063f,   /* eye */
                0.087028f,   /* shoulder */
                0.21985f,    /* upper_arm */
                0.21469f,    /* lower_arm */
                0.042882f,   /* hand */
                0.046598f,   /* thumb_proximal */
                0.028663f,   /* thumb_intermediate */
                0.014332f,   /* thumb_distal */
                0.033269f,   /* index_proximal */
                0.020483f,   /* index_intermediate */
                0.010242f,   /* index_distal */
                0.037057f,   /* middle_proximal */
                0.022861f,   /* middle_intermediate */
                0.011431f,   /* middle_distal */
                0.034375f,   /* ring_proximal */
                0.019838f,   /* ring_intermediate */
                0.009919f,   /* ring_distal */
                0.031443f,   /* little_proximal */
                0.018122f,   /* little_intermediate */
                0.009061f,   /* little_distal */
                0.35298f,    /* upper_leg */
                0.41549f,    /* lower_leg */
                0.11103f,    /* foot */
                0.055517f    /* toe */
                );

        public static SystemStructure.ST_HUMANOID_BONES_OFFSET ROBOT_BONES_OFFSET =
            new SystemStructure.ST_HUMANOID_BONES_OFFSET(
                new Vector3(0.008009288f, -0.0001471706f, -0.001309633f),
                new Vector3(-0.009136208f, -0.0003208603f, 0.001889467f),
                new Vector3(0.003085306f, -0.0003066637f, 0.0003384352f),
                new Vector3(0.01285714f, -0.0003780869f, 0.004050851f),
                new Vector3(0.02606085f, -0.01608787f, -0.04014361f),
                new Vector3(0.02606267f, 0.01531522f, -0.04005444f),
                new Vector3(0.02011912f, -0.02269733f, -0.02294326f),
                new Vector3(0.02012179f, 0.0220933f, -0.02281606f)
                );

        /* Initial waist down rotation */
        public static Quaternion HIP_ROTATION_INIT = 
            _define_quaternion(0.99802f, 0.0f, 0.06289843f, 0.0f);
        public static Quaternion SPINE_ROTATION_INIT = 
            _define_quaternion(0.9999183f, 0.0f, 0.01279679f, 0.0f);
        public static Quaternion CHEST_ROTATION_INIT = 
            _define_quaternion(0.9906787f, 0.0f, -0.136221f, 0.0f);
        public static Quaternion UPPER_CHEST_ROTATION_INIT = 
            _define_quaternion(0.9941538f, 0.0f, -0.1079757f, 0.0f);
        public static Quaternion RIGHT_SHOULDER_ROTATION_INIT =
            _define_quaternion(0.9975039f, 0.0706143f, 0.0f, 0.0f);
        public static Quaternion LEFT_SHOULDER_ROTATION_INIT =
            _define_quaternion(0.9975039f, -0.07061425f, 0.0f, 0.0f);

        /* Initial neck, head, eye rotation */
        public static Quaternion NECK_ROTATION_INIT = 
            _define_quaternion(0.9739257f, 0.0f, 0.2268686f, 0.0f);
        public static Quaternion HEAD_ROTATION_INIT = 
            _define_quaternion(0.9985233f, 0.0f, -0.05433499f, 0.0f);
        public static Quaternion RIGHT_EYE_ROTATION_INIT = 
            _define_quaternion(0.982246f, 0.006212222f, 0.02037925f, -0.1863875f);
        public static Quaternion LEFT_EYE_ROTATION_INIT = 
            _define_quaternion(0.982246f, -0.006212222f, 0.02037925f, 0.1863875f);

        /* Initial arm rotation for catering mode */
        /* upper arm */
        public static Quaternion RIGHT_UPPER_ARM_CATERING_ROTATION_INIT = 
            _define_quaternion(0.8735355f, 0.4547339f, -0.1540279f, 0.08018184f);
        public static Quaternion LEFT_UPPER_ARM_CATERING_ROTATION_INIT =
            _define_quaternion(0.8735355f, -0.4547339f, -0.1540279f, -0.08018184f);
        /* lower arm */
        public static Quaternion RIGHT_LOWER_ARM_CATERING_ROTATION_INIT =
            _define_quaternion(0.7660447f, 0.0f, 0.0f, 0.6427881f);
        public static Quaternion LEFT_LOWER_ARM_CATERING_ROTATION_INIT =
            _define_quaternion(0.7660447f, 0.0f, 0.0f, -0.6427881f);
        /* hand */
        public static Quaternion RIGHT_HAND_CATERING_ROTATION_INIT =
            _define_quaternion(0.8923998f, -0.09904589f, -0.369644f, -0.2391178f);
        public static Quaternion LEFT_HAND_CATERING_ROTATION_INIT =
            _define_quaternion(0.8923998f, 0.09904589f, -0.369644f, 0.2391178f);
        /* finger thumb */
        public static Quaternion RIGHT_THUMB_PROXIMAL_CATERING_ROTATION_INIT =
            _define_quaternion(0.8001037f, 0.3314139f, 0.1913419f, 0.4619401f);
        public static Quaternion LEFT_THUMB_PROXIMAL_CATERING_ROTATION_INIT =
            _define_quaternion(0.8001037f, -0.3314139f, 0.1913419f, -0.4619401f);
        public static Quaternion RIGHT_THUMB_INTERMEDIATE_CATERING_ROTATION_INIT =
            _define_quaternion(0.8660254f, 0.0f, 0.0f, -0.5f);
        public static Quaternion LEFT_THUMB_INTERMEDIATE_CATERING_ROTATION_INIT = 
            _define_quaternion(0.8660254f, 0.0f, 0.0f, 0.5f);
        public static Quaternion RIGHT_THUMB_DISTAL_CATERING_ROTATION_INIT =
            _define_quaternion(0.8660254f, 0.0f, 0.0f, -0.5f);
        public static Quaternion LEFT_THUMB_DISTAL_CATERING_ROTATION_INIT =
            _define_quaternion(0.8660254f, 0.0f, 0.0f, 0.5f);
        /* finger index */
        public static Quaternion RIGHT_INDEX_PROXIMAL_CATERING_ROTATION_INIT =
            _define_quaternion(0.766045f, 0.6427881f, 0.0f, 0.0f);
        public static Quaternion LEFT_INDEX_PROXIMAL_CATERING_ROTATION_INIT =
            _define_quaternion(0.766045f, -0.6427881f, 0.0f, 0.0f);
        public static Quaternion RIGHT_INDEX_INTERMEDIATE_CATERING_ROTATION_INIT =
            _define_quaternion(0.766045f, 0.6427881f, 0.0f, 0.0f);
        public static Quaternion LEFT_INDEX_INTERMEDIATE_CATERING_ROTATION_INIT =
            _define_quaternion(0.766045f, -0.6427881f, 0.0f, 0.0f);
        public static Quaternion RIGHT_INDEX_DISTAL_CATERING_ROTATION_INIT =
            _define_quaternion(0.766045f, 0.6427881f, 0.0f, 0.0f);
        public static Quaternion LEFT_INDEX_DISTAL_CATERING_ROTATION_INIT =
            _define_quaternion(0.766045f, -0.6427881f, 0.0f, 0.0f);
        /* finger middle */
        public static Quaternion RIGHT_MIDDLE_PROXIMAL_CATERING_ROTATION_INIT =
            _define_quaternion(0.766045f, 0.6427881f, 0.0f, 0.0f);
        public static Quaternion LEFT_MIDDLE_PROXIMAL_CATERING_ROTATION_INIT =
            _define_quaternion(0.766045f, -0.6427881f, 0.0f, 0.0f);
        public static Quaternion RIGHT_MIDDLE_INTERMEDIATE_CATERING_ROTATION_INIT =
            _define_quaternion(0.766045f, 0.6427881f, 0.0f, 0.0f);
        public static Quaternion LEFT_MIDDLE_INTERMEDIATE_CATERING_ROTATION_INIT =
            _define_quaternion(0.766045f, -0.6427881f, 0.0f, 0.0f);
        public static Quaternion RIGHT_MIDDLE_DISTAL_CATERING_ROTATION_INIT =
            _define_quaternion(0.766045f, 0.6427881f, 0.0f, 0.0f);
        public static Quaternion LEFT_MIDDLE_DISTAL_CATERING_ROTATION_INIT =
            _define_quaternion(0.766045f, -0.6427881f, 0.0f, 0.0f);
        /* finger ring */
        public static Quaternion RIGHT_RING_PROXIMAL_CATERING_ROTATION_INIT =
            _define_quaternion(0.766045f, 0.6427881f, 0.0f, 0.0f);
        public static Quaternion LEFT_RING_PROXIMAL_CATERING_ROTATION_INIT =
            _define_quaternion(0.766045f, -0.6427881f, 0.0f, 0.0f);
        public static Quaternion RIGHT_RING_INTERMEDIATE_CATERING_ROTATION_INIT =
            _define_quaternion(0.766045f, 0.6427881f, 0.0f, 0.0f);
        public static Quaternion LEFT_RING_INTERMEDIATE_CATERING_ROTATION_INIT =
            _define_quaternion(0.766045f, -0.6427881f, 0.0f, 0.0f);
        public static Quaternion RIGHT_RING_DISTAL_CATERING_ROTATION_INIT =
            _define_quaternion(0.766045f, 0.6427881f, 0.0f, 0.0f);
        public static Quaternion LEFT_RING_DISTAL_CATERING_ROTATION_INIT =
            _define_quaternion(0.766045f, -0.6427881f, 0.0f, 0.0f);
        /* finger little */
        public static Quaternion RIGHT_LITTLE_PROXIMAL_CATERING_ROTATION_INIT =
            _define_quaternion(0.766045f, 0.6427881f, 0.0f, 0.0f);
        public static Quaternion LEFT_LITTLE_PROXIMAL_CATERING_ROTATION_INIT =
            _define_quaternion(0.766045f, -0.6427881f, 0.0f, 0.0f);
        public static Quaternion RIGHT_LITTLE_INTERMEDIATE_CATERING_ROTATION_INIT =
            _define_quaternion(0.766045f, 0.6427881f, 0.0f, 0.0f);
        public static Quaternion LEFT_LITTLE_INTERMEDIATE_CATERING_ROTATION_INIT =
            _define_quaternion(0.766045f, -0.6427881f, 0.0f, 0.0f);
        public static Quaternion RIGHT_LITTLE_DISTAL_CATERING_ROTATION_INIT =
            _define_quaternion(0.766045f, 0.6427881f, 0.0f, 0.0f);
        public static Quaternion LEFT_LITTLE_DISTAL_CATERING_ROTATION_INIT =
            _define_quaternion(0.766045f, -0.6427881f, 0.0f, 0.0f);

        /* Initial arm rotation for hand holing mode */
        /* upper arm */
        public static Quaternion RIGHT_UPPER_ARM_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.8191520f, 0.5735764f, 0.0f, 0.0f);
        public static Quaternion LEFT_UPPER_ARM_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.8191520f, -0.5735764f, 0.0f, 0.0f);
        /* lower arm */
        public static Quaternion RIGHT_LOWER_ARM_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(1.0f, 0.0f, 0.0f, 0.0f);
        public static Quaternion LEFT_LOWER_ARM_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(1.0f, 0.0f, 0.0f, 0.0f);
        /* hand */
        public static Quaternion RIGHT_HAND_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(1.0f, 0.0f, 0.0f, 0.0f);
        public static Quaternion LEFT_HAND_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(1.0f, 0.0f, 0.0f, 0.0f);
        /* finger thumb */
        public static Quaternion RIGHT_THUMB_PROXIMAL_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.8001037f, 0.3314139f, 0.1913419f, 0.4619401f);
        public static Quaternion LEFT_THUMB_PROXIMAL_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.8001037f, -0.3314139f, 0.1913419f, -0.4619401f);
        public static Quaternion RIGHT_THUMB_INTERMEDIATE_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.8660254f, 0.0f, 0.0f, -0.5f);
        public static Quaternion LEFT_THUMB_INTERMEDIATE_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.8660254f, 0.0f, 0.0f, 0.5f);
        public static Quaternion RIGHT_THUMB_DISTAL_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.8660254f, 0.0f, 0.0f, -0.5f);
        public static Quaternion LEFT_THUMB_DISTAL_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.8660254f, 0.0f, 0.0f, 0.5f);
        /* finger index */
        public static Quaternion RIGHT_INDEX_PROXIMAL_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.766045f, 0.6427881f, 0.0f, 0.0f);
        public static Quaternion LEFT_INDEX_PROXIMAL_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.766045f, -0.6427881f, 0.0f, 0.0f);
        public static Quaternion RIGHT_INDEX_INTERMEDIATE_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.766045f, 0.6427881f, 0.0f, 0.0f);
        public static Quaternion LEFT_INDEX_INTERMEDIATE_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.766045f, -0.6427881f, 0.0f, 0.0f);
        public static Quaternion RIGHT_INDEX_DISTAL_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.766045f, 0.6427881f, 0.0f, 0.0f);
        public static Quaternion LEFT_INDEX_DISTAL_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.766045f, -0.6427881f, 0.0f, 0.0f);
        /* finger middle */
        public static Quaternion RIGHT_MIDDLE_PROXIMAL_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.766045f, 0.6427881f, 0.0f, 0.0f);
        public static Quaternion LEFT_MIDDLE_PROXIMAL_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.766045f, -0.6427881f, 0.0f, 0.0f);
        public static Quaternion RIGHT_MIDDLE_INTERMEDIATE_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.766045f, 0.6427881f, 0.0f, 0.0f);
        public static Quaternion LEFT_MIDDLE_INTERMEDIATE_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.766045f, -0.6427881f, 0.0f, 0.0f);
        public static Quaternion RIGHT_MIDDLE_DISTAL_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.766045f, 0.6427881f, 0.0f, 0.0f);
        public static Quaternion LEFT_MIDDLE_DISTAL_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.766045f, -0.6427881f, 0.0f, 0.0f);
        /* finger ring */
        public static Quaternion RIGHT_RING_PROXIMAL_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.766045f, 0.6427881f, 0.0f, 0.0f);
        public static Quaternion LEFT_RING_PROXIMAL_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.766045f, -0.6427881f, 0.0f, 0.0f);
        public static Quaternion RIGHT_RING_INTERMEDIATE_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.766045f, 0.6427881f, 0.0f, 0.0f);
        public static Quaternion LEFT_RING_INTERMEDIATE_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.766045f, -0.6427881f, 0.0f, 0.0f);
        public static Quaternion RIGHT_RING_DISTAL_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.766045f, 0.6427881f, 0.0f, 0.0f);
        public static Quaternion LEFT_RING_DISTAL_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.766045f, -0.6427881f, 0.0f, 0.0f);
        /* finger little */
        public static Quaternion RIGHT_LITTLE_PROXIMAL_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.766045f, 0.6427881f, 0.0f, 0.0f);
        public static Quaternion LEFT_LITTLE_PROXIMAL_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.766045f, -0.6427881f, 0.0f, 0.0f);
        public static Quaternion RIGHT_LITTLE_INTERMEDIATE_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.766045f, 0.6427881f, 0.0f, 0.0f);
        public static Quaternion LEFT_LITTLE_INTERMEDIATE_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.766045f, -0.6427881f, 0.0f, 0.0f);
        public static Quaternion RIGHT_LITTLE_DISTAL_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.766045f, 0.6427881f, 0.0f, 0.0f);
        public static Quaternion LEFT_LITTLE_DISTAL_HAND_HOLDING_ROTATION_INIT =
            _define_quaternion(0.766045f, -0.6427881f, 0.0f, 0.0f);

        /* low level neck control */
        public static float NECK_YAW_RESET_LIMIT = 45.0f;
        public static float NECK_DELTA_POSE_LOW_LIMIT = 10.0f;

        public static float NECK_POSE_RESET_WAIT_TIME = 30.0f;
        public static float NECK_FACE_TRACK_START_WAIT_TIME = 2.0f;
        public static float NECK_CALCULATE_PERIOD = 5.0f;
        public static float NECK_RANDOM_LOOK_PERIOD = 6.0f;
        public static float NECK_NOD_PERIOD = 1.0f;
        public static float NECK_INCLINE_PERIOD = 3.5f;

        public static float NECK_SPEED_INITIAL_GAIN = 2.5f;
        public static float NECK_SPEED_FACE_TRACK_GAIN = 1.5f;
        public static float NECK_SPEED_RANDOM_MOVE_GAIN = 1.2f;

        public static float TARGET_LOOK_YAW_RANDOM_RANGE_MIN = -20.0f;
        public static float TARGET_LOOK_YAW_RANDOM_RANGE_MAX = 20.0f;
        public static float TARGET_LOOK_PITCH_RANDOM_RANGE_MIN = -8.0f;
        public static float TARGET_LOOK_PITCH_RANDOM_RANGE_MAX = 8.0f;
        public static float TARGET_LOOK_ROLL_RANDOM_RANGE_MIN = -2.0f;
        public static float TARGET_LOOK_ROLL_RANDOM_RANGE_MAX = 2.0f;

        public static float LOOK_SEND_TIME_RANDOM_RANGE_MIN = 3.0f;
        public static float LOOK_SEND_TIME_RANDOM_RANGE_MAX = 5.0f;

        public static float RANDOM_LOOK_YAW_MIN = -30.0f;
        public static float RANDOM_LOOK_YAW_MAX = 30.0f; 
        public static float RANDOM_LOOK_PITCH_MIN = -20.0f;
        public static float RANDOM_LOOK_PITCH_MAX = 20.0f; 
        public static float RANDOM_LOOK_ROLL_MIN = -10.0f;
        public static float RANDOM_LOOK_ROLL_MAX = 10.0f;

        public static float NECK_LIMIT_YAW_MIN = -50.0f;
        public static float NECK_LIMIT_YAW_MAX = 50.0f;
        public static float NECK_LIMIT_PITCH_MIN = -30.0f;
        public static float NECK_LIMIT_PITCH_MAX = 40.0f;
        public static float NECK_LIMIT_ROLL_MIN = -3.0f;
        public static float NECK_LIMIT_ROLL_MAX = 3.0f;
        public static float NECK_LIMIT_ROLL_OUTPUT_MIN = -30.0f;
        public static float NECK_LIMIT_ROLL_OUTPUT_MAX = 30.0f;

        public static float ROLL_PITCH_LIMIT = 50.0f;

        public static float FACE_TRACKING_ROLL_FACTOR = 3.0f;
        public static float NOD_TARGET_PITCH_FACTOR_1 = -40.0f;
        public static float NOD_TARGET_PITCH_FACTOR_2 = 2.0f;
        public static float INCLINE_TARGET_ROLL_ANGLE = 4.0f;

        public static int NECK_FACE_TRACK_AVERAGE_BUFFER_LENGTH = 100;

        /* low level eye control */
        public static int EYE_FACE_TRACK_AVERAGE_BUFFER_LENGTH = 100;

        public static SystemStructure.ST_CARRY_OBJECT_POSITION_AND_ID[] CARRY_OBJECT_POSITION_AND_ID = {
            new SystemStructure.ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(-4.56f, 0.89f, 3.13f), SystemStructure.CARRY_OBJECT_PLACE.ON_FLASKET),
            new SystemStructure.ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(3.2f, 0.59f, -3.8f), SystemStructure.CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_1),
            new SystemStructure.ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(3.2f, 0.59f, -2.8f), SystemStructure.CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_2),
            new SystemStructure.ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(1.8f, 0.59f, -3.8f), SystemStructure.CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_3),
            new SystemStructure.ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(1.8f, 0.59f, -2.8f), SystemStructure.CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_4),
            new SystemStructure.ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(-3.7f, 0.59f, -2.8f), SystemStructure.CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_5),
            new SystemStructure.ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(-3.7f, 0.59f, -3.9f), SystemStructure.CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_6),
            new SystemStructure.ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(-3.7f, 0.59f, -1.3f), SystemStructure.CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_7),
            new SystemStructure.ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(-3.7f, 0.59f, -0.3f), SystemStructure.CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_8),
            new SystemStructure.ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(-2.0f, 0.59f, 3.7f), SystemStructure.CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_9),
            new SystemStructure.ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(-1.0f, 0.59f, 3.7f), SystemStructure.CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_10),
            new SystemStructure.ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(0.5f, 0.59f, 3.7f), SystemStructure.CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_11),
            new SystemStructure.ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(1.5f, 0.59f, 3.7f), SystemStructure.CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_12),
            new SystemStructure.ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(3.0f, 0.59f, 3.7f), SystemStructure.CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_13),
            new SystemStructure.ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(4.0f, 0.59f, 3.7f), SystemStructure.CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_14)
        };

        public static SystemStructure.INPUT_MODE INITIAL_INPUT_MODE = SystemStructure.INPUT_MODE.COMMUNICATION;
        public static float KEYBOARD_MODE_WAIT_TIME = 0.5f;

        public static SystemStructure.OBJECTS_STATE INITIAL_OBJECTS_STATE = SystemStructure.OBJECTS_STATE.INACTIVATE;
        public static float OBJECTS_STATE_WAIT_TIME = 0.5f;

        public static SystemStructure.ROBOT_CARRYING_STATE INITIAL_ROBOT_CARRYING_STATE = SystemStructure.ROBOT_CARRYING_STATE.NONE;
        public static float MAX_PICK_AND_PLACE_DISTANCE = 1.0f;

        public static float QUARTER_CIRCLE_DEG = 90.0f;
        public static float HALF_CIRCLE_DEG = 180.0f;
        public static float CIRCLE_DEG = 360.0f;
        public static float TO_PERCENT = 100.0f;

        public static int MAX_TABLE_NUM = 7;
        public static int MAX_CHAIR_NUM = 14;

        public static int COLOR_ELEMENTS_NUM = 3;
        public static int COLOR_RESOLUTION = 255;
        public static string CAMERA_IMAGE_ENCODING = "rgb8";

        public static string FACE_NAME = "Face";
        public static string RIGHT_EYE_BLEND_SHAPE_NAME = "Fcl_EYE_Close_R";
        public static string LEFT_EYE_BLEND_SHAPE_NAME = "Fcl_EYE_Close_L";
        public static float EYE_BLINK_WEIGHT_MAX = 100.0f;
        public static float EYE_BLINK_WEIGHT_MIN = 0.0f;
        public static float EYE_BLINKING_TIME_INTERVAL = 0.5f;
        public static float EYE_RANDOM_BLINKING_THRESHOLD = 0.8f;
        public static float EYE_BLINK_TIMING_RANDOM_SEED_MAX = 1.0f;
        public static float EYE_BLINK_TIMING_RANDOM_SEED_MIN = 0.0f;
        public static float EYE_BLINK_FETCH_LENGTH = 2.0f * EYE_BLINK_WEIGHT_MAX;

        public static int ROBOT_UDP_RECEIVER_PORT = 50000;
        public static int ROBOT_UDP_SENDER_PORT = 50001;

        public static string UDP_LOCAL_HOST = "127.0.0.1";
        public static string UDP_MESSAGE_TERMINATOR = "\n";
        public static string UDP_MESSAGE_DELIMINATOR = ",";

        public static string ROS_NAMESPACE = "maid_robot_system";
        public static string MRS_CAST_NAME = "Misen";

        public static string ROS_SUB_NAMESPACE_HEAD = "/" + MRS_CAST_NAME + "/head_unit";
        public static string ROS_SUB_NAMESPACE_WAIST_DOWN = "/" + MRS_CAST_NAME + "/waist_down_unit";
        public static string ROS_SUB_NAMESPACE_CONTROLLER = "/controller";

        public static string INPUT_MOVE_VELOCITY_REFERENCE_NAME =
            ROS_NAMESPACE + ROS_SUB_NAMESPACE_WAIST_DOWN + ROS_SUB_NAMESPACE_CONTROLLER + "/move_velocity_reference";

        public static string ROS_SUB_NAMESPACE_LOGIC = "/logic";
        public static string INPUT_EYE_REFERENCE_NAME =
            ROS_NAMESPACE + ROS_SUB_NAMESPACE_HEAD + ROS_SUB_NAMESPACE_LOGIC + "/eye";
        public static string INPUT_NECK_REFERENCE_NAME =
            ROS_NAMESPACE + ROS_SUB_NAMESPACE_HEAD + ROS_SUB_NAMESPACE_LOGIC + "/neck";
        public static string INPUT_LIP_REFERENCE_NAME =
            ROS_NAMESPACE + ROS_SUB_NAMESPACE_HEAD + ROS_SUB_NAMESPACE_LOGIC + "/lip";
        public static string INPUT_HEAD_STATUS_NAME =
            ROS_NAMESPACE + ROS_SUB_NAMESPACE_HEAD + ROS_SUB_NAMESPACE_LOGIC + "/status";

        public static string OUTPUT_POSITION_ROTATION_NAME =
            ROS_NAMESPACE + ROS_SUB_NAMESPACE_WAIST_DOWN + ROS_SUB_NAMESPACE_CONTROLLER + "/robot_position_rotation";

        public static string ROS_SUB_NAMESPACE_ARM = "/" + MRS_CAST_NAME + "/arm_unit";
        public static string ROS_SUB_NAMESPACE_ARM_CONTROLLER = "/controller";
        public static string OUTPUT_HAND_POSITION_NAME =
            ROS_NAMESPACE + ROS_SUB_NAMESPACE_ARM + ROS_SUB_NAMESPACE_ARM_CONTROLLER + "/hand_position";

        public static string ROS_SUB_NAMESPACE_EYE_CAMERA_IMAGE = "/controller/topic_image/image";
        public static string OUTPUT_RIGHT_EYE_CAMERA_IMAGE_NAME =
            ROS_NAMESPACE + ROS_SUB_NAMESPACE_HEAD + ROS_SUB_NAMESPACE_EYE_CAMERA_IMAGE + "/right";
        public static string OUTPUT_LEFT_EYE_CAMERA_IMAGE_NAME =
            ROS_NAMESPACE + ROS_SUB_NAMESPACE_HEAD + ROS_SUB_NAMESPACE_EYE_CAMERA_IMAGE + "/left";

        public static string EYE_CAMERA_LOG_FOLDER = "Logs/Camera_log";
        public static string EYE_CAMERA_LOG_RIGHT_FILE_NAME = "right_eye_";
        public static string EYE_CAMERA_LOG_LEFT_FILE_NAME = "left_eye_";

        public static string ROBOT_ATTACHED_CAMERA_NAME = "Main Camera";
        public static string AREA_FIXED_CAMERA_NAME = "AreaCamera";
        public static Vector3 AREA_CAMERA_POSITION_FAR = new Vector3(6.0f, 10.0f, 0.0f);
        public static Vector3 AREA_CAMERA_EULER_FAR = new Vector3(60.0f, -90.0f, 0.0f);
        public static Vector3 AREA_CAMERA_POSITION_NEAR = new Vector3(1.0f, 2.0f, 0.0f);
        public static Vector3 AREA_CAMERA_EULER_NEAR = new Vector3(40.0f, -90.0f, 0.0f);
        public static Vector3 ATTACHED_CAMERA_POSITION_FAR = new Vector3(0.0f, 3.0f, 4.0f);
        public static Vector3 ATTACHED_CAMERA_EULER_FAR = new Vector3(40.0f, -180.0f, 0.0f);
        public static Vector3 ATTACHED_CAMERA_POSITION_NEAR = new Vector3(0.0f, 0.8f, 1.5f);
        public static Vector3 ATTACHED_CAMERA_EULER_NEAR = new Vector3(0.0f, -180.0f, 0.0f);

        /* Inverse Kenimatics */
        public static float RIGHT_UPPER_ARM_ROLL_ANGLE_MAX = 180.0f * Mathf.Deg2Rad;
        public static float RIGHT_UPPER_ARM_ROLL_ANGLE_MIN = -180.0f * Mathf.Deg2Rad;
        public static float RIGHT_UPPER_ARM_PITCH_ANGLE_MAX = 180.0f * Mathf.Deg2Rad;
        public static float RIGHT_UPPER_ARM_PITCH_ANGLE_MIN = -180.0f * Mathf.Deg2Rad;
        public static float RIGHT_UPPER_ARM_YAW_ANGLE_MAX = 0.1f * Mathf.Deg2Rad;
        public static float RIGHT_UPPER_ARM_YAW_ANGLE_MIN = -0.1f * Mathf.Deg2Rad;

        public static float RIGHT_LOWER_ARM_ROLL_ANGLE_MAX = 180.0f * Mathf.Deg2Rad;
        public static float RIGHT_LOWER_ARM_ROLL_ANGLE_MIN = -180.0f * Mathf.Deg2Rad;
        public static float RIGHT_LOWER_ARM_PITCH_ANGLE_MAX = 180.0f * Mathf.Deg2Rad;
        public static float RIGHT_LOWER_ARM_PITCH_ANGLE_MIN = -180.0f * Mathf.Deg2Rad;
        public static float RIGHT_LOWER_ARM_YAW_ANGLE_MAX = 0.1f * Mathf.Deg2Rad;
        public static float RIGHT_LOWER_ARM_YAW_ANGLE_MIN = -0.1f * Mathf.Deg2Rad;

        public static float LEFT_UPPER_ARM_ROLL_ANGLE_MAX = 180.0f * Mathf.Deg2Rad;
        public static float LEFT_UPPER_ARM_ROLL_ANGLE_MIN = -180.0f * Mathf.Deg2Rad;
        public static float LEFT_UPPER_ARM_PITCH_ANGLE_MAX = 180.0f * Mathf.Deg2Rad;
        public static float LEFT_UPPER_ARM_PITCH_ANGLE_MIN = -180.0f * Mathf.Deg2Rad;
        public static float LEFT_UPPER_ARM_YAW_ANGLE_MAX = 0.1f * Mathf.Deg2Rad;
        public static float LEFT_UPPER_ARM_YAW_ANGLE_MIN = -0.1f * Mathf.Deg2Rad;

        public static float LEFT_LOWER_ARM_ROLL_ANGLE_MAX = 180.0f * Mathf.Deg2Rad;
        public static float LEFT_LOWER_ARM_ROLL_ANGLE_MIN = -180.0f * Mathf.Deg2Rad;
        public static float LEFT_LOWER_ARM_PITCH_ANGLE_MAX = 180.0f * Mathf.Deg2Rad;
        public static float LEFT_LOWER_ARM_PITCH_ANGLE_MIN = -180.0f * Mathf.Deg2Rad;
        public static float LEFT_LOWER_ARM_YAW_ANGLE_MAX = 0.1f * Mathf.Deg2Rad;
        public static float LEFT_LOWER_ARM_YAW_ANGLE_MIN = -0.1f * Mathf.Deg2Rad;

        public static int TARGET_GAME_FRAME_RATE = 100;

        /* Debug parameters */
        public static int DEBUG_TEXT_FONT_SIZE = 20;
        private static int DEBUG_TEXT_START_POS = 40;

        public static Rect LEFT_SIDE_DEBUG_TEXT_POS = new Rect(10,
            DEBUG_TEXT_START_POS, 500, 40);
        public static Rect RIGHT_SIDE_DEBUG_TEXT_POS = new Rect(1000,
            DEBUG_TEXT_START_POS, 500, 40);
        public static int RIGHT_SIDE_DEBUG_TEXT_OFFSET = 150;

        public static float FPS_DEBUG_TEXT_LPF_WEIGHT = 0.01f;

        public static int IMAGE_PROCESS_SUCCESS_FLAG_BUFFER_LENGTH = 100;

        /*********************************************************
        * Private functions
        *********************************************************/
        private static Quaternion _define_quaternion(float q0, float q1, float q2, float q3)
        {
            Quaternion quaternion = new Quaternion(q1, q2, q3, q0);
            return quaternion;
        }
    }

}
