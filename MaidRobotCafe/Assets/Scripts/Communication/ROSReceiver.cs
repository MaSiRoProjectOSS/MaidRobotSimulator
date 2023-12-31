/**
 * @file ROSReceiver.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Receive data with ROS/ROS2.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using RosMessageTypes.MaidRobotSystemInterfaces;

namespace MaidRobotSimulator.MaidRobotCafe
{

    public class ROSReceiver
    {
        /*********************************************************
         * Private variables
         *********************************************************/
        private ROSConnection _ros;  /*!< ROS object */
        private MessageStructure.ST_GEOMETRY_MSGS_TWIST _move_velocity_reference =
            new MessageStructure.ST_GEOMETRY_MSGS_TWIST(
                new MessageStructure.ST_LINEAR(0.0, 0.0, 0.0),
                new MessageStructure.ST_ANGULAR(0.0, 0.0, 0.0));  /*!< reference velocity for moving */

        private bool _next_move_velocity_reference_received = false;  /*!< flag for next data received */

        private MessageStructure.ST_MRS_EYE _eye_reference =
            new MessageStructure.ST_MRS_EYE(
                0, 0, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);

        private MessageStructure.ST_MRS_LIP _lip_reference =
            new MessageStructure.ST_MRS_LIP(0);

        private MessageStructure.ST_MRS_NECK _neck_reference =
            new MessageStructure.ST_MRS_NECK(
                0.0f, 0.0f, 0.0f, 0.0f);

        private MessageStructure.ST_HEAD_STATUS _head_status =
            new MessageStructure.ST_HEAD_STATUS(false);

        private SystemStructure.ROS_ERROR_KIND _error = SystemStructure.ROS_ERROR_KIND.NONE;  /*!< error status */

        /*********************************************************
         * Constructor
         *********************************************************/
        public ROSReceiver()
        {
            ROSConnection.GetOrCreateInstance().Subscribe<TwistMsg>(
                CommonParameter.INPUT_MOVE_VELOCITY_REFERENCE_NAME, receive_move_velocity_reference_data);

            ROSConnection.GetOrCreateInstance().Subscribe<MrsEyeMsg>(
                CommonParameter.INPUT_EYE_REFERENCE_NAME, receive_eye_reference_data);

            ROSConnection.GetOrCreateInstance().Subscribe<MrsLipMsg>(
                CommonParameter.INPUT_LIP_REFERENCE_NAME, receive_lip_reference_data);

            ROSConnection.GetOrCreateInstance().Subscribe<MrsNeckMsg>(
                CommonParameter.INPUT_NECK_REFERENCE_NAME, receive_neck_reference_data);

            ROSConnection.GetOrCreateInstance().Subscribe<MrsHeadStatusMsg>(
                CommonParameter.INPUT_HEAD_STATUS_NAME, receive_head_status_data);
        }

        /*********************************************************
         * Public functions
         *********************************************************/
        public SystemStructure.ROS_ERROR_KIND get_error_kind()
        {
            return this._error;
        }

        public bool is_data_received()
        {
            return this._next_move_velocity_reference_received;
        }

        public void receive_move_velocity_reference_data(TwistMsg received_data)
        {
            this._move_velocity_reference.linear.x = received_data.linear.x;
            this._move_velocity_reference.linear.y = received_data.linear.y;
            this._move_velocity_reference.linear.z = received_data.linear.z;
            this._move_velocity_reference.angular.x = received_data.angular.x;
            this._move_velocity_reference.angular.y = received_data.angular.y;
            this._move_velocity_reference.angular.z = received_data.angular.z;

            this._next_move_velocity_reference_received = true;
        }

        public void receive_eye_reference_data(MrsEyeMsg received_data)
        {
            this._eye_reference.emotions = received_data.emotions;
            this._eye_reference.cornea_effect = received_data.cornea_effect;
            this._eye_reference.dimensions = received_data.dimensions;
            this._eye_reference.distance = received_data.distance;
            this._eye_reference.left_y = received_data.left_y;
            this._eye_reference.left_z = received_data.left_z;
            this._eye_reference.right_y = received_data.right_y;
            this._eye_reference.right_z = received_data.right_z;
        }

        public void receive_lip_reference_data(MrsLipMsg received_data)
        {
            this._lip_reference.percent = received_data.percent;
        }

        public void receive_neck_reference_data(MrsNeckMsg received_data)
        {
            this._neck_reference.x = received_data.x;
            this._neck_reference.y = received_data.y;
            this._neck_reference.z = received_data.z;
            this._neck_reference.w = received_data.w;
        }

        public void receive_head_status_data(MrsHeadStatusMsg received_data)
        {
            this._head_status.human_detected = received_data.human_detected;
        }

        public MessageStructure.ST_GEOMETRY_MSGS_TWIST get_move_velocity_reference()
        {
            this._next_move_velocity_reference_received = false;

            return this._move_velocity_reference;
        }

        public MessageStructure.ST_MRS_EYE get_eye_reference()
        {
            return this._eye_reference;
        }

        public MessageStructure.ST_MRS_LIP get_lip_reference()
        {
            return this._lip_reference;
        }

        public MessageStructure.ST_MRS_NECK get_neck_reference()
        {
            return this._neck_reference;
        }

        public MessageStructure.ST_HEAD_STATUS get_head_status()
        {
            return this._head_status;
        }

        /*********************************************************
         * Destructor
         *********************************************************/
        ~ROSReceiver()
        {
        }
    }

}
