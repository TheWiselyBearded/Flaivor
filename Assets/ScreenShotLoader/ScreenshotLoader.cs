#nullable enable

using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScreenshotLoader : MonoBehaviour
{
    public UnityEvent OnScreenShotLoadedEvent;

    public byte[] imgBytes;
    private const int MAX_CHECK_PERMISSION_COUNT = 100;

    [SerializeField] private Button requestPermissionButton = default!;
    [SerializeField] private Button loadScreenShotButton = default!;
    [SerializeField] private RawImage rawImage = default!;
    [SerializeField] private float checkPermissionInterval = 1.0f;
    [SerializeField] private float checkImageIntervalPerSecond = 1.0f;
    [SerializeField] private int maxAttempts = 30;
    public bool autoLoadActive = false;

    private Texture2D texture = default!;
    private bool permissionGranted = false;
    private float lastFileSize = 0f;
    private int attempts = 0;

    public void ReceiveHasReadStoragePermissionCallback(string message)
    {
        if (message.Equals("true"))
        {
            permissionGranted = true;
            //requestPermissionButton.gameObject.SetActive(false);
            //loadScreenShotButton.gameObject.SetActive(true);
        }
    }

    private void Awake()
    {
        requestPermissionButton.onClick.AddListener(RequestPermission);
        loadScreenShotButton.onClick.AddListener(LoadScreenShot);

        //loadScreenShotButton.gameObject.SetActive(false);

        texture = new Texture2D(1, 1);
        rawImage.texture = texture;

#if UNITY_ANDROID && !UNITY_EDITOR
        RequestCheckPermission();
#endif
    }

    private void OnDestroy()
    {
        if (requestPermissionButton != null)
        {
            requestPermissionButton.onClick.RemoveListener(RequestPermission);
        }
        if (loadScreenShotButton != null)
        {
            loadScreenShotButton.onClick.RemoveListener(LoadScreenShot);
        }
    }

    public void SetImageLoaderActive() {
        autoLoadActive = true;
        InvokeRepeating("CheckForNewImages", 0.5f, (1.0f/checkImageIntervalPerSecond));
    }

    private void RequestCheckPermission()
    {
        if (permissionGranted)
        {
            return;
        }

        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (var loaderClass = new AndroidJavaClass("com.example.screenshotloader.ScreenshotLoader"))
                {
                    loaderClass.CallStatic("requestHasReadStoragePermissionCallback", currentActivity, name, "ReceiveHasReadStoragePermissionCallback");
                }
            }
        }
    }

    private void RequestPermission()
    {
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (var loaderClass = new AndroidJavaClass("com.example.screenshotloader.ScreenshotLoader"))
                {
                    loaderClass.CallStatic("requestPermissionIfNeeded", currentActivity);
                    StartCoroutine(CheckPermissionCoroutine());
                }
            }
        }
    }

    private void LoadScreenShot()
    {
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (var loaderClass = new AndroidJavaClass("com.example.screenshotloader.ScreenshotLoader"))
                {
                    using(var loader = loaderClass.CallStatic<AndroidJavaObject>("getInstance"))
                    {
                        var succeeded = loader.Call<bool>("getLatestScreenshot");
                        if (succeeded)
                        {
                            var screenshotBytes = loader.Call<byte[]>("getLatestScreenshotBytes");
                            Debug.Log($"Load latest screenshot: size={screenshotBytes.Length}");
                            imgBytes = new byte[screenshotBytes.Length];
                            Buffer.BlockCopy(screenshotBytes, 0, imgBytes, 0, screenshotBytes.Length);
                            texture.LoadImage(screenshotBytes);
                            texture.Apply();
                            rawImage.texture = texture;
                            OnScreenShotLoadedEvent?.Invoke();
                            autoLoadActive = false;
                            CancelInvoke("CheckForNewImages");
                        }
                    }
                }
            }
        }
    }

    private int LoadLastScreenshotBytes() {
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
            using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
                using (var loaderClass = new AndroidJavaClass("com.example.screenshotloader.ScreenshotLoader")) {
                    using (var loader = loaderClass.CallStatic<AndroidJavaObject>("getInstance")) {
                        var succeeded = loader.Call<bool>("getLatestScreenshot");
                        if (succeeded) {
                            var screenshotBytes = loader.Call<byte[]>("getLatestScreenshotBytes");
                            return screenshotBytes.Length; // Return the size of screenshotBytes
                        }
                    }
                }
            }
        }
        return 0; // Return 0 if no screenshot was loaded
    }


    public void LoadImageBytesToTexture() {
        texture.LoadImage(imgBytes);
        texture.Apply();
        rawImage.texture = texture;
    }

    private IEnumerator CheckPermissionCoroutine()
    {
        foreach (var i in Enumerable.Range(0, MAX_CHECK_PERMISSION_COUNT))
        {
            if (permissionGranted)
            {
                yield break;
            }
            RequestCheckPermission();

            yield return new WaitForSeconds(checkPermissionInterval);
        }
    }


    private void CheckForNewImages() {
        float currentFileSize = LoadLastScreenshotBytes();
        if (lastFileSize == 0) currentFileSize = lastFileSize;

        if (currentFileSize != lastFileSize) {
            lastFileSize = currentFileSize;
            LoadScreenShot();
            attempts = 0; // reset attempts since we found a new image
        }
        else {
            attempts++;
            if (attempts >= maxAttempts) {
                LoadScreenShot();
                attempts = 0; // reset attempts after loading the most recent image
            }
        }            
    }

    /// <summary>
    /// invoked via gui button for debugging purposes
    /// </summary>
    public void DebugLoadScreenshot()
    {
        OnScreenShotLoadedEvent?.Invoke();
    }
}
