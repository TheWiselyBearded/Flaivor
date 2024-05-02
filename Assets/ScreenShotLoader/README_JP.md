# Meta Quest Screenshot Loader
Meta Questから最新のスクリーンショット画像を取得するUnity Androidプラグイン．
パススルーと組み合わせることでカメラ撮影することもできる．

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![CodeFactor](https://www.codefactor.io/repository/github/t-34400/metaquestscreenshotloader/badge)](https://www.codefactor.io/repository/github/t-34400/metaquestscreenshotloader)

## 動作確認環境
- Meta Quest Pro
- Unity Editor 2022.3.1.f1

## 導入方法
1. ファイルをAssets以下に配置する．
2. メニューバーから`File` > `Build Settings`を選び，ダイアログの`Platform`タブからAndroidをクリックし，`Switch Platform`ボタンを押す．
3. `Player Settings`ボタンを押し，`Publishing Settings` > `Build`以下の`Custom Main Manifest`, `Custom Main Gradle Template`, `Custom Gradle Properties Template`にチェックを入れる．
4. 3で生成された各ファイルを以下のように編集する．
    - `Assets/Plugins/Android/AndroidManifest.xml`
        - manifestタグ以下に次のタグを追加する．
            ```xml
            <manifest ...
                    ... >
                <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
                <uses-permission android:name="com.oculus.permission.SET_VR_DEVICE_PARAMS" />
                <uses-permission android:name="com.oculus.permission.READ_VR_DEVICE_PARAMS" />                
            ```
    - `Assets/Plugins/Android/mainTemplate.gradle`
        - ファイル最下部にdependenciesを追加する．
            ```gradle
            dependencies {
                implementation 'androidx.appcompat:appcompat:1.6.1'
            }
            ```
    - `Assets/Plugins/Android/gradleTemplate.properties`
        - 以下を追加する．
            ```
            android.useAndroidX=true
            android.enableJetifier=true
            ```
    - サンプルとして，同梱の[Plugins](./Plugins/)内の各ファイルを参照すること．

## シーンの実装
1. シーンに適当なキャンバスを設置し，ストレージアクセスの権限のリクエスト用のボタン(`Request Permission Button`)と，スクリーンショット読み込み用のボタン(`Load Screenshot Button`)を配置する．
2. シーンに受信したテクスチャ表示用の`RawImage`を設置する．
3. 適当なゲームオブジェクトに`ScreenshotLoader`コンポーネントを追加し，1,2で追加したUIを設定する．

## 操作方法
1. アプリにストレージアクセスの権限が与えられていない場合`Request Permission Button`が表示されるため，これをクリックして出てきたシステムダイアログから権限を与える．
2. 右コントローラのOculusボタンとトリガーボタンの同時押しで画面のスクリーンショットを撮影する
    - カメラ映像を撮影する場合は[後述](#カメラ画像の撮影について)
3. `Load Screenshot Button`を押してスクリーンショットをロードする．

## 受信した画像データの利用方法
- `ScreenshotLoader`のインスタンスメソッド`loadScreenShot()`内の`screenshotBytes`が画像(JPEG)のバイナリデータを格納しているので，このあとの処理を改変して使う．
- `loadScreenShot()`の呼び出しと同じスレッドで処理が行われるため，Unityのメインスレッドから呼べばシーンの操作も可能である．

## カメラ画像の撮影について
カメラ画像を撮影する場合，以下の方法がある．
1. システムからパススルーを起動して，右コントローラのOculusボタンとトリガーボタンの同時押しでカメラ映像を撮影する．
2. Passthrough APIを導入してパススルーをアプリ内に表示した上で，右コントローラのOculusボタンとトリガーボタンの同時押しでカメラ映像を撮影する．

# ライセンス
[MITライセンス](./LICENSE)