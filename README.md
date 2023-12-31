# Maid Robot Simulator

[English](./README_en.md)

本シミュレーターは、メイドロボットの制御アルゴリズム、ソフトウェアをテストするためのものです。

## ver2の機能

Maid Robot Simulator ver2 でできることは以下の通りです。

- メイドロボットを一人配置し、キーボードで操作する。
- カフェの配膳業務（カップを持って運ぶ）を模擬する。
- 外部入力で位置と姿勢を変更する。
- 外部出力で現在の位置姿勢情報を出力する。
- 外部入力でメイドロボットの手の位置を変更する（手つなぎモード）。
- MaidRobotSystemと連携し、
  - 人認識をして顔の向きと目線を制御する。
  - 手の位置から速度を制御する（手つなぎ移動）。

外部入出力には、ROS, ROS2を利用することができます。

## 利用するアプリ

利用するアプリについては、以下をご確認ください。

[アプリとサポートバージョン](./doc/version/support_version.md)

[Notes]

- Vroid Studioは、ロボットを自作する場合に必要になります。デフォルトで入っているロボットを用いる場合は必要ありません。

## インストール方法

インストール方法については、以下を参照してください。

[インストール方法](/doc/install/install_doc.md)

## 使い方

使い方については、以下を参照してください。

[使い方](/doc/how_to_use/how_to_use_doc.md)

## スクリプトについて

本シミュレーターで設計したC#スクリプトについて、以下の資料で解説しています。

[スクリプトについて解説](./doc/about_script/explain_script.md)

## Dockerについて

シミュレーターと連携して動作するMaidRobotSystemの環境をDockerで用意しています。それを利用するためのDockerのコマンドをまとめていますので、以下をご参照ください。

[インストールとDocker関連のスクリプトについて](./MaidRobotSystem/scripts_description.md)

## シミュレーターで動くメイドロボット

本シミュレーターには、メイドロボットが一人含まれています。メイドロボットはVroid Studioで作成し、VRM1.0のファイル形式で出力しています。

モデルは以下のリンク先（Vroid Hub）でも公開しています。

<https://hub.vroid.com/characters/1254861402411331672/models/5998322075644097762>

## メイドロボットアバターの利用規約

本アバターはすべてのユーザーが利用できます。

本アバターを用いて宗教目的、政治目的での表現を行うことは禁止します。また、反社会的、憎悪表現を行うことは禁止します。

個人、法人による本アバターの商用利用を許可します。

本アバターの再配布は禁止します。

本アバターの改変は許可します。改変したアバターを再配布することは許可します。

本アバターを利用する際、クレジットの表記は不要です。

著作権は MaSiRo Project に帰属します。

Copyright (c) MaSiRo Project. 2023-.
