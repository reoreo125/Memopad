# Memopad (Notepad Clone)
Windows純正メモ帳（クラシック）の機能を再現した、WPF製のテキストエディタです。
Prismによるクリーンアーキテクチャと、R3 (ReactiveProperty) によるリアクティブな状態管理を学習・実践するプロジェクトとして開発しています。

![ScreenShot](https://raw.githubusercontent.com/reoreo125/Memopad/refs/heads/develop/screenshot.jpg)

## 特徴
* UIレイアウト、メニュー構成、フォント設定などクラシックなメモ帳を再現。
* Prismライブラリを用いたMVVMパターン。
* R3によるReactiveプログラミングを用いたUIイベント処理。

## 使用ライブラリ
|ライブラリ名|ライセンス|URL|
|:-----------|:-------------:|:------------:|
|Newtonsoft.Json|[MIT](https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md)|<https://github.com/JamesNK/Newtonsoft.Json>|
|Prism.Unity|[License](https://github.com/PrismLibrary/Prism/blob/master/LICENSE)|<https://github.com/PrismLibrary/Prism>|
|Prism.Wpf|^ |^ |
|R3|[MIT](https://github.com/Cysharp/R3/blob/main/LICENSE)|<https://github.com/Cysharp/R3>| 
|R3Extension.WPF|^|^
|UTF.Unknown|[Mozilla Public License version 1.1](https://github.com/CharsetDetector/UTF-unknown/blob/master/license/MPL-1.1.txt)|<https://github.com/CharsetDetector/UTF-unknown>|
|xunit|[License](https://github.com/xunit/xunit/blob/main/LICENSE)|<https://github.com/xunit/xunit>|

## 動作/開発環境
* .NET 10 (WPF)
* Windows 11

## インストール・実行方法
1. このリポジトリをクローンします。
`git clone https://github.com/reoreo125/Memopad.git`
2. Visual Studio 2026 または Visual Studio Codeでソリューションファイルを開きます。
3. ビルドを実行し`Memopad.exe`を起動してください。

## ライセンス
このプロジェクト自体は MIT License の下で公開されています。
