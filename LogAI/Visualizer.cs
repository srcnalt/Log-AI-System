using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Visualizer : MonoBehaviour
{
    [Header("Visualizer Options")]
    public bool debugSections;
    public bool drawPath;
    public bool drawPathBySection;
    public bool highlightActiveCube;
    public bool displayActionDistribution;

    [Header("Unit Cube Variables")]
    private int unitSize;
    public Color drawColor;
    public Color activeCubeColor;

    [Header("Bounding Box Variables")]
    public Vector3 boundingBoxSize;
    public Vector3 boundingBoxPivot;
    public Color boundingBoxColor;

    [HideInInspector] public TextAsset output;
    private List<TableCell> cells;
    
    [HideInInspector] public TextAsset selectedLog;
    private List<LogLine> selectedLogLines;

    //unit size and bounding box veriables are set
    private void Start()
    {
        unitSize = AgentManager.instance.unitSize;

        AgentManager.instance.boundingBoxPivot = boundingBoxPivot;
        AgentManager.instance.boundingBoxSize = boundingBoxSize;

    }

    //debugging is done in OnDrawGizmos of MonoBehavior
    //the visual changes are shown only in scene window but not in the game window
    private void OnDrawGizmos()
    {
        if (unitSize < 1) return;

        DrawPathFromActiveRecording();

        ViewLogDistribution();
        
        if (debugSections)
        {
            DrawUnitCubes();
            DrawBoundingBox();
        }
    }

    //draw the bounding box of the map, it should contain all of the map
    private void DrawBoundingBox()
    {
        Gizmos.color = new Color(1, 0, 0);
        Gizmos.DrawSphere(boundingBoxPivot, 0.1f);

        Vector3 boundingBoxCenter = new Vector3(boundingBoxPivot.x + boundingBoxSize.x / 2, boundingBoxPivot.y + boundingBoxSize.y / 2, boundingBoxPivot.z + boundingBoxSize.z / 2);
        
        Gizmos.color = boundingBoxColor;
        Gizmos.DrawWireCube(boundingBoxCenter, boundingBoxSize);
    }

    //draw sectors of the game map, filling the bounding box by unitSize cubes
    private void DrawUnitCubes()
    {
        for (float x = 0; x < boundingBoxSize.x; x += unitSize)
        {
            for (float y = 0; y < boundingBoxSize.y; y += unitSize)
            {
                for (float z = 0; z < boundingBoxSize.z; z += unitSize)
                {
                    Vector3 cubeCenter = new Vector3(boundingBoxPivot.x + unitSize / 2f + x,
                                                     boundingBoxPivot.y + unitSize / 2f + y,
                                                     boundingBoxPivot.z + unitSize / 2f + z);

                    //if displayActionDistribution market true, and outpu file is loaded display the density of actions by unit cubes
                    if (displayActionDistribution && cells != null)
                    {
                        TableCell cell = cells.Find(o => o.key.Equals(new Point3(x / unitSize, y / unitSize, z / unitSize)));

                        if (cell != null)
                        {
                            Gizmos.color = new Color(activeCubeColor.r, activeCubeColor.g, activeCubeColor.b, cell.values.Count * 0.2f);
                            Gizmos.DrawCube(cubeCenter, new Vector3(unitSize, unitSize, unitSize));
                            continue;
                        }
                    }

                    if (IsPlayerInTheBox(new Vector3(x, y, z)) && highlightActiveCube)
                    {
                        Gizmos.color = activeCubeColor;
                        Gizmos.DrawCube(cubeCenter, new Vector3(unitSize, unitSize, unitSize));
                    }
                    else
                    {
                        Gizmos.color = drawColor;
                        Gizmos.DrawWireCube(cubeCenter, new Vector3(unitSize, unitSize, unitSize));
                    }
                }
            }
        }
    }

    //if a session log is loaded, display the trajectory. By log sections is wanted.
    private void DrawPathFromActiveRecording()
    {
        if (drawPath && selectedLog)
        {
            if (drawPathBySection)
            {
                List<LogSection> logSections = JsonUtility.FromJson<SessionLog>(selectedLog.text).logSections;
                Vector3 lastPoint = Vector3.zero;
                int counter = 0;

                foreach (LogSection section in logSections)
                {
                    if (lastPoint != Vector3.zero)
                    {
                        Gizmos.DrawLine(lastPoint, section.logLines[0].playerPosition.Vector3);
                        Gizmos.DrawSphere(section.logLines[0].playerPosition.Vector3, 0.1f);
                    }

                    for (int i = 0; i < section.logLines.Count; i++)
                    {
                        Gizmos.color = Color.black;

                        if (section.logLines[i].action != ActionEnum.Idle)
                        {
                            Handles.Label(section.logLines[i].playerPosition.Vector3, section.logLines[i].action.ToString());
                            Gizmos.DrawSphere(section.logLines[i].playerPosition.Vector3, 0.05f);
                        }

                        if (section.logLines[i].targetPoint.Vector3 != Vector3.zero)
                        {
                            Gizmos.color = Color.red;
                            Gizmos.DrawSphere(section.logLines[i].targetPoint.Vector3, 0.1f);
                        }

                        Gizmos.color = (counter % 2 == 0) ? Color.yellow : Color.blue;

                        if (i < section.logLines.Count - 1)
                            Gizmos.DrawLine(section.logLines[i].playerPosition.Vector3, section.logLines[i + 1].playerPosition.Vector3);
                        else
                        {
                            lastPoint = section.logLines[i].playerPosition.Vector3;
                        }
                    }

                    counter++;
                }
            }
            else
            {
                GetLogLines();

                for (int i = 0; i < selectedLogLines.Count; i++)
                {
                    Gizmos.color = Color.black;

                    if (selectedLogLines[i].action != ActionEnum.Idle)
                    {
                        Handles.Label(selectedLogLines[i].playerPosition.Vector3, selectedLogLines[i].action.ToString());
                        Gizmos.DrawSphere(selectedLogLines[i].playerPosition.Vector3, 0.05f);
                    }

                    if (selectedLogLines[i].action != ActionEnum.Idle)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(selectedLogLines[i].targetPoint.Vector3, 0.2f);
                    }

                    Gizmos.color = Color.yellow;

                    if(i < selectedLogLines.Count - 1)
                        Gizmos.DrawLine(selectedLogLines[i].playerPosition.Vector3, selectedLogLines[i + 1].playerPosition.Vector3);
                }
            }
        }
    }

    //check if the player is in cubeIndex unit cube
    private bool IsPlayerInTheBox(Vector3 cubeIndex)
    {
        Vector3 pos = AgentManager.instance.player.position;

        if (((pos.x > boundingBoxPivot.x + cubeIndex.x) && (pos.x < boundingBoxPivot.x + cubeIndex.x + unitSize)) &&
        ((pos.y > boundingBoxPivot.y + cubeIndex.y) && (pos.y < boundingBoxPivot.y + cubeIndex.y + unitSize)) &&
        ((pos.z > boundingBoxPivot.z + cubeIndex.z) && (pos.z < boundingBoxPivot.z + cubeIndex.z + unitSize)))
        {
            return true;
        }

        return false;
    }

    //initiate the replay the selected session log
    public void Replay()
    {
        Debug.Log("Replay started.");
        AgentManager.instance.DisableVRControls();
        GetLogLines();
        PlayRecordingSteps();
    }

    //load text asset into list of log sections
    public void GetLogLines()
    {
        List<LogSection> logSections = JsonUtility.FromJson<SessionLog>(selectedLog.text).logSections;
        selectedLogLines = new List<LogLine>();

        foreach (LogSection ls in logSections)
        {
            selectedLogLines.AddRange(ls.logLines);
        }
    }

    //reset the player position and initial log line and call AgentManager.Move
    private void PlayRecordingSteps()
    {
        List<LogLine> logLines = selectedLogLines;
        LogLine previousLog = logLines[0];
        AgentManager.instance.player.position = previousLog.playerPosition.Vector3;

        AgentManager.instance.Move(logLines, previousLog);
    }

    //load lookup table from text asset
    public void ViewLogDistribution()
    {
        if (output)
        {
            Table logBatch = JsonUtility.FromJson<Table>(output.text);
            cells = logBatch.table;
        }
    }
}
