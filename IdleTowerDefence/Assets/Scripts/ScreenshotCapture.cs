using UnityEngine;

public class ScreenshotCapture : MonoBehaviour
{
    // Specify the key to trigger the screenshot
    public KeyCode screenshotKey = KeyCode.F12;

    // Specify the folder path to save the screenshots
    public string screenshotFolder = "Screenshots";

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
    void Update()
    {
        // Check if the specified key is pressed
        if (Input.GetKeyDown(screenshotKey))
        {
            // Create the folder if it doesn't exist
            if (!System.IO.Directory.Exists(screenshotFolder))
            {
                System.IO.Directory.CreateDirectory(screenshotFolder);
            }

            // Generate a unique filename based on the current date and time
            string filename = $"{screenshotFolder}/Screenshot_{System.DateTime.Now.ToString("yyyyMMdd_HHmmss")}.png";

            // Capture the screenshot and save it to the specified file
            ScreenCapture.CaptureScreenshot(filename);

            // Log the path to the console
            Debug.Log($"Screenshot captured: {filename}");
        }
    }
}
