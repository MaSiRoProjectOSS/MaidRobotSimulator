/**
 * @file GamepadReceiver.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Receive gamepad input.
 *
 * @copyright Copyright (c) MaSiRo Project. 2024-.
 *
 */

using UnityEngine;
using UnityEngine.InputSystem;

namespace MaidRobotSimulator.MaidRobotCafe
{
    public class GamepadReceiver
    {
        /*********************************************************
         * Private variables
         *********************************************************/
        private bool _gamepad_exist_flag = false; /*!< gamepad exist flag */

        private bool _start_button_flag = false; /*!< start button flag */
        private bool _select_button_flag = false; /*!< select button flag */

        private bool _player_hand_holding_flag = false; /*!< player hand holding flag */

        private SystemStructure.ST_FIRST_PERSON_DIRECTION_VELOCITY _move_player_velocity =
            new SystemStructure.ST_FIRST_PERSON_DIRECTION_VELOCITY(
                           0.0f, 0.0f, 0.0f, 0.0f, 0.0f); /*!< move player velocity */

        private SystemStructure.ST_FIRST_PERSON_DIRECTION_VELOCITY _move_hand_velocity =
            new SystemStructure.ST_FIRST_PERSON_DIRECTION_VELOCITY(
                           0.0f, 0.0f, 0.0f, 0.0f, 0.0f); /*!< move hand velocity */

        private bool _right_shoulder_button_flag = false; /*!< right shoulder button pressed flag */
        private bool _left_shoulder_button_flag = false; /*!< left shoulder button pressed flag */

        /*********************************************************
         * Constructor
         *********************************************************/
        public GamepadReceiver()
        {
            if (Gamepad.current != null)
            {
                this._gamepad_exist_flag = true;
            }
        }

        /*********************************************************
         * Public functions
         *********************************************************/
        public bool get_start_button_flag()
        {
            return this._start_button_flag;
        }

        public bool get_select_button_flag()
        {
            return this._select_button_flag;
        }

        public bool get_player_hand_holding_flag()
        {
            return this._player_hand_holding_flag;
        }

        public void get_move_player_velocity(ref SystemStructure.ST_FIRST_PERSON_DIRECTION_VELOCITY velocity)
        {
            velocity.forward_backward = this._move_player_velocity.forward_backward;
            velocity.left_right = this._move_player_velocity.left_right;
            velocity.look_up_down = this._move_player_velocity.look_up_down;
            velocity.turn_left_right = this._move_player_velocity.turn_left_right;
            velocity.up_down = this._move_player_velocity.up_down;
        }

        public void get_move_hand_velocity(ref SystemStructure.ST_FIRST_PERSON_DIRECTION_VELOCITY velocity)
        {
            velocity.forward_backward = this._move_hand_velocity.forward_backward;
            velocity.left_right = this._move_hand_velocity.left_right;
            velocity.look_up_down = this._move_hand_velocity.look_up_down;
            velocity.turn_left_right = this._move_hand_velocity.turn_left_right;
            velocity.up_down = this._move_hand_velocity.up_down;
        }

        public void update_gamepad_status()
        {
            if (true == this._gamepad_exist_flag)
            {
                this._get_gamepad_inputs();
            }
        }

        /*********************************************************
         * Private functions
         *********************************************************/
        private void _get_gamepad_inputs()
        {
            float left_stick_up = Gamepad.current.leftStick.up.ReadValue();
            float left_stick_down = Gamepad.current.leftStick.down.ReadValue();
            float left_stick_right = Gamepad.current.leftStick.right.ReadValue();
            float left_stick_left = Gamepad.current.leftStick.left.ReadValue();

            float right_stick_up = Gamepad.current.rightStick.up.ReadValue();
            float right_stick_down = Gamepad.current.rightStick.down.ReadValue();
            float right_stick_right = Gamepad.current.rightStick.right.ReadValue();
            float right_stick_left = Gamepad.current.rightStick.left.ReadValue();

            if (left_stick_up > CommonParameter.MIN_GAMEPAD_STICK_VALUE)
            {
                this._move_player_velocity.forward_backward = left_stick_up;
            }
            else if (left_stick_down > CommonParameter.MIN_GAMEPAD_STICK_VALUE)
            {
                this._move_player_velocity.forward_backward = -left_stick_down;
            }
            else
            {
                this._move_player_velocity.forward_backward = 0.0f;
            }

            if (left_stick_right > CommonParameter.MIN_GAMEPAD_STICK_VALUE)
            {
                this._move_player_velocity.left_right = -left_stick_right;
            }
            else if (left_stick_left > CommonParameter.MIN_GAMEPAD_STICK_VALUE)
            {
                this._move_player_velocity.left_right = left_stick_left;
            }
            else
            {
                this._move_player_velocity.left_right = 0.0f;
            }

            if (right_stick_up > CommonParameter.MIN_GAMEPAD_STICK_VALUE)
            {
                this._move_player_velocity.look_up_down = -right_stick_up;
            }
            else if (right_stick_down > CommonParameter.MIN_GAMEPAD_STICK_VALUE)
            {
                this._move_player_velocity.look_up_down = right_stick_down;
            }
            else
            {
                this._move_player_velocity.look_up_down = 0.0f;
            }

            if (right_stick_right > CommonParameter.MIN_GAMEPAD_STICK_VALUE)
            {
                this._move_player_velocity.turn_left_right = -right_stick_right;
            }
            else if (right_stick_left > CommonParameter.MIN_GAMEPAD_STICK_VALUE)
            {
                this._move_player_velocity.turn_left_right = right_stick_left;
            }
            else
            {
                this._move_player_velocity.turn_left_right = 0.0f;
            }

            if ((false == this._start_button_flag) &&
                 (true == Gamepad.current.startButton.wasPressedThisFrame))
            {
                this._start_button_flag = true;
            }
            else if ((true == this._start_button_flag) &&
                      (true == Gamepad.current.startButton.wasReleasedThisFrame))
            {
                this._start_button_flag = false;
            }

            if ((false == this._select_button_flag) &&
                 (true == Gamepad.current.selectButton.wasPressedThisFrame))
            {
                this._select_button_flag = true;
            }
            else if ((true == this._select_button_flag) &&
                      (true == Gamepad.current.selectButton.wasReleasedThisFrame))
            {
                this._select_button_flag = false;
            }

            if ((false == this._player_hand_holding_flag) &&
                 (true == Gamepad.current.buttonWest.wasPressedThisFrame))
            {
                this._player_hand_holding_flag = true;
            }
            else if ((true == this._player_hand_holding_flag) &&
                      (true == Gamepad.current.buttonWest.wasReleasedThisFrame))
            {
                this._player_hand_holding_flag = false;
            }

            Vector2 d_pad = Gamepad.current.dpad.ReadValue();

            this._move_hand_velocity.forward_backward = d_pad.y;
            this._move_hand_velocity.left_right = -d_pad.x;

            if((false == this._right_shoulder_button_flag) &&
               (true == Gamepad.current.rightShoulder.wasPressedThisFrame))
            {
                this._right_shoulder_button_flag = true;
            }
            else if ((true == this._right_shoulder_button_flag) &&
                     (true == Gamepad.current.rightShoulder.wasReleasedThisFrame))
            {
                this._right_shoulder_button_flag = false;
            }

            if ((false == this._left_shoulder_button_flag) &&
               (true == Gamepad.current.leftShoulder.wasPressedThisFrame))
            {
                this._left_shoulder_button_flag = true;
            }
            else if ((true == this._left_shoulder_button_flag) &&
                     (true == Gamepad.current.leftShoulder.wasReleasedThisFrame))
            {
                this._left_shoulder_button_flag = false;
            }

            if (this._left_shoulder_button_flag)
            {
                this._move_hand_velocity.up_down = 1.0f;
            }
            else if (this._right_shoulder_button_flag)
            {
                this._move_hand_velocity.up_down = -1.0f;
            }
            else
            {
                this._move_hand_velocity.up_down = 0.0f;
            }
        }

        ~GamepadReceiver()
        {
        }
    }
}
