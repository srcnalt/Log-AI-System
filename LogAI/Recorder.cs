using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Recorder : MonoBehaviour
{
    //recorder is another singleton, which should be called when
    //VR controller actions are taken for recording those actions
    public static Recorder instance;

    [Header("Recording Parameters")]
    public float loggerTick;
    private float unitSize;
    private float distance = 0.1f;

    private SessionLog sessionLog;
    private LogSection logSection;
    [HideInInspector]
    public bool isRecording = false;

    //player state variables
    [HideInInspector]
    public ActionEnum currentAction = ActionEnum.Idle;
    private StateEnum currentState = StateEnum.OnGround;
    private StateEnum previousState = StateEnum.InAir;

    //init singleton
    private void Awake()
    {
        if (instance != this)
            Destroy(instance);

        instance = this;

        DontDestroyOnLoad(this);
    }

    //set unitSize as float
    private void Start()
    {
        unitSize = AgentManager.instance.unitSize * 1f;
    }

    //continiously check whether the player is in the air or on the ground
    private void Update()
    {
        CheckState();
    }

    //helper method alled from grip button to start and stop recording gameplays
    public void GripButtonRecorder()
    {
        if (isRecording) Stop();
        else Record();
    }

    //initializes the session log and invokes Logger method every loggerTick 
    public void Record()
    {
        Debug.Log("Recording Started");

        isRecording = true;

        sessionLog = new SessionLog(DateTime.Now.ToString("dd-MM-yy-HH-mm-ss"), SceneManager.GetActiveScene().name);

        InvokeRepeating("Logger", 0, loggerTick);
    }

    //Stops invoking logger method, saves session log and resets some variables
    public void Stop()
    {
        Debug.Log("Recording Ended");

        CancelInvoke();
        isRecording = false;

        sessionLog.logSections.Add(logSection);
        sessionLog.sessionEnd = DateTime.Now.ToString("dd-MM-yy-HH-mm-ss");

        string json = JsonUtility.ToJson(sessionLog, true);

        File.WriteAllText(Application.dataPath + "/Resources/Logs/session_log_" + sessionLog.sessionStart + ".json", json);

        //clean
        currentState = StateEnum.OnGround;
        previousState = StateEnum.InAir;
        logSection = new LogSection();
    }

    //logger method without parameters, repeatedly invoked, created log lines have no action and target point
    public void Logger()
    {
        Logger(Vector3.zero);
    }
    
    //logger method logs are grouped in log sections and all the body parts of the player is captured
    public void Logger(Vector3 targetPoint)
    {
        if (isRecording)
        {
            if (previousState == StateEnum.InAir && currentState == StateEnum.OnGround)
            {
                if (logSection != null)
                    sessionLog.logSections.Add(logSection);
                
                logSection = new LogSection();
                logSection.sector = Point3.DivideRound(AgentManager.instance.playerHead.position - AgentManager.instance.boundingBoxPivot, unitSize) - Point3.one;
            }

            LogLine logLine = new LogLine();

            logLine.time = Time.time;
            logLine.state = currentState;
            logLine.action = currentAction;

            logLine.playerPosition = Point3.ToPoint(AgentManager.instance.player.position);

            logLine.cameraPosition = logLine.playerPosition - Point3.ToPoint(AgentManager.instance.playerHead.position);
            logLine.cameraRotation = Point3.ToPoint(AgentManager.instance.playerHead.rotation.eulerAngles);

            logLine.leftHandPosition = logLine.playerPosition - Point3.ToPoint(AgentManager.instance.leftHand.position);
            logLine.leftHandRotation = Point3.ToPoint(AgentManager.instance.leftHand.rotation.eulerAngles);

            logLine.rightHandPosition = logLine.playerPosition - Point3.ToPoint(AgentManager.instance.rightHand.position);
            logLine.rightHandRotation = Point3.ToPoint(AgentManager.instance.rightHand.rotation.eulerAngles);

            logLine.targetPoint = Point3.ToPoint(targetPoint);

            logSection.logLines.Add(logLine);

            previousState = currentState;
        }
    }

    //casting a ray to the ground and changing the state of the player if it has a hit
    //used for grouping log lines into sections
    private void CheckState()
    {
        RaycastHit hit = new RaycastHit();
        Debug.DrawRay(AgentManager.instance.player.position - Vector3.down * distance, Vector3.down * distance * 2, Color.red);

        if (Physics.Raycast(AgentManager.instance.player.position - Vector3.down * distance, Vector3.down, out hit, distance * 2))
        {
            if (currentState == StateEnum.OnGround) return;

            currentState = StateEnum.OnGround;
        }
        else
        {
            if (currentState == StateEnum.InAir) return;

            currentState = StateEnum.InAir;
        }
    }
}
