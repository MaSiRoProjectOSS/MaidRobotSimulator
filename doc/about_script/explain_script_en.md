# About the Script

## Explanation

In the "Project" pane, under the "Assets" and "Scripts" folders, there are C# scripts that perform control and communication.

- CommonParameter
  - Various parameters are set.
  - If you want to change the communication method from ROS to UDP, change ROBOT_COMMUNICATION_MODE to COMMUNICATION_MODE.UDP.
- ROSStructure
  - It defines the structure used in ROS topics.
- KeyboardReceiver
  - It processes keyboard inputs.
  - It is attached to the game object "KeyboardInput".
- CarryObjectController
  - It controls the object to be carried (in this case, a cup).
  - It is attached to the game object "CarryObjects".
- RobotController
  - It controls the robot.
  - It is attached to the game object "Rei".
- CommReceiver
  - It receives from external applications.
  - It is attached to the game object "Communicator".
- UDPReceiver
  - It receives via UDP communication.
- ROSReceiver
  - It receives via ROS communication.
- CommSender
  - It sends to external applications.
  - It is attached to the game object "Communicator".
- UDPSender
  - It sends via UDP communication.
- ROSSender
  - It sends via ROS communication.

### Class Diagram

![Class Diagram](MaidRobotCafe_v1_class_diagram.svg)

