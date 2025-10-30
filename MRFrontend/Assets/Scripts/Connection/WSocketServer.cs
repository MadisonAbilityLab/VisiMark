using Microsoft.MixedReality.SampleQRCodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR;
using WebSocketSharp;
using WebSocketSharp.Server;


public class WSocketServer : MonoBehaviour
{
    private WebSocketServer wsServer;

    public static String strLog = "Start Debug!";
    public static String strMonitoring = "Start Monitoring!";
    public static String strRay = "Start Raycast!";

    public static String strLog_prev = "";
    public static String strMonitoring_prev = "";
    public static String strRay_prev = "";

    long start_time = -1;
    long last_update_time = -1;
    const float UpdateTime = (float)0.5;
    bool ifInitialized = false;

    void GetLog(String str)
    {
        strLog = str;
        recordData(Application.persistentDataPath + "/logs_" + start_time + "/debug_log_"+start_time, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()+" " + strLog);
    }
    void GetRayLog(String str)
    {
        strRay = str;
        recordData(Application.persistentDataPath + "/logs_" + start_time + "/ray_log_" + start_time, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString() + " " + strRay);
    }
    void Start()
    {
        CreateAugmentations.MessageSender += GetLog;
        CreateColorHallways.MessageSender += GetLog;
        CreateSignBoards.MessageSender += GetLog;
        MyQRVis.MessageSender += GetLog;
        MyQRCode.MessageSender += GetLog;
        AxisAdapter.MessageSender += GetLog;
        SetArrowSizeAndMaterial.MessageSender += GetLog;
        LoadCustomization.MessageSender += GetLog;
        Tutorial.MessageSender += GetLog;


        RayOfParticipants.MessageSender += GetRayLog;


        start_time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        last_update_time = start_time;
        Directory.CreateDirectory(Application.persistentDataPath + "/logs_" + start_time);


        wsServer = new WebSocketServer(8012);

        wsServer.AddWebSocketService<ControlSocketBehavior>("/control");
        wsServer.AddWebSocketService<DebugSocketBehavior>("/debug");
        wsServer.AddWebSocketService<MonitoringSocketBehavior>("/monitoring");

        wsServer.Start();
    }

    public void initializeLog()
    {
        strMonitoring = "(route:" + AxisAdapter.route + ";)"
            + "(anchor0:x" + AxisAdapter.anchor0.x + ";y" + AxisAdapter.anchor0.y + ";z" + AxisAdapter.anchor0.z + ";)"
            + "(vectorX:x" + AxisAdapter.vectorX.x + ";y" + AxisAdapter.vectorX.y + ";z" + AxisAdapter.vectorX.z + ";)"
            + "(vectorY:x" + AxisAdapter.vectorY.x + ";y" + AxisAdapter.vectorY.y + ";z" + AxisAdapter.vectorY.z + ";)"
            + "(vectorZ:x" + AxisAdapter.vectorZ.x + ";y" + AxisAdapter.vectorZ.y + ";z" + AxisAdapter.vectorZ.z + ";)";
        ifInitialized = true;
    }



    void OnDestroy()
    {
        
        wsServer.Stop();
    }

    void Update()
    {
        if (!ifInitialized)
        {
            return;
        }
        long tmp_time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        if (tmp_time - last_update_time >= UpdateTime)
        {
            if (!InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 headPosition))
            {
                headPosition = Vector3.zero;
            }
            if (!InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion headRotation))
            {
                headRotation = Quaternion.identity;
            }
            Vector3 headForward = headRotation * Vector3.forward;
            strMonitoring = "(headPosition:x" + headPosition.x + ";y" + headPosition.y + ";z" + headPosition.z + ";)" +
                "(headForward:x" + headForward.x + ";y" + headForward.y + ";z" + headForward.z + ";)"
                + "(anchor0:x" + AxisAdapter.anchor0.x + ";y" + AxisAdapter.anchor0.y + ";z" + AxisAdapter.anchor0.z + ";)"
            + "(vectorX:x" + AxisAdapter.vectorX.x + ";y" + AxisAdapter.vectorX.y + ";z" + AxisAdapter.vectorX.z + ";)"
            + "(vectorY:x" + AxisAdapter.vectorY.x + ";y" + AxisAdapter.vectorY.y + ";z" + AxisAdapter.vectorY.z + ";)"
            + "(vectorZ:x" + AxisAdapter.vectorZ.x + ";y" + AxisAdapter.vectorZ.y + ";z" + AxisAdapter.vectorZ.z + ";)"
             + "(route:" + AxisAdapter.route + ";)" + "(time:" + tmp_time.ToString() + ";)";
            recordData(Application.persistentDataPath + "/logs_" + start_time + "/head_movement_" + start_time,
                 "(headPosition:x" + headPosition.x + ";y" + headPosition.y + ";z" + headPosition.z + ";)" +
                "(headForward:x" + headForward.x + ";y" + headForward.y + ";z" + headForward.z + ";)"
                + "(anchor0:x" + AxisAdapter.anchor0.x + ";y" + AxisAdapter.anchor0.y + ";z" + AxisAdapter.anchor0.z + ";)"
            + "(vectorX:x" + AxisAdapter.vectorX.x + ";y" + AxisAdapter.vectorX.y + ";z" + AxisAdapter.vectorX.z + ";)"
            + "(vectorY:x" + AxisAdapter.vectorY.x + ";y" + AxisAdapter.vectorY.y + ";z" + AxisAdapter.vectorY.z + ";)"
            + "(vectorZ:x" + AxisAdapter.vectorZ.x + ";y" + AxisAdapter.vectorZ.y + ";z" + AxisAdapter.vectorZ.z + ";)"
             + "(route:" + AxisAdapter.route + ";)" + "(time:" + tmp_time.ToString() + ";)");
            last_update_time = tmp_time;
        }
    }

    private void recordData(string PathOut, String s)
    {
        StreamWriter sw = File.AppendText(PathOut);
        sw.WriteLine(s);
        sw.Flush();
        sw.Close();
    }
}
public class ControlSocketBehavior : WebSocketBehavior
{
    public delegate void receiveMessage(String str);
    public static event receiveMessage MessageReceiver;
    protected override void OnMessage(MessageEventArgs e)
    {
        Send("Received: " + e.Data);
        Debug.Log("Received: " + e.Data);
        MessageReceiver(e.Data);
    }
}
public class DebugSocketBehavior : WebSocketBehavior
{
    protected override void OnMessage(MessageEventArgs e)
    {
        if (!WSocketServer.strLog.Equals(WSocketServer.strLog_prev))
        {
            Send(WSocketServer.strLog);
            WSocketServer.strLog_prev = WSocketServer.strLog;
        }
    }
}
public class MonitoringSocketBehavior : WebSocketBehavior
{
    protected override void OnMessage(MessageEventArgs e)
    {
        //if (!WSocketServer.strMonitoring.Equals(WSocketServer.strMonitoring_prev))
        //{
        //    Send(WSocketServer.strMonitoring);
        //    WSocketServer.strMonitoring_prev = WSocketServer.strMonitoring;
        //}
        if (!WSocketServer.strRay.Equals(WSocketServer.strRay_prev))
        {
            Send(WSocketServer.strRay);
            WSocketServer.strRay_prev = WSocketServer.strRay;
        }
    }
}