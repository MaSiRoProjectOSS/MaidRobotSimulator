/**
 * @file CarryObjectController.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Control Carry Object.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

using UnityEngine;
using UniHumanoid;
using MaidRobotSimulator.MaidRobotCafe;

public class CarryObjectController : MonoBehaviour
{
    /*********************************************************
     * Private variables
     *********************************************************/
    private GameObject _Robot_GameObject; /*!< Robot controller object */
    private RobotController _RobotController;
    private GameObject _Tray_GameObject;            /*!< Tray object */

    private CommonStateMachine<SystemStructure.OBJECTS_STATE> _objects_state_state_machine;       /*!< object state */

    private int _num_of_objects = 0;          /*!< number of objects */
    private GameObject[] _objects_GameObject; /*!< game object array */

    private SystemStructure.ST_CARRY_OBJECT_POSITION_AND_ID _carry_object_stay_place; /*!< place list to put the cup */

    private Vector3 _distance_from_right_hand_to_tray = Vector3.zero;

    private string _logText = ""; /*!< log text */

    /*********************************************************
     * Public functions
     *********************************************************/
    public bool place_on_robot_tray()
    {
        bool return_value = false;

        if (this._are_objects_near_enough(this._Tray_GameObject.transform, this._objects_GameObject[0].transform))
        {

            if (this._objects_state_state_machine.get_elapsed_time_after_mode_changed()
                    > CommonParameter.OBJECTS_STATE_WAIT_TIME)
            {
                if (SystemStructure.OBJECTS_STATE.STAY == this._objects_state_state_machine.get_mode())
                {
                    this._objects_state_state_machine.update_mode(SystemStructure.OBJECTS_STATE.ON_TRAY);
                    this._objects_state_state_machine.reset_elapsed_time();

                    return return_value = true;
                }
            }
        }

        return return_value;
    }

    public bool place_on_nearest_table()
    {
        bool return_value = false;

        if (this._objects_state_state_machine.get_elapsed_time_after_mode_changed()
                > CommonParameter.OBJECTS_STATE_WAIT_TIME)
        {
            if (SystemStructure.OBJECTS_STATE.ON_TRAY == this._objects_state_state_machine.get_mode())
            {
                float place_distance = Mathf.Infinity;
                float place_distance_min = Mathf.Infinity;
                int place_id = 0;

                for (int i = (int)SystemStructure.CARRY_OBJECT_PLACE.ON_FLASKET;
                     i <= (int)SystemStructure.CARRY_OBJECT_PLACE.FRONT_OF_CHAIR_14;
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

                    this._objects_state_state_machine.update_mode(SystemStructure.OBJECTS_STATE.STAY);
                    this._carry_object_stay_place = CommonParameter.CARRY_OBJECT_POSITION_AND_ID[place_id];

                    this._objects_state_state_machine.reset_elapsed_time();

                    return_value = true;
                }
            }
        }

        return return_value;
    }

    public void update_distance_from_right_hand_to_tray(Vector3 distance)
    {
        this._distance_from_right_hand_to_tray = distance;
    }

    public void destroy_tray()
    {
        this._Tray_GameObject.SetActive(false);
    }

    public void generate_tray()
    {
        this._Tray_GameObject.SetActive(true);
        this._update_tray_position();
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
        this._Robot_GameObject = GameObject.Find(CommonParameter.ROBOT_NAME);
        this._RobotController = this._Robot_GameObject.GetComponent<RobotController>();

        Transform Tray_Transform = this._Robot_GameObject.transform.Find(CommonParameter.TRAY_NAME);
        this._Tray_GameObject = Tray_Transform.gameObject;

        this._objects_state_state_machine = new CommonStateMachine<SystemStructure.OBJECTS_STATE>(SystemStructure.OBJECTS_STATE.STAY);

        if (SystemStructure.ROBOT_MODE.CATERING == this._RobotController.get_robot_mode())
        {
            this._carry_object_stay_place =
                CommonParameter.CARRY_OBJECT_POSITION_AND_ID[(int)SystemStructure.CARRY_OBJECT_PLACE.ON_FLASKET];

            /* get child objects information */
            this._num_of_objects = transform.childCount;
            this._objects_GameObject = new GameObject[_num_of_objects];

            for (int i = 0; i < _num_of_objects; i++)
            {
                Transform childTransform = transform.GetChild(i);
                this._objects_GameObject[i] = childTransform.gameObject;
            }
        }
        else if (SystemStructure.ROBOT_MODE.HAND_HOLDING == this._RobotController.get_robot_mode())
        {
            this._Tray_GameObject.SetActive(false);
        }
        else
        {
            /* abnormal mode */
        }
    }

    /* Update is called once per frame */
    void Update()
    {
        /* Update tray position */
        this._update_tray_position();

        /* Update objects position */
        this._update_objects_position();

        this._objects_state_state_machine.advance_elapsed_time(Time.deltaTime);

        this._log_for_debug();
    }

    private bool _are_objects_near_enough(Transform from_object_transform, Transform to_object_transform)
    {
        bool return_value = false;

        float translational_distance = Vector3.Distance(from_object_transform.position, to_object_transform.position);

        if (translational_distance <= CommonParameter.MAX_PICK_AND_PLACE_DISTANCE)
        {
            return_value = true;
        }

        return return_value;
    }

    private void _update_tray_position()
    {
        if (SystemStructure.ROBOT_MODE.CATERING == this._RobotController.get_robot_mode())
        {
            Humanoid humanoid_component = this._Robot_GameObject.GetComponent<Humanoid>();
            this._Tray_GameObject.transform.position =
                humanoid_component.RightHand.transform.position +
                this._Robot_GameObject.transform.rotation * this._distance_from_right_hand_to_tray;
        }
    }

    private void _update_objects_position()
    {
        if (SystemStructure.ROBOT_MODE.CATERING == this._RobotController.get_robot_mode())
        {
            if (SystemStructure.OBJECTS_STATE.ON_TRAY == this._objects_state_state_machine.get_mode())
            {
                this._objects_GameObject[0].transform.position = this._Tray_GameObject.transform.position;
                this._objects_GameObject[0].transform.rotation = this._Tray_GameObject.transform.rotation;
            }
        }
    }

    private void _log_for_debug()
    {
        this._logText = "Object state: " + this._objects_state_state_machine.get_mode().ToString();
    }
}
