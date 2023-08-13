# How to Use

## About Keyboard Operations

List of Keyboard Operations

| Button | Function |
| -- | -- |
| W | Move Forward |
| S | Move Backward |
| A | Rotate Counterclockwise |
| D | Rotate Clockwise |
| Space | Switch Input Mode |
| O | Pick Up Cargo |
| P | Put Down Cargo |

## About Movement

The movement speed when a key is pressed is 1[m/s] for forward and backward movement, and 1[rad/s] for rotation.

## About Switching Input Modes

During execution, you can switch the input mode by pressing the Space key on the keyboard. At startup, it is in COMMUNICATION mode.

- COMMUNICATION Mode
  - This is a mode to operate the robot with input from an external application.
  - You cannot move the robot with input from the keyboard.
- KEYBOARD Mode
  - This is a mode to operate the robot using the forward, backward, and rotation inputs from the keyboard.
  - You cannot move the robot with input from an external application.

## About Transportation

You can pick up the cup placed on the game object "flasket", put it on the tray, carry it, and place it on the desk.

When the robot gets within 1[m] of the cup, it can pick up the cup. Also, when the robot gets within 1[m] of the place to put the cup, it can put down the cup.

The places where you can put the cup are the initial position of the flasket and on the desk where each chair is located.

## ROS Communication

If you are going to communicate with ROS, you need to start the ROS TCP Endpoint in advance. The Maid Robot System uses ROS2.

The ROS TCP Endpoint requires installation work in advance. For details of the work, please refer to the installation method documentation.
[Installation Method](../install/install_doc_en.md)

### Starting the ROS TCP Endpoint

Execute the setup command. This is not necessary if you have set up to execute the setup command when Ubuntu starts.

Next, execute the following command.

> ros2 run ros_tcp_endpoint default_server_endpoint --ros-args -p ROS_IP:=127.0.0.1

In the above command, the IP address is set to 127.0.0.1, but you can use a different address. In that case, you need to change the IP address to that different address from the menu bar of the Unity editing screen, "Robotics" -> "ROS Settings".

### ROS Topics

The topics being sent and received are as follows.

| Received Topic Name | Data Type | Remarks |
| ---- | ---- | ---- |
| move_velocity_reference | geometry_msgs/Twist | This is the robot's 3-axis speed and 3-axis angular speed command value. However, at present, only the x-axis speed and the angular speed around the z-axis received as command values are used as command values. |

<br>

| Sent Topic Name | Data Type | Remarks |
| ---- | ---- | ---- |
| robot_position_orientation | geometry_msgs/PoseStamped | This is information about the robot's current XYZ position and posture expressed in quaternion. This topic is sent at intervals of 0.5[s]. |

