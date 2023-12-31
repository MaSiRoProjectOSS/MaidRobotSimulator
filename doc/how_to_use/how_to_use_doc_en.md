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
| T | Switching the robot's mode |
| F1 | Switching the camera mode |

How to move the hand position in HAND_HOLDING mode

| Button | Function |
| -- | -- |
| Arrow key right | Move to Y-axis positive direction from robot's point of view. |
| Arrow key left | Move to Y-axis negative direction from robot's point of view |
| Arrow key up | Move to X-axis negative direction from robot's point of view |
| Arrow key down | Move to X-axis positive direction from robot's point of view |
| Shift + Arrow keys up | Move Z-axis positive direction from the robot's view |
| Shift + Arrow key down | Move Z-axis negative direction from robot's view |

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

## About catering

You can pick up the cup placed on the game object "flasket", put it on the tray, carry it, and place it on the desk.

When the robot gets within 1[m] of the cup, it can pick up the cup. Also, when the robot gets within 1[m] of the place to put the cup, it can put down the cup.

The places where you can put the cup are the initial position of the flasket and on the desk where each chair is located.

## About the robot's modes

During execution, the robot's mode can be switched by pressing the T key on the keyboard. At startup, the robot is in CATERING mode.

- CATERING mode
  - This mode reproduces the operation of carrying drinks and other items to customers in a cafe.
- HAND_HOLDING mode
  - This mode reproduces the movement of the robot by hand holding.

## About camera mode

The camera mode can be switched by pressing the F1 key on the keyboard during execution. At startup, the camera is in FAR mode.

- FAR mode
  - This mode allows you to view the entire area in a wide area.
- NEAR mode
  - This mode allows you to view the robot in detail.

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
| maid_robot_system/Misen/waist_down_unit/controller/move_velocity_reference | geometry_msgs/Twist | 3-axis velocity and 3-axis angular velocity command values of the robot. However, only x-axis velocity and angular velocity around z-axis are used as command values among the received command values at this time. |
| maid_robot_system/Misen/head_unit/logic/eye | maid_robot_system_interfaces/MrsEyeMsg | Angle command values for the eyes. Receives angle command values in the pitch angle and yaw angle directions for the left and right eyes. |
| maid_robot_system/Misen/head_unit/logic/neck | maid_robot_system_interfaces/MrsNeckMsg | Angle command value of the head. It is received in quaternion. |
| maid_robot_system/Misen/head_unit/logic/lip | maid_robot_system_interfaces/MrsLipMsg | The command value for the degree of mouth opening is received. Currently, this command value is not used. |
| maid_robot_system/Misen/head_unit/logic/status | maid_robot_system_interfaces/MrsHeadStatusMsg | Receives information whether or not a person is detected by the eye camera. | Receives information on whether or not the eye camera is detecting people. |

<br>

| Sent Topic Name | Data Type | Remarks |
| ---- | ---- | ---- |
| maid_robot_system/Misen/waist_down_unit/controller/robot_position_orientation | geometry_msgs/PoseStamped | Information on the robot's current XYZ position and orientation expressed in quaternions. This topic is sent at 0.1[s] intervals. |
| maid_robot_system/Misen/arm_unit/controller/hand_position | geometry_msgs/PointMsg | Hand position information in hand-holding mode. This information is sent at intervals of 0.1[s] for this topic. |
| maid_robot_system/Misen/head_unit/controller/topic_image/image/right | sensor_msgs/Image | The camera image of the right eye. It is sent at intervals of 0.1[s] for this topic. |
| image_left | sensor_msgs/Image | The camera image of the left eye. It is sent at intervals of 0.1[s] for this topic. Receives information on whether or not the eye camera is detecting people. |

<br>

The "maid_robot_system_interfaces" is a custom message for MaidRobotSystem. For more information, please refer to the following link.

<https://github.com/MaSiRoProjectOSS/MaidRobotSystem/tree/main/src/planetary_module/ros/package/maid_robot_system_interfaces>

## About MaidRobotSystem

To reproduce face recognition and hand-holding control, it is necessary to communicate with MaidRobotSystem.

<https://github.com/MaSiRoProjectOSS/MaidRobotSystem>
