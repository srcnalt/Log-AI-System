using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Refiner : MonoBehaviour
{
    //log and lookup table paths
    private string logPath = "/Resources/Logs/";
    private string tablePath = "/Resources/Tables/Table";

    private List<LogSection> logSections;
    private List<LogSection> sectionsToRemove;
    private Table logBatch;
    private int numOfLogs = 0;
    
    //main call to list of refining functionality
    public void RefineLogs()
    {
        Clear();
        CollectLogSections();
        SetSuccessValue();
        CleanLogs();
        SortBySuccessValue();
        CreateLookuptable();
    }

    //reset the variables
    private void Clear()
    {
        logSections = new List<LogSection>();
        sectionsToRemove = new List<LogSection>();
        logBatch = new Table();
    }

    //get all the log files from log path and add each of them to a main log section list
    private void CollectLogSections()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + logPath);
        FileInfo[] logFiles = dir.GetFiles("*.json");

        foreach (FileInfo log in logFiles)
        {
            numOfLogs++;

            StreamReader streamReader = log.OpenText();
            string json = streamReader.ReadToEnd();

            SessionLog sessionLog = JsonUtility.FromJson<SessionLog>(json);

            logSections.AddRange(sessionLog.logSections);
        }
    }

    //success value of each log section is calculated depending on their attributes and set
    private void SetSuccessValue()
    {
        foreach (LogSection section in logSections)
        {
            if (section.logLines.Count <= 1) continue;

            float time = section.logLines[section.logLines.Count - 1].time - section.logLines[0].time;
            int actionCount = 0;
            float distance = 0;

            Vector3 oldPosition = Vector3.zero;

            foreach (LogLine line in section.logLines)
            {
                if (line.action == ActionEnum.PressLeft || line.action == ActionEnum.PressRight) actionCount++;

                if(oldPosition == Vector3.zero)
                {
                    oldPosition = line.playerPosition.Vector3;
                }
                else
                {
                    distance += Vector3.Distance(line.playerPosition.Vector3, oldPosition);
                    oldPosition = line.playerPosition.Vector3;
                }
            }

            if (actionCount > 0)
                section.successValue = distance / (time * actionCount);
            else
                section.successValue = 0;

            //set sector
            section.sector = Point3.DivideRound(section.logLines[0].playerPosition.Vector3 - section.logLines[0].cameraPosition.Vector3 - AgentManager.instance.boundingBoxPivot, AgentManager.instance.unitSize) - Point3.one;
        }
    }

    //log sections those fall below 2nd standard deviation
    private void CleanLogs()
    {
        //remove all zeros
        //due to huge number of residual pieces all zero success value log sections are removed
        logSections.RemoveAll(section => section.successValue == 0);

        float total = 0;
        float mean = 0;
        float variance = 0;
        float sd = 0;

        // 1 added to log section count for assuming there is one zero success value log section
        float count = logSections.Count + 1;

        foreach (LogSection section in logSections)
        {
            total += section.successValue;
        }

        mean = total / count;
        total = 0;

        foreach (LogSection section in logSections)
        {
            total += Mathf.Pow(section.successValue - mean, 2);
        }
        
        variance = total / (count - 1);

        sd = Mathf.Sqrt(variance);

        logSections.RemoveAll(section => section.successValue < mean - sd * 2);
    }

    //log files are sorted by their success values
    private void SortBySuccessValue()
    {
        logSections = logSections.OrderBy(o => o.successValue).ToList();
    }
    
    //a dictionary of log section list created as a product to be used in navigation
    //table has sectors as keys and log section list as value
    private void CreateLookuptable()
    {
        logBatch.mapName = SceneManager.GetActiveScene().name;

        foreach (LogSection section in logSections)
        {
            logBatch.Add(section.sector, section);
        }
        
        string tableText = JsonUtility.ToJson(logBatch);

        File.WriteAllText(Application.dataPath + tablePath + "_" + logBatch.mapName + "_" + AgentManager.instance.unitSize + "_" + DateTime.Now.ToString("HHmmss") + ".json", tableText);

        Debug.Log("Table Created (" + numOfLogs + " logs)");

        numOfLogs = 0;
    }
}
