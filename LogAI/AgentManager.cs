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
        for (int i = 0; i < logLines.Count; i++)
        {
            if (i == 0) continue;

            float time = 0;
            float step = logLines[0].time - oldTime;

            while (time <= step)
            {
                time += Time.deltaTime;

                //if action is not idle the controller will look at the target point at that frame
                //and the other controller keep interpolating its position and rotation
                if (logLines[0].action == ActionEnum.Idle)
                {
                    if (logLines[0].action == ActionEnum.PressLeft)
                    {
                        leftHand.LookAt(logLines[0].targetPoint.Vector3);
                        leftHand.Rotate(Vector3.right, weaponAngle);
                        rightHand.rotation = Quaternion.Lerp(Quaternion.Euler(previousLog.rightHandRotation.Vector3), Quaternion.Euler(logLines[0].rightHandRotation.Vector3), time / step);
                    }
                    else
                    {
                        rightHand.LookAt(logLines[0].targetPoint.Vector3);
                        rightHand.Rotate(Vector3.right, weaponAngle);
                        leftHand.rotation = Quaternion.Lerp(Quaternion.Euler(previousLog.leftHandRotation.Vector3), Quaternion.Euler(logLines[0].leftHandRotation.Vector3), time / step);
                    }
                }
                else
                {
                    leftHand.rotation = Quaternion.Lerp(Quaternion.Euler(previousLog.leftHandRotation.Vector3), Quaternion.Euler(logLines[0].leftHandRotation.Vector3), time / step);
                    rightHand.rotation = Quaternion.Lerp(Quaternion.Euler(previousLog.rightHandRotation.Vector3), Quaternion.Euler(logLines[0].rightHandRotation.Vector3), time / step);
                }

                leftHand.position = Vector3.Lerp(player.position - previousLog.leftHandPosition.Vector3, player.position - logLines[0].leftHandPosition.Vector3, time / step);
                rightHand.position = Vector3.Lerp(player.position - previousLog.rightHandPosition.Vector3, player.position - logLines[0].rightHandPosition.Vector3, time / step);

                playerHead.position = Vector3.Lerp(player.position - previousLog.cameraPosition.Vector3, player.position - logLines[0].cameraPosition.Vector3, time / step);
                playerHead.rotation = Quaternion.Lerp(Quaternion.Euler(previousLog.cameraRotation.Vector3), Quaternion.Euler(logLines[0].cameraRotation.Vector3), time / step);
                
                //call the action
                if (logLines[0].action != ActionEnum.Idle && !invoked)
                {
                    actionList[logLines[0].action.ToString()].Invoke();
                    invoked = true;
                }

                yield return null;
            }

            //values set for next iteration
            invoked = false;
            oldTime = logLines[0].time;
            previousLog = logLines[0];

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
