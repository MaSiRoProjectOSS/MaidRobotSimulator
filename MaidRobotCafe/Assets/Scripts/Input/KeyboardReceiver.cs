/**
 * @file KeyboardReceiver.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Receive keyboard input.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

using UnityEngine;

namespace MaidRobotSimulator.MaidRobotCafe
{
    public class KeyboardReceiver
    {
        /*********************************************************
         * Private variables
         *********************************************************/
        private SystemStructure.ST_8_DIRECTION_MOVE _WASD_direction =
            new SystemStructure.ST_8_DIRECTION_MOVE(
                false, false, false, false, false, false, false, false); /*!< move robot direction */

        private SystemStructure.ST_8_DIRECTION_MOVE _move_ArrowKey_direction =
            new SystemStructure.ST_8_DIRECTION_MOVE(
                false, false, false, false, false, false, false, false); /*!< move robot direction */

        private bool _place_on_robot_tray_flag = false;    /*!< place on robot tray flag */
        private bool _place_on_nearest_table_flag = false; /*!< place on nearest table flag */

        private bool _robot_mode_switch_flag = false;
        private bool _camera_switch_flag = false;
        private bool _input_mode_change_flag = false;

        /*********************************************************
        * Constructor
        *********************************************************/
        public KeyboardReceiver()
        {
        }

        /*********************************************************
         * Public functions
         *********************************************************/
        public void get_robot_direction_input(ref SystemStructure.ST_8_DIRECTION_MOVE direction_input)
        {
            direction_input.forward = this._WASD_direction.forward;
            direction_input.backward = this._WASD_direction.backward;
            direction_input.right = this._WASD_direction.right;
            direction_input.left = this._WASD_direction.left;
            direction_input.up = this._WASD_direction.up;
            direction_input.down = this._WASD_direction.down;
            direction_input.turn_right = this._WASD_direction.turn_right;
            direction_input.turn_left = this._WASD_direction.turn_left;
        }

        public void get_hand_direction_input(ref SystemStructure.ST_8_DIRECTION_MOVE direction_input)
        {
            direction_input.forward = this._move_ArrowKey_direction.forward;
            direction_input.backward = this._move_ArrowKey_direction.backward;
            direction_input.right = this._move_ArrowKey_direction.right;
            direction_input.left = this._move_ArrowKey_direction.left;
            direction_input.up = this._move_ArrowKey_direction.up;
            direction_input.down = this._move_ArrowKey_direction.down;
            direction_input.turn_right = this._move_ArrowKey_direction.turn_right;
            direction_input.turn_left = this._move_ArrowKey_direction.turn_left;
        }

        public bool get_camera_switch_flag()
        {
            return this._camera_switch_flag;
        }

        public bool get_input_mode_change_flag()
        {
            return this._input_mode_change_flag;
        }

        public bool get_robot_mode_switch_flag()
        {
            return this._robot_mode_switch_flag;
        }

        public bool get_place_on_robot_tray_flag()
        {
            return this._place_on_robot_tray_flag;
        }

        public bool get_place_on_nearest_table_flag()
        {
            return this._place_on_nearest_table_flag;
        }

        public void update_keyboard_status()
        {
            /* Switch robot mode */
            this._robot_mode_switch_flag = Input.GetKey(KeyCode.T);

            /* Move robot */
            this._WASD_direction.forward = Input.GetKey(KeyCode.W);
            this._WASD_direction.backward = Input.GetKey(KeyCode.S);
            this._WASD_direction.right = Input.GetKey(KeyCode.D);
            this._WASD_direction.left = Input.GetKey(KeyCode.A);

            /* Switch camera */
            this._camera_switch_flag = Input.GetKey(KeyCode.F1);

            /* Input mode */
            this._input_mode_change_flag = Input.GetKey(KeyCode.Space);

            /* Pick and place object */
            this._place_on_robot_tray_flag = Input.GetKey(KeyCode.O);
            this._place_on_nearest_table_flag = Input.GetKey(KeyCode.P);

            /* Move Hand */
            this._get_hand_direction();
        }

        /*********************************************************
         * Private functions
         *********************************************************/
        private void _get_hand_direction()
        {
            if (true == Input.GetKey(KeyCode.LeftShift) || true == Input.GetKey(KeyCode.RightShift))
            {
                this._move_ArrowKey_direction.forward = false;
                this._move_ArrowKey_direction.backward = false;
                this._move_ArrowKey_direction.right = false;
                this._move_ArrowKey_direction.left = false;
                this._move_ArrowKey_direction.up = Input.GetKey(KeyCode.UpArrow);
                this._move_ArrowKey_direction.down = Input.GetKey(KeyCode.DownArrow);
                this._move_ArrowKey_direction.turn_right = Input.GetKey(KeyCode.RightArrow);
                this._move_ArrowKey_direction.turn_left = Input.GetKey(KeyCode.LeftArrow);
            }
            else
            {
                this._move_ArrowKey_direction.forward = Input.GetKey(KeyCode.UpArrow);
                this._move_ArrowKey_direction.backward = Input.GetKey(KeyCode.DownArrow);
                this._move_ArrowKey_direction.right = Input.GetKey(KeyCode.RightArrow);
                this._move_ArrowKey_direction.left = Input.GetKey(KeyCode.LeftArrow);
                this._move_ArrowKey_direction.up = false;
                this._move_ArrowKey_direction.down = false;
                this._move_ArrowKey_direction.turn_right = false;
                this._move_ArrowKey_direction.turn_left = false;
            }
        }

        ~KeyboardReceiver()
        {
        }
    }
}
