using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Visualizer))]
public class VisualizerEditor : Editor
{
    Visualizer visualizer;

    public override void OnInspectorGUI()
    {
        visualizer = (Visualizer)target;

        DrawDefaultInspector();

        SelectedLog();
        SelectedOutput();
    }

    private void SelectedLog()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Selected Log to Display", EditorStyles.boldLabel);

        visualizer.selectedLog = EditorGUILayout.ObjectField("Selected Log", visualizer.selectedLog, typeof(TextAsset), true) as TextAsset;

        if (!visualizer.selectedLog)
        {
            EditorGUILayout.HelpBox("Log file not found...", MessageType.Warning);
        }
        else
        {
            if (GUILayout.Button("Start Replay"))
            {
                visualizer.Replay();
            }
        }
    }

    private void SelectedOutput()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Log Batch to View", EditorStyles.boldLabel);

        visualizer.output = EditorGUILayout.ObjectField("Output", visualizer.output, typeof(TextAsset), true) as TextAsset;

        if (visualizer.displayActionDistribution)
        {
            visualizer.ViewLogDistribution();
        }
    }
}
