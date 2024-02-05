/**
 * @file PlayerController.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Control Player.
 *
 * @copyright Copyright (c) MaSiRo Project. 2024-.
 *
 */

using System;
using UniHumanoid;
using UnityEngine;
using MaidRobotSimulator.MaidRobotCafe;

public class PlayerController : MonoBehaviour
{
    /*********************************************************
     * Private variables
     *********************************************************/
    private GameObject _player_GameObject; /*!< player object */
    private GameObject _robot_GameObject; /*!< robot object */
    private RobotController _RobotController;

    private EnvironmentController _EnvironmentController;

    private InputManager _InputManager;

    private CommonStateMachine<SystemStructure.PLAYER_MODE> _player_mode_state_machine; /*!< player mode */

    private Vector3 _position_robot_axis = CommonParameter.POSITION_INIT;    /*!< player position */
    private Quaternion _yaw_rotation_robot_axis = CommonParameter.ROTATION_INIT; /*!< player quaternion only yaw angle */
    private Quaternion _rotation_robot_axis = CommonParameter.ROTATION_INIT; /*!< player quaternion */
    private float _pitch_angle_robot_axis = 0.0f; /*!< player pitch angle */

    private float _move_velocity_factor = CommonParameter.PLAYER_MOVE_VELOCITY; /*!< move velocity factor */
    private float _turn_velocity_factor = CommonParameter.PLAYER_TURN_VELOCITY; /*!< turn velocity factor */

    private SystemStructure.ST_FIRST_PERSON_DIRECTION_VELOCITY _first_person_velocity_reference_relative =
        new SystemStructure.ST_FIRST_PERSON_DIRECTION_VELOCITY(
                       0.0f, 0.0f, 0.0f, 0.0f, 0.0f); /*!< move player velocity */

    private Humanoid _humanoid_component;
    private Quaternion _initial_head_rotation_unity_axis = Quaternion.identity;

    private Vector3 _hand_holding_absolute_position = Vector3.zero;
    private Vector3 _hand_holding_relative_position = CommonParameter.PLAYER_HAND_HOLDING_RELATIVE_POSITION;

    private float _delta_time = 0.0f; /*!< delta time */

    private string _logText = ""; /*!< log text */

    /*********************************************************
     * Public functions
     *********************************************************/
    public Vector3 get_hand_holding_position()
    {
        Vector3 return_value = Vector3.zero;

        if(SystemStructure.PLAYER_MODE.FREE == this._player_mode_state_machine.get_mode())
        {
            /* return zero. */
        }
        else if (SystemStructure.PLAYER_MODE.HAND_HOLDING == this._player_mode_state_machine.get_mode())
        {
            float speed_limit = CommonParameter.PLAYER_MOVE_HAND_SPEED_LIMIT * this._delta_time;
            Vector3 distance_from_hand_to_target =
                this._hand_holding_absolute_position - this._RobotController.get_holding_hand_absolute_position();

            distance_from_hand_to_target *= CommonParameter.PLAYER_HAND_HOLDING_POSITION_AVERAGING_WAIT;

            if (distance_from_hand_to_target.magnitude < speed_limit)
            {
                return_value = distance_from_hand_to_target;
            }
            else
            {
                return_value = Vector3.Normalize(distance_from_hand_to_target) * speed_limit;
            }
        }
        else
        {
            /* return zero. */
        }

        return return_value;
    }

    public SystemStructure.PLAYER_MODE get_player_mode()
    {
        return this._player_mode_state_machine.get_mode();
    }

    public Vector3 get_hand_holding_reference_absolute_position()
    {
        return this._hand_holding_absolute_position;
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
        this._robot_GameObject = GameObject.Find(CommonParameter.ROBOT_NAME);
        this._RobotController = this._robot_GameObject.GetComponent<RobotController>();

        this._EnvironmentController = GameObject.Find(CommonParameter.ENVIRONMENT_NAME).GetComponent<EnvironmentController>();
        this._player_GameObject = GameObject.Find(this._EnvironmentController.get_player_name());

        this._InputManager = GameObject.Find(CommonParameter.INPUT_MANAGER_NAME).GetComponent<InputManager>();

        this._player_mode_state_machine =
            new CommonStateMachine<SystemStructure.PLAYER_MODE>(CommonParameter.INITIAL_PLAYER_MODE);

        this._initialize_player_transform();

        this._initialize_neck_rotation();
    }

    /* Update is called once per frame */
    void Update()
    {
        this._delta_time = Time.deltaTime;

        this._switch_player_mode();

        this._get_reference();
        this._move_player();

        this._update_hand_holding_position();

        this._log_for_debug();
    }
    void LateUpdate()
    {
        this._draw_player();
    }

    /*********************************************************
     * Private functions
     *********************************************************/
    private void _get_reference()
    {
        SystemStructure.ST_FIRST_PERSON_DIRECTION_VELOCITY velocity_reference
            = this._InputManager.get_move_player_velocity();

        float yaw_angle = this._rotation_robot_axis.eulerAngles.z * Mathf.Deg2Rad;

        this._first_person_velocity_reference_relative.forward_backward =
            velocity_reference.forward_backward * Mathf.Cos(yaw_angle)
            - velocity_reference.left_right * Mathf.Sin(yaw_angle);

        this._first_person_velocity_reference_relative.left_right =
            + velocity_reference.forward_backward * Mathf.Sin(yaw_angle)
            + velocity_reference.left_right * Mathf.Cos(yaw_angle);

        this._first_person_velocity_reference_relative.turn_left_right =
            velocity_reference.turn_left_right;

        this._first_person_velocity_reference_relative.look_up_down =
            velocity_reference.look_up_down;
    }

    private void _move_player()
    {
        this._position_robot_axis.x += Time.deltaTime * this._move_velocity_factor
            * this._first_person_velocity_reference_relative.forward_backward;
        this._position_robot_axis.y += Time.deltaTime * this._move_velocity_factor
            * this._first_person_velocity_reference_relative.left_right;

        this._pitch_angle_robot_axis += Time.deltaTime * this._turn_velocity_factor
            * this._first_person_velocity_reference_relative.look_up_down;
        this._pitch_angle_robot_axis = Mathf.Clamp(this._pitch_angle_robot_axis,
            CommonParameter.PLAYER_PITCH_ANGLE_LIMIT_MIN, CommonParameter.PLAYER_PITCH_ANGLE_LIMIT_MAX);

        float yaw_angular_speed = Time.deltaTime * this._turn_velocity_factor
            * this._first_person_velocity_reference_relative.turn_left_right;

        Quaternion dif_rotation_yaw_robot_axis = Quaternion.Euler(
            0.0f, 0.0f, yaw_angular_speed * Mathf.Rad2Deg);
        this._yaw_rotation_robot_axis *= dif_rotation_yaw_robot_axis;

        this._rotation_robot_axis = this._yaw_rotation_robot_axis;
    }

    private void _draw_player()
    {
        Vector3 position_unity_axis = Vector3.zero;
        CommonTransform.transform_position_from_robot_to_unity(this._position_robot_axis, ref position_unity_axis);

        this._player_GameObject.transform.position = position_unity_axis;

        Quaternion rotation_unity_axis = Quaternion.identity;
        CommonTransform.transform_relative_rotation_from_robot_to_unity(this._rotation_robot_axis, ref rotation_unity_axis);

        this._player_GameObject.transform.rotation = rotation_unity_axis * Quaternion.Inverse(CommonParameter.AXIS_REMOVE_OFFSET_Q);

        Quaternion pitch_rotation_unity_axis = Quaternion.identity;
        CommonTransform.transform_relative_rotation_from_robot_to_unity(
            Quaternion.Euler(0.0f, this._pitch_angle_robot_axis * Mathf.Rad2Deg, 0.0f), ref pitch_rotation_unity_axis);

        Quaternion player_rotation_unity_axis = Quaternion.identity;
        CommonTransform.transform_relative_rotation_from_robot_to_unity(this._rotation_robot_axis, ref player_rotation_unity_axis);

        this._humanoid_component.Neck.rotation = 
            player_rotation_unity_axis * this._initial_head_rotation_unity_axis * pitch_rotation_unity_axis;
    }

    private void _initialize_player_transform()
    {
        CommonTransform.transform_position_from_unity_to_robot(this._player_GameObject.transform.position,
            ref this._position_robot_axis);

        Quaternion rotation_unity_axis = this._player_GameObject.transform.rotation * CommonParameter.AXIS_REMOVE_OFFSET_Q;
        CommonTransform.transform_relative_rotation_from_unity_to_robot(rotation_unity_axis,
            ref this._rotation_robot_axis);
        this._yaw_rotation_robot_axis = this._rotation_robot_axis;
    }

    private void _initialize_neck_rotation()
    {
        this._humanoid_component = this._player_GameObject.GetComponent<Humanoid>();

        Quaternion player_rotation_unity_axis = Quaternion.identity;
        CommonTransform.transform_relative_rotation_from_robot_to_unity(this._rotation_robot_axis, ref player_rotation_unity_axis);
        this._initial_head_rotation_unity_axis =
            Quaternion.Inverse(player_rotation_unity_axis) * this._humanoid_component.Neck.rotation;
    }

    private void _switch_player_mode()
    {
        Vector3 distance_vector_to_robot = this._RobotController.get_robot_base_position() - this._position_robot_axis;

        if (distance_vector_to_robot.magnitude > CommonParameter.PLAYER_IS_NEAR_ROBOT_DISTANCE)
        {
            this._player_mode_state_machine.update_mode(SystemStructure.PLAYER_MODE.FREE);
        }
        else
        {
            this._player_mode_state_machine.successive_switch_mode_and_update_elapsed_time(
                this._InputManager.get_player_hand_holding_flag(),
                CommonParameter.PLAYER_MODE_WAIT_TIME, Time.deltaTime);
        }
    }

    private void _update_hand_holding_position()
    {
        
        SystemStructure.ST_FIRST_PERSON_DIRECTION_VELOCITY move_hand_velocity =
            this._InputManager.get_move_hand_velocity();

        if (SystemStructure.PLAYER_MODE.HAND_HOLDING == this._player_mode_state_machine.get_mode())
        {
            Vector3 hand_holding_dif_position = Vector3.zero;

            hand_holding_dif_position.x = Time.deltaTime * CommonParameter.PLAYER_MOVE_HAND_SPEED_FACTOR
                * move_hand_velocity.forward_backward;
            hand_holding_dif_position.y = Time.deltaTime * CommonParameter.PLAYER_MOVE_HAND_SPEED_FACTOR
                * move_hand_velocity.left_right;
            hand_holding_dif_position.z = Time.deltaTime * CommonParameter.PLAYER_MOVE_HAND_SPEED_FACTOR
                * move_hand_velocity.up_down;

            this._hand_holding_relative_position += hand_holding_dif_position;
        }
        else
        {
            this._hand_holding_relative_position = CommonParameter.PLAYER_HAND_HOLDING_RELATIVE_POSITION;
        }

        this._hand_holding_absolute_position = this._position_robot_axis
           + this._rotation_robot_axis * this._hand_holding_relative_position;
    }

    private void _log_for_debug()
    {
        Vector3 euler_angle = this._rotation_robot_axis.eulerAngles;

        float pitch_angle = this._pitch_angle_robot_axis * Mathf.Rad2Deg;

        this._logText = "PlayerMode: " + this._player_mode_state_machine.get_mode().ToString() + Environment.NewLine
                      + "PlayerX: " + this._position_robot_axis.x.ToString() + Environment.NewLine
                      + "PlayerY: " + this._position_robot_axis.y.ToString() + Environment.NewLine
                      + "PlayerZ: " + this._position_robot_axis.z.ToString() + Environment.NewLine
                      + "PlayerHeadPitch: " + pitch_angle.ToString() + Environment.NewLine
                      + "PlayerYaw: " + euler_angle.z.ToString() + Environment.NewLine;
    }
}
