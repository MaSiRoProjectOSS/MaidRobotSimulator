/**
 * @file ROSSender.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Send data with ROS/ROS2.
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

    public class ROSSender
    {
        /*********************************************************
         * Private variables
         *********************************************************/
        private ROSConnection _ros; /*!< ROS object */
        private PoseStampedMsg _robot_position_orientation = new PoseStampedMsg(); /*!< send data */

        private CommonParameter.ROS_ERROR_KIND _error = CommonParameter.ROS_ERROR_KIND.NONE; /*!< error status */

        /*********************************************************
         * Constructor
         *********************************************************/
        public ROSSender()
        {
            this._ros = ROSConnection.GetOrCreateInstance();
            this._ros.RegisterPublisher<PoseStampedMsg>(CommonParameter.ROS_INFORMATION_NAME);
        }

        /*********************************************************
         * Public functions
         *********************************************************/
        public CommonParameter.ROS_ERROR_KIND get_error_kind()
        {
            return this._error;
        }

        public void set_position_and_rotation(float[] position_in, float[] rotation_in)
        {
            this._robot_position_orientation.pose.position.x = position_in[0];
            this._robot_position_orientation.pose.position.y = position_in[1];
            this._robot_position_orientation.pose.position.z = position_in[2];
            this._robot_position_orientation.pose.orientation.w = rotation_in[0];
            this._robot_position_orientation.pose.orientation.x = rotation_in[1];
            this._robot_position_orientation.pose.orientation.y = rotation_in[2];
            this._robot_position_orientation.pose.orientation.z = rotation_in[3];
        }

        /*********************************************************
         * Destructor
         *********************************************************/
        public void send_data()
        {
            this._ros.Publish(CommonParameter.ROS_INFORMATION_NAME, this._robot_position_orientation);
        }

        ~ROSSender()
        {
        }
    }

}
