# Sample Project for Meta Quest Screenshot Loader
This is a sample project for the Unity Android plugin [Meta Quest Screenshot Loader](https://github.com/t-34400/MetaQuestScreenshotLoader/), which retrieves the latest screenshot images from Meta Quest.

## Environment
- Meta Quest Pro
- Unity Editor 2022.3.26f

## Installation
1. Clone the project using the following command:
```bash
git clone --recurse-submodules https://github.com/t-34400/MetaQuestScreenShotLoaderSample.git
```
2. Select `Add` > `Add project from disk` from the `Projects` tab in Unity Hub, and open the cloned directory.
3. Click on the added project to launch the editor.
4. In the editor, open `Assets/Scenes/SampleScene` in the Project window.
5. Add the `ScreenShotLoader` component to the `Views/Scene/LoadScreenShotCanvas/Controllers` object in the scene and configure it as shown in the image below:
![alt text](Images/Component.png "Component")
6. From the menu bar, select `File` > `Build Settings`, then choose Android in the  window and press the `Switch Platform` button.
7. From the menu bar, select `Edit` > `Project Settings`, then open `Meta XR` in the window and select the `Fix All` and `Apply All` buttons if available.
8. Build the project.
