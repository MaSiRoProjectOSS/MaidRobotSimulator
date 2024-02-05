/**
 * @file SimpleStepInverseKinematics.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Simple Step Inverse Kinematics algorithm.
 *
 * @copyright Copyright (c) MaSiRo Project. 2024-.
 *
 */

using System;
using UnityEngine;

namespace MaidRobotSimulator.MaidRobotCafe
{

    public class SimpleStepInverseKinematics
    {
        /*********************************************************
         * Constants
         *********************************************************/
        private enum LINK_TYPE
        {
            HORIZONTAL_MOVE_LINK,
            VERTICAL_MOVE_LINK
        }

        private const int _SPACE_DIMENSION_NUM = 3;

        public const float _HALF_CIRCLE_DEG = 180.0f;
        public const float _CIRCLE_DEG = 360.0f;

        private const float _LINK_MOVE_LENGTH_RATE = 0.1f;
        private const float _MOVE_CALCULATE_RATE = 0.01f;

        private const int _OUTER_LOOP_NUM_MAX = 10;
        private const int _INNER_LOOP_NUM_MAX = 50;
        private const float _INNER_LOOP_ONE_STEP_SIZE = 0.5f;

        private const float _SINGULAR_POINT_LIMIT_BASE = 1e-4f;
        
        private const float _INSIDE_STEP_FACTOR = 10.0f;

        private const float _ZERO_DIVIDE_LIMIT = 1e-10f;


        /*********************************************************
         * Private Variables
         *********************************************************/
        private RigidBodyTree _rigid_body_tree;
        private Vector3 _goal_position;

        private float _max_move_length;
        private float _min_calculate_length;
        private int _outer_loop_num;
        private int[] _inner_loop_nums;

        private float _distance_from_end_point_to_goal;
        private Vector3 _direction_from_end_point_to_goal;
        private float[] _outer_loop_each_step_length;

        private float _singular_point_limit;

        private LINK_TYPE[] _link_move_type;

        private float[] _rotated_distance;
        private Quaternion[] _rotation_before;
        private Quaternion[] _rotation_after;
        private float[] _rotated_angle_norm;
        private float[] _rotation_distance_rate;
        private float[,] _inner_step_vector_norms;

        /*********************************************************
         * Constructors
         *********************************************************/
        public SimpleStepInverseKinematics(RigidBodyTree rigid_body_tree)
        {
            this._rigid_body_tree = rigid_body_tree;
            this._goal_position = rigid_body_tree.get_end_point();

            float average_link_length = this._rigid_body_tree.get_average_link_length();
            this._max_move_length = average_link_length * _LINK_MOVE_LENGTH_RATE;
            this._min_calculate_length = this._max_move_length * _MOVE_CALCULATE_RATE;
            this._outer_loop_each_step_length = new float[_OUTER_LOOP_NUM_MAX];
            this._inner_loop_nums = new int[_OUTER_LOOP_NUM_MAX];

            this._singular_point_limit = _SINGULAR_POINT_LIMIT_BASE * average_link_length;

            this._rotated_distance = new float[_OUTER_LOOP_NUM_MAX];
            this._rotation_before = new Quaternion[this._rigid_body_tree.get_link_num()];
            this._rotation_after = new Quaternion[this._rigid_body_tree.get_link_num()];
            this._rotated_angle_norm = new float[_OUTER_LOOP_NUM_MAX];
            this._rotation_distance_rate = new float[_OUTER_LOOP_NUM_MAX];
            this._inner_step_vector_norms = new float[_OUTER_LOOP_NUM_MAX, _INNER_LOOP_NUM_MAX];


            this._link_move_type = new LINK_TYPE[this._rigid_body_tree.get_link_num()];
            for (int joint_index = 0; joint_index < this._rigid_body_tree.get_link_num(); joint_index++)
            {
                if(this._is_even_number(joint_index))
                {
                    this._link_move_type[joint_index] = LINK_TYPE.HORIZONTAL_MOVE_LINK;
                }
                else
                {
                    this._link_move_type[joint_index] = LINK_TYPE.VERTICAL_MOVE_LINK;
                }
            }

        }

        /*********************************************************
         * Public functions
         *********************************************************/
        public float[] get_rotated_distance()
        {
            return this._rotated_distance;
        }

        public float[] get_rotated_angle_norm()
        {
            return this._rotated_angle_norm;
        }

        public float[] get_rotation_distance_rate()
        {
            return this._rotation_distance_rate;
        }

        public RigidBodyTree get_rigid_body_tree()
        {
            return this._rigid_body_tree;
        }

        public int get_outer_loop_num()
        {
            return this._outer_loop_num;
        }

        public int[] get_inner_loop_nums()
        {
            return this._inner_loop_nums;
        }

        public float[,] get_inner_step_vector_norms()
        {
            return this._inner_step_vector_norms;
        }

        public void set_goal_position(Vector3 goal_position)
        {
            this._goal_position = goal_position;

            Vector3 distance_from_base_to_goal = goal_position - this._rigid_body_tree.get_base_position();

            float link_length_sum = 0;
            for (int i = 0; i < this._rigid_body_tree.get_link_num(); i++)
            {
                link_length_sum = link_length_sum + this._rigid_body_tree.get_link_length(i);
            }

            float length_from_base_to_goal = distance_from_base_to_goal.magnitude;
            if (length_from_base_to_goal > link_length_sum)
            {
                this._goal_position = this._rigid_body_tree.get_base_position()
                    + distance_from_base_to_goal / length_from_base_to_goal * link_length_sum;
            }
        }

        public void calculate()
        {
            this._calculate_loop_num_and_goal_vector();

            this._calculate_IK();
        }

        /*********************************************************
         * Private functions
         *********************************************************/
        private void _calculate_loop_num_and_goal_vector()
        {
            Vector3 end_point_to_goal_vector = this._goal_position - this._rigid_body_tree.get_end_point();
            this._distance_from_end_point_to_goal = end_point_to_goal_vector.magnitude;
            Array.Clear(this._inner_step_vector_norms, 0, this._inner_step_vector_norms.Length);

            if (this._distance_from_end_point_to_goal < this._min_calculate_length)
            {
                this._outer_loop_num = 0;
            }
            else
            {
                this._outer_loop_num = Mathf.CeilToInt(this._distance_from_end_point_to_goal / this._max_move_length);
                this._outer_loop_num = Mathf.Min(this._outer_loop_num, _OUTER_LOOP_NUM_MAX);

                this._direction_from_end_point_to_goal = end_point_to_goal_vector / this._distance_from_end_point_to_goal;

                Array.Clear(this._outer_loop_each_step_length, 0, this._outer_loop_each_step_length.Length);
                for (int i = 0; i < this._outer_loop_num; i++)
                {
                    if (i < this._outer_loop_num - 1)
                    {
                        this._outer_loop_each_step_length[i] = this._max_move_length;
                    }
                    else
                    {
                        this._outer_loop_each_step_length[i] =
                            this._distance_from_end_point_to_goal - this._max_move_length * i;
                    }
                }
            }
        }

        private void _calculate_IK()
        {
            Vector3 inner_goal_position = this._rigid_body_tree.get_end_point();
            Array.Clear(this._rotated_angle_norm, 0, this._rotated_angle_norm.Length);
            Array.Clear(this._rotation_distance_rate, 0, this._rotation_distance_rate.Length);

            for (int outer_index = 0; outer_index < this._outer_loop_num; outer_index++)
            {
                int inner_index = 0;

                inner_goal_position += this._outer_loop_each_step_length[outer_index] * this._direction_from_end_point_to_goal;

                float total_inner_step_length = 0.0f;

                for (int joint_index = 0; joint_index < this._rigid_body_tree.get_link_num(); joint_index++)
                {
                    this._rotation_before[joint_index] = this._rigid_body_tree.get_joint_rotation(joint_index);
                }

                float inner_step_vector_norm = 0.0f;

                for (inner_index = 0; inner_index < _INNER_LOOP_NUM_MAX; inner_index++)
                {
                    Vector3[] joints_position = this._rigid_body_tree.get_joints_position();

                    bool inside_flag = this._check_is_inside(joints_position, inner_goal_position);

                    Vector3 inner_step_vector =
                        (inner_goal_position - joints_position[joints_position.Length - 1])
                        * _INNER_LOOP_ONE_STEP_SIZE;
                    inner_step_vector_norm = inner_step_vector.magnitude;
                    this._inner_step_vector_norms[outer_index, inner_index] = inner_step_vector_norm;

                    if (inner_index == 0)
                    {
                        total_inner_step_length = inner_step_vector_norm;
                    }

                    if (inner_step_vector_norm < this._min_calculate_length)
                    {
                        break;
                    }

                    this._update_each_joint_rotation(
                        inner_step_vector, inside_flag, inner_goal_position);
                }

                this._inner_loop_nums[outer_index] = inner_index;

                this._calculate_rotation_distance_rate(
                    total_inner_step_length, inner_step_vector_norm, outer_index);

                if (this._rotation_distance_rate[outer_index] < this._singular_point_limit)
                {
                    for (int joint_index = 0; joint_index < this._rigid_body_tree.get_link_num(); joint_index++)
                    {
                        this._rigid_body_tree.set_joint_rotation(
                            joint_index, this._rotation_before[joint_index]);
                    }
                }
                else
                {
                    break; /* exit outer loop. */
                }
            }
        }

        private bool _check_is_inside(Vector3[] joints_position, Vector3 inner_goal_position)
        {
            bool inside_flag = false;

            Vector3 base_to_goal = inner_goal_position - this._rigid_body_tree.get_base_position();
            Vector3 base_to_point = joints_position[joints_position.Length - 1] - this._rigid_body_tree.get_base_position();

            if (Vector3.Dot(base_to_goal, base_to_goal) < Vector3.Dot(base_to_point, base_to_point))
            {
                inside_flag = true;
            }

            return inside_flag;
        }

        private void _update_each_joint_rotation(
            Vector3 inner_step_vector, bool inside_flag, Vector3 inner_goal_position)
        {
            for (int joint_index = 0; joint_index < this._rigid_body_tree.get_link_num(); joint_index++)
            {
                Vector3[] joints_position = this._rigid_body_tree.get_joints_position();

                if (joint_index > 1)
                {
                    inner_step_vector = (inner_goal_position - joints_position[joints_position.Length - 1])
                        * _INNER_LOOP_ONE_STEP_SIZE;
                }
                float inner_step_vector_norm = inner_step_vector.magnitude;

                Vector3 joint_to_point_vector = joints_position[joints_position.Length - 1] - joints_position[joint_index];

                /*  tune inner_step_vector */
                if (LINK_TYPE.VERTICAL_MOVE_LINK == this._link_move_type[joint_index])
                {
                    Vector3 base_to_point = joints_position[joints_position.Length - 1] - this._rigid_body_tree.get_base_position();

                    if (true == inside_flag)
                    {
                        base_to_point = -base_to_point;

                        inner_step_vector = (inner_step_vector.normalized + base_to_point.normalized)
                            * inner_step_vector_norm * _INSIDE_STEP_FACTOR;
                    }
                }

                float dif_angle = 0.0f;
                if (LINK_TYPE.HORIZONTAL_MOVE_LINK == this._link_move_type[joint_index])
                {
                    float cosine_factor = Vector3.Dot(
                        inner_step_vector.normalized, joint_to_point_vector.normalized);
                    dif_angle = Mathf.Atan2(
                        inner_step_vector.magnitude, joint_to_point_vector.magnitude)
                        * (1.0f - Mathf.Abs(cosine_factor));
                }
                else
                {
                    dif_angle = Mathf.Atan2(
                        inner_step_vector.magnitude, joint_to_point_vector.magnitude);
                }

                Vector3 step_rotation_vector = this._cross_with_avoiding_zero_divide(
                    joint_to_point_vector, inner_step_vector);

                Quaternion next_rotation = this._rotate_quaternion_with_axis_and_angle(
                    step_rotation_vector, dif_angle,
                    this._rigid_body_tree.get_joint_rotation(joint_index));

                next_rotation = this._check_and_correct_with_rotation_limit(
                    next_rotation, joint_index);

                this._rigid_body_tree.set_joint_rotation(joint_index, next_rotation);
            }
        }

        private void _calculate_rotation_distance_rate(
            float total_inner_step_length, float inner_step_vector_norm, int outer_index)
        {
            this._rotated_distance[outer_index] = total_inner_step_length - inner_step_vector_norm;

            this._rotated_angle_norm[outer_index] = 0.0f;
            for (int joint_index = 0; joint_index < this._rigid_body_tree.get_link_num(); joint_index++)
            {
                this._rotation_after[joint_index] = this._rigid_body_tree.get_joint_rotation(joint_index);

                Quaternion dif_q = Quaternion.Inverse(this._rotation_before[joint_index]) * this._rotation_after[joint_index];
                this._rotated_angle_norm[outer_index] += Mathf.Abs(Mathf.Rad2Deg * 2.0f * Mathf.Acos(dif_q.w));
            }

            this._rotation_distance_rate[outer_index] = this._rotated_distance[outer_index]
                / this._rotated_angle_norm[outer_index];
        }

        private Quaternion _check_and_correct_with_rotation_limit(
            Quaternion q, int joint_index)
        {
            Quaternion correct_q = Quaternion.identity;

            Vector3 euler_angles = q.eulerAngles;
            float roll = this._round_angle_range_in_pi(euler_angles.x);
            float pitch = this._round_angle_range_in_pi(euler_angles.y);
            float yaw = this._round_angle_range_in_pi(euler_angles.z);

            RigidBodyTree.Joint.Constraint joint_constraints = this._rigid_body_tree.get_joint_constraint(joint_index);

            bool need_for_correct = false;
            float correct_roll = 0.0f;
            if (roll < joint_constraints.roll_min)
            {
                correct_roll = joint_constraints.roll_min - roll;
                need_for_correct = true;
            }
            if (roll > joint_constraints.roll_max)
            {
                correct_roll = joint_constraints.roll_max - roll;
                need_for_correct = true;
            }

            float correct_pitch = 0.0f;
            if (pitch < joint_constraints.pitch_min)
            {
                correct_pitch = joint_constraints.pitch_min - pitch;
                need_for_correct = true;
            }
            if (pitch > joint_constraints.pitch_max)
            {
                correct_pitch = joint_constraints.pitch_max - pitch;
                need_for_correct = true;
            }

            float correct_yaw = 0.0f;
            if (yaw < joint_constraints.yaw_min)
            {
                correct_yaw = joint_constraints.yaw_min - yaw;
                need_for_correct = true;
            }
            if (yaw > joint_constraints.yaw_max)
            {
                correct_yaw = joint_constraints.yaw_max - yaw;
                need_for_correct = true;
            }

            if (true == need_for_correct)
            {
                float[] corrected_euler_angles = new float[_SPACE_DIMENSION_NUM] {
                    correct_roll, correct_pitch, correct_yaw };
                correct_q = q * this._euler_to_quaternion(corrected_euler_angles);
            }
            else
            {
                correct_q = q;
            }

            return correct_q;
        }

        private Quaternion _rotate_quaternion_with_axis_and_angle(
            Vector3 rotation_axis_vector, float angle_rad, Quaternion rotation)
        {
            Vector3 rotation_axis_vector_n = rotation_axis_vector.normalized;

            float q_dif_0 = Mathf.Cos(angle_rad / 2.0f);
            float q_dif_1 = rotation_axis_vector_n.x * Mathf.Sin(angle_rad / 2.0f);
            float q_dif_2 = rotation_axis_vector_n.y * Mathf.Sin(angle_rad / 2.0f);
            float q_dif_3 = rotation_axis_vector_n.z * Mathf.Sin(angle_rad / 2.0f);
            Quaternion q_dif = new Quaternion(q_dif_1, q_dif_2, q_dif_3, q_dif_0);

            Quaternion next_rotation = q_dif * rotation;

            return next_rotation;
        }

        private Quaternion _rotate_quaternion_with_angular_velocity_vector(
                       Vector3 angular_velocity_vector, Quaternion rotation)
        {
            Quaternion dif_rotation = Quaternion.identity;
            Quaternion next_rotation = Quaternion.identity;

            float q0 = rotation.w;
            float q1 = rotation.x;
            float q2 = rotation.y;
            float q3 = rotation.z;

            dif_rotation.w = 0.5f * (
                -q1 * angular_velocity_vector.x
                - q2 * angular_velocity_vector.y
                - q3 * angular_velocity_vector.z);
            dif_rotation.x = 0.5f * (
                q2 * angular_velocity_vector.z
                - q3 * angular_velocity_vector.y
                + q0 * angular_velocity_vector.x);
            dif_rotation.y = 0.5f * (
                -q1 * angular_velocity_vector.z
                + q3 * angular_velocity_vector.x
                + q0 * angular_velocity_vector.y);
            dif_rotation.z = 0.5f * (
                q1 * angular_velocity_vector.y
                - q2 * angular_velocity_vector.x
                + q0 * angular_velocity_vector.z);

            next_rotation.w = rotation.w + dif_rotation.w;
            next_rotation.x = rotation.x + dif_rotation.x;
            next_rotation.y = rotation.y + dif_rotation.y;
            next_rotation.z = rotation.z + dif_rotation.z;
            next_rotation.Normalize();

            return next_rotation;
        }

        private Quaternion _euler_to_quaternion(float[] euler_angles)
        {
            Quaternion q = new Quaternion();

            float wx = Mathf.Deg2Rad * euler_angles[0];
            float wy = Mathf.Deg2Rad * euler_angles[1];
            float wz = Mathf.Deg2Rad * euler_angles[2];
            float w_norm = Mathf.Sqrt(wx * wx + wy * wy + wz * wz);

            float q0 = Mathf.Cos(w_norm / 2.0f);
            float q1 = wx / w_norm * Mathf.Sin(w_norm / 2.0f);
            float q2 = wy / w_norm * Mathf.Sin(w_norm / 2.0f);
            float q3 = wz / w_norm * Mathf.Sin(w_norm / 2.0f);

            q.w = q0;
            q.x = q1;
            q.y = q2;
            q.z = q3;

            return q;
        }

        private bool _is_even_number(int number)
        {
            return (number % 2 == 0);
        }

        private float _round_angle_range_in_pi(float angle_deg)
        {
            if (angle_deg > _HALF_CIRCLE_DEG)
            {
                angle_deg -= _CIRCLE_DEG;
            }
            else if (angle_deg < -_HALF_CIRCLE_DEG)
            {
                angle_deg += _CIRCLE_DEG;
            }

            return angle_deg;
        }

        private Vector3 _cross_with_avoiding_zero_divide(Vector3 vector_a, Vector3 vector_b)
        {
            Vector3 cross_vector = Vector3.Cross(vector_a, vector_b);
            float cross_vector_norm = cross_vector.magnitude;

            if (cross_vector_norm < _ZERO_DIVIDE_LIMIT)
            {
                float sqrt_1_3 = Mathf.Sqrt(1.0f / 3.0f);
                cross_vector = new Vector3(sqrt_1_3, sqrt_1_3, sqrt_1_3);
            }

            return cross_vector;
        }

        /*********************************************************
         * Destructor
         *********************************************************/
        ~SimpleStepInverseKinematics()
        {

        }
    }
}
