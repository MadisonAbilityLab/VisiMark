using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.XR;

public class MonitoringLogs : MonoBehaviour
{
    long start_time;
    long last_update_time;
    const float UpdateTime = (float)0.5;
    bool ifInitialized = false;

    static Socket socket_client;

    private int port = 8010;
    String strLog = "Start Monitoring!";
    String strLog_prev = "";

    void SendLog(String str)
    {
        strLog = str;
        //Debug.Log(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + " " + str);
    }

    public void Send()
    {
        while (true)
        {
            try
            {
                String tmp_str = strLog;
                if (tmp_str.Length > 0 && tmp_str.Length < 32768)
                {
                    if (tmp_str.Equals(strLog_prev) == false)
                    {
                        byte[] buffer = new byte[32768];
                        buffer = System.Text.Encoding.UTF8.GetBytes(tmp_str, 0, tmp_str.Length);
                        socket_client.Send(buffer);
                        strLog_prev = tmp_str;
                    }
                }
            }
            catch (System.Exception e)
            {
                byte[] buffer = new byte[32768];
                buffer = System.Text.Encoding.UTF8.GetBytes(e.Message, 0, e.Message.Length);
                socket_client.Send(buffer);
                Debug.Log(e.Message);
                //throw;
            }
        }
    }
    public void initializeLog()
    {
        SendLog("(route:" + AxisAdapter.route+ ";)"
            + "(anchor0:x" + AxisAdapter.anchor0.x + ";y" + AxisAdapter.anchor0.y + ";z" + AxisAdapter.anchor0.z + ";)"
            + "(vectorX:x" + AxisAdapter.vectorX.x + ";y" + AxisAdapter.vectorX.y + ";z" + AxisAdapter.vectorX.z + ";)"
            +"(vectorY:x" + AxisAdapter.vectorY.x + ";y" + AxisAdapter.vectorY.y + ";z" + AxisAdapter.vectorY.z + ";)"
            +"(vectorZ:x" + AxisAdapter.vectorZ.x + ";y" + AxisAdapter.vectorZ.y + ";z" + AxisAdapter.vectorZ.z + ";)");
        ifInitialized = true;
    }

    public void ConnectServer()
    {
        try
        {
            IPAddress pAddress = IPAddress.Parse(HttpNet.IPAddress);
            IPEndPoint pEndPoint = new IPEndPoint(pAddress, port);
            socket_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket_client.Connect(pEndPoint);
            //byte[] buffer = System.Text.Encoding.UTF8.GetBytes("Monitor Start!");
            //socket_client.Send(buffer);
            Thread c_thread = new Thread(Send);
            c_thread.IsBackground = true;
            c_thread.Start();
        }
        catch (System.Exception)
        {
            Debug.Log("Wrong IP");
        }
    }
    void Start()
    {
        start_time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        last_update_time = start_time;
        ConnectServer();
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
            SendLog("(headPosition:x" + headPosition.x + ";y" + headPosition.y + ";z" + headPosition.z + ";)" +
                "(headForward:x" + headForward.x + ";y" + headForward.y + ";z" + headForward.z + ";)"
                + "(anchor0:x" + AxisAdapter.anchor0.x + ";y" + AxisAdapter.anchor0.y + ";z" + AxisAdapter.anchor0.z + ";)"
            + "(vectorX:x" + AxisAdapter.vectorX.x + ";y" + AxisAdapter.vectorX.y + ";z" + AxisAdapter.vectorX.z + ";)"
            + "(vectorY:x" + AxisAdapter.vectorY.x + ";y" + AxisAdapter.vectorY.y + ";z" + AxisAdapter.vectorY.z + ";)"
            + "(vectorZ:x" + AxisAdapter.vectorZ.x + ";y" + AxisAdapter.vectorZ.y + ";z" + AxisAdapter.vectorZ.z + ";)"
             + "(route:" + AxisAdapter.route + ";)" + "(time:" + tmp_time.ToString() + ";)");
            //recordData(Application.persistentDataPath + "/head_movement", tmp_time.ToString()+" " +s);
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
