using System;
using System.Collections.Generic;

//table cells of lookup table, key (point3) and value (list of log lines) structure
[Serializable]
public class TableCell
{
    public Point3 key;
    public List<LogSection> values;

    public TableCell(Point3 key, List<LogSection> values)
    {
        this.key = key;
        this.values = values;
    }
}
