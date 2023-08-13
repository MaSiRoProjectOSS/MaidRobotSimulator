# スクリプトについて

## 解説

「Project」ペインの「Assets」「Scripts」フォルダに、制御と通信を行うC#スクリプトがあります。

- CommonParameter
  - 各種パラメーターが設定されています。
  - 通信方法をROSからUDPに変更したい時は、ROBOT_COMMUNICATION_MODE を COMMUNICATION_MODE.UDP に変更します。
- ROSStructure
  - ROSトピックで用いる構造体を定義しています。
- KeyboardReceiver
  - キーボードの入力処理を行います。
  - ゲームオブジェクト「KeyboardInput」にアタッチされています。
- CarryObjectController
  - 運ばれる物（ここではカップ）を制御します。
  - ゲームオブジェクト「CarryObjects」にアタッチされています。
- RobotController
  - ロボットを制御します。
  - ゲームオブジェクト「Rei」にアタッチされています。
- CommReceiver
  - 外部アプリからの受信を行います。
  - ゲームオブジェクト「Communicator」にアタッチされています。
- UDPReceiver
  - UDP通信による受信を行います。
- ROSReceiver
  - ROS通信による受信を行います。
- CommSender
  - 外部アプリへの送信を行います。
  - ゲームオブジェクト「Communicator」にアタッチされています。
- UDPSender
  - UDP通信による送信を行います。
- ROSSender
  - ROS通信による送信を行います。

### クラス図

![Class Diagram](MaidRobotCafe_v1_class_diagram.svg)
