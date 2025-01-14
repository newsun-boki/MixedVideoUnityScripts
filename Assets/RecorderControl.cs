using UnityEngine;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEditor;

public class RecorderControl : MonoBehaviour
{
    private RecorderControllerSettings recorderControllerSettings;
    private RecorderController recorderController;

    public Camera targetCamera; // Assign your target camera in the inspector

    void Start()
    {
        SetupRecorder();
        StartRecording();
    }

    void SetupRecorder()
    {
        // Create a new RecorderControllerSettings
        recorderControllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();

        // Create a new MovieRecorderSettings
        var movieRecorderSettings = ScriptableObject.CreateInstance<MovieRecorderSettings>();
        movieRecorderSettings.name = "My Video Recorder";
        movieRecorderSettings.Enabled = true;

        // Set the output file path
        movieRecorderSettings.OutputFile = "MyVideoOutput/MyVideo";

        // Set the image input settings to record from the target camera
        movieRecorderSettings.ImageInputSettings = new CameraInputSettings
        {
            Source = ImageSource.TaggedCamera,
            OutputWidth = 1920,
            OutputHeight = 1080,
            CameraTag = targetCamera.tag // Ensure your camera has a unique tag
        };

        // Add the movie recorder settings to the controller settings
        recorderControllerSettings.AddRecorderSettings(movieRecorderSettings);

        // Create a new RecorderController
        recorderController = new RecorderController(recorderControllerSettings);
    }

    void StartRecording()
    {
        // Start the recording session
        recorderController.PrepareRecording();
        recorderController.StartRecording();
    }

    void StopRecording()
    {
        // Stop the recording session
        if (recorderController.IsRecording())
        {
            recorderController.StopRecording();
        }
    }

    void OnDestroy()
    {
        // Ensure recording is stopped when the object is destroyed
        StopRecording();
    }
}