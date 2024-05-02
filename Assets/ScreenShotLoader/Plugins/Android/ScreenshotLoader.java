package com.example.screenshotloader;

import com.unity3d.player.UnityPlayer;

import android.Manifest;
import android.app.Activity;
import android.content.pm.PackageManager;
import android.util.Log;

import androidx.core.app.ActivityCompat;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.util.Arrays;
import java.util.Comparator;

public class ScreenshotLoader {
    private static final int REQUEST_PERMISSION_CODE = 222;
    private static final String SCREENSHOT_DIRECTORY_PATH = "/sdcard/Oculus/Screenshots/";

    private byte[] latestScreenshotBytes = new byte[0];

    public boolean getLatestScreenshot() {
        String latestScreenshotPath = getLatestImageFilePath(SCREENSHOT_DIRECTORY_PATH);
        Log.d("ScreenshotLoader", "Latest screenshot path: " + latestScreenshotPath);
        if (latestScreenshotPath != null) {
            try {
                latestScreenshotBytes = loadImageData(latestScreenshotPath);
                return true;
            } catch (IOException e) {
                return false;
            }
        }
        return false;
    }

    public byte[] getLatestScreenshotBytes() {
        return latestScreenshotBytes;
    }

    public static ScreenshotLoader getInstance() {
        return new ScreenshotLoader();
    }

    public static void requestHasReadStoragePermissionCallback(Activity activity, String gameObjectName, String callbackMethodName) {
        if (hasReadStoragePermission(activity)) {
            UnityPlayer.UnitySendMessage(gameObjectName, callbackMethodName, "true");
        }
        else {
            UnityPlayer.UnitySendMessage(gameObjectName, callbackMethodName, "false");
        }
    }
    public static void requestPermissionIfNeeded(Activity activity) {
        activity.runOnUiThread(() -> {
                    if (!hasReadStoragePermission(activity)) {
                        ActivityCompat.requestPermissions(activity,
                                new String[]{Manifest.permission.READ_EXTERNAL_STORAGE},
                                REQUEST_PERMISSION_CODE);
                    }
                }
        );
    }

    private static Boolean hasReadStoragePermission(Activity activity) {
        return activity.checkSelfPermission(Manifest.permission.READ_EXTERNAL_STORAGE) == PackageManager.PERMISSION_GRANTED;
    }

    private static String getLatestImageFilePath(String directoryPath) {
        File directory = new File(directoryPath);
        File[] files = directory.listFiles((dir, name) -> name.toLowerCase().endsWith(".jpg"));

        if (files != null && files.length > 0) {
            Arrays.sort(files, Comparator.comparingLong(File::lastModified).reversed());
            return files[0].getAbsolutePath();
        } else {
            return null;
        }
    }

    private static byte[] loadImageData(String imagePath) throws IOException {
        File imageFile = new File(imagePath);
        byte[] imageData = new byte[(int) imageFile.length()];

        try (FileInputStream fileInputStream = new FileInputStream(imageFile)) {
            if (fileInputStream.read(imageData) == -1) {
                throw new IOException("Failed to read image data.");
            }
        }

        return imageData;
    }
}
