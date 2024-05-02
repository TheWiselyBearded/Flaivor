# Meta Quest Screenshot Loader
Unity Android plugin for retrieving the latest screenshot images from Meta Quest.
It can be combined with passthrough to capture camera images.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![CodeFactor](https://www.codefactor.io/repository/github/t-34400/metaquestscreenshotloader/badge)](https://www.codefactor.io/repository/github/t-34400/metaquestscreenshotloader)

## Environment
- Meta Quest Pro
- Unity Editor 2022.3.1.f1

## Installation
1. Place the files in the `Assets` folder.
2. Choose `File` > `Build Settings` from the menu bar, click on Android in the Platform tab, and press the `Switch Platform` button.
3. Press the `Player Settings` button, check `Custom Main Manifest`, `Custom Main Gradle Template`, and `Custom Gradle Properties Template` under `Publishing Settings` > `Build`.
4. Edit the files generated in step 3 as follows:
    - `Assets/Plugins/Android/AndroidManifest.xml`
        - Add the following tags below the manifest tag:
            ```xml
            <manifest ...
                    ... >
                <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE"
                                android:maxSdkVersion="32" />
                <uses-permission android:name="com.oculus.permission.SET_VR_DEVICE_PARAMS" />
                <uses-permission android:name="com.oculus.permission.READ_VR_DEVICE_PARAMS" />                
            ```
    - `Assets/Plugins/Android/mainTemplate.gradle`
        - Add dependencies at the bottom of the file:
            ```gradle
            dependencies {
                implementation 'androidx.appcompat:appcompat:1.6.1'
            }
            ```
    - `Assets/Plugins/Android/gradleTemplate.properties`
        - Add the following:
            ```
            android.useAndroidX=true
            android.enableJetifier=true
            ```
    - Refer to the files in the included [Plugins](./Plugins/) directory as a sample.

## Scene Implementation
1. Place a suitable canvas in the scene, and add a button for requesting storage access (`Request Permission Button`) and a button for loading screenshots (`Load Screenshot Button`).
2. Place a `RawImage` for displaying the received texture in the scene.
3. Add the `ScreenshotLoader` component to a suitable game object and configure it with the UI added in steps 1 and 2.

## Usage
1. If the app does not have storage access permission, the `Request Permission Button` is displayed. Click it and grant permission from the system dialog that appears.
2. Capture a screenshot of the screen by simultaneously pressing the Oculus button and trigger button on the right controller.
    - To capture camera images, see [details below](#capturing-camera-images).
3. Press the `Load Screenshot Button` to load the screenshot.

## Using Received Image Data
- The `screenshotBytes` inside the `loadScreenShot()` method of the `ScreenshotLoader` instance contains the binary data of the image (JPEG). Modify the subsequent processing to use it.
- As `loadScreenShot()` is executed on the same thread as the call, it can be called from Unity's main thread, allowing scene manipulation.

## Capturing Camera Images
To capture camera images, there are two methods:
1. Launch passthrough from the system, and capture camera images by simultaneously pressing the Oculus button and trigger button on the right controller.
2. Introduce the Passthrough API, display passthrough within the app, and capture camera images by simultaneously pressing the Oculus button and trigger button on the right controller.

# License
[MIT License](./LICENSE)
