using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Recorder))]
public class RecorderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Recorder recorder = (Recorder)target;

        DrawDefaultInspector();
        
        if (recorder.isRecording)
        {
            if (GUILayout.Button("Stop and Save"))
            {
                recorder.Stop();
            }
        }
        else
        {
            if (GUILayout.Button("Start Recording"))
            {
                recorder.Record();
            }
        }
    }
}
