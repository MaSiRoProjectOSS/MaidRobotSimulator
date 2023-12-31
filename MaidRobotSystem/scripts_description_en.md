## About installation and Docker related scripts

## Scripts for installation

- install_docker_environment.sh
  - Install Docker environment on Ubuntu.
- install_MaidRobotSystem_environment_for_simulator.sh
  - Prepare an environment on Ubuntu 22.04 to run the nodes necessary to work with the simulator.

## Script for executing Docker commands

- launch_controller_for_simulator_in_MaidRobotSystem_docker.sh
  - Execute the launch file for MaidRobotSystem simulator from Docker.
- start_ros_tcp_endpoint_in_MaidRobotSystem_docker.sh
  - Start ROS TCP Endpoint from Docker.
- pull_MaidRobotSystem_docker_image.sh
  - Download MaidRobotSystem Docker image from Docker Hub.

## Script for creating Docker image

- create_maid_robot_system_docker_image.sh
  - Script to create Docker image.
- install_MaidRobotSystem_environment_for_simulator_docker.sh
  - This is a collection of commands to be installed when creating a Docker image.
- launch_controller_for_simulator.sh
  - This command executes the launch file for the MaidRobotSystem simulator in the Docker container.
- start_ros_tcp_endpoint.sh
  - Command to start ROS-TCP-Endpoint in Docker container.
