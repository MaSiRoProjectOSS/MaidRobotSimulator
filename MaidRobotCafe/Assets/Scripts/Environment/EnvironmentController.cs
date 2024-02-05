/**
 * @file EnvironmentController.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Control environment of the game.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

/* Comment below if you don't need debug texts. */
#define SHOW_STATUS_FOR_DEBUG

using UniHumanoid;
using UnityEngine;
using MaidRobotSimulator.MaidRobotCafe;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnvironmentController : MonoBehaviour
{
    /*********************************************************
     * Private variables
     *********************************************************/
    private GameObject _Robot_GameObject;
    private RobotController _RobotController;

    private GameObject _Player_GameObject;
    private PlayerController _PlayerController;

    private GameObject _AreaCamera_GameObject;
    private GameObject _AttachedCamera_GameObject;

    private GameObject _Hand_Pointer;
    private Renderer _Hand_Pointer_Renderer;

    private InputManager _InputManager;

    private CarryObjectController _CarryObjectController;
    private CommunicationReceiver _CommunicationReceiver;

    private Camera _robot_attached_camera;
    private Camera _area_fixed_camera;
    private Camera _player_camera;

    private CommonStateMachine<SystemStructure.CAMERA_MODE> _camera_mode_state_machine;

    private string _robot_name = CommonParameter.ROBOT_NAME;
    private string _player_name = "";

    private SystemStructure.SCENE_MODE _scene_mode = SystemStructure.SCENE_MODE.CAFE;

    private CommonStateMachine<SystemStructure.START_MENU_MODE> _open_menu_state_machine; /*!< open menu state */
    private CommonStateMachine<SystemStructure.PLAYER_CAMERA_MODE> _player_camera_mode;

    private GameObject _canvas_GameObject;
    private Image _how_to_use_gamepad_image;

    private CommonRateManager<float> _now_fps;

#if SHOW_STATUS_FOR_DEBUG
    private string _log_text_left_side = ""; /*!< log text */
    private GUIStyle _gui_style_left_side;   /*!< log GUI style */

    private string _log_text_right_side = ""; /*!< fps text */
    private GUIStyle _gui_style_right_side;   /*!< fps text GUI style */

    private Rect _right_side_debug_pos;
#endif

    /*********************************************************
     * MonoBehaviour functions
     *********************************************************/
    void Awake()
    {
        if (false == this._check_robot_is_default())
        {
            this._update_initial_robot_bones_parameter();
        }

        this._get_camera_class();

        this._check_current_scene();

        this._check_player_is_valid();

#if SHOW_STATUS_FOR_DEBUG
        this._gui_style_left_side = new GUIStyle();
        this._gui_style_left_side.fontSize = CommonParameter.DEBUG_TEXT_FONT_SIZE;
        this._gui_style_left_side.normal.textColor = Color.white;

        this._gui_style_right_side = new GUIStyle();
        this._gui_style_right_side.fontSize = CommonParameter.DEBUG_TEXT_FONT_SIZE;
        this._gui_style_right_side.normal.textColor = Color.white;

        this._right_side_debug_pos = CommonParameter.RIGHT_SIDE_DEBUG_TEXT_POS;
#endif
    }

    /* Start is called before the first frame update */
    void Start()
    {
        Application.targetFrameRate = CommonParameter.TARGET_GAME_FRAME_RATE;

        this._Robot_GameObject = GameObject.Find(CommonParameter.ROBOT_NAME);
        this._RobotController = this._Robot_GameObject.GetComponent<RobotController>();

        this._InputManager = GameObject.Find(CommonParameter.INPUT_MANAGER_NAME).GetComponent<InputManager>();

        this._AreaCamera_GameObject = GameObject.Find(CommonParameter.AREA_FIXED_CAMERA_NAME);
        this._AttachedCamera_GameObject = GameObject.Find(CommonParameter.ROBOT_ATTACHED_CAMERA_NAME);

        this._Hand_Pointer = GameObject.Find(CommonParameter.HAND_POINTER_NAME);
        this._Hand_Pointer_Renderer = this._Hand_Pointer.GetComponent<Renderer>();

        if (SystemStructure.SCENE_MODE.EVENT != this._scene_mode)
        {
            GameObject CarryObjectController_GameObject = GameObject.Find(CommonParameter.CARRY_OBJECTS_NAME);
            this._CarryObjectController = CarryObjectController_GameObject.GetComponent<CarryObjectController>();
        }

        GameObject CommunicationReceiver_GameObject = GameObject.Find(CommonParameter.COMMUNICATOR_NAME);
        this._CommunicationReceiver = CommunicationReceiver_GameObject.GetComponent<CommunicationReceiver>();

        this._camera_mode_state_machine = new CommonStateMachine<SystemStructure.CAMERA_MODE>(CommonParameter.CAMERA_MODE_INIT);

        this._open_menu_state_machine =
            new CommonStateMachine<SystemStructure.START_MENU_MODE>(SystemStructure.START_MENU_MODE.CLOSE);
        this._player_camera_mode =
            new CommonStateMachine<SystemStructure.PLAYER_CAMERA_MODE>(SystemStructure.PLAYER_CAMERA_MODE.NORMAL);

        this._canvas_GameObject = GameObject.Find(CommonParameter.CANVAS_NAME);
        this._how_to_use_gamepad_image = this._canvas_GameObject.transform.Find(CommonParameter.HOW_TO_USE_GAMEPAD_IMAGE_OBJECT_NAME).GetComponent<Image>();
        this._how_to_use_gamepad_image.enabled = false;

        this._now_fps = new CommonRateManager<float>(CommonParameter.FPS_DEBUG_TEXT_AVERAGE_BUFFER, 0.0f);

        this._switch_camera_position_and_angle();
    }

    /* Update is called once per frame */
    void Update()
    {
        this._camera_mode_state_machine.successive_switch_mode_and_update_elapsed_time(
            this._InputManager.get_camera_switch_flag(),
            CommonParameter.CAMERA_MODE_WAIT_TIME,
            Time.deltaTime);

        this._open_menu_state_machine.successive_switch_mode_and_update_elapsed_time(
            this._InputManager.get_start_button_flag(),
            CommonParameter.START_MENU_STATE_WAIT_TIME, Time.deltaTime);

        this._player_camera_mode.successive_switch_mode_and_update_elapsed_time(
            this._InputManager.get_select_button_flag(),
            CommonParameter.PLAYER_CAMERA_MODE_WAIT_TIME, Time.deltaTime);

        this._change_camera_mode_with_player();

        this._move_how_to_use_gamepad_image();

        this._show_hide_start_menu();

        this._switch_camera_position_and_angle();

        this._now_fps.set_moving_average_value(1.0f / Time.deltaTime);

        this._update_hand_pointer();

#if SHOW_STATUS_FOR_DEBUG
        if (SystemStructure.SCENE_MODE.EVENT != this._scene_mode)
        {
            this._log_text_left_side =
                this._RobotController.get_log_text() + Environment.NewLine + Environment.NewLine
                + this._CarryObjectController.get_log_text() + Environment.NewLine + Environment.NewLine
                + this._CommunicationReceiver.get_log_text();
        }
        else 
        {
            this._log_text_left_side =
                this._RobotController.get_log_text() + Environment.NewLine + Environment.NewLine
                + this._CommunicationReceiver.get_log_text();
        }

        this._log_text_right_side =
            "FPS: " + this._now_fps.get_moving_average_value().ToString() + Environment.NewLine + Environment.NewLine;

        if(null != this._PlayerController)
        {
            this._log_text_right_side += this._PlayerController.get_log_text() + Environment.NewLine + Environment.NewLine;
        }

        this._right_side_debug_pos.x =
            Screen.width - CommonParameter.RIGHT_SIDE_DEBUG_TEXT_OFFSET;
#endif
    }

    void OnGUI()
    {
        /* Show the logged data in display */
#if SHOW_STATUS_FOR_DEBUG
        GUI.Label(CommonParameter.LEFT_SIDE_DEBUG_TEXT_POS, this._log_text_left_side, this._gui_style_left_side);
        GUI.Label(this._right_side_debug_pos, this._log_text_right_side, this._gui_style_right_side);
#endif
    }

    /*********************************************************
     * Public functions
     *********************************************************/
    public SystemStructure.SCENE_MODE get_current_scene_mode()
    {
        return this._scene_mode;
    }

    public string get_player_name()
    {
        return this._player_name;
    }

    /*********************************************************
     * Private functions
     *********************************************************/
    private void _change_camera_mode_with_player()
    {
        if( (this._player_camera_mode.is_transition(
              SystemStructure.PLAYER_CAMERA_MODE.NORMAL, SystemStructure.PLAYER_CAMERA_MODE.FOCUS_TO_PLAYER_VIEW)) ||
              (this._open_menu_state_machine.is_transition(
                SystemStructure.START_MENU_MODE.CLOSE, SystemStructure.START_MENU_MODE.OPEN)) )
        {
            this._area_fixed_camera.enabled = false;
            
            this._player_camera.rect = CommonParameter.PLAYER_CAMERA_RECT_FOCUS_TO_PLAYER_VIEW;
        }
        else if ((this._player_camera_mode.is_transition(
                    SystemStructure.PLAYER_CAMERA_MODE.FOCUS_TO_PLAYER_VIEW, SystemStructure.PLAYER_CAMERA_MODE.NORMAL) && 
                    (SystemStructure.START_MENU_MODE.OPEN != this._open_menu_state_machine.get_mode()) ) ||
                 (SystemStructure.PLAYER_CAMERA_MODE.NORMAL == this._player_camera_mode.get_mode() &&
                    (this._open_menu_state_machine.is_transition(
                        SystemStructure.START_MENU_MODE.OPEN, SystemStructure.START_MENU_MODE.CLOSE)) ) )
        {
            this._area_fixed_camera.enabled = true;

            this._player_camera.rect = CommonParameter.PLAYER_CAMERA_RECT_NORMAL;
        }
    }

    private void _move_how_to_use_gamepad_image()
    {
        if (null != this._PlayerController)
        {
            this._how_to_use_gamepad_image.transform.position = this._player_camera.transform.position
                + this._player_camera.transform.rotation * CommonParameter.HOW_TO_USE_GAMEPAD_IMAGE_POSITION_OFFSET_UNITY_AXIS;
            this._how_to_use_gamepad_image.transform.rotation = this._player_camera.transform.rotation;
        }
    }

    private void _show_hide_start_menu()
    {
        if (this._open_menu_state_machine.is_transition(
                SystemStructure.START_MENU_MODE.CLOSE, SystemStructure.START_MENU_MODE.OPEN))
        {
            this._how_to_use_gamepad_image.enabled = true;
        }
        else if (this._open_menu_state_machine.is_transition(
                    SystemStructure.START_MENU_MODE.OPEN, SystemStructure.START_MENU_MODE.CLOSE))
        {
            this._how_to_use_gamepad_image.enabled = false;
        }
    }

    private void _switch_camera_position_and_angle()
    {
        if (this._camera_mode_state_machine.is_transition(SystemStructure.CAMERA_MODE.NEAR, SystemStructure.CAMERA_MODE.FAR))
        {
            this._AreaCamera_GameObject.transform.position =
                CommonParameter.AREA_CAMERA_POSITION_FAR;
            this._AreaCamera_GameObject.transform.rotation =
                Quaternion.Euler(CommonParameter.AREA_CAMERA_EULER_FAR);

            this._AttachedCamera_GameObject.transform.localPosition =
                CommonParameter.ATTACHED_CAMERA_POSITION_FAR;
            this._AttachedCamera_GameObject.transform.localRotation =
                Quaternion.Euler(CommonParameter.ATTACHED_CAMERA_EULER_FAR);
        }
        else if (this._camera_mode_state_machine.is_transition(SystemStructure.CAMERA_MODE.FAR, SystemStructure.CAMERA_MODE.NEAR))
        {
            this._AreaCamera_GameObject.transform.position =
                this._update_area_camera_near_position();
            this._AreaCamera_GameObject.transform.rotation =
                Quaternion.Euler(CommonParameter.AREA_CAMERA_EULER_NEAR);

            this._AttachedCamera_GameObject.transform.localPosition =
                CommonParameter.ATTACHED_CAMERA_POSITION_NEAR;
            this._AttachedCamera_GameObject.transform.localRotation =
                Quaternion.Euler(CommonParameter.ATTACHED_CAMERA_EULER_NEAR);
        }
    }

    private Vector3 _update_area_camera_near_position()
    {
        Vector3 next_position = 
            this._RobotController.transform.position + CommonParameter.AREA_CAMERA_POSITION_NEAR;

        return next_position;
    }

    private void _get_camera_class()
    {
        Camera[] all_cameras = Camera.allCameras;
        foreach (Camera camera in all_cameras)
        {
            if (camera.name.Equals(CommonParameter.ROBOT_ATTACHED_CAMERA_NAME))
            {
                this._robot_attached_camera = camera;
            }

            if (camera.name.Equals(CommonParameter.AREA_FIXED_CAMERA_NAME))
            {
                this._area_fixed_camera = camera;
            }

            if (camera.name.Equals(CommonParameter.PLAYER_CAMERA_NAME))
            {
                this._player_camera = camera;
            }
        }
    }

    private bool _check_robot_is_default()
    {
        bool return_value = false;
        RobotController[] robot_GameObjects = GameObject.FindObjectsOfType<RobotController>();

        if (robot_GameObjects.Length == 0)
        {
            throw new Exception("There is no robot in the scene.");
        }
        else if (robot_GameObjects.Length >= 2)
        {
            throw new Exception("There are more than one robot in the scene.");
        }
        else
        {
            this._robot_name = robot_GameObjects[0].name;

            if (this._robot_name.Equals(CommonParameter.ROBOT_NAME))
            {
                return_value = true;
            }
            else
            {
                CommonParameter.ROBOT_NAME = this._robot_name;
            }
        }

        return return_value;
    }

    private bool _check_player_is_valid()
    {
        bool return_value = false;
        PlayerController[] player_GameObjects = GameObject.FindObjectsOfType<PlayerController>();

        if (player_GameObjects.Length >= 2)
        {
            throw new Exception("There are more than one player in the scene.");
        }
        else if (player_GameObjects.Length == 1)
        {
            this._player_name = player_GameObjects[0].name;
            this._Player_GameObject = GameObject.Find(this._player_name);
            this._PlayerController = this._Player_GameObject.GetComponent<PlayerController>();

            GameObject player_camera = GameObject.Find(CommonParameter.PLAYER_CAMERA_NAME);
            if (null != player_camera)
            {
                this._robot_attached_camera.enabled = false;
                this._player_camera.enabled = true;
            }

            return_value = true;
        }
        else
        {
            /* Do Nothing */
        }

        return return_value;
    }

    private void _update_initial_robot_bones_parameter()
    {
        this._Robot_GameObject = GameObject.Find(this._robot_name);
        Humanoid humanoid_component = this._Robot_GameObject.GetComponent<Humanoid>();

        Quaternion hip_rotation = Quaternion.identity;
        CommonTransform.transform_relative_rotation_from_unity_to_robot(
            Quaternion.Inverse(this._Robot_GameObject.transform.rotation) * humanoid_component.Hips.rotation,
            ref hip_rotation);
        CommonParameter.HIP_ROTATION_INIT = hip_rotation;

        Quaternion spine_rotation = Quaternion.identity;
        CommonTransform.transform_relative_rotation_from_unity_to_robot(
            Quaternion.Inverse(this._Robot_GameObject.transform.rotation) * humanoid_component.Spine.rotation,
            ref spine_rotation);
        CommonParameter.SPINE_ROTATION_INIT = Quaternion.Inverse(hip_rotation) * spine_rotation;

        Quaternion chest_rotation = Quaternion.identity;
        CommonTransform.transform_relative_rotation_from_unity_to_robot(
            Quaternion.Inverse(this._Robot_GameObject.transform.rotation) * humanoid_component.Chest.rotation,
            ref chest_rotation);
        CommonParameter.CHEST_ROTATION_INIT = Quaternion.Inverse(spine_rotation) * chest_rotation;

        Quaternion upper_chest_rotation = Quaternion.identity;
        CommonTransform.transform_relative_rotation_from_unity_to_robot(
            Quaternion.Inverse(this._Robot_GameObject.transform.rotation) * humanoid_component.UpperChest.rotation,
            ref upper_chest_rotation);
        CommonParameter.UPPER_CHEST_ROTATION_INIT = Quaternion.Inverse(chest_rotation) * upper_chest_rotation;

        Quaternion right_shoulder_rotation = Quaternion.identity;
        CommonTransform.transform_relative_rotation_from_unity_to_robot(
            Quaternion.Inverse(this._Robot_GameObject.transform.rotation) * humanoid_component.RightShoulder.rotation,
            ref right_shoulder_rotation);
        CommonParameter.RIGHT_SHOULDER_ROTATION_INIT = right_shoulder_rotation;

        Quaternion left_shoulder_rotation = Quaternion.identity;
        CommonTransform.transform_relative_rotation_from_unity_to_robot(
            Quaternion.Inverse(this._Robot_GameObject.transform.rotation) * humanoid_component.LeftShoulder.rotation,
            ref left_shoulder_rotation);
        CommonParameter.LEFT_SHOULDER_ROTATION_INIT = left_shoulder_rotation;

        Quaternion neck_rotation = Quaternion.identity;
        CommonTransform.transform_relative_rotation_from_unity_to_robot(
            Quaternion.Inverse(this._Robot_GameObject.transform.rotation) * humanoid_component.Neck.rotation,
            ref neck_rotation);
        CommonParameter.NECK_ROTATION_INIT = Quaternion.Inverse(upper_chest_rotation) * neck_rotation;

        Quaternion head_rotation = Quaternion.identity;
        CommonTransform.transform_relative_rotation_from_unity_to_robot(
            Quaternion.Inverse(this._Robot_GameObject.transform.rotation) * humanoid_component.Head.rotation,
            ref head_rotation);
        CommonParameter.HEAD_ROTATION_INIT = Quaternion.Inverse(neck_rotation) * head_rotation;

        Quaternion right_eye_rotation = Quaternion.identity;
        CommonTransform.transform_relative_rotation_from_unity_to_robot(
            Quaternion.Inverse(this._Robot_GameObject.transform.rotation) * humanoid_component.RightEye.rotation,
            ref right_eye_rotation);
        CommonParameter.RIGHT_EYE_ROTATION_INIT = Quaternion.Inverse(head_rotation) * right_eye_rotation;

        Quaternion left_eye_rotation = Quaternion.identity;
        CommonTransform.transform_relative_rotation_from_unity_to_robot(
            Quaternion.Inverse(this._Robot_GameObject.transform.rotation) * humanoid_component.LeftEye.rotation,
            ref left_eye_rotation);
        CommonParameter.LEFT_EYE_ROTATION_INIT = Quaternion.Inverse(head_rotation) * left_eye_rotation;
    }

    private void _check_current_scene()
    {
        string current_scene_name = SceneManager.GetActiveScene().name;

        if (current_scene_name.Equals(CommonParameter.CAFE_SCENE_NAME))
        {
            this._scene_mode = SystemStructure.SCENE_MODE.CAFE;
        }
        else if (current_scene_name.Equals(CommonParameter.ROOM_SCENE_NAME))
        {
            this._scene_mode = SystemStructure.SCENE_MODE.ROOM;
        }
        else if (current_scene_name.Equals(CommonParameter.EVENT_SCENE_NAME))
        {
            this._scene_mode = SystemStructure.SCENE_MODE.EVENT;
        }
        else
        {
            throw new Exception("The scene name is not correct.");
        }
    }

    private void _update_hand_pointer()
    {
        if (null != this._PlayerController)
        {
            if (SystemStructure.PLAYER_MODE.HAND_HOLDING == this._PlayerController.get_player_mode())
            {
                this._Hand_Pointer_Renderer.enabled = true;

                this._move_hand_pointer(this._PlayerController.get_hand_holding_reference_absolute_position());
            }
            else
            {
                this._Hand_Pointer_Renderer.enabled = false;
            }
        }
        else
        {
            if (SystemStructure.ROBOT_MODE.HAND_HOLDING == this._RobotController.get_robot_mode())
            {
                this._Hand_Pointer_Renderer.enabled = true;

                this._move_hand_pointer(this._RobotController.get_robot_hand_reference_absolute_position());
            }
            else
            {
                this._Hand_Pointer_Renderer.enabled = false;
            }
        }
    }

    private void _move_hand_pointer(Vector3 pointer_reference_position)
    {
        Vector3 unity_position = Vector3.zero;
        CommonTransform.transform_position_from_robot_to_unity(
            pointer_reference_position,
            ref unity_position);

        this._Hand_Pointer.transform.position = unity_position;
    }
}
