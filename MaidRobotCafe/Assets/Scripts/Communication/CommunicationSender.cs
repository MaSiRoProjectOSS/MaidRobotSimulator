/**
 * @file CommunicationSender.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Send data to outside of Simulator.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

using UnityEngine;
using MaidRobotSimulator.MaidRobotCafe;

public class CommunicationSender : MonoBehaviour
{
    /*********************************************************
     * Private variables
     *********************************************************/
    private float[] _robot_position = new float[SystemStructure.SPACE_DIMENSION_NUM];       /*!< position */
    private float[] _robot_rotation = new float[SystemStructure.QUATERNION_DIMENSTION_NUM]; /*!< rotation */
    private float[] _hand_position = new float[SystemStructure.SPACE_DIMENSION_NUM];       /*!< position */

    private MessageStructure.ST_SENSOR_MSGS_IMAGE _right_eye_camera_image;
    private MessageStructure.ST_SENSOR_MSGS_IMAGE _left_eye_camera_image;

    private UDPSender _UDPSender; /*!< UDP Sender class */
    private ROSSender _ROSSender; /*!< ROS Sender class */

    private float _position_rotation_send_interval =
        CommonParameter.ROBOT_POSITION_ROTATION_SEND_INTERVAL; /*!< interval time to send position and rotation */
    private float _hand_position_send_interval =
        CommonParameter.HAND_POSITION_SEND_INTERVAL; /*!< interval time to send hand position */
    private float _eye_images_send_interval =
        CommonParameter.EYE_CAMERA_IMAGE_SEND_INTERVAL; /*!< interval time to send eye images */

    private CommonStateMachine<SystemStructure.SEND_MODE> _position_rotation_send_state_machine
        = new CommonStateMachine<SystemStructure.SEND_MODE>(SystemStructure.SEND_MODE.WAIT);  /*!< manage send timing for position and rotation */
    private CommonStateMachine<SystemStructure.SEND_MODE> _hand_position_send_state_machine
        = new CommonStateMachine<SystemStructure.SEND_MODE>(SystemStructure.SEND_MODE.WAIT);  /*!< manage send timing for hand position */
    private CommonStateMachine<SystemStructure.SEND_MODE> _eye_images_send_state_machine
        = new CommonStateMachine<SystemStructure.SEND_MODE>(SystemStructure.SEND_MODE.WAIT);  /*!< manage send timing for eye images */

    /*********************************************************
     * Public functions
     *********************************************************/
    public void set_position_rotation_send_interval(float interval)
    {
        this._position_rotation_send_interval = interval;
    }

    public void set_eye_images_send_interval(float interval)
    {
        this._eye_images_send_interval = interval;
    }

    /*********************************************************
     * MonoBehaviour functions
     *********************************************************/
    /* Start is called before the first frame update */
    void Start()
    {
        this._position_rotation_send_state_machine.set_time_interval(this._position_rotation_send_interval);
        this._eye_images_send_state_machine.set_time_interval(this._eye_images_send_interval);
        this._hand_position_send_state_machine.set_time_interval(this._hand_position_send_interval);

        switch (CommonParameter.ROBOT_COMMUNICATION_MODE)
        {
            case SystemStructure.COMMUNICATION_MODE.UDP:
                this._UDPSender = new UDPSender();
                break;
            case SystemStructure.COMMUNICATION_MODE.ROS:
                this._ROSSender = new ROSSender();
                break;
            default:
                break;
        }
    }

    /* Update is called once per frame */
    void Update()
    {
        /* position and rotation */
        this._position_rotation_send_state_machine.set_time_interval(this._position_rotation_send_interval);
        if (SystemStructure.SEND_MODE.SEND == 
                this._position_rotation_send_state_machine.triggered_switch_mode_by_elapsed_time(Time.deltaTime))
        {
            switch (CommonParameter.ROBOT_COMMUNICATION_MODE)
            {
                case SystemStructure.COMMUNICATION_MODE.UDP:
                    this._UDPSender.set_position_and_rotation(this._robot_position, this._robot_rotation);
                    this._UDPSender.send_position_orientation();
                    break;
                case SystemStructure.COMMUNICATION_MODE.ROS:
                    this._ROSSender.set_position_and_rotation(this._robot_position, this._robot_rotation);
                    this._ROSSender.send_position_orientation();
                    break;
                default:
                    break;
            }
        }

        /* hand position */
        this._hand_position_send_state_machine.set_time_interval(this._hand_position_send_interval);
        if (SystemStructure.SEND_MODE.SEND == 
                           this._hand_position_send_state_machine.triggered_switch_mode_by_elapsed_time(Time.deltaTime))
        {
            switch (CommonParameter.ROBOT_COMMUNICATION_MODE)
            {
                case SystemStructure.COMMUNICATION_MODE.UDP:
                    break;
                case SystemStructure.COMMUNICATION_MODE.ROS:
                    this._ROSSender.set_hand_position(this._hand_position);
                    this._ROSSender.send_hand_position();
                    break;
                default:
                    break;
            }
        }

        /* eye images */
        this._eye_images_send_state_machine.set_time_interval(this._eye_images_send_interval);
        if (SystemStructure.SEND_MODE.SEND == 
                this._eye_images_send_state_machine.triggered_switch_mode_by_elapsed_time(Time.deltaTime))
        {
            switch (CommonParameter.ROBOT_COMMUNICATION_MODE)
            {
                case SystemStructure.COMMUNICATION_MODE.UDP:

                    break;
                case SystemStructure.COMMUNICATION_MODE.ROS:
                    this._ROSSender.set_eye_images(
                        this._right_eye_camera_image, this._left_eye_camera_image);
                    this._ROSSender.send_eye_images();
                    break;
                default:
                    break;
            }
        }
    }

    /*********************************************************
     * Public functions
     *********************************************************/
    public void set_robot_position(Vector3 position)
    {
        this._robot_position[0] = position.x;
        this._robot_position[1] = position.y;
        this._robot_position[2] = position.z;
    }

    public void set_robot_rotation(Quaternion rotation)
    {
        this._robot_rotation[0] = rotation.w;
        this._robot_rotation[1] = rotation.x;
        this._robot_rotation[2] = rotation.y;
        this._robot_rotation[3] = rotation.z;
    }

    public void set_hand_position(Vector3 position)
    { 
        this._hand_position[0] = position.x;
        this._hand_position[1] = position.y;
        this._hand_position[2] = position.z;
    }

    public void set_eyes_camera_images(
        MessageStructure.ST_SENSOR_MSGS_IMAGE right_eye_camera_image,
        MessageStructure.ST_SENSOR_MSGS_IMAGE left_eye_camera_image)
    {
        this._right_eye_camera_image = right_eye_camera_image;
        this._left_eye_camera_image = left_eye_camera_image;
    }
}
