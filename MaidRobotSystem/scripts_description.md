# インストールとDocker関連のスクリプトについて

## インストール用スクリプト

- install_docker_environment.sh
  - UbuntuにDocker環境をインストールします。
- install_MaidRobotSystem_environment_for_simulator.sh
  - Ubuntu 22.04 にシミュレーターと連携動作するのに必要なノードを実行できる環境を整えます。

## Dockerコマンド実行用スクリプト

- launch_controller_for_simulator_in_MaidRobotSystem_docker.sh
  - DockerからMaidRobotSystemのシミュレーター用launchファイルを実行します。
- start_ros_tcp_endpoint_in_MaidRobotSystem_docker.sh
  - DockerからROS TCP Endpointを起動します。
- pull_MaidRobotSystem_docker_image.sh
  - Docker Hub からMaidRobotSystemのDockerイメージをダウンロードします。

## Dockerイメージ作成用スクリプト

- create_maid_robot_system_docker_image.sh
  - Dockerイメージ作成を行うスクリプトです。
- install_MaidRobotSystem_environment_for_simulator_docker.sh
  - Dockerイメージ作成の時に、インストールするコマンドをまとめています。
- launch_controller_for_simulator.sh
  - Dockerコンテナ内でMaidRobotSystemのシミュレーター用launchファイルを実行するコマンドです。
- start_ros_tcp_endpoint.sh
  - Dockerコンテナ内でROS TCP Endpointを起動するコマンドです。
