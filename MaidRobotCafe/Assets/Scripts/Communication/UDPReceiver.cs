/**
 * @file UDPReceiver.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Receive data with UDP.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MaidRobotSimulator.MaidRobotCafe
{
    public class UDPReceiver
    {
        /*********************************************************
         * Private variables
         *********************************************************/
        private UdpClient _client;           /*!< UDP client */
        private IPEndPoint _remote_EndPoint; /*!< IP endpoint */
        private Thread _thread;              /*!< thread */

        private string _received_data = "";  /*!< received data */

        private MessageStructure.ST_GEOMETRY_MSGS_TWIST _move_velocity_reference =
            new MessageStructure.ST_GEOMETRY_MSGS_TWIST(
                new MessageStructure.ST_LINEAR(0.0, 0.0, 0.0),
                new MessageStructure.ST_ANGULAR(0.0, 0.0, 0.0)); /*!< reference velocity for moving */

        private bool _next_move_velocity_reference_received = false; /*!< flag for next data received */

        private string _socket_error_message = "";
        private SystemStructure.UDP_ERROR_KIND _error = SystemStructure.UDP_ERROR_KIND.NONE; /*!< error status */

        /*********************************************************
         * Constructor
         *********************************************************/
        public UDPReceiver()
        {
            this._client = new UdpClient(CommonParameter.ROBOT_UDP_RECEIVER_PORT);
            this._remote_EndPoint = new IPEndPoint(IPAddress.Any, 0);

            this._thread = new Thread(new ThreadStart(_receive_data));
            this._thread.IsBackground = true;
            this._thread.Start();
        }

        /*********************************************************
         * Public functions
         *********************************************************/
        public SystemStructure.UDP_ERROR_KIND get_error_kind()
        {
            return this._error;
        }

        public MessageStructure.ST_GEOMETRY_MSGS_TWIST get_move_velocity_reference()
        {
            this._next_move_velocity_reference_received = false;

            return this._move_velocity_reference;
        }

        public bool is_data_received()
        {
            return this._next_move_velocity_reference_received;
        }

        /*********************************************************
         * Private functions
         *********************************************************/
        private void _receive_data()
        {
            while (true)
            {
                try
                {
                    byte[] data = _client.Receive(ref _remote_EndPoint);
                    string text = Encoding.UTF8.GetString(data);
                    this._received_data += text;
                    this._decode_data();
                }
                catch (SocketException ex)
                {
                    this._socket_error_message = ex.Message;
                    this._error = SystemStructure.UDP_ERROR_KIND.SOCKET_EXCEPTION;
                }
            }
        }

        private void _decode_data()
        {
            int index;
            while ((index = this._received_data.IndexOf(CommonParameter.UDP_MESSAGE_TERMINATOR)) >= 0)
            {
                string message = this._received_data.Substring(0, index);
                this._received_data = this._received_data.Substring(index + 1);

                string[] split_text = message.Split(CommonParameter.UDP_MESSAGE_DELIMINATOR);
                if (split_text.Length == 3)
                {
                    this._move_velocity_reference.linear.x = double.Parse(split_text[1]);
                    this._move_velocity_reference.angular.z = double.Parse(split_text[2]);

                    this._next_move_velocity_reference_received = true;
                }
                else
                {
                    this._error = SystemStructure.UDP_ERROR_KIND.MESSAGE_LENGTH;
                }
            }
        }

        /*********************************************************
         * Destructor
         *********************************************************/
        ~UDPReceiver()
        {
            this._thread?.Abort();
            this._client?.Close();
        }
    }

}
