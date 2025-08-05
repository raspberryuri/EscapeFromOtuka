using UnityEngine;
using System.IO;

public class ScreenShotCapturer : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string folderPath = @"C:\Users\uriri\Desktop\MAKE\OtukaProject\EscapeFromOtuka\EscapeFromOtuka\Assets\Source\Images\ScreenShot";
            Directory.CreateDirectory(folderPath); // なければ作成

            string filePath = Path.Combine(folderPath, "ScreenShot.png");

            CaptureScreenShot(filePath);
        }
    }

    private void CaptureScreenShot(string filePath)
    {
        ScreenCapture.CaptureScreenshot(filePath);
        Debug.Log($"スクリーンショットを保存しました: {filePath}");
    }
}
