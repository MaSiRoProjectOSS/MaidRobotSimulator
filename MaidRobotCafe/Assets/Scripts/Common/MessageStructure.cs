/**
 * @file MessageStructure.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Define ROS message data structure to be independent from ROS library.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

using System;

namespace MaidRobotSimulator.MaidRobotCafe
{

    public class MessageStructure
    {
        public struct ST_LINEAR
        {
            public double x;
            public double y;
            public double z;

            public ST_LINEAR(double x_in, double y_in, double z_in)
            {
                this.x = x_in;
                this.y = y_in;
                this.z = z_in;
            }
        }

        public struct ST_ANGULAR
        {
            public double x;
            public double y;
            public double z;

            public ST_ANGULAR(double x_in, double y_in, double z_in)
            {
                this.x = x_in;
                this.y = y_in;
                this.z = z_in;
            }
        }

        public struct ST_GEOMETRY_MSGS_TWIST
        {
            public ST_LINEAR linear;
            public ST_ANGULAR angular;

            public ST_GEOMETRY_MSGS_TWIST(ST_LINEAR linear_in, ST_ANGULAR angular_in)
            {
                this.linear = linear_in;
                this.angular = angular_in;
            }
        }

        public struct ST_TIME
        {
            public ulong sec;
            public ulong nsec;

            public ST_TIME(ulong sec_in, ulong nsec_in)
            {
                this.sec = sec_in;
                this.nsec = nsec_in;
            }
        }

        public struct ST_STD_MSGS_HEADER
        {
            public ulong seq;
            public ST_TIME stamp;
            public string frame_id;

            public ST_STD_MSGS_HEADER(ulong seq_in, ST_TIME stamp_in, string frame_id_in)
            {
                this.seq = seq_in;
                this.stamp = stamp_in;
                this.frame_id = frame_id_in;
            }
        }

        public struct ST_POINT
        {
            public double x;
            public double y;
            public double z;

            public ST_POINT(double x_in, double y_in, double z_in)
            {
                this.x = x_in;
                this.y = y_in;
                this.z = z_in;
            }
        }

        public struct ST_QUATERNION
        {
            public double x;
            public double y;
            public double z;
            public double w;

            public ST_QUATERNION(double x_in, double y_in, double z_in, double w_in)
            {
                this.x = x_in;
                this.y = y_in;
                this.z = z_in;
                this.w = w_in;
            }
        }

        public struct ST_GEOMETRY_MSGS_POSE
        {
            public ST_POINT position;
            public ST_QUATERNION orientation;

            public ST_GEOMETRY_MSGS_POSE(ST_POINT position_in, ST_QUATERNION orientation_in)
            {
                this.position = position_in;
                this.orientation = orientation_in;
            }
        }

        public struct ST_GEOMETRY_MSGS_POSESTAMPED
        {
            public ST_STD_MSGS_HEADER header;
            public ST_GEOMETRY_MSGS_POSE pose;

            public ST_GEOMETRY_MSGS_POSESTAMPED(ST_STD_MSGS_HEADER header_in, ST_GEOMETRY_MSGS_POSE pose_in)
            {
                this.header = header_in;
                this.pose = pose_in;
            }
        }

        public struct ST_SENSOR_MSGS_IMAGE
        {
            public ST_STD_MSGS_HEADER header;
            public ulong height;
            public ulong width;
            public string encoding;
            public byte is_bigendian;
            public ulong step;
            public byte[] data;

            public ST_SENSOR_MSGS_IMAGE(ST_STD_MSGS_HEADER header_in,
                ulong height_in, ulong width_in, string encoding_in,
                byte is_bigendian_in, ulong step_in, byte[] data_in)
            {
                this.header = header_in;
                this.height = height_in;
                this.width = width_in;
                this.encoding = encoding_in;
                this.is_bigendian = is_bigendian_in;
                this.step = step_in;
                this.data = data_in;
            }
        }

        public struct ST_MRS_EYE
        {
            public static Int32 EMOTION_NORMAL = 0;
            public static Int32 EMOTION_CLOSE = 1;
            public static Int32 EMOTION_SMILE = 2;
            public static Int32 EMOTION_CLOSE_LEFT = 3;
            public static Int32 EMOTION_CLOSE_RIGHT = 4;
            public static Int32 EMOTION_WINK_LEFT = 5;
            public static Int32 EMOTION_WINK_RIGHT = 6;
            public static Int32 EMOTION_MIENS_KEEP_NORMAL = 7;

            public static Int32 EFFECT_CORNEA_NORMAL = 0;
            public static Int32 EFFECT_CORNEA_ORDER = 1;

            public Int32 emotions;
            public Int32 cornea_effect;
            public float dimensions;
            public float distance;
            public float left_y;
            public float left_z;
            public float right_y;
            public float right_z;

            public ST_MRS_EYE(Int32 emotions_in,
            Int32 cornea_effect_in,
            float dimensions_in,
            float distance_in,
            float left_y_in,
            float left_z_in,
            float right_y_in,
            float right_z_in)
            {
                this.emotions = emotions_in;
                this.cornea_effect = cornea_effect_in;
                this.dimensions = dimensions_in;
                this.distance = distance_in;
                this.left_y = left_y_in;
                this.left_z = left_z_in;
                this.right_y = right_y_in;
                this.right_z = right_z_in;
            }

            public static Int32 get_EMOTION_NORMAL()
            {
                return EMOTION_NORMAL;
            }

            public static Int32 get_EMOTION_CLOSE()
            {
                return EMOTION_CLOSE;
            }

            public static Int32 get_EMOTION_SMILE()
            {
                return EMOTION_SMILE;
            }

            public static Int32 get_EMOTION_CLOSE_LEFT()
            {
                return EMOTION_CLOSE_LEFT;
            }

            public static Int32 get_EMOTION_CLOSE_RIGHT()
            {
                return EMOTION_CLOSE_RIGHT;
            }

            public static Int32 get_EMOTION_WINK_LEFT()
            {
                return EMOTION_WINK_LEFT;
            }

            public static Int32 get_EMOTION_WINK_RIGHT()
            {
                return EMOTION_WINK_RIGHT;
            }

            public static Int32 get_EMOTION_MIENS_KEEP_NORMAL()
            {
                return EMOTION_MIENS_KEEP_NORMAL;
            }

            public static Int32 get_EFFECT_CORNEA_NORMAL()
            {
                return EFFECT_CORNEA_NORMAL;
            }

            public static Int32 get_EFFECT_CORNEA_ORDER()
            {
                return EFFECT_CORNEA_ORDER;
            }
        }

        public struct ST_MRS_LIP
        {
            public Int32 percent;

            public ST_MRS_LIP(Int32 percent_in)
            {
                this.percent = percent_in;
            }
        }

        public struct ST_MRS_NECK
        {
            public float x;
            public float y;
            public float z;
            public float w;

            public ST_MRS_NECK(
                float x_in,
                float y_in,
                float z_in,
                float w_in)
            {
                this.x = x_in;
                this.y = y_in;
                this.z = z_in;
                this.w = w_in;
            }
        }

        public struct ST_HEAD_STATUS
        {
            public bool human_detected;

            public ST_HEAD_STATUS(bool human_detected_in)
            {
                this.human_detected = human_detected_in;
            }
        }
    }
}
