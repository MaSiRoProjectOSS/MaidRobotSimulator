/**
 * @file CommonParameter.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Common Parameter for Maid Robot Simulator.
 * @version 1.0.0
 * @date 2023-08-05
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

using System;
using UnityEngine;

namespace MaidRobotSimulator.MaidRobotCafe
{
    public class CommonParameter
    {
        public static int SPACE_DIMENSION_NUM = 3;
        public static int QUATERNION_DIMENSTION_NUM = 4;

        public enum DIRECTION
        {
            FORWARD,
            BACKWARD,
            RIGHT,
            LEFT
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
        public enum CARRY_OBJECTS
        {
            TEACUP_1
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
                this.position = new float[CommonParameter.SPACE_DIMENSION_NUM];
                this.rotation = new float[CommonParameter.SPACE_DIMENSION_NUM];
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

        public static ST_CARRY_OBJECTS_NAME_AND_ID[] CARRY_OBJECTS_GAME_OBJECT_AND_ID = {
            new ST_CARRY_OBJECTS_NAME_AND_ID("TEACUP_1", CARRY_OBJECTS.TEACUP_1)
        };

        public static ST_CARRY_OBJECT_POSITION_AND_ID[] CARRY_OBJECT_POSITION_AND_ID = {
            new ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(-4.56f, 0.89f, 3.13f), CARRY_OBJECT_PLACE.ON_FLASKET),
            new ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(3.2f, 0.59f, -3.8f), CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_1),
            new ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(3.2f, 0.59f, -2.8f), CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_2),
            new ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(1.8f, 0.59f, -3.8f), CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_3),
            new ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(1.8f, 0.59f, -2.8f), CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_4),
            new ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(-3.7f, 0.59f, -2.8f), CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_5),
            new ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(-3.7f, 0.59f, -3.9f), CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_6),
            new ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(-3.7f, 0.59f, -1.3f), CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_7),
            new ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(-3.7f, 0.59f, -0.3f), CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_8),
            new ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(-2.0f, 0.59f, 3.7f), CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_9),
            new ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(-1.0f, 0.59f, 3.7f), CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_10),
            new ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(0.5f, 0.59f, 3.7f), CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_11),
            new ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(1.5f, 0.59f, 3.7f), CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_12),
            new ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(3.0f, 0.59f, 3.7f), CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_13),
            new ST_CARRY_OBJECT_POSITION_AND_ID(new Vector3(4.0f, 0.59f, 3.7f), CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_14)
        };

        public static int DEBUG_TEXT_FONT_SIZE = 20;
        private static int DEBUG_TEXT_START_POS = 40;
        private static int DEBUG_TEXT_ONE_LINE = 24;
        private static int DEBUG_TEXT_SPACE = 20;

        public static Rect KEYBOARD_RECEIVER_DEBUG_TEXT_POS = new Rect(10,
            DEBUG_TEXT_START_POS,
            500, 40);
        public static Rect THIRD_PERSON_CONTROLLER_DEBUG_TEXT_POS = new Rect(10,
            KEYBOARD_RECEIVER_DEBUG_TEXT_POS.y + DEBUG_TEXT_ONE_LINE + DEBUG_TEXT_SPACE,
            500, 40);
        public static Rect CARRY_OBJECT_CONTROLLER_DEBUG_TEXT_POS = new Rect(10,
            THIRD_PERSON_CONTROLLER_DEBUG_TEXT_POS.y + DEBUG_TEXT_ONE_LINE * 8 + DEBUG_TEXT_SPACE,
            500, 40);
        public static Rect COMMUNICATOR_DEBUG_TEXT_POS = new Rect(10,
            CARRY_OBJECT_CONTROLLER_DEBUG_TEXT_POS.y + DEBUG_TEXT_ONE_LINE + DEBUG_TEXT_SPACE,
            500, 40);

        public static string ROBOT_NAME = "Misen";
        public static COMMUNICATION_MODE ROBOT_COMMUNICATION_MODE = COMMUNICATION_MODE.ROS;

        public static SYNC_MODE ROBOT_SYNC_MODE = SYNC_MODE.ASYNC;

        public static Vector3 VELOCITY_INIT = new Vector3(0.0f, 0.0f, 0.0f);
        public static Vector3 ANGULAR_VELOCITY_INIT = new Vector3(0.0f, 0.0f, 0.0f);
        public static Quaternion AXIS_REMOVE_OFFSET_Q = Quaternion.Euler(0, -90, 0);
        public static Quaternion AXIS_UNITY_TO_VEHICLE = Quaternion.Euler(90, 0, 0);
        public static Vector3 AXIS_RIGHT_TO_LEFT = new Vector3(1, -1, 1);
        public static Vector3 INVERSE_ROTATION = new Vector3(-1, -1, -1);

        public static float WALKING_MOTION_SPEED = 1.0f;
        public static float WALKING_MOTION_MPS_TO_ANIMATION_FACTOR = 3.0f;
        public static float WALKING_ROTATION_SPEED = 1.0f;

        public static INPUT_MODE INITIAL_INPUT_MODE = INPUT_MODE.COMMUNICATION;
        public static float KEYBOARD_MODE_WAIT_TIME = 0.5f;

        public static OBJECTS_STATE INITIAL_OBJECTS_STATE = OBJECTS_STATE.INACTIVATE;
        public static float OBJECTS_STATE_WAIT_TIME = 0.5f;

        public static ROBOT_CARRYING_STATE INITIAL_ROBOT_CARRYING_STATE = ROBOT_CARRYING_STATE.NONE;
        public static float MAX_PICK_AND_PLACE_DISTANCE = 1.0f;

        public static float QUARTER_CIRCLE_DEG = 90.0f;
        public static float HALF_CIRCLE_DEG = 180.0f;
        public static float CIRCLE_DEG = 360.0f;

        public static int MAX_TABLE_NUM = 7;
        public static int MAX_CHAIR_NUM = 14;

        public static float SEND_INTERVAL = 0.5f;

        public static int ROBOT_UDP_RECEIVER_PORT = 50000;
        public static int ROBOT_UDP_SENDER_PORT = 50001;

        public static string UDP_LOCAL_HOST = "127.0.0.1";
        public static string UDP_MESSAGE_TERMINATOR = "\n";
        public static string UDP_MESSAGE_DELIMINATOR = ",";

        public static string UDP_REFERENCE_NAME = "move_velocity_reference";
        public static string UDP_INFORMATION_NAME = "robot_position_orientation";
        public static string ROS_REFERENCE_NAME = UDP_REFERENCE_NAME;
        public static string ROS_INFORMATION_NAME = UDP_INFORMATION_NAME;
    }

}
