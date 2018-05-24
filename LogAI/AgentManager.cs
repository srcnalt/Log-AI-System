using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    //agent manager singleton
    public static AgentManager instance;

    public IEnumerator move;
    [HideInInspector] public bool isMoving = false;

    //amount of angle to nullify initial hand rotation (an issue of current game objects)
    private float weaponAngle = -45;

    //unit size for sectors
    public int unitSize;

    //register your actions to this dictionary
    public Dictionary<string, Action> actionList = new Dictionary<string, Action>();

    //player body parts
    [Header("Player Components")]
    public Transform player;
    public Transform playerHead;
    public Transform rightHand;
    public Transform leftHand;

    //pointers of VR controllers, use these types for Vive
    /*
    public HTC.UnityPlugin.Vive.VivePoseTracker pointerRight;
    public HTC.UnityPlugin.Vive.VivePoseTracker pointerLeft;
    */

    //map bounding box for recording area
    [HideInInspector] public Vector3 boundingBoxSize;
    [HideInInspector] public Vector3 boundingBoxPivot;

    //instance is set and made sure there is only one instance
    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            DestroyObject(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    //register actions when game starts
    /*
    private void Start()
    {
        RegisterActions();
    }
    */

    /*
     Move method and move coroutine are used in both Visualizer and Navigator
     for replay and agent movement, to avoid code duplication the code moved here
    */
    public void Move(List<LogLine> logLines, LogLine previousLog)
    {
        if(move != null) StopCoroutine(move);
        move = MoveCoroutine(logLines, previousLog);
        StartCoroutine(move);
    }
    
    private IEnumerator MoveCoroutine(List<LogLine> logLines, LogLine previousLog)
    {
    
        // isMoving is checked in calling component to wait for the next movement desicion
        isMoving = true;

        //for continuity previous between log sections, log line's time is set to oldTime  
        float oldTime = previousLog.time;
        
        // invoked is used for calling action method only once
        bool invoked = false;
        
        // body parts of the agent are linearly interpolated to the each next log line
        // during the time difference of current and previous log line
        foreach (LogLine line in logLines)
        {
            if (logLines.IndexOf(line) == 0) continue;

            float time = 0;
            float step = line.time - oldTime;

            while (time <= step)
            {
                time += Time.deltaTime;

                //if action is not idle the controller will look at the target point at that frame
                //and the other controller keep interpolating its position and rotation
                if (line.targetPoint.Vector3 != Vector3.zero)
                {
                    if (line.action == ActionEnum.PressLeft)
                    {
                        leftHand.LookAt(line.targetPoint.Vector3);
                        leftHand.Rotate(Vector3.right, weaponAngle);
                        rightHand.rotation = Quaternion.Lerp(Quaternion.Euler(previousLog.rightHandRotation.Vector3), Quaternion.Euler(line.rightHandRotation.Vector3), time / step);
                    }
                    else
                    {
                        rightHand.LookAt(line.targetPoint.Vector3);
                        rightHand.Rotate(Vector3.right, weaponAngle);
                        leftHand.rotation = Quaternion.Lerp(Quaternion.Euler(previousLog.leftHandRotation.Vector3), Quaternion.Euler(line.leftHandRotation.Vector3), time / step);
                    }
                }
                else
                {
                    leftHand.rotation = Quaternion.Lerp(Quaternion.Euler(previousLog.leftHandRotation.Vector3), Quaternion.Euler(line.leftHandRotation.Vector3), time / step);
                    rightHand.rotation = Quaternion.Lerp(Quaternion.Euler(previousLog.rightHandRotation.Vector3), Quaternion.Euler(line.rightHandRotation.Vector3), time / step);
                }

                leftHand.position = Vector3.Lerp(player.position - previousLog.leftHandPosition.Vector3, player.position - line.leftHandPosition.Vector3, time / step);
                rightHand.position = Vector3.Lerp(player.position - previousLog.rightHandPosition.Vector3, player.position - line.rightHandPosition.Vector3, time / step);

                playerHead.position = Vector3.Lerp(player.position - previousLog.cameraPosition.Vector3, player.position - line.cameraPosition.Vector3, time / step);
                playerHead.rotation = Quaternion.Lerp(Quaternion.Euler(previousLog.cameraRotation.Vector3), Quaternion.Euler(line.cameraRotation.Vector3), time / step);
                
                //call the action
                if (line.action != ActionEnum.Idle && !invoked)
                {
                    actionList[line.action.ToString()].Invoke();
                    invoked = true;
                }

                yield return null;
            }

            //values set for next iteration
            invoked = false;
            oldTime = line.time;
            previousLog = line;

            yield return null;
        }

        // caller component is free to send a new list of log lines
        isMoving = false;
    }

    // to be able to unstrap agent from VR controls trackers and pointers needs to be disabled
    // if needed hands or weapons needs to be initialized here
    public void DisableVRControls()
    {
        /*
        rightHand.gameObject.SetActive(true);
        leftHand.gameObject.SetActive(true);

        rightHand.GetComponent<SteamVR_TrackedObject>().enabled = false;
        leftHand.GetComponent<SteamVR_TrackedObject>().enabled = false;

        pointerLeft.enabled = false;
        pointerRight.enabled = false;

        rightHand.GetComponent<Hand>().SetActiveWeapon(0);
        leftHand.GetComponent<Hand>().SetActiveWeapon(0);
        
        playerHead.GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.None;
        */
    }
    
    // register controller actions to the action dictionary by the same names of ActionEnum
    // for more actions exten this enum
    private void RegisterActions()
    {
        /*
        actionList.Add("PressRight", rightHand.GetComponent<Hand>().WeaponPrimary);
        actionList.Add("ReleaseRight", rightHand.GetComponent<Hand>().WeaponFireEnd);
        actionList.Add("PressLeft", leftHand.GetComponent<Hand>().WeaponPrimary);
        actionList.Add("ReleaseLeft", leftHand.GetComponent<Hand>().WeaponFireEnd);
        */
    }
}
