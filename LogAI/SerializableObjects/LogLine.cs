using System;

//log information that is saved for every loggerTick and action

[Serializable]
public class LogLine
{
    public float time;
    public StateEnum state;
    public ActionEnum action;

    //body
    public Point3 playerPosition;

    //head
    public Point3 cameraPosition;
    public Point3 cameraRotation;

    //left hand
    public Point3 leftHandPosition;
    public Point3 leftHandRotation;

    //right hand
    public Point3 rightHandPosition;
    public Point3 rightHandRotation;

    //target point
    public Point3 targetPoint;
}