using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TutorialSocket : MonoBehaviour
{
    public delegate void receiveMessage(String str);
    public static event receiveMessage MessageReceiver;

    String strLog = "Start Tutorial!";
    String strLog_prev = "";

    static Socket socket_client;

    private int port = 8011;

    void Start()
    {
        ConnectServer();
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
            socket_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket_client.Connect(pEndPoint);
            Thread c_thread = new Thread(Receive);
            c_thread.IsBackground = true;
            c_thread.Start();
            Thread c_thread_send = new Thread(Send);
            c_thread_send.IsBackground = true;
            c_thread_send.Start();
        }
        catch (System.Exception)
        {
            Debug.Log("Wrong IP: "+port);
        }
    }


    public void Receive()
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
            catch (System.Exception)
            {
                throw;
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
                if (tmp_str.Length > 0)
                {
                    if (tmp_str.Equals(strLog_prev) == false)
                    {
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(tmp_str);
                        socket_client.Send(buffer);
                        strLog_prev = tmp_str;
                    }
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
    void Update()
    {
        
    }
}
