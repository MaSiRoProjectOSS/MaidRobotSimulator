/**
 * @file ROSReceiver.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Receive data with ROS/ROS2.
 * @version 1.0.0
 * @date 2023-08-05
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

namespace MaidRobotSimulator.MaidRobotCafe
{

    public class ROSReceiver
    {
        /*********************************************************
         * Private variables
         *********************************************************/
        private ROSConnection _ros;  /*!< ROS object */
        private ROSStructure.ST_GEOMETRY_MSGS_TWIST _move_velocity_reference =
            new ROSStructure.ST_GEOMETRY_MSGS_TWIST(
                new ROSStructure.ST_LINEAR(0.0, 0.0, 0.0),
                new ROSStructure.ST_ANGULAR(0.0, 0.0, 0.0));  /*!< reference velocity for moving */

        private bool _next_data_received = false;  /*!< flag for next data received */

        private CommonParameter.ROS_ERROR_KIND _error = CommonParameter.ROS_ERROR_KIND.NONE;  /*!< error status */

        /*********************************************************
         * Constructor
         *********************************************************/
        public ROSReceiver()
        {
            ROSConnection.GetOrCreateInstance().Subscribe<TwistMsg>(
                CommonParameter.ROS_REFERENCE_NAME, receive_data);
        }

        /*********************************************************
         * Public functions
         *********************************************************/
        public CommonParameter.ROS_ERROR_KIND get_error_kind()
        {
            return this._error;
        }

        public bool is_data_received()
        {
            return this._next_data_received;
        }

        public void receive_data(TwistMsg received_data)
        {
            this._move_velocity_reference.linear.x = received_data.linear.x;
            this._move_velocity_reference.linear.y = received_data.linear.y;
            this._move_velocity_reference.linear.z = received_data.linear.z;
            this._move_velocity_reference.angular.x = received_data.angular.x;
            this._move_velocity_reference.angular.y = received_data.angular.y;
            this._move_velocity_reference.angular.z = received_data.angular.z;

            this._next_data_received = true;
        }

        public ROSStructure.ST_GEOMETRY_MSGS_TWIST get_move_velocity_reference()
        {
            this._next_data_received = false;

            return this._move_velocity_reference;
        }

        /*********************************************************
         * Destructor
         *********************************************************/
        ~ROSReceiver()
        {
        }
    }

}
