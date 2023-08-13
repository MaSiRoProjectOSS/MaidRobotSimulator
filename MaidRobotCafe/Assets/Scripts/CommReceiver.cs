/**
 * @file CommReceiver.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Receive data from outside of Simulator.
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

public class CommReceiver : MonoBehaviour
{
    /*********************************************************
     * Private variables
     *********************************************************/
    private float _forward_velocity;     /*!< forward velocity */
    private float _yaw_angular_velocity; /*!< yaw angle velocity */

    private UDPReceiver _UDPReceiver; /*!< UDP Receiver class */
    private ROSReceiver _ROSReceiver; /*!< ROS Receiver class */

#if SHOW_STATUS_FOR_DEBUG
    private string _logText = ""; /*!< log text */
    private GUIStyle _guiStyle;   /*!< log GUI style */
#endif

    /*********************************************************
     * Public variables
     *********************************************************/
    public float get_forward_velocity()
    {
        return this._forward_velocity;
    }

    public float get_yaw_angular_velocity()
    {
        return this._yaw_angular_velocity;
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
                this._UDPReceiver = new UDPReceiver();
                break;
            case CommonParameter.COMMUNICATION_MODE.ROS:
                this._ROSReceiver = new ROSReceiver();
                break;
            default:
                break;
        }
    }

    /* Update is called once per frame */
    void Update()
    {
        switch (CommonParameter.ROBOT_COMMUNICATION_MODE)
        {
            case CommonParameter.COMMUNICATION_MODE.UDP:
                if (CommonParameter.ROBOT_SYNC_MODE == CommonParameter.SYNC_MODE.SYNC)
                {
                    while (false == this._UDPReceiver.is_data_received())
                    {
                        /* Wait until receiving data. */
                    }
                }

                ROSStructure.ST_GEOMETRY_MSGS_TWIST cmd_vel_UDP =
                    this._UDPReceiver.get_move_velocity_reference();
                this._forward_velocity = (float)cmd_vel_UDP.linear.x;
                this._yaw_angular_velocity = (float)cmd_vel_UDP.angular.z;

                break;
            case CommonParameter.COMMUNICATION_MODE.ROS:
                if (CommonParameter.ROBOT_SYNC_MODE == CommonParameter.SYNC_MODE.SYNC)
                {
                    while (false == this._ROSReceiver.is_data_received())
                    {
                        /* Wait until receiving data. */
                    }
                }

                ROSStructure.ST_GEOMETRY_MSGS_TWIST cmd_vel_ROS =
                    this._ROSReceiver.get_move_velocity_reference();
                this._forward_velocity = (float)cmd_vel_ROS.linear.x;
                this._yaw_angular_velocity = (float)cmd_vel_ROS.angular.z;

                break;
            default:
                break;
        }

#if SHOW_STATUS_FOR_DEBUG
        this._logText = "Forward velocity: " + this._forward_velocity.ToString() + Environment.NewLine
                      + "Yaw angular velocity: " + this._yaw_angular_velocity.ToString();
#endif
    }

    private void OnGUI()
    {
        /* Show the logged data in display */
#if SHOW_STATUS_FOR_DEBUG
        GUI.Label(CommonParameter.COMMUNICATOR_DEBUG_TEXT_POS, this._logText, this._guiStyle);
#endif
    }

}
