/**
 * @file CommSender.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Send data to outside of Simulator.
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

public class CommSender : MonoBehaviour
{
    /*********************************************************
     * Private variables
     *********************************************************/
    private float[] _position = new float[CommonParameter.SPACE_DIMENSION_NUM];       /*!< position */
    private float[] _rotation = new float[CommonParameter.QUATERNION_DIMENSTION_NUM]; /*!< rotation */

    private UDPSender _UDPSender; /*!< UDP Sender class */
    private ROSSender _ROSSender; /*!< ROS Sender class */

    private float _elapsed_time_after_last_send = Mathf.Infinity; /*!< elapsed time after last send data */

    private float _send_interval = CommonParameter.SEND_INTERVAL; /*!< interval time to send data */

#if SHOW_STATUS_FOR_DEBUG
    // private string _logText = ""; /*!< log text */ /* Uncomment if you use debug text. */
    private GUIStyle _guiStyle; /*!< log GUI style */
#endif

    /*********************************************************
     * Public functions
     *********************************************************/
    public void set_send_interval(float interval)
    {
        this._send_interval = interval;
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
        switch (CommonParameter.ROBOT_COMMUNICATION_MODE)
        {
            case CommonParameter.COMMUNICATION_MODE.UDP:
                this._UDPSender = new UDPSender();
                break;
            case CommonParameter.COMMUNICATION_MODE.ROS:
                this._ROSSender = new ROSSender();
                break;
            default:
                break;
        }
    }

    /* Update is called once per frame */
    void Update()
    {
        if (this._elapsed_time_after_last_send >= this._send_interval)
        {
            switch (CommonParameter.ROBOT_COMMUNICATION_MODE)
            {
                case CommonParameter.COMMUNICATION_MODE.UDP:
                    this._UDPSender.set_position_and_rotation(this._position, this._rotation);
                    this._UDPSender.send_data();
                    break;
                case CommonParameter.COMMUNICATION_MODE.ROS:
                    this._ROSSender.set_position_and_rotation(this._position, this._rotation);
                    this._ROSSender.send_data();
                    break;
                default:
                    break;
            }

            this._elapsed_time_after_last_send = 0.0f;
        }
        else
        {
            this._elapsed_time_after_last_send += Time.deltaTime;
        }
    }

    private void OnGUI()
    {
        /* Show the logged data in display */
        /* UNcomment if you use debug text. */
        /*
#if SHOW_STATUS_FOR_DEBUG
        GUI.Label(CommonParameter.COMMUNICATOR_DEBUG_TEXT_POS, _logText, _guiStyle);
#endif
        */
    }

    public void set_position(Vector3 position)
    {
        this._position[0] = position.x;
        this._position[1] = position.y;
        this._position[2] = position.z;
    }

    public void set_rotation(Quaternion rotation)
    {
        this._rotation[0] = rotation.w;
        this._rotation[1] = rotation.x;
        this._rotation[2] = rotation.y;
        this._rotation[3] = rotation.z;
    }
}
