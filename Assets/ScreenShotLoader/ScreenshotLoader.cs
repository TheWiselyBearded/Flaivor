#nullable enable

using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotLoader : MonoBehaviour
{
    private const int MAX_CHECK_PERMISSION_COUNT = 100;

    [SerializeField] private Button requestPermissionButton = default!;
    [SerializeField] private Button loadScreenShotButton = default!;
    [SerializeField] private RawImage rawImage = default!;
    [SerializeField] private float checkPermissionInterval = 1.0f;

    private Texture2D texture = default!;
    private bool permissionGranted = false;

    public void ReceiveHasReadStoragePermissionCallback(string message)
    {
        if (message.Equals("true"))
        {
            permissionGranted = true;
            requestPermissionButton.gameObject.SetActive(false);
            loadScreenShotButton.gameObject.SetActive(true);
        }
    }

    private void Awake()
    {
        requestPermissionButton.onClick.AddListener(RequestPermission);
        loadScreenShotButton.onClick.AddListener(loadScreenShot);

        loadScreenShotButton.gameObject.SetActive(false);

        texture = new Texture2D(1, 1);
        rawImage.texture = texture;
        
        RequestCheckPermission();
    }

    private void OnDestroy()
    {
        if (requestPermissionButton != null)
        {
            requestPermissionButton.onClick.RemoveListener(RequestPermission);
        }
        if (loadScreenShotButton != null)
        {
            loadScreenShotButton.onClick.RemoveListener(loadScreenShot);
        }
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

    private void loadScreenShot()
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
                            texture.LoadImage(screenshotBytes);
                            texture.Apply();
                            rawImage.texture = texture;
                        }
                    }
                }
            }
        }
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
}
