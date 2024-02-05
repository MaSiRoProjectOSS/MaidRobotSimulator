/**
 * @file RigidBodyTree.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Model Rigid Body Tree.
 *
 * @copyright Copyright (c) MaSiRo Project. 2024-.
 *
 */

using UnityEngine;

namespace MaidRobotSimulator.MaidRobotCafe
{
    public class RigidBodyTree
    {
        /*********************************************************
         * Structs
         *********************************************************/
        public struct Link
        {
            public float length;

            public Link(float length_in)
            {
                this.length = length_in;
            }
        }

        public struct Joint
        {
            public Quaternion rotation;

            public struct Constraint
            {
                public float roll_min;
                public float roll_max;
                public float pitch_min;
                public float pitch_max;
                public float yaw_min;
                public float yaw_max;

                public Constraint(float roll_min_in, float roll_max_in, float pitch_min_in, float pitch_max_in, float yaw_min_in, float yaw_max_in)
                {
                    this.roll_min = roll_min_in;
                    this.roll_max = roll_max_in;
                    this.pitch_min = pitch_min_in;
                    this.pitch_max = pitch_max_in;
                    this.yaw_min = yaw_min_in;
                    this.yaw_max = yaw_max_in;
                }
            }

            public Constraint constraint;

            public Joint(Quaternion rotation_in, Constraint constraint_in)
            {
                this.rotation = rotation_in;
                this.constraint = constraint_in;
            }
        }

        /*********************************************************
         * Constants
         *********************************************************/
        private const float _LINK_DEFAULT_LENGTH = 1.0f; /* [m] */
        private static Quaternion _JOINT_DEFAULT_ROTATION = Quaternion.identity;
        private const float _ROLL_MIN_DEFAULT = -180.0f; /* [deg] */
        private const float _ROLL_MAX_DEFAULT = 180.0f;  /* [deg] */
        private const float _PITCH_MIN_DEFAULT = -90.0f; /* [deg] */
        private const float _PITCH_MAX_DEFAULT = 90.0f;  /* [deg] */
        private const float _YAW_MIN_DEFAULT = -180.0f;  /* [deg] */
        private const float _YAW_MAX_DEFAULT = 180.0f;   /* [deg] */

        private static Vector3 _BASE_POSITION_DEFAULT = new Vector3(0.0f, 0.0f, 0.0f); /* [m] */
        private static Vector3 _IDENTITY_VECTOR_DEFAULT = new Vector3(1.0f, 0.0f, 0.0f);  /* [m] */

        /*********************************************************
         * Private Variables
         *********************************************************/
        private int _link_num;
        private Vector3 _identity_vector;

        private Link[] _links;
        private Joint[] _joints;

        private Vector3 _base_position;

        /*********************************************************
         * Constructors
         *********************************************************/

        public RigidBodyTree(int link_num)
        {
            this._link_num = link_num;

            this._links = new Link[link_num];
            this._joints = new Joint[link_num];

            for (int i = 0; i < link_num; i++)
            {
                this._links[i] = new Link(_LINK_DEFAULT_LENGTH);

                this._joints[i] = new Joint(
                    _JOINT_DEFAULT_ROTATION,
                        new Joint.Constraint(
                            _ROLL_MIN_DEFAULT, _ROLL_MAX_DEFAULT,
                            _PITCH_MIN_DEFAULT, _PITCH_MAX_DEFAULT,
                            _YAW_MIN_DEFAULT, _YAW_MAX_DEFAULT));
            }

            this._base_position = _BASE_POSITION_DEFAULT;
            this._identity_vector = _IDENTITY_VECTOR_DEFAULT;
        }

        /*********************************************************
         * Public functions
         *********************************************************/
        public int get_link_num()
        {
            return this._link_num;
        }

        public Quaternion get_joint_rotation(int joint_index)
        {
            return this._joints[joint_index].rotation;
        }

        public Vector3[] get_joints_position()
        {
            Vector3[] joints_position = new Vector3[this._link_num + 1];
            joints_position[0] = this._base_position;
            Vector3 link_vector = this._identity_vector;

            for (int i = 1; i < joints_position.Length; i++)
            {
                link_vector = this._joints[i - 1].rotation * link_vector;

                joints_position[i] = joints_position[i - 1]
                    + link_vector * this._links[i - 1].length;
            }

            return joints_position;
        }

        public Vector3 get_end_point()
        {
            Vector3[] joints_position = this.get_joints_position();
            return joints_position[joints_position.Length - 1];
        }

        public float get_link_length(int link_index)
        {
            return this._links[link_index].length;
        }

        public float get_average_link_length()
        {
            float average_link_length = 0.0f;

            for (int i = 0; i < this._link_num; i++)
            {
                average_link_length += this.get_link_length(i);
            }

            average_link_length /= this._link_num;

            return average_link_length;
        }

        public Vector3 get_base_position()
        {
            return this._base_position;
        }

        public Joint.Constraint get_joint_constraint(int joint_index)
        {
            return this._joints[joint_index].constraint;
        }

        public void set_link_length(int link_index, float length)
        {
            this._links[link_index].length = length;
        }

        public void set_joint_rotation(int joint_index, Quaternion rotation)
        {
            this._joints[joint_index].rotation = rotation;
        }

        public void set_joint_constraint(int joint_index,
            float roll_min, float roll_max,
            float pitch_min, float pitch_max,
            float yaw_min, float yaw_max)
        {
            this._joints[joint_index].constraint.roll_min = roll_min;
            this._joints[joint_index].constraint.roll_max = roll_max;
            this._joints[joint_index].constraint.pitch_min = pitch_min;
            this._joints[joint_index].constraint.pitch_max = pitch_max;
            this._joints[joint_index].constraint.yaw_min = yaw_min;
            this._joints[joint_index].constraint.yaw_max = yaw_max;
        }

        public void set_base_position(Vector3 base_position)
        {
            this._base_position = base_position;
        }

        public void set_identity_vector(Vector3 vector)
        {
            this._identity_vector = vector;
        }

        /*********************************************************
         * Destructor
         *********************************************************/
        ~RigidBodyTree()
        {
            
        }
    }
}
