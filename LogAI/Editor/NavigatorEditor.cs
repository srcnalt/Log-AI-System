using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Navigator))]
public class NavigatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Navigator navigator = (Navigator)target;

        DrawDefaultInspector();
        
        if (navigator.tableAsset == null)
        {
            EditorGUILayout.HelpBox("Load log batch to proceed.", MessageType.Warning);
        }
        else if (GUILayout.Button("Play Agent"))
        {
            navigator.PlayAgent();
        }
    }
}
