# スクリプトについて

## 解説

「Project」ペインの「Assets」「Scripts」フォルダに、制御と通信を行うC#スクリプトがあります。

- Common
  - CommonImageProcessor
    - Unityカメラ画像処理の共通機能をまとめています。
  - CommonParameter
    - 各種パラメーターが設定されています。
  - CommonRateManager
    - 時間的な処理経過の監視やフィルター機能をまとめています。
  - CommonTransform
    - 座標変換の共通関数を定義しています。
  - MessageStructure
    - ROSトピックで用いる構造体を定義しています。
  - SystemStructure
    - 共通の列挙型や構造体の定義をまとめています。
  - TrajectoryInterpolationManager
    - 軌道の補間機能をまとめています。

- Input
  - InputManager
    - PCからの入力処理を行います。
    - ゲームオブジェクト「KeyboardInput」にアタッチされています。
  - KeyboardReceiver
    - キーボードの入力処理を行います。
  - GamepadReceiver
    - ゲームパッドの入力処理を行います。

- CarryObject
  - CarryObjectController
    - 運ばれる物（ここではカップ）を制御します。
    - ゲームオブジェクト「CarryObjects」にアタッチされています。

- Environment
  - EnvironmentController
    - カメラ操作など、ゲーム環境の制御を行います。
    - ゲームオブジェクト「Environment」にアタッチされています。

- Player
  - PlayerController
    - プレイヤーを制御します。

- Robot
  - RobotController
    - ロボットを制御します。
    - ロボットアバターのゲームオブジェクトにアタッチされていま  す。
  - HeadUnitController
    - HeadUnitを制御します。
  - EyeController
    - 目の動きを制御します。
  - NeckController
    - 頭の動きを制御制御します。
  - ArmUnitController
    - ArmUnitを制御します。
  - WaistDownUnitController
    - WaistDownUnitを制御します。
  - InverseKinematicsManager
    - 逆運動学アルゴリズムに関する処理をまとめています。
  - SimpleStepInverseKinematics
    - 自作の逆運動学アルゴリズムです。
    - ただし、まだ未完成です。
  - RigidBodyTree
    - 剛体とジョイントのモデルを表現しています。

- Communication
  - CommunicationReceiver
    - 外部アプリからの受信を行います。
    - ゲームオブジェクト「Communicator」にアタッチされています。
  - UDPReceiver
    - UDP通信による受信を行います。ただし、UDPによる通信機能は未完成です。
  - ROSReceiver
    - ROS通信による受信を行います。
  - CommunicationSender
    - 外部アプリへの送信を行います。
    - ゲームオブジェクト「Communicator」にアタッチされています。
  - UDPSender
    - UDP通信による送信を行います。UDPによる通信機能は未完成です。
  - ROSSender
    - ROS通信による送信を行います。

### クラス図

![Class Diagram](MaidRobotCafe_v2_class_diagram.svg)
