/**
 * @file RobotController.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Control Robot.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

using System;
using UnityEngine;
using UniHumanoid;
using MaidRobotSimulator.MaidRobotCafe;

public class RobotController : MonoBehaviour
{
    /*********************************************************
     * Public variables
     *********************************************************/
    public bool freeze_camera_function = CommonParameter.CAMERA_DEBUG_FREEZE_CAMERA_FUNCTION;

    /*********************************************************
     * Private variables
     *********************************************************/
    private GameObject _Robot_GameObject;
    private GameObject _Tray_GameObject;

    private EnvironmentController _EnvironmentController;

    private InputManager _InputManager;                    /*!< Input Manager object */

    private CarryObjectController _CarryObjectController;  /*!< Carry Object Controller object */
    private Animator _animator;                            /*!< Animator object */
    private Humanoid _humanoid_component;

    private CommunicationReceiver _CommunicationReceiver;                    /*!< CommReceiver object */
    private CommunicationSender _CommunicationSender;                        /*!< CommunicationSender object */

    private HeadUnitController _HeadUnitController;            /*!< HeadUnitController object */
    private WaistDownUnitController _WaistDownUnitController;  /*!< WaistDownUnitController object */
    private ArmUnitController _ArmUnitController;              /*!< ArmUnitController object */

    private InverseKinematicsManager _InverseKinematicsManager;

    private CommonStateMachine<SystemStructure.ROBOT_MODE> _robot_mode_state_machine;
    private CommonStateMachine<SystemStructure.ROBOT_CARRYING_STATE> _carrying_state_state_machine; /*!< carrying state */

    private SystemStructure.ST_8_DIRECTION_MOVE _robot_direction_input =
        new SystemStructure.ST_8_DIRECTION_MOVE(
            false, false, false, false, false, false, false, false); /*!< direction input */

    private Vector3 _IK_robot_hand_relative_position_robot_axis = Vector3.zero;

    private bool _collide_flag = false;       /*!< collide flag */
    private string _collide_object_name = ""; /*!< names of collided object */

    private string _logText = ""; /*!< log text */

    /*********************************************************
     * Public functions
     *********************************************************/
    public bool get_collide_flag()
    {
        return this._collide_flag;
    }

    public SystemStructure.ROBOT_MODE get_robot_mode()
    {
        return this._robot_mode_state_machine.get_mode();
    }

    public InverseKinematicsManager get_inverse_kinematics_manager()
    {
        return this._InverseKinematicsManager;
    }

    public Vector3 get_robot_base_position()
    {
        return this._WaistDownUnitController.get_current_position();
    }

    public Quaternion get_robot_base_rotation()
    {
        return this._WaistDownUnitController.get_current_rotation();
    }

    public Vector3 get_holding_hand_absolute_position()
    {
        return this._ArmUnitController.get_holding_hand_absolute_position();
    }

    public string get_log_text()
    {
        return this._logText;
    }

    public Vector3 get_robot_hand_reference_absolute_position()
    {
        Vector3 absolute_position = Vector3.zero;

        absolute_position = this._WaistDownUnitController.get_current_position()
            + this._WaistDownUnitController.get_current_rotation() * this._IK_robot_hand_relative_position_robot_axis;

        return absolute_position;
    }

    /*********************************************************
     * MonoBehaviour functions
     *********************************************************/
    void Awake()
    {
        this._robot_mode_state_machine = new CommonStateMachine<SystemStructure.ROBOT_MODE>(CommonParameter.INITIAL_ROBOT_MODE);
    }

    /* Start is called before the first frame update */
    void Start()
    {
        this._EnvironmentController = GameObject.Find(CommonParameter.ENVIRONMENT_NAME).GetComponent<EnvironmentController>();

        this._InputManager = GameObject.Find(CommonParameter.INPUT_MANAGER_NAME).GetComponent<InputManager>();

        if (SystemStructure.SCENE_MODE.EVENT == this._EnvironmentController.get_current_scene_mode())
        {
            this._robot_mode_state_machine.update_mode(SystemStructure.ROBOT_MODE.HAND_HOLDING);
        }

        this._Robot_GameObject = GameObject.Find(CommonParameter.ROBOT_NAME);

        this._animator = this._Robot_GameObject.GetComponent<Animator>();

        GameObject Communicator_GameObject = GameObject.Find(CommonParameter.COMMUNICATOR_NAME);
        this._CommunicationReceiver = Communicator_GameObject.GetComponent<CommunicationReceiver>();
        this._CommunicationSender = Communicator_GameObject.GetComponent<CommunicationSender>();

        this._HeadUnitController = new HeadUnitController();
        this._WaistDownUnitController = new WaistDownUnitController();
        this._ArmUnitController = new ArmUnitController(this._WaistDownUnitController);

        this._InverseKinematicsManager = new InverseKinematicsManager(
            SystemStructure.ROBOT_HAND_HOLDING_SIDE.RIGHT,
            this._ArmUnitController);

        this._WaistDownUnitController.update_orientation(
            this.transform.position, this.transform.rotation);

        this._animator.SetFloat(CommonParameter.ANIMATOR_SPEED_NAME, CommonParameter.WALKING_MOTION_SPEED);

        this._carrying_state_state_machine = 
            new CommonStateMachine<SystemStructure.ROBOT_CARRYING_STATE>(SystemStructure.ROBOT_CARRYING_STATE.NONE);

        /* Set initial camera image */
        this._HeadUnitController.take_images_from_eyes(Time.deltaTime);
        this._CommunicationSender.set_eyes_camera_images(
            this._HeadUnitController.get_current_right_eye_image(),
            this._HeadUnitController.get_current_left_eye_image());

        this._humanoid_component = this._Robot_GameObject.GetComponent<Humanoid>();

        this._WaistDownUnitController.draw_hip_rotation(ref this._Robot_GameObject, this.transform.rotation);

        if (SystemStructure.SCENE_MODE.EVENT != this._EnvironmentController.get_current_scene_mode())
        {
            GameObject CarryObjectController_GameObject = GameObject.Find(CommonParameter.CARRY_OBJECTS_NAME);
            this._CarryObjectController = CarryObjectController_GameObject.GetComponent<CarryObjectController>();

            Transform Tray_Transform = this._Robot_GameObject.transform.Find(CommonParameter.TRAY_NAME);
            this._Tray_GameObject = Tray_Transform.gameObject;

            this._adjust_tray_position(ref Tray_Transform);
        }

        /* Coroutine */
        StartCoroutine(this._HeadUnitController.excite_blink_event_randomly());
    }

    void OnAnimatorIK()
    {
        if (SystemStructure.ROBOT_MODE.HAND_HOLDING == this._robot_mode_state_machine.get_mode())
        {
            this._InverseKinematicsManager.calculate_IK_and_draw_arm_rotation(
                this._IK_robot_hand_relative_position_robot_axis, this._WaistDownUnitController);
        }
        else
        {
            this._ArmUnitController.draw_arm_rotation(ref this._Robot_GameObject);
        }

        this._HeadUnitController.draw_head_rotation(ref this._Robot_GameObject);
        this._HeadUnitController.draw_eyes_rotation(ref this._Robot_GameObject);
    }

    /* Update is called once per frame */
    void Update()
    {
        this._check_and_switch_robot_mode();

        this._update_robot_move_reference();

        /* Waist Down Unit (move robot) */
        this._WaistDownUnitController.move_robot_position_rotation(
            Time.deltaTime, ref this._Robot_GameObject);

        this._animator.SetFloat(CommonParameter.MOTION_SPEED_NAME,
            this._WaistDownUnitController.get_current_velocity_magnitude() *
                CommonParameter.WALKING_MOTION_MPS_TO_ANIMATION_FACTOR);

        this._pick_and_place_objects();

        this._CommunicationSender.set_robot_position(this._WaistDownUnitController.get_current_position());
        this._CommunicationSender.set_robot_rotation(this._WaistDownUnitController.get_current_rotation());

        /* Head Unit (move head and eye) */
        this._HeadUnitController.set_head_status(this._CommunicationReceiver.get_head_status());

        this._HeadUnitController.update_head_and_eyes_rotation(Time.deltaTime,
            this._CommunicationReceiver.get_neck_reference(),
            this._CommunicationReceiver.get_eye_reference());
        this._HeadUnitController.update_eyes_blink(Time.deltaTime);

        if (false == freeze_camera_function)
        {
            this._HeadUnitController.take_images_from_eyes(Time.deltaTime);
            this._CommunicationSender.set_eyes_camera_images(
                this._HeadUnitController.get_current_right_eye_image(),
                this._HeadUnitController.get_current_left_eye_image());
        }

        /* Arm Unit (move arm) */
        this._update_arm_position(Time.deltaTime);

        if (SystemStructure.ROBOT_MODE.HAND_HOLDING == this._robot_mode_state_machine.get_mode())
        {
            this._update_hand_position();
        }

        this._set_hand_position_for_hand_holding();

        /* Tray */
        this._update_tray_appearance();


        this._log_for_debug();
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

    void LateUpdate()
    {
        Humanoid humanoid_component = this._Robot_GameObject.GetComponent<Humanoid>();

        this._WaistDownUnitController.draw_hip_rotation(ref this._Robot_GameObject, this.transform.rotation);
    }

    /*********************************************************
     * Private functions
     *********************************************************/
    private void _adjust_tray_position(ref Transform Tray_Transform)
    {
       Vector3 right_hand_position_unity = Vector3.zero;
        CommonTransform.transform_position_from_robot_to_unity(
            this._ArmUnitController.get_right_hand_absolute_position(), ref right_hand_position_unity);

        Vector3 distance = Quaternion.Inverse(this._Robot_GameObject.transform.rotation) *
            (Tray_Transform.position - right_hand_position_unity);
        this._CarryObjectController.update_distance_from_right_hand_to_tray(distance);
    }

    private void _update_robot_move_reference()
    {
        Vector3 velocity_unity_axis = CommonParameter.TRANSLATIONAL_VELOCITY_INIT;
        Vector3 angular_velocity_unity_axis = CommonParameter.ANGULAR_VELOCITY_INIT;

        this._InputManager.get_robot_direction_input(ref this._robot_direction_input);

        SystemStructure.INPUT_MODE current_input_mode = this._InputManager.get_current_input_mode();
        switch (current_input_mode)
        {
            case SystemStructure.INPUT_MODE.COMMUNICATION:
                velocity_unity_axis.x = 0.0f;
                velocity_unity_axis.y = 0.0f;
                velocity_unity_axis.z =
                    Mathf.Clamp(this._CommunicationReceiver.get_forward_velocity(),
                    CommonParameter.TRANSLATIONAL_VELOCITY_LIMIT_MIN, CommonParameter.TRANSLATIONAL_VELOCITY_LIMIT_MAX);
                angular_velocity_unity_axis.x = 0.0f;
                angular_velocity_unity_axis.y =
                    Mathf.Clamp(-this._CommunicationReceiver.get_yaw_angular_velocity(),
                    CommonParameter.ANGULAR_VELOCITY_LIMIT_MIN, CommonParameter.ANGULAR_VELOCITY_LIMIT_MAX);
                angular_velocity_unity_axis.z = 0.0f;
                break;

            case SystemStructure.INPUT_MODE.KEYBOARD:
                velocity_unity_axis = Vector3.zero;
                angular_velocity_unity_axis = Vector3.zero;

                if (this._robot_direction_input.forward)
                {
                    velocity_unity_axis.z += CommonParameter.WALKING_MOTION_SPEED;
                }
                if (this._robot_direction_input.backward)
                {
                    velocity_unity_axis.z -= CommonParameter.WALKING_MOTION_SPEED;
                }
                if (this._robot_direction_input.right)
                {
                    angular_velocity_unity_axis.y += CommonParameter.WALKING_ROTATION_SPEED;
                }
                if (this._robot_direction_input.left)
                {
                    angular_velocity_unity_axis.y -= CommonParameter.WALKING_ROTATION_SPEED;
                }
                break;

            default:
                break;
        }

        this._WaistDownUnitController.set_reference_translational_velocity_unity_axis(velocity_unity_axis);
        this._WaistDownUnitController.set_reference_angular_velocity_unity_axis(angular_velocity_unity_axis);
    }

    private void _update_hand_position()
    {
        this._IK_robot_hand_relative_position_robot_axis =
            this._InverseKinematicsManager.get_hand_position_for_IK();       
    }

    private void _pick_and_place_objects()
    {
        if (SystemStructure.ROBOT_MODE.CATERING == this._robot_mode_state_machine.get_mode())
        {
            SystemStructure.ROBOT_CARRYING_STATE next_state = this._carrying_state_state_machine.get_mode();

            if (this._InputManager.check_place_on_robot_tray_flag())
            {
                if (this._CarryObjectController.place_on_robot_tray())
                {
                    next_state = SystemStructure.ROBOT_CARRYING_STATE.CARRY_TEACUP;
                }
            }
            if (this._InputManager.check_place_on_nearest_table_flag())
            {
                if (this._CarryObjectController.place_on_nearest_table())
                {
                    next_state = SystemStructure.ROBOT_CARRYING_STATE.NONE;
                }
            }

            this._carrying_state_state_machine.update_mode(next_state);
        }
    }

    private void _check_and_switch_robot_mode()
    {
        if(SystemStructure.SCENE_MODE.EVENT == this._EnvironmentController.get_current_scene_mode())
        {
            this._robot_mode_state_machine.update_mode(SystemStructure.ROBOT_MODE.HAND_HOLDING);
        }
        else
        {
            this._robot_mode_state_machine.successive_switch_mode_and_update_elapsed_time(
                this._InputManager.check_robot_mode_switch_flag(),
                CommonParameter.ROBOT_MODE_WAIT_TIME, Time.deltaTime);
        }
    }

    private void _update_arm_position(float delta_time)
    {
        Quaternion upper_arm_rotation = Quaternion.identity;
        Quaternion lower_arm_rotation = Quaternion.identity;

        if (this._robot_mode_state_machine.is_transition(
               SystemStructure.ROBOT_MODE.CATERING, SystemStructure.ROBOT_MODE.HAND_HOLDING))
        {
            this._ArmUnitController.initialize_arm_and_hand_angles();
            this._ArmUnitController.initialize_arm_position_for_hand_holding(this._WaistDownUnitController);
            this._InverseKinematicsManager.initialize_arm_rotation();
        }
        else if (this._robot_mode_state_machine.is_transition(
                SystemStructure.ROBOT_MODE.HAND_HOLDING, SystemStructure.ROBOT_MODE.CATERING))
        {
            this._ArmUnitController.initialize_arm_and_hand_angles();
        }

        if (this._robot_mode_state_machine.is_during(SystemStructure.ROBOT_MODE.HAND_HOLDING))
        {
            this._ArmUnitController.move_holding_hand(this._humanoid_component, delta_time);
            this._InverseKinematicsManager.read_arm_rotation_after_IK_calculation(
                ref upper_arm_rotation, ref lower_arm_rotation);
            this._ArmUnitController.update_hand_holding_arm_rotation(
                upper_arm_rotation, lower_arm_rotation);
        }
    }

    private void _set_hand_position_for_hand_holding()
    {
        if (SystemStructure.ROBOT_MODE.HAND_HOLDING == this._robot_mode_state_machine.get_mode())
        {
            this._CommunicationSender.set_hand_position(
            this._ArmUnitController.get_hand_position_for_hand_holding());
        }
        else
        {
            this._CommunicationSender.set_hand_position(Vector3.zero);
        }
    }

    private void _update_tray_appearance()
    {
        if (SystemStructure.SCENE_MODE.EVENT != this._EnvironmentController.get_current_scene_mode())
        {
            if (this._robot_mode_state_machine.is_transition(
                 SystemStructure.ROBOT_MODE.CATERING, SystemStructure.ROBOT_MODE.HAND_HOLDING))
            {
                this._CarryObjectController.destroy_tray();
            }
            else if
                (this._robot_mode_state_machine.is_transition(
                    SystemStructure.ROBOT_MODE.HAND_HOLDING, SystemStructure.ROBOT_MODE.CATERING))
            {
                this._CarryObjectController.generate_tray();
            }
        }
    }

    /* This function is for debug. */
    private void _check_relative_rotation_from_bone_to_bone(ref Quaternion relative_rotation)
    {
        Humanoid humanoid_component = this._Robot_GameObject.GetComponent<Humanoid>();

        CommonTransform.transform_relative_rotation_from_unity_to_robot(
            Quaternion.Inverse(humanoid_component.Neck.rotation) * humanoid_component.Head.rotation,
            ref relative_rotation);
    }

    private void _log_for_debug()
    {
        Vector3 position = this._WaistDownUnitController.get_current_position();
        //Vector3 euler_angle = this._WaistDownUnitController.get_current_euler_angle();
        Vector3 hand_position = this._ArmUnitController.get_hand_position_for_hand_holding();

        Quaternion head_rotation = this._HeadUnitController.get_head_rotation();
        Vector3 head_angle = head_rotation.eulerAngles;

        Vector3 upper_arm_euler_angles = this._InverseKinematicsManager.get_upper_arm_rotation_IK().eulerAngles;
        Vector3 lower_arm_euler_angles = this._InverseKinematicsManager.get_lower_arm_rotation_IK().eulerAngles;

        this._logText = "InputMode: " + this._InputManager.get_current_input_mode().ToString() + Environment.NewLine
                      + "RobotX: " + position.x.ToString() + Environment.NewLine
                      + "RobotY: " + position.y.ToString() + Environment.NewLine
                      + "RobotZ: " + position.z.ToString() + Environment.NewLine
                      + "HeadRoll: " + head_angle.x.ToString() + Environment.NewLine
                      + "HeadPitch: " + head_angle.y.ToString() + Environment.NewLine
                      + "HeadYaw: " + head_angle.z.ToString() + Environment.NewLine
                      + "HandX: " + hand_position.x.ToString() + Environment.NewLine
                      + "HandY: " + hand_position.y.ToString() + Environment.NewLine
                      + "HandZ: " + hand_position.z.ToString() + Environment.NewLine
                      + "UpperArmRoll: " + upper_arm_euler_angles.x.ToString() + Environment.NewLine
                      + "UpperArmPitch: " + upper_arm_euler_angles.y.ToString() + Environment.NewLine
                      + "UpperArmYaw: " + upper_arm_euler_angles.z.ToString() + Environment.NewLine
                      + "LowerArmRoll: " + lower_arm_euler_angles.x.ToString() + Environment.NewLine
                      + "LowerArmPitch: " + lower_arm_euler_angles.y.ToString() + Environment.NewLine
                      + "LowerArmYaw: " + lower_arm_euler_angles.z.ToString() + Environment.NewLine
                      + "RightEyeProcess: "
                        + this._HeadUnitController.get_right_eye_image_process_rate().ToString() + Environment.NewLine
                      + "LeftEyeProcess: "
                        + this._HeadUnitController.get_left_eye_image_process_rate().ToString() + Environment.NewLine
                      + "CarryingState: " + this._carrying_state_state_machine.get_mode().ToString() + Environment.NewLine
                      + "RobotMode: " + this._robot_mode_state_machine.get_mode().ToString() + Environment.NewLine
                      + "NeckPoseMode: " + this._HeadUnitController.get_neck_pose_mode().ToString();

        if (this._collide_flag)
        {
            this._logText += Environment.NewLine + "Collided: " + this._collide_object_name;
        }
    }
}
