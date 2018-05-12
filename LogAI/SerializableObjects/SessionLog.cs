using System;
using System.Collections.Generic;

//encapsulating structure for log files, contains log metadata and log sections recorded during the game session
[Serializable]
public class SessionLog
{
    public string sessionStart;
    public string sessionEnd;
    public string mapName;
    public List<LogSection> logSections;

    public SessionLog(string sessionStart, string mapName)
    {
        this.sessionStart = sessionStart;
        this.sessionEnd = "";
        this.mapName = mapName;
        this.logSections = new List<LogSection>();
    }
}