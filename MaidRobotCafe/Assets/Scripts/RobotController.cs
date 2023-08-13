/**
 * @file RobotController.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Control Robot.
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

public class RobotController : MonoBehaviour
{
    /*********************************************************
     * Private variables
     *********************************************************/
    private Vector3 _position;    /*!< robot position */
    private Vector3 _euler_angle; /*!< robot euler angle */
    private Quaternion _rotation; /*!< robot quaternion */

    private KeyboardReceiver _KeyboardReceiver;            /*!< Keyboard Receiver object */
    private CarryObjectController _CarryObjectController;  /*!< Carry Object Controller object */
    private Animator _animator;                            /*!< Animator object */
    private CommReceiver _CommReceiver;                    /*!< CommReceiver object */
    private CommSender _CommSender;                        /*!< CommSender object */

    private Vector3 _velocity = CommonParameter.VELOCITY_INIT;                 /*!< velocity reference */
    private Vector3 _angular_velocity = CommonParameter.ANGULAR_VELOCITY_INIT; /*!< angular velocity reference */

    private CommonParameter.ROBOT_CARRYING_STATE _carrying_state = CommonParameter.INITIAL_ROBOT_CARRYING_STATE; /*!< carrying state */

    private bool[] _direction_input = new bool[Enum.GetNames(typeof(CommonParameter.DIRECTION)).Length]; /*!< direction input */

#if SHOW_STATUS_FOR_DEBUG
    private string _logText = ""; /*!< log text */
    private GUIStyle _guiStyle;   /*!< log GUI style */
#endif
    private bool _collide_flag = false;       /*!< collide flag */
    private string _collide_object_name = ""; /*!< names of collided object */

    /*********************************************************
     * Public functions
     *********************************************************/
    public bool get_collide_flag()
    {
        return this._collide_flag;
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
        GameObject KeyboardInput_GameObject = GameObject.Find("KeyboardInput");
        this._KeyboardReceiver = KeyboardInput_GameObject.GetComponent<KeyboardReceiver>();

        GameObject CarryObjectController_GameObject = GameObject.Find("CarryObjects");
        this._CarryObjectController = CarryObjectController_GameObject.GetComponent<CarryObjectController>();

        this._animator = GetComponent<Animator>();

        GameObject Communicator_GameObject = GameObject.Find("Communicator");
        this._CommReceiver = Communicator_GameObject.GetComponent<CommReceiver>();
        this._CommSender = Communicator_GameObject.GetComponent<CommSender>();

        update_orientation();
        this._animator.SetFloat("Speed", CommonParameter.WALKING_MOTION_SPEED);

        this._carrying_state = CommonParameter.ROBOT_CARRYING_STATE.NONE;
    }

    /* Update is called once per frame */
    void Update()
    {
        this._update_reference();

        /* move robot */
        Vector3 d_pos = _velocity * Time.deltaTime;
        transform.Translate(d_pos, Space.Self);

        Quaternion delta_euler_angle = Quaternion.Euler(_angular_velocity * Mathf.Rad2Deg * Time.deltaTime);
        transform.rotation = transform.rotation * delta_euler_angle;

        update_orientation();

        this._animator.SetFloat("MotionSpeed", _velocity.magnitude * CommonParameter.WALKING_MOTION_MPS_TO_ANIMATION_FACTOR);

        /* pick and place objects */
        if (this._KeyboardReceiver.check_place_on_robot_tray_flag())
        {
            if (this._CarryObjectController.place_on_robot_tray())
            {
                this._carrying_state = CommonParameter.ROBOT_CARRYING_STATE.CARRY_TEACUP;
            }
        }
        if (this._KeyboardReceiver.check_place_on_nearest_table_flag())
        {
            if (this._CarryObjectController.place_on_nearest_table())
            {
                this._carrying_state = CommonParameter.ROBOT_CARRYING_STATE.NONE;
            }
        }

        this._CommSender.set_position(_position);
        this._CommSender.set_rotation(_rotation);

        /* For debug */
#if SHOW_STATUS_FOR_DEBUG
        // this._logText = "Forward: " + this._direction_input[(int)CommonParameter.DIRECTION.FORWARD].ToString()
        //  + " Backward: " + this._direction_input[(int)CommonParameter.DIRECTION.BACKWARD].ToString()
        //  + " Right: " + this._direction_input[(int)CommonParameter.DIRECTION.RIGHT].ToString()
        //  + " Left: " + this._direction_input[(int)CommonParameter.DIRECTION.LEFT].ToString();
        this._logText = "x: " + _position.x.ToString() + Environment.NewLine
                      + "y: " + _position.y.ToString() + Environment.NewLine
                      + "z: " + _position.z.ToString() + Environment.NewLine
                      + "Roll: " + _euler_angle.x.ToString() + Environment.NewLine
                      + "Pitch: " + _euler_angle.y.ToString() + Environment.NewLine
                      + "Yaw: " + _euler_angle.z.ToString() + Environment.NewLine
                      + "Carrying state: " + _carrying_state.ToString();
        if (this._collide_flag)
        {
            this._logText += Environment.NewLine + "Collided: " + this._collide_object_name;
        }
#endif
    }

    void OnTriggerEnter(Collider other)
    {
        /* check collision for tables  */
        for (int i = 0; i < CommonParameter.MAX_TABLE_NUM; i++)
        {
            string table_name = "table_" + i.ToString();

            if (other.gameObject.name == table_name)
            {
                this._collide_object_name = this._collide_object_name + ", " + other.gameObject.name;
                this._collide_flag = true;
            }
        }

        /* check collision for chairs  */
        for (int i = 0; i < CommonParameter.MAX_CHAIR_NUM; i++)
        {
            string chair_name = "chair_" + i.ToString();

            if (other.gameObject.name == chair_name)
            {
                this._collide_object_name = _collide_object_name + ", " + other.gameObject.name;
                this._collide_flag = true;
            }
        }

        /* check collision for others */
        if (other.gameObject.name == "flasket")
        {
            this._collide_object_name = _collide_object_name + ", " + other.gameObject.name;
            this._collide_flag = true;
        }
        if (other.gameObject.name == "kitchen_table")
        {
            this._collide_object_name = _collide_object_name + ", " + other.gameObject.name;
            this._collide_flag = true;
        }
    }

    private void OnGUI()
    {
        /* Show the logged data in display */
#if SHOW_STATUS_FOR_DEBUG
        GUI.Label(CommonParameter.THIRD_PERSON_CONTROLLER_DEBUG_TEXT_POS, _logText, _guiStyle);
#endif

    }

    public void OnFootstep()
    {
    }

    private void update_orientation()
    {
        this._position = CommonParameter.AXIS_UNITY_TO_VEHICLE * transform.position;
        this._position = Vector3.Scale(CommonParameter.AXIS_RIGHT_TO_LEFT, this._position);

        Quaternion temp_q;
        temp_q = CommonParameter.AXIS_REMOVE_OFFSET_Q * transform.rotation;
        this._rotation.w = temp_q.w;
        this._rotation.x = -temp_q.z;
        this._rotation.y = temp_q.x;
        this._rotation.z = -temp_q.y;

        this._euler_angle.x = Mathf.Repeat(this._rotation.eulerAngles.x + CommonParameter.HALF_CIRCLE_DEG, CommonParameter.CIRCLE_DEG)
             - CommonParameter.HALF_CIRCLE_DEG;
        this._euler_angle.y = Mathf.Repeat(this._rotation.eulerAngles.y + CommonParameter.HALF_CIRCLE_DEG, CommonParameter.CIRCLE_DEG)
             - CommonParameter.HALF_CIRCLE_DEG;
        this._euler_angle.z = Mathf.Repeat(this._rotation.eulerAngles.z + CommonParameter.HALF_CIRCLE_DEG, CommonParameter.CIRCLE_DEG)
             - CommonParameter.HALF_CIRCLE_DEG;
    }

    /*********************************************************
     * Private functions
     *********************************************************/
    private void _update_reference()
    {
        this._KeyboardReceiver.get_4_direction_input(this._direction_input);

        CommonParameter.INPUT_MODE current_input_mode = this._KeyboardReceiver.get_current_input_mode();
        switch (current_input_mode)
        {
            case CommonParameter.INPUT_MODE.COMMUNICATION:
                this._velocity = new Vector3(0.0f, 0.0f, this._CommReceiver.get_forward_velocity());
                this._angular_velocity = new Vector3(0.0f, -this._CommReceiver.get_yaw_angular_velocity(), 0.0f);
                break;

            case CommonParameter.INPUT_MODE.KEYBOARD:
                this._velocity = new Vector3(0.0f, 0.0f, 0.0f);
                this._angular_velocity = new Vector3(0.0f, 0.0f, 0.0f);

                if (this._direction_input[(int)CommonParameter.DIRECTION.FORWARD])
                {
                    this._velocity.z += CommonParameter.WALKING_MOTION_SPEED;
                }
                if (this._direction_input[(int)CommonParameter.DIRECTION.BACKWARD])
                {
                    this._velocity.z -= CommonParameter.WALKING_MOTION_SPEED;
                }
                if (this._direction_input[(int)CommonParameter.DIRECTION.RIGHT])
                {
                    this._angular_velocity.y += CommonParameter.WALKING_ROTATION_SPEED;
                }
                if (this._direction_input[(int)CommonParameter.DIRECTION.LEFT])
                {
                    this._angular_velocity.y -= CommonParameter.WALKING_ROTATION_SPEED;
                }
                break;

            default:
                break;
        }
    }
}
