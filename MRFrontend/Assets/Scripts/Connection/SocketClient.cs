using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using System.Net;
using System.Net.Sockets;
using System;
using System.Text;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using Microsoft.MixedReality.SampleQRCodes;

public class SocketClient : MonoBehaviour
{
    Socket socket_client;
    Socket socket_server;

    public delegate void receiveMessage(String str);
    public static event receiveMessage MessageReceiver;

    //public static string ip_address = "10.140.198.88";
    public int port;
    String strLog = "Start Debug!";
    String strLog_prev = "";
    void Start()
    {
        //AzureAnchorsSetAndRetrieve.MessageSender += GetLog;
        TapAndRecord.MessageSender += GetLog;
        CreateAugmentations.MessageSender += GetLog;
        CreateColorHallways.MessageSender += GetLog;
        CreateSignBoards.MessageSender += GetLog;
        MyQRVis.MessageSender += GetLog;
        MyQRCode.MessageSender += GetLog;
        AxisAdapter.MessageSender += GetLog;
        SetArrowSizeAndMaterial.MessageSender += GetLog;
        LoadCustomization.MessageSender += GetLog;
        Tutorial.MessageSender += GetLog;
        RayOfParticipants.MessageSender += GetLog;
        ConnectServer();
    }

    void Update()
    {
        //Debug.Log(strLog + ' ' + strLog_prev);
    }

    void GetLog(String str)
    {
        strLog = str;
        //Debug.Log(strLog);
    }

    public void ConnectServer()
    {
        try
        {
            IPAddress pAddress = IPAddress.Parse(HttpNet.IPAddress);
            IPEndPoint pEndPoint = new IPEndPoint(pAddress, port);
            //client
            //socket_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //socket_client.Connect(pEndPoint);

            //server
            socket_server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket_server.Bind(pEndPoint);
            socket_server.Listen(20);
            socket_client = socket_server.Accept();

            Debug.Log("Connected");
            Thread c_thread = new Thread(Send);
            c_thread.IsBackground = true;
            c_thread.Start();
            Thread c_thread_receive = new Thread(Received);
            c_thread_receive.IsBackground = true;
            c_thread_receive.Start();
        }
        catch (System.Exception)
        {
            Debug.Log("Wrong IP: " + port);
        }
    }

    public void Received()
    {
        while (true)
        {
            try
            {
                byte[] buffer = new byte[32768];
                int len = socket_client.Receive(buffer, 32768,0);
                if (len == 0) break;
                string str = Encoding.UTF8.GetString(buffer, 0, len);
                MessageReceiver(str);
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
}
