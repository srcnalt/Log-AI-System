using System;
using System.Collections.Generic;

//log section is the structure that holds a set of log lines with their starting point (sector) and success value
[Serializable]
public class LogSection
{
    public Point3 sector;
    public float successValue;
    public List<LogLine> logLines = new List<LogLine>();
}
