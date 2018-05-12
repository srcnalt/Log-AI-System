using System;
using UnityEngine;

//Equavalent to Unity's Vector3 method but which is serializable
//Serializable classes can be saved in JSON files
[Serializable]
public struct Point3
{
    public float x;
    public float y;
    public float z;
    
    public Point3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    //property for returning Point3 as Unity's (non-serializable) Vector3
    public Vector3 Vector3
    {
        get {
            return new Vector3(x, y, z);
        }

        set {
            x = value.x;
            y = value.y;
            z = value.z;
        }
    }

    //returns identitity vector 
    public static Point3 one
    {
        get
        {
            return new Point3(1, 1, 1);
        }
    }

    //Vector3 to Point3 conversion
    public static Point3 ToPoint(Vector3 v)
    {
        return new Point3(v.x, v.y, v.z);
    }

    //Returns ceiling values for current position divided by unit size
    //Used for determining current sector
    public static Point3 DivideRound(Vector3 v, float unitSize)
    {
        return new Point3(
            Mathf.Ceil(v.x / unitSize),
            Mathf.Ceil(v.y / unitSize),
            Mathf.Ceil(v.z / unitSize)
        );
    }

    public override string ToString()
    {
        return "[" + x + ", " + y + ", " + z + "]";
    }

    public override bool Equals(object o)
    {
        //return false if type mismatch
        if (o == null || GetType() != o.GetType())
            return false;

        //cast to point3
        Point3 p = (Point3)o;

        return p.x == x && p.y == y && p.z == z;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static Point3 operator - (Point3 p1, Point3 p2)
    {
        return new Point3(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);
    }

    public static Point3 operator + (Point3 p1, Point3 p2)
    {
        return new Point3(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);
    }
}
