using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Refiner))]
public class RefinerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Refiner refiner = (Refiner)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Collect Logs"))
        {
            refiner.RefineLogs();
        }
    }
}
