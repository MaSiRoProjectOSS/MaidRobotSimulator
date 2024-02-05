FROM ubuntu:22.04

COPY install_MaidRobotSystem_environment_for_simulator_docker.sh start_ros_tcp_endpoint.sh launch_controller_for_simulator.sh /

RUN chmod +x /install_MaidRobotSystem_environment_for_simulator_docker.sh && \
    chmod +x start_ros_tcp_endpoint.sh && \
    chmod +x launch_controller_for_simulator.sh && \
    /install_MaidRobotSystem_environment_for_simulator_docker.sh
