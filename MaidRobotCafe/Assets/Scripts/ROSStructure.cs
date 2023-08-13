/**
 * @file ROSStructure.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Define ROS data structure.
 * @version 1.0.0
 * @date 2023-08-05
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

using System;

namespace MaidRobotSimulator.MaidRobotCafe
{

    public class ROSStructure
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
            public ST_TIME time;
            public string frame_id;

            public ST_STD_MSGS_HEADER(ulong seq_in, ST_TIME time_in, string frame_id_in)
            {
                this.seq = seq_in;
                this.time = time_in;
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
    }

}
