/**
 * @file EyeController.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Control eye rotation.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

using UnityEngine;

namespace MaidRobotSimulator.MaidRobotCafe
{
    public class EyeController
    {
        private SystemStructure.HEAD_UNIT_MODE _head_unit_mode = SystemStructure.HEAD_UNIT_MODE.LOOK_FORWARD;

        private Quaternion _right_eye_rotation_input = Quaternion.identity;
        private Quaternion _left_eye_rotation_input = Quaternion.identity;
        private Quaternion _right_eye_rotation_output = Quaternion.identity;
        private Quaternion _left_eye_rotation_output = Quaternion.identity;

        private CommonRateManager<float> _right_eye_rotation_average_manager;
        private CommonRateManager<float> _left_eye_rotation_average_manager;
        public EyeController()
        {
            this._right_eye_rotation_average_manager =
                new CommonRateManager<float>(CommonParameter.EYE_FACE_TRACK_AVERAGE_BUFFER_LENGTH,
                this._quaternion_to_array(Quaternion.identity));
            this._left_eye_rotation_average_manager =
                new CommonRateManager<float>(CommonParameter.EYE_FACE_TRACK_AVERAGE_BUFFER_LENGTH,
                this._quaternion_to_array(Quaternion.identity));
        }

        /*********************************************************
         * Public functions
         *********************************************************/
        public void set_eye_rotation_input(MessageStructure.ST_MRS_EYE eye_reference)
        {
            float pitch_angle_right = CommonParameter.POSE_TO_EYE_ANGLE_PITCH_FACTOR
                * (eye_reference.right_z - CommonParameter.POSE_TO_EYE_ANGLE_CENTER_Z);
            float yaw_angle_right = CommonParameter.POSE_TO_EYE_ANGLE_YAW_FACTOR
                * (eye_reference.right_y - CommonParameter.POSE_TO_EYE_ANGLE_CENTER_Y);
            float pitch_angle_left = CommonParameter.POSE_TO_EYE_ANGLE_PITCH_FACTOR
                * (eye_reference.left_z - CommonParameter.POSE_TO_EYE_ANGLE_CENTER_Z);
            float yaw_angle_left = CommonParameter.POSE_TO_EYE_ANGLE_YAW_FACTOR
                * (eye_reference.left_y - CommonParameter.POSE_TO_EYE_ANGLE_CENTER_Y);

            this._right_eye_rotation_input = Quaternion.Euler(0.0f, pitch_angle_right, yaw_angle_right);
            this._left_eye_rotation_input = Quaternion.Euler(0.0f, pitch_angle_left, yaw_angle_left);
        }

        public void get_output_eye_rotation(
            ref Quaternion right_eye_rotation, ref Quaternion left_eye_rotation)
        {
            right_eye_rotation = this._right_eye_rotation_output;
            left_eye_rotation = this._left_eye_rotation_output;
        }

        public void set_head_unit_mode(SystemStructure.HEAD_UNIT_MODE head_unit_mode)
        {
            this._head_unit_mode = head_unit_mode;
        }

        public void calculate()
        {
            if (SystemStructure.HEAD_UNIT_MODE.FACE_TRACKING == this._head_unit_mode)
            {
                this._right_eye_rotation_average_manager.set_moving_average_value(
                    this._quaternion_to_array(this._right_eye_rotation_input));
                this._left_eye_rotation_average_manager.set_moving_average_value(
                    this._quaternion_to_array(this._left_eye_rotation_input));

                this._right_eye_rotation_output = this._array_to_quaternion(
                    this._right_eye_rotation_average_manager.get_moving_average_values());
                this._right_eye_rotation_output.Normalize();
                this._left_eye_rotation_output = this._array_to_quaternion(
                    this._left_eye_rotation_average_manager.get_moving_average_values());
                this._left_eye_rotation_output.Normalize();
            }
            else if (SystemStructure.HEAD_UNIT_MODE.LOOK_FORWARD == this._head_unit_mode)
            {
                this._right_eye_rotation_output = Quaternion.identity;
                this._left_eye_rotation_output = Quaternion.identity;
            }
            else
            {
                /* Do Nothing. */
            }
        }

        /*********************************************************
         * Private functions
         *********************************************************/
        float[] _quaternion_to_array(Quaternion q)
        {
            float[] array = new float[4];
            array[0] = q.x;
            array[1] = q.y;
            array[2] = q.z;
            array[3] = q.w;

            return array;
        }

        Quaternion _array_to_quaternion(float[] array)
        {
            Quaternion q = new Quaternion();
            q.x = array[0];
            q.y = array[1];
            q.z = array[2];
            q.w = array[3];

            return q;
        }

        /*********************************************************
        * Destructor
        *********************************************************/
        ~EyeController()
        {
        }
    }
}
