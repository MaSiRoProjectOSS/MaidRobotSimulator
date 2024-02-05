/**
 * @file InputManager.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Manage PC inputs.
 *
 * @copyright Copyright (c) MaSiRo Project. 2024-.
 *
 */

using UnityEngine;
using MaidRobotSimulator.MaidRobotCafe;

public class InputManager : MonoBehaviour
{
    /*********************************************************
    * Private variables
    *********************************************************/
    private KeyboardReceiver _KeyboardReceiver;            /*!< Keyboard Receiver object */
    private GamepadReceiver _GamepadReceiver;              /*!< Gamepad Receiver object */

    private SystemStructure.ST_8_DIRECTION_MOVE _move_robot_direction =
        new SystemStructure.ST_8_DIRECTION_MOVE(
            false, false, false, false, false, false, false, false); /*!< move robot direction */

    private SystemStructure.ST_8_DIRECTION_MOVE _move_hand_direction =
        new SystemStructure.ST_8_DIRECTION_MOVE(
            false, false, false, false, false, false, false, false); /*!< move robot direction */

    private SystemStructure.ST_FIRST_PERSON_DIRECTION_VELOCITY _move_player_velocity =
        new SystemStructure.ST_FIRST_PERSON_DIRECTION_VELOCITY(
                       0.0f, 0.0f, 0.0f, 0.0f, 0.0f); /*!< move player velocity */

    private SystemStructure.ST_FIRST_PERSON_DIRECTION_VELOCITY _move_hand_velocity =
        new SystemStructure.ST_FIRST_PERSON_DIRECTION_VELOCITY(
                       0.0f, 0.0f, 0.0f, 0.0f, 0.0f); /*!< move hand velocity */

    private CommonStateMachine<SystemStructure.INPUT_MODE> _input_mode_state_machine;

    private bool _place_on_robot_tray_flag = false;    /*!< place on robot tray flag */
    private bool _place_on_nearest_table_flag = false; /*!< place on nearest table flag */

    private bool _robot_mode_switch_flag = false;
    private bool _camera_switch_flag = false;

    private bool _player_hand_holding_flag = false; /*!< player hand holding flag */

    private bool _start_button_flag = false; /*!< start button flag */
    private bool _select_button_flag = false; /*!< select button flag */

    /*********************************************************
    * MonoBehaviour functions
    *********************************************************/
    /* Start is called before the first frame update */
    void Start()
    {
        this._KeyboardReceiver = new KeyboardReceiver();
        this._GamepadReceiver = new GamepadReceiver();

        this._input_mode_state_machine = new CommonStateMachine<SystemStructure.INPUT_MODE>(CommonParameter.INITIAL_INPUT_MODE);
    }

    /* Update is called once per frame */
    void Update()
    {
        this._KeyboardReceiver.update_keyboard_status();
        this._GamepadReceiver.update_gamepad_status();

        /* Switch robot mode */
        if (false == this._robot_mode_switch_flag)
        {
            this._robot_mode_switch_flag = this._KeyboardReceiver.get_robot_mode_switch_flag();
        }

        /* Switch camera */
        this._camera_switch_flag = this._KeyboardReceiver.get_camera_switch_flag();

        /* Input mode */
        this._input_mode_state_machine.successive_switch_mode_and_update_elapsed_time(
                this._KeyboardReceiver.get_input_mode_change_flag(),
                CommonParameter.KEYBOARD_MODE_WAIT_TIME, Time.deltaTime);

        /* Pick and place object */
        if (false == this._place_on_robot_tray_flag)
        {
            this._place_on_robot_tray_flag = this._KeyboardReceiver.get_place_on_robot_tray_flag();
        }
        if (false == this._place_on_nearest_table_flag)
        {
            this._place_on_nearest_table_flag = this._KeyboardReceiver.get_place_on_nearest_table_flag();
        }

        SystemStructure.ST_8_DIRECTION_MOVE direction_input =
            new SystemStructure.ST_8_DIRECTION_MOVE(
                false, false, false, false, false, false, false, false);

        /* Move robot */
        this._KeyboardReceiver.get_robot_direction_input(ref direction_input);
        this._move_robot_direction.forward = direction_input.forward;
        this._move_robot_direction.backward = direction_input.backward;
        this._move_robot_direction.right = direction_input.right;
        this._move_robot_direction.left = direction_input.left;

        /* Move Hand */
        this._KeyboardReceiver.get_hand_direction_input(ref direction_input);
        this._move_hand_direction.forward = direction_input.forward;
        this._move_hand_direction.backward = direction_input.backward;
        this._move_hand_direction.right = direction_input.right;
        this._move_hand_direction.left = direction_input.left;
        this._move_hand_direction.up = direction_input.up;
        this._move_hand_direction.down = direction_input.down;
        this._move_hand_direction.turn_right = direction_input.turn_right;
        this._move_hand_direction.turn_left = direction_input.turn_left;

        SystemStructure.ST_FIRST_PERSON_DIRECTION_VELOCITY velocity =
            new SystemStructure.ST_FIRST_PERSON_DIRECTION_VELOCITY(
                0.0f, 0.0f, 0.0f, 0.0f, 0.0f);

        /* Player velocity */
        this._GamepadReceiver.get_move_player_velocity(ref velocity);
        this._move_player_velocity.forward_backward = velocity.forward_backward;
        this._move_player_velocity.left_right = velocity.left_right;
        this._move_player_velocity.look_up_down = velocity.look_up_down;
        this._move_player_velocity.turn_left_right = velocity.turn_left_right;
        this._move_player_velocity.up_down = velocity.up_down;

        /* Hand velocity */
        this._GamepadReceiver.get_move_hand_velocity(ref velocity);
        this._move_hand_velocity.forward_backward = velocity.forward_backward;
        this._move_hand_velocity.left_right = velocity.left_right;
        this._move_hand_velocity.look_up_down = velocity.look_up_down;
        this._move_hand_velocity.turn_left_right = velocity.turn_left_right;
        this._move_hand_velocity.up_down = velocity.up_down;

        /* Start menu */
        this._start_button_flag = this._GamepadReceiver.get_start_button_flag();

        /* Select action */
        this._select_button_flag = this._GamepadReceiver.get_select_button_flag();

        /* Player hand holding */
        this._player_hand_holding_flag = this._GamepadReceiver.get_player_hand_holding_flag();
    }

    /*********************************************************
    * Public functions
    *********************************************************/
    public void get_robot_direction_input(ref SystemStructure.ST_8_DIRECTION_MOVE direction_input)
    {
        direction_input.forward = this._move_robot_direction.forward;
        direction_input.backward = this._move_robot_direction.backward;
        direction_input.right = this._move_robot_direction.right;
        direction_input.left = this._move_robot_direction.left;
        direction_input.up = this._move_robot_direction.up;
        direction_input.down = this._move_robot_direction.down;
        direction_input.turn_right = this._move_robot_direction.turn_right;
        direction_input.turn_left = this._move_robot_direction.turn_left;
    }

    public void get_hand_direction_input(ref SystemStructure.ST_8_DIRECTION_MOVE direction_input)
    {
        direction_input.forward = this._move_hand_direction.forward;
        direction_input.backward = this._move_hand_direction.backward;
        direction_input.right = this._move_hand_direction.right;
        direction_input.left = this._move_hand_direction.left;
        direction_input.up = this._move_hand_direction.up;
        direction_input.down = this._move_hand_direction.down;
        direction_input.turn_right = this._move_hand_direction.turn_right;
        direction_input.turn_left = this._move_hand_direction.turn_left;
    }

    public bool check_place_on_robot_tray_flag()
    {
        bool return_value = this._place_on_robot_tray_flag;

        this._place_on_robot_tray_flag = false;

        return return_value;
    }

    public bool check_place_on_nearest_table_flag()
    {
        bool return_value = this._place_on_nearest_table_flag;

        this._place_on_nearest_table_flag = false;

        return return_value;
    }

    public bool check_robot_mode_switch_flag()
    {
        bool return_value = this._robot_mode_switch_flag;

        this._robot_mode_switch_flag = false;

        return return_value;
    }

    public SystemStructure.INPUT_MODE get_current_input_mode()
    {
        return this._input_mode_state_machine.get_mode();
    }

    public bool get_camera_switch_flag()
    {
        return this._camera_switch_flag;
    }

    public bool get_player_hand_holding_flag()
    {
        return this._player_hand_holding_flag;
    }

    public bool get_start_button_flag()
    {
        return this._start_button_flag;
    }

    public bool get_select_button_flag()
    {
        return this._select_button_flag;
    }

    public SystemStructure.ST_FIRST_PERSON_DIRECTION_VELOCITY get_move_player_velocity()
    {
        return this._move_player_velocity;
    }

    public SystemStructure.ST_FIRST_PERSON_DIRECTION_VELOCITY get_move_hand_velocity()
    {
        return this._move_hand_velocity;
    }
}
