/**
 * @file CommunicationReceiver.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Receive data from outside of Simulator.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

using System;
using UnityEngine;
using MaidRobotSimulator.MaidRobotCafe;

public class CommunicationReceiver : MonoBehaviour
{
    /*********************************************************
     * Private variables
     *********************************************************/
    private float _forward_velocity = 0.0f;     /*!< forward velocity */
    private float _yaw_angular_velocity = 0.0f; /*!< yaw angle velocity */

    private MessageStructure.ST_MRS_EYE _eye_reference =
            new MessageStructure.ST_MRS_EYE(
                0, 0, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);

    private int _lip_percent = 0;
    private Quaternion _neck_rotation = Quaternion.identity;

    private MessageStructure.ST_HEAD_STATUS _head_status =
            new MessageStructure.ST_HEAD_STATUS(false);

    private UDPReceiver _UDPReceiver; /*!< UDP Receiver class */
    private ROSReceiver _ROSReceiver; /*!< ROS Receiver class */

    private string _logText = ""; /*!< log text */

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

    public MessageStructure.ST_MRS_EYE get_eye_reference()
    {
        return this._eye_reference;
    }

    public int get_lip_percent()
    {
        return this._lip_percent;
    }

    public Quaternion get_neck_reference()
    {
        return this._neck_rotation;
    }

    public MessageStructure.ST_HEAD_STATUS get_head_status()
    {
        return this._head_status;
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
        switch (CommonParameter.ROBOT_COMMUNICATION_MODE)
        {
            case SystemStructure.COMMUNICATION_MODE.UDP:
                this._UDPReceiver = new UDPReceiver();
                break;
            case SystemStructure.COMMUNICATION_MODE.ROS:
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
            case SystemStructure.COMMUNICATION_MODE.UDP:
                if (CommonParameter.ROBOT_SYNC_MODE == SystemStructure.SYNC_MODE.SYNC)
                {
                    while (false == this._UDPReceiver.is_data_received())
                    {
                        /* Wait until receiving data. */
                    }
                }

                MessageStructure.ST_GEOMETRY_MSGS_TWIST move_velocity_UDP =
                    this._UDPReceiver.get_move_velocity_reference();
                this._forward_velocity = (float)move_velocity_UDP.linear.x;
                this._yaw_angular_velocity = (float)move_velocity_UDP.angular.z;

                break;
            case SystemStructure.COMMUNICATION_MODE.ROS:
                if (CommonParameter.ROBOT_SYNC_MODE == SystemStructure.SYNC_MODE.SYNC)
                {
                    while (false == this._ROSReceiver.is_data_received())
                    {
                        /* Wait until receiving data. */
                    }
                }

                MessageStructure.ST_GEOMETRY_MSGS_TWIST move_velocity_ROS =
                    this._ROSReceiver.get_move_velocity_reference();
                this._forward_velocity = (float)move_velocity_ROS.linear.x;
                this._yaw_angular_velocity = (float)move_velocity_ROS.angular.z;

                MessageStructure.ST_MRS_EYE eye_reference_ROS =
                    this._ROSReceiver.get_eye_reference();
                this._eye_reference.emotions = eye_reference_ROS.emotions;
                this._eye_reference.cornea_effect = eye_reference_ROS.cornea_effect;
                this._eye_reference.dimensions = eye_reference_ROS.dimensions;
                this._eye_reference.distance = eye_reference_ROS.distance;
                this._eye_reference.left_y = eye_reference_ROS.left_y;
                this._eye_reference.left_z = eye_reference_ROS.left_z;
                this._eye_reference.right_y = eye_reference_ROS.right_y;
                this._eye_reference.right_z = eye_reference_ROS.right_z;

                MessageStructure.ST_MRS_LIP lip_reference =
                    this._ROSReceiver.get_lip_reference();
                this._lip_percent = lip_reference.percent;

                MessageStructure.ST_MRS_NECK neck_reference =
                    this._ROSReceiver.get_neck_reference();
                this._neck_rotation.w = neck_reference.w;
                this._neck_rotation.x = neck_reference.x;
                this._neck_rotation.y = neck_reference.y;
                this._neck_rotation.z = neck_reference.z;

                MessageStructure.ST_HEAD_STATUS head_status =
                    this._ROSReceiver.get_head_status();

                this._head_status = head_status;

                break;
            default:
                break;
        }

        this._log_for_debug();
    }

    private void _log_for_debug()
    {
        this._logText = "ForwardVelocity: " + this._forward_velocity.ToString() + Environment.NewLine
                      + "YawVelocity: " + this._yaw_angular_velocity.ToString() + Environment.NewLine
                      + "RightEyeReference Y: " + this._eye_reference.right_y.ToString() + Environment.NewLine
                      + "RightEyeReference Z: " + this._eye_reference.right_z.ToString() + Environment.NewLine
                      + "RightEyeReference Y: " + this._eye_reference.left_y.ToString() + Environment.NewLine
                      + "RightEyeReference Z: " + this._eye_reference.left_z.ToString() + Environment.NewLine
                      + "Neck W: " + this._neck_rotation.w.ToString() + Environment.NewLine
                      + "Neck X: " + this._neck_rotation.x.ToString() + Environment.NewLine
                      + "Neck Y: " + this._neck_rotation.y.ToString() + Environment.NewLine
                      + "Neck Z: " + this._neck_rotation.z.ToString() + Environment.NewLine;
    }
}
