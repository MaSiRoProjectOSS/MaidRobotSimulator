/**
 * @file KeyboardReceiver.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Receive keyboard input.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

using UnityEngine;
using MaidRobotSimulator.MaidRobotCafe;

public class KeyboardReceiver : MonoBehaviour
{
    /*********************************************************
     * Private variables
     *********************************************************/
    private SystemStructure.ST_8_DIRECTION_MOVE _move_robot_direction = 
        new SystemStructure.ST_8_DIRECTION_MOVE(
            false, false, false, false, false, false, false, false); /*!< move robot direction */

    private SystemStructure.ST_8_DIRECTION_MOVE _move_hand_direction =
        new SystemStructure.ST_8_DIRECTION_MOVE(
            false, false, false, false, false, false, false, false); /*!< move robot direction */

    private bool _place_on_robot_tray_flag = false;    /*!< place on robot tray flag */
    private bool _place_on_nearest_table_flag = false; /*!< place on nearest table flag */

    private bool _robot_mode_switch_flag = false;
    private bool _camera_switch_flag = false;

    private CommonStateMachine<SystemStructure.INPUT_MODE> _input_mode_state_machine;
 
    private string _logText = ""; /*!< log text */

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
        direction_input.down= this._move_robot_direction.down;
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

    public SystemStructure.INPUT_MODE get_current_input_mode()
    {
        return this._input_mode_state_machine.get_mode();
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

    public bool check_camera_switch_flag()
    {
        return this._camera_switch_flag;
    }

    public string get_log_text()
    {
        return this._logText;
    }

    /*********************************************************
     * MonoBehaviour functions
     *********************************************************/
    /* Start is called before the first frame update */
    void Start()
    {
        this._input_mode_state_machine = new CommonStateMachine<SystemStructure.INPUT_MODE>(CommonParameter.INITIAL_INPUT_MODE);
    }

    /* Update is called once per frame */
    void Update()
    {
        /* Switch robot mode */
        if (false == this._robot_mode_switch_flag)
        {
            this._robot_mode_switch_flag = Input.GetKey(KeyCode.T);
        }

        /* Move robot */
        this._move_robot_direction.forward = Input.GetKey(KeyCode.W);
        this._move_robot_direction.backward = Input.GetKey(KeyCode.S);
        this._move_robot_direction.right = Input.GetKey(KeyCode.D);
        this._move_robot_direction.left = Input.GetKey(KeyCode.A);

        /* Switch camera */
        this._camera_switch_flag = Input.GetKey(KeyCode.F1);

        /* Input mode */
        this._input_mode_state_machine.successive_switch_mode_and_update_elapsed_time(
                Input.GetKey(KeyCode.Space),
                CommonParameter.KEYBOARD_MODE_WAIT_TIME, Time.deltaTime);

        /* Pick and place object */
        if (false == this._place_on_robot_tray_flag)
        {
            this._place_on_robot_tray_flag = Input.GetKey(KeyCode.O);
        }
        if (false == this._place_on_nearest_table_flag)
        {
            this._place_on_nearest_table_flag = Input.GetKey(KeyCode.P);
        }

        /* Move Hand */
        this._get_hand_direction();


        this._log_for_debug();
    }

    private void _get_hand_direction()
    {
        if(true == Input.GetKey(KeyCode.LeftShift) || true == Input.GetKey(KeyCode.RightShift))
        {
            this._move_hand_direction.forward = false;
            this._move_hand_direction.backward = false;
            this._move_hand_direction.right = false;
            this._move_hand_direction.left = false;
            this._move_hand_direction.up = Input.GetKey(KeyCode.UpArrow);
            this._move_hand_direction.down = Input.GetKey(KeyCode.DownArrow);
            this._move_hand_direction.turn_right = Input.GetKey(KeyCode.RightArrow);
            this._move_hand_direction.turn_left = Input.GetKey(KeyCode.LeftArrow);
        }
        else
        {
            this._move_hand_direction.forward = Input.GetKey(KeyCode.UpArrow);
            this._move_hand_direction.backward = Input.GetKey(KeyCode.DownArrow);
            this._move_hand_direction.right = Input.GetKey(KeyCode.RightArrow);
            this._move_hand_direction.left = Input.GetKey(KeyCode.LeftArrow);
            this._move_hand_direction.up = false;
            this._move_hand_direction.down = false;
            this._move_hand_direction.turn_right = false;
            this._move_hand_direction.turn_left = false;
        }
    }

    private void _log_for_debug()
    {
        this._logText = "Input mode: " + this._input_mode_state_machine.get_mode().ToString();
    }
}
