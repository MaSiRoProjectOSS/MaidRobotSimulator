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

public class EnvironmentController : MonoBehaviour
{
    /*********************************************************
     * Private variables
     *********************************************************/
    private GameObject _Robot_GameObject;
    private RobotController _RobotController;

    private GameObject _AreaCamera_GameObject;
    private GameObject _AttachedCamera_GameObject;

    private KeyboardReceiver _KeyboardReceiver;
    private CarryObjectController _CarryObjectController;
    private CommunicationReceiver _CommunicationReceiver;

    private CommonStateMachine<SystemStructure.CAMERA_MODE> _camera_mode_state_machine;

    private string _robot_name = CommonParameter.ROBOT_NAME;

    private float _now_fps = 0.0f;
    private float _now_fps_LPF_weight = CommonParameter.FPS_DEBUG_TEXT_LPF_WEIGHT;

#if SHOW_STATUS_FOR_DEBUG
    private string _log_text = ""; /*!< log text */
    private GUIStyle _gui_style;   /*!< log GUI style */

    private string _fps_text = ""; /*!< fps text */
    private GUIStyle _fps_style;   /*!< fps text GUI style */
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

#if SHOW_STATUS_FOR_DEBUG
        this._gui_style = new GUIStyle();
        this._gui_style.fontSize = CommonParameter.DEBUG_TEXT_FONT_SIZE;
        this._gui_style.normal.textColor = Color.white;

        this._fps_style = new GUIStyle();
        this._fps_style.fontSize = CommonParameter.DEBUG_TEXT_FONT_SIZE;
        this._fps_style.normal.textColor = Color.white;

        this._right_side_debug_pos = CommonParameter.RIGHT_SIDE_DEBUG_TEXT_POS;
#endif
    }

    /* Start is called before the first frame update */
    void Start()
    {
        Application.targetFrameRate = CommonParameter.TARGET_GAME_FRAME_RATE;

        this._Robot_GameObject = GameObject.Find(CommonParameter.ROBOT_NAME);
        this._RobotController = this._Robot_GameObject.GetComponent<RobotController>();

        this._AreaCamera_GameObject = GameObject.Find(CommonParameter.AREA_FIXED_CAMERA_NAME);
        this._AttachedCamera_GameObject = GameObject.Find(CommonParameter.ROBOT_ATTACHED_CAMERA_NAME);

        GameObject KeyboardInput_GameObject = GameObject.Find(CommonParameter.KEYBOARD_INPUT_NAME);
        this._KeyboardReceiver = KeyboardInput_GameObject.GetComponent<KeyboardReceiver>();

        GameObject CarryObjectController_GameObject = GameObject.Find(CommonParameter.CARRY_OBJECTS_NAME);
        this._CarryObjectController = CarryObjectController_GameObject.GetComponent<CarryObjectController>();

        GameObject CommunicationReceiver_GameObject = GameObject.Find(CommonParameter.COMMUNICATOR_NAME);
        this._CommunicationReceiver = CommunicationReceiver_GameObject.GetComponent<CommunicationReceiver>();

        this._camera_mode_state_machine = new CommonStateMachine<SystemStructure.CAMERA_MODE>(CommonParameter.CAMERA_MODE_INIT);

        this._switch_camera_position_and_angle();
    }

    /* Update is called once per frame */
    void Update()
    {
        this._camera_mode_state_machine.successive_switch_mode_and_update_elapsed_time(
            this._KeyboardReceiver.check_camera_switch_flag(),
            CommonParameter.CAMERA_MODE_WAIT_TIME,
            Time.deltaTime);

        this._switch_camera_position_and_angle();

        this._now_fps = (1.0f - this._now_fps_LPF_weight) * this._now_fps
                        + this._now_fps_LPF_weight * (1.0f / Time.deltaTime);

#if SHOW_STATUS_FOR_DEBUG
        this._log_text = this._KeyboardReceiver.get_log_text() + Environment.NewLine + Environment.NewLine
                       + this._RobotController.get_log_text() + Environment.NewLine + Environment.NewLine
                       + this._CarryObjectController.get_log_text() + Environment.NewLine + Environment.NewLine
                       + this._CommunicationReceiver.get_log_text();

        this._fps_text = "FPS: " + this._now_fps.ToString();

        this._right_side_debug_pos.x =
            Screen.width - CommonParameter.RIGHT_SIDE_DEBUG_TEXT_OFFSET;
#endif
    }

    void OnGUI()
    {
        /* Show the logged data in display */
#if SHOW_STATUS_FOR_DEBUG
        GUI.Label(CommonParameter.LEFT_SIDE_DEBUG_TEXT_POS, _log_text, _gui_style);
        GUI.Label(this._right_side_debug_pos, _fps_text, _fps_style);
#endif
    }

    /*********************************************************
     * Private functions
     *********************************************************/
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
}
