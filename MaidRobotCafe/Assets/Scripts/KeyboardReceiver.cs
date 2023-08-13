/**
 * @file KeyboardReceiver.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Receive keyboard input.
 * @version 1.0.0
 * @date 2023-08-05
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

/* Comment below if you don't need debug texts. */
#define SHOW_STATUS_FOR_DEBUG

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MaidRobotSimulator.MaidRobotCafe;

public class KeyboardReceiver : MonoBehaviour
{
    /*********************************************************
     * Private variables
     *********************************************************/
    private bool _move_forward = false;  /*!< move forward input */
    private bool _move_backward = false; /*!< move backward input */
    private bool _move_right = false;    /*!< move right input */
    private bool _move_left = false;     /*!< move left input */

    private bool _place_on_robot_tray_flag = false;    /*!< place on robot tray flag */
    private bool _place_on_nearest_table_flag = false; /*!< place on nearest table flag */

    private CommonParameter.INPUT_MODE _input_mode = CommonParameter.INITIAL_INPUT_MODE; /*!< input mode */
    private float _elapsed_time_after_mode_changed = Mathf.Infinity;                     /*!< elapsed time after mode changed */

#if SHOW_STATUS_FOR_DEBUG
    private string _logText = ""; /*!< log text */
    private GUIStyle _guiStyle;   /*!< log GUI style */
#endif

    /*********************************************************
     * Public functions
     *********************************************************/
    public void get_4_direction_input(bool[] direction_input)
    {
        direction_input[(int)CommonParameter.DIRECTION.FORWARD] = this._move_forward;
        direction_input[(int)CommonParameter.DIRECTION.BACKWARD] = this._move_backward;
        direction_input[(int)CommonParameter.DIRECTION.RIGHT] = this._move_right;
        direction_input[(int)CommonParameter.DIRECTION.LEFT] = this._move_left;
    }

    public CommonParameter.INPUT_MODE get_current_input_mode()
    {
        return this._input_mode;
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

    /*********************************************************
     * MonoBehaviour functions
     *********************************************************/
    private void Awake()
    {
#if SHOW_STATUS_FOR_DEBUG
        this._guiStyle = new GUIStyle();
        this._guiStyle.fontSize = CommonParameter.DEBUG_TEXT_FONT_SIZE;
        this._guiStyle.normal.textColor = Color.white;
#endif
    }

    /* Start is called before the first frame update */
    void Start()
    {
    }

    /* Update is called once per frame */
    void Update()
    {
        /* Move robot */
        this._move_forward = Input.GetKey(KeyCode.W);
        this._move_backward = Input.GetKey(KeyCode.S);
        this._move_right = Input.GetKey(KeyCode.D);
        this._move_left = Input.GetKey(KeyCode.A);

        if (Input.GetKey(KeyCode.Space))
        {
            if (this._elapsed_time_after_mode_changed > CommonParameter.KEYBOARD_MODE_WAIT_TIME)
            {
                switch (this._input_mode)
                {
                    case CommonParameter.INPUT_MODE.COMMUNICATION:
                        this._input_mode = CommonParameter.INPUT_MODE.KEYBOARD;
                        break;

                    case CommonParameter.INPUT_MODE.KEYBOARD:
                        this._input_mode = CommonParameter.INPUT_MODE.COMMUNICATION;
                        break;

                    default:
                        this._input_mode = CommonParameter.INPUT_MODE.COMMUNICATION;
                        break;
                }

                this._elapsed_time_after_mode_changed = 0.0f;
            }
        }

        /* Pick and place object */
        if (this._place_on_robot_tray_flag == false)
        {
            this._place_on_robot_tray_flag = Input.GetKey(KeyCode.O);
        }
        if (this._place_on_nearest_table_flag == false)
        {
            this._place_on_nearest_table_flag = Input.GetKey(KeyCode.P);
        }

        /* state */
        this._elapsed_time_after_mode_changed += Time.deltaTime;

        /* debug */
#if SHOW_STATUS_FOR_DEBUG
        this._logText = "Input mode: " + this._input_mode.ToString();
#endif
    }

    private void OnGUI()
    {
        /* Show the logged data in display */
#if SHOW_STATUS_FOR_DEBUG
        GUI.Label(CommonParameter.KEYBOARD_RECEIVER_DEBUG_TEXT_POS, this._logText, this._guiStyle);
#endif
    }

}
