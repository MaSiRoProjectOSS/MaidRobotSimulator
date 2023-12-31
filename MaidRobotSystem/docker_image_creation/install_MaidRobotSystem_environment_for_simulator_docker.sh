ln -sf /usr/share/zoneinfo/Asia/Tokyo /etc/localtime

## Install ROS2 Humble
apt update -q
apt install -y -q software-properties-common

add-apt-repository universe
apt install curl -y
curl -sSL https://raw.githubusercontent.com/ros/rosdistro/master/ros.key -o /usr/share/keyrings/ros-archive-keyring.gpg
echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/ros-archive-keyring.gpg] http://packages.ros.org/ros2/ubuntu $(lsb_release -cs) main" | tee /etc/apt/sources.list.d/ros2.list > /dev/null
apt install -y -q locales
locale-gen en_US en_US.UTF-8
update-locale LC_ALL=en_US.UTF-8 LANG=en_US.UTF-8

export LANG=en_US.UTF-8

apt autoremove -y -q
apt update -q
apt upgrade -y -q

apt install -y -q build-essential

apt install -y -q ros-humble-desktop
apt install -y -q ros-dev-tools

apt install -y -q python3-rosdep libpython3-dev python3-pip

## Install Json
apt install -y -q python3-colcon-common-extensions
apt install -y -q jq

## Install QT
apt-get install -y -q build-essential libfontconfig1 mesa-common-dev libglu1-mesa-dev qt*5-dev qml-module-qtquick-controls qml-module-qtquick-controls2
apt install -y -q nlohmann-json3-dev

## Install CV
apt install -y -q libcv-bridge-dev python3-cv-bridge python3-image-geometry libimage-geometry-dev

## Install Python (resolve warning)
python3 -m pip install --upgrade pip
pip install setuptools==58.2.0
pip install numpy==1.23.0

## Install MediaPipe
pip install opencv-python mediapipe

## Clone MaidRobotSystem
cd /opt
git clone -b for_simulator https://github.com/MaSiRoProjectOSS/MaidRobotSystem.git

## Build MaidRobotSystem ROS2 nodes
/opt/MaidRobotSystem/src/planetary_module/ros/mrs.sh --mrs_config=/opt/MaidRobotSystem/src/planetary_module/ros/config/default.json mrs_build build release

## Clone ROS-TCP-Endpoint
cd /opt/MaidRobotSystem/.colcon
mkdir src
cd /opt/MaidRobotSystem/.colcon/src
git clone -b main-ros2 https://github.com/Unity-Technologies/ROS-TCP-Endpoint.git
cd /opt/MaidRobotSystem/.colcon/

## Build ROS-TCP-Endpoint
source /opt/ros/humble/setup.bash
colcon build --packages-select ros_tcp_endpoint
cd ~
