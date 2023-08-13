/**
 * @file CarryObjectController.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Control Carry Object.
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

public class CarryObjectController : MonoBehaviour
{
    /*********************************************************
     * Private variables
     *********************************************************/
    private GameObject _RobotController_GameObject; /*!< Robot controller object */
    private GameObject _Tray_GameObject;            /*!< Tray object */

    private CommonParameter.OBJECTS_STATE _objects_state = CommonParameter.INITIAL_OBJECTS_STATE; /*!< object state */

    private float _elapsed_time_after_mode_changed = Mathf.Infinity; /*!< elapsed time after mode changed */

    private int _num_of_objects = 0;          /*!< number of objects */
    private GameObject[] _objects_GameObject; /*!< game object array */

    private CommonParameter.ST_CARRY_OBJECT_POSITION_AND_ID _carry_object_stay_place; /*!< place list to put the cup */

#if SHOW_STATUS_FOR_DEBUG
    private string _logText = ""; /*!< log text */
    private GUIStyle _guiStyle;   /*!< log GUI style */
#endif

    /*********************************************************
     * Public functions
     *********************************************************/
    public bool place_on_robot_tray()
    {
        bool return_value = false;

        if (are_objects_near_enough(this._Tray_GameObject.transform, this._objects_GameObject[0].transform))
        {

            if (this._elapsed_time_after_mode_changed > CommonParameter.OBJECTS_STATE_WAIT_TIME)
            {
                if (this._objects_state == CommonParameter.OBJECTS_STATE.STAY)
                {
                    this._objects_state = CommonParameter.OBJECTS_STATE.ON_TRAY;

                    this._elapsed_time_after_mode_changed = 0.0f;

                    return_value = true;
                }
            }
        }

        return return_value;
    }

    public bool place_on_nearest_table()
    {
        bool return_value = false;

        if (this._elapsed_time_after_mode_changed > CommonParameter.OBJECTS_STATE_WAIT_TIME)
        {
            if (this._objects_state == CommonParameter.OBJECTS_STATE.ON_TRAY)
            {

                float place_distance = Mathf.Infinity;
                float place_distance_min = Mathf.Infinity;
                int place_id = 0;

                for (int i = (int)CommonParameter.CARRY_OBJECT_PLACE.ON_FLASKET;
                     i <= (int)CommonParameter.CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_14;
                     i++)
                {
                    place_distance = Vector3.Distance(_objects_GameObject[0].transform.position,
                        CommonParameter.CARRY_OBJECT_POSITION_AND_ID[i].POSITION);
                    if (place_distance_min > place_distance)
                    {
                        place_distance_min = place_distance;
                        place_id = i;
                    }
                }

                if (place_distance_min <= CommonParameter.MAX_PICK_AND_PLACE_DISTANCE)
                {
                    this._objects_GameObject[0].transform.position =
                        CommonParameter.CARRY_OBJECT_POSITION_AND_ID[place_id].POSITION;
                    this._objects_state = CommonParameter.OBJECTS_STATE.STAY;
                    this._carry_object_stay_place = CommonParameter.CARRY_OBJECT_POSITION_AND_ID[place_id];

                    this._elapsed_time_after_mode_changed = 0.0f;

                    return_value = true;
                }
            }
        }

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
        this._RobotController_GameObject = GameObject.Find(CommonParameter.ROBOT_NAME);
        Transform Tray_Transform = this._RobotController_GameObject.transform.Find("Tray");
        this._Tray_GameObject = Tray_Transform.gameObject;

        this._objects_state = CommonParameter.OBJECTS_STATE.STAY;
        this._carry_object_stay_place =
            CommonParameter.CARRY_OBJECT_POSITION_AND_ID[(int)CommonParameter.CARRY_OBJECT_PLACE.ON_FLASKET];

        /* get child objects information */
        this._num_of_objects = transform.childCount;
        this._objects_GameObject = new GameObject[_num_of_objects];

        for (int i = 0; i < _num_of_objects; i++)
        {
            Transform childTransform = transform.GetChild(i);
            this._objects_GameObject[i] = childTransform.gameObject;
        }
    }

    /* Update is called once per frame */
    void Update()
    {
        if (this._objects_state == CommonParameter.OBJECTS_STATE.ON_TRAY)
        {
            this._objects_GameObject[0].transform.position = _Tray_GameObject.transform.position;
            this._objects_GameObject[0].transform.rotation = _Tray_GameObject.transform.rotation;
        }

        this._elapsed_time_after_mode_changed += Time.deltaTime;

#if SHOW_STATUS_FOR_DEBUG
        this._logText = "Object state: " + this._objects_state.ToString();
#endif
    }

    private void OnGUI()
    {
        /* Show the logged data in display */
#if SHOW_STATUS_FOR_DEBUG
        GUI.Label(CommonParameter.CARRY_OBJECT_CONTROLLER_DEBUG_TEXT_POS, this._logText, this._guiStyle);
#endif
    }

    private bool are_objects_near_enough(Transform from_object_transform, Transform to_object_transform)
    {
        bool return_value = false;

        float translational_distance = Vector3.Distance(from_object_transform.position, to_object_transform.position);

        if (translational_distance <= CommonParameter.MAX_PICK_AND_PLACE_DISTANCE)
        {
            return_value = true;
        }

        return return_value;
    }
}
