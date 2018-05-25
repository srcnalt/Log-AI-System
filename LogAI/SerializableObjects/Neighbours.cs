using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//this class pre defines the neighbours of a sector in 3D environment, of total 26 unit cubes
public class Neighbours
{
    public int length = 26;

    //sorted keys
    public float[] keys;
    public Dictionary<float, Point3> distanceMap = new Dictionary<float, Point3>();

    //neighbours
    public Point3[] members = new [] {
        new Point3(-1,-1, -1),
        new Point3(0, -1, -1),
        new Point3(1, -1, -1),

        new Point3(-1, 0, -1),
        new Point3(0,  0, -1),
        new Point3(1,  0, -1),

        new Point3(-1, 1, -1),
        new Point3(0,  1, -1),
        new Point3(1,  1, -1),

        new Point3(-1, -1, 0),
        new Point3(0,  -1, 0),
        new Point3(1,  -1, 0),

        new Point3(-1, 0,  0),
        new Point3(1,  0,  0),

        new Point3(-1, 1,  0),
        new Point3(0,  1,  0),
        new Point3(1,  1,  0),

        new Point3(-1, -1, 1),
        new Point3(0,  -1, 1),
        new Point3(1,  -1, 1),

        new Point3(-1, 0,  1),
        new Point3(0,  0,  1),
        new Point3(1,  0,  1),

        new Point3(-1, 1,  1),
        new Point3(0,  1,  1),
        new Point3(1,  1,  1),
    };

    //distance map created depending on players position in the sector
    //depending on how close the agent is to each neighbour sector a dictionary is created
    public void CreateDistanceMap(Point3 sector)
    {
        Point3 center = new Point3(AgentManager.instance.unitSize / 2f, AgentManager.instance.unitSize / 2f, AgentManager.instance.unitSize / 2f);

        for (int i = 0; i < length; i++)
        {
            //players position in the boinding box
            Vector3 relativePlayerPosition = AgentManager.instance.playerHead.position - AgentManager.instance.boundingBoxPivot;
            
            //center of ith neighbour
            Vector3 neighbourCenter = ((sector + members[i]) * AgentManager.instance.unitSize + center).Vector3;
            
            //distance between player and the center of ith neighbour sector
            float distance = Vector3.Distance(relativePlayerPosition, neighbourCenter);
            
            distanceMap.Add(distance, members[i] + sector);
        }

        keys = distanceMap.Keys.ToArray();
        Array.Sort(keys);
    }

    //retuns the neighbour with given key
    public Point3 GetNeighbour(float key)
    {
        return distanceMap[key];
    }
}
