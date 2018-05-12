using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Navigator : MonoBehaviour
{
    [Header("Difficulty Range")]
    public MinMax diffRange = new MinMax();

    [Header("Navigation Parameters")]
    public bool agentIsOnline = false;
    private int unitSize;

    [Header("Selected Batch File for Replay")]
    public TextAsset tableAsset;
    [HideInInspector] public Table logBatch;

    LogLine previousLog;
    LogLine oldLog;

    //set unitSize
    public void Start()
    {
        unitSize = AgentManager.instance.unitSize;
    }

    //Update method in Navigator works as a main game loop for the agent
    //but it always wait for AgentManager.Move to end then run again
    private void Update()
    {
        if (agentIsOnline && !AgentManager.instance.isMoving)
        {
            TableCell cell = GetCell();
            List<LogSection> logSectionList = cell.values;
            List<LogLine> randomLogLines = GetLogLines(logSectionList);

            if (previousLog == null)
                previousLog = randomLogLines[0];
            else
            {
                previousLog = oldLog;
                previousLog.time = randomLogLines[0].time - Recorder.instance.loggerTick;
            }

            AgentManager.instance.Move(randomLogLines, previousLog);

            oldLog = randomLogLines[randomLogLines.Count - 1];
        }
    }

    //Loading lookup table and setting agent online, to enable update loop
    public void PlayAgent()
    {
        AgentManager.instance.DisableVRControls();
        logBatch = JsonUtility.FromJson<Table>(tableAsset.text);
        agentIsOnline = true;
    }

    //Gets the table cell which matches the current sector the agent is in
    private TableCell GetCell()
    {
        Point3 currentSector = Point3.DivideRound(AgentManager.instance.playerHead.position - AgentManager.instance.boundingBoxPivot, unitSize) - Point3.one;
        TableCell cell = logBatch.table.Find(o => o.key.Equals(currentSector));

        if (cell != null) return cell;

        return FindClosestCell(currentSector);        
    }

    //A random log line is choosen which fits given the difficulty range
    private List<LogLine> GetLogLines(List<LogSection> logSectionList)
    {
        int length = logSectionList.Count - 1;

        float div = length / diffRange.maxLimit;

        float rand = UnityEngine.Random.Range(diffRange.pivotMin * div, diffRange.pivotMax * div);

        int index = Mathf.RoundToInt(rand);

        return logSectionList[index].logLines;
    }

    //if the current sector empty, looking for closest table cells matching neighbour sectors
    private TableCell FindClosestCell(Point3 sector)
    {
        Neighbours nb = new Neighbours();

        nb.CreateDistanceMap(sector);

        float[] keys = nb.keys;

        for (int i = 0; i < nb.length; i++)
        {
            float key = nb.keys[i];

            TableCell cell = logBatch.table.Find(o => o.key.Equals(nb.GetNeighbour(key)));

            if (cell != null) return cell;
        }

        agentIsOnline = false;

        throw new Exception("All neighbours are empty!");
    }
}
