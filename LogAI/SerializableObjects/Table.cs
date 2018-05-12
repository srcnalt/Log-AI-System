using System;
using System.Collections.Generic;

//lookup table structure, works similar to a hash table or dictionary
//difference is one key holds a list of items
//items with similar key added to the end of that keys list
[Serializable]
public class Table
{
    public string mapName;
    public List<TableCell> table = new List<TableCell>();

    public void Add(Point3 key, LogSection value)
    {
        TableCell cell = table.Find(o => o.key.Equals(key));

        if (cell != null)
        {
            cell.values.Add(value);
        }
        else
        {
            List<LogSection> values = new List<LogSection>();
            values.Add(value);

            table.Add(new TableCell(key, values));
        }
    }
}