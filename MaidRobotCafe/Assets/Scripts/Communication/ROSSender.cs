/**
 * @file ROSSender.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Send data with ROS/ROS2.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;

namespace MaidRobotSimulator.MaidRobotCafe
{
    public class ROSSender
    {
        /*********************************************************
         * Private variables
         *********************************************************/
        private ROSConnection _ros; /*!< ROS object */
        private PoseStampedMsg _robot_position_orientation = new PoseStampedMsg(); /*!< send data */
        private PointMsg _hand_position = new PointMsg(); /*!< send data */
        private ImageMsg _right_eye_camera_image = new ImageMsg(); /*!< send eye image data */
        private ImageMsg _left_eye_camera_image = new ImageMsg(); /*!< send eye image data */

        private SystemStructure.ROS_ERROR_KIND _error = SystemStructure.ROS_ERROR_KIND.NONE; /*!< error status */

        /*********************************************************
         * Constructor
         *********************************************************/
        public ROSSender()
        {
            this._ros = ROSConnection.GetOrCreateInstance();
            this._ros.RegisterPublisher<PoseStampedMsg>(CommonParameter.OUTPUT_POSITION_ROTATION_NAME);
            this._ros.RegisterPublisher<PointMsg>(CommonParameter.OUTPUT_HAND_POSITION_NAME);
            this._ros.RegisterPublisher<ImageMsg>(CommonParameter.OUTPUT_RIGHT_EYE_CAMERA_IMAGE_NAME);
            this._ros.RegisterPublisher<ImageMsg>(CommonParameter.OUTPUT_LEFT_EYE_CAMERA_IMAGE_NAME);
        }

        /*********************************************************
         * Public functions
         *********************************************************/
        public SystemStructure.ROS_ERROR_KIND get_error_kind()
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

        public void set_hand_position(float[] position_in)
        {
            this._hand_position.x = position_in[0];
            this._hand_position.y = position_in[1];
            this._hand_position.z = position_in[2];
        }

        public void set_eye_images(
            MessageStructure.ST_SENSOR_MSGS_IMAGE right_eye_camera_image,
            MessageStructure.ST_SENSOR_MSGS_IMAGE left_eye_camera_image)
        {
            /* header */
            this._right_eye_camera_image.header.stamp.sec = (int)right_eye_camera_image.header.stamp.sec;
            this._right_eye_camera_image.header.stamp.nanosec = (uint)right_eye_camera_image.header.stamp.nsec;
            this._right_eye_camera_image.header.frame_id = right_eye_camera_image.header.frame_id;

            this._left_eye_camera_image.header.stamp.sec = (int)left_eye_camera_image.header.stamp.sec;
            this._left_eye_camera_image.header.stamp.nanosec = (uint)left_eye_camera_image.header.stamp.nsec;
            this._left_eye_camera_image.header.frame_id = left_eye_camera_image.header.frame_id;

            /* data */
            this._right_eye_camera_image.height = (uint)right_eye_camera_image.height;
            this._right_eye_camera_image.width = (uint)right_eye_camera_image.width;
            this._right_eye_camera_image.encoding = right_eye_camera_image.encoding;
            this._right_eye_camera_image.is_bigendian = right_eye_camera_image.is_bigendian;
            this._right_eye_camera_image.step = (uint)right_eye_camera_image.step;
            this._right_eye_camera_image.data = right_eye_camera_image.data;

            this._left_eye_camera_image.height = (uint)left_eye_camera_image.height;
            this._left_eye_camera_image.width = (uint)left_eye_camera_image.width;
            this._left_eye_camera_image.encoding = left_eye_camera_image.encoding;
            this._left_eye_camera_image.is_bigendian = left_eye_camera_image.is_bigendian;
            this._left_eye_camera_image.step = (uint)left_eye_camera_image.step;
            this._left_eye_camera_image.data = left_eye_camera_image.data;
        }

        public void send_position_orientation()
        {
            this._ros.Publish(CommonParameter.OUTPUT_POSITION_ROTATION_NAME,
                this._robot_position_orientation);
        }

        public void send_hand_position()
        {
            this._ros.Publish(CommonParameter.OUTPUT_HAND_POSITION_NAME,
                this._hand_position);
        }

        public void send_eye_images()
        {
            this._ros.Publish(CommonParameter.OUTPUT_RIGHT_EYE_CAMERA_IMAGE_NAME,
                this._right_eye_camera_image);
            this._ros.Publish(CommonParameter.OUTPUT_LEFT_EYE_CAMERA_IMAGE_NAME,
                this._left_eye_camera_image);
        }

        /*********************************************************
         * Destructor
         *********************************************************/
        ~ROSSender()
        {
        }
    }

}
