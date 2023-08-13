/**
 * @file UDPSender.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Send data with UDP.
 * @version 1.0.0
 * @date 2023-08-05
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MaidRobotSimulator.MaidRobotCafe
{
    public class UDPSender
    {
        /*********************************************************
         * Private variables
         *********************************************************/
        private UdpClient _client; /*!< UDP client */
        private IPEndPoint _remote_EndPoint; /*!< Endpoint */

        private ROSStructure.ST_GEOMETRY_MSGS_POSESTAMPED _robot_position_orientation =
            new ROSStructure.ST_GEOMETRY_MSGS_POSESTAMPED(
                new ROSStructure.ST_STD_MSGS_HEADER(
                    0,
                    new ROSStructure.ST_TIME(0, 0),
                    CommonParameter.UDP_INFORMATION_NAME
                    ),
                new ROSStructure.ST_GEOMETRY_MSGS_POSE(
                    new ROSStructure.ST_POINT(0.0, 0.0, 0.0),
                    new ROSStructure.ST_QUATERNION(0.0, 0.0, 0.0, 0.0)
                    )
            ); /*!< send data object */

        private string _send_message = ""; /*!< send message text */

        private string _socket_error_message = ""; /*!< socket error message */
        private CommonParameter.UDP_ERROR_KIND _error = CommonParameter.UDP_ERROR_KIND.NONE; /*!< error status */

        /*********************************************************
         * Public functions
         *********************************************************/
        public UDPSender()
        {
            this._client = new UdpClient();
            this._remote_EndPoint = new IPEndPoint(
                IPAddress.Parse(CommonParameter.UDP_LOCAL_HOST), CommonParameter.ROBOT_UDP_SENDER_PORT);
        }

        public CommonParameter.UDP_ERROR_KIND get_error_kind()
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

        public void send_data()
        {
            this._encode_data();

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(_send_message);
                this._client.Send(data, data.Length, _remote_EndPoint);
            }
            catch (SocketException ex)
            {
                this._socket_error_message = ex.Message;
                this._error = CommonParameter.UDP_ERROR_KIND.SOCKET_EXCEPTION;
            }
        }

        /*********************************************************
         * Private functions
         *********************************************************/
        private void _encode_data()
        {
            this._send_message = CommonParameter.UDP_INFORMATION_NAME + ", "
                  + this._robot_position_orientation.pose.position.x.ToString() + ", "
                  + this._robot_position_orientation.pose.position.y.ToString() + ", "
                  + this._robot_position_orientation.pose.position.z.ToString() + ", "
                  + this._robot_position_orientation.pose.orientation.w.ToString() + ", "
                  + this._robot_position_orientation.pose.orientation.x.ToString() + ", "
                  + this._robot_position_orientation.pose.orientation.y.ToString() + ", "
                  + this._robot_position_orientation.pose.orientation.z.ToString()
                  + CommonParameter.UDP_MESSAGE_TERMINATOR;
        }

        /*********************************************************
         * Destructor
         *********************************************************/
        ~UDPSender()
        {
            this._client?.Close();
        }
    }

}
