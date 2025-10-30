using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.MixedReality.SampleQRCodes;

public class HttpNet : MonoBehaviour
{
    public static string IPAddress = "10.140.210.69";
    private HttpListener listener;

    public delegate void receiveMessage(String str);
    public static event receiveMessage MessageReceiver;
    private void Start()
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
        RayOfParticipants.MessageSender += GetLog;
        StartServer();
    }

    String strLog = "Start Debug!";

    void GetLog(String str)
    {
        strLog = str;
        //recordData(Application.persistentDataPath + "/debug_log", new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()+" " + strLog);
    }

    private void StartServer()
    {
        listener = new HttpListener();
        listener.Prefixes.Add("http://"+ HttpNet.IPAddress+":8012/");
        listener.Start();
        Debug.Log("Server started on port 8012...");

        ThreadPool.QueueUserWorkItem((state) =>
        {
            while (listener.IsListening)
            {
                try
                {
                    var context = listener.GetContext();
                    ProcessRequest(context);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error: {ex.Message}");
                }
            }
        });
    }

    private void recordData(string PathOut, String s)
    {
        StreamWriter sw = File.AppendText(PathOut);
        sw.WriteLine(s);
        sw.Flush();
        sw.Close();
    }
    private void ProcessRequest(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;

        try
        {
            if (request.HttpMethod == "GET")
            {
                byte[] buffer = Encoding.UTF8.GetBytes(strLog);
                response.ContentType = "text/plain";
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            //else if (request.HttpMethod == "POST")
            //{
            //    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            //    {
            //        string postData = reader.ReadToEnd();
            //        var data = JsonUtility.FromJson<JsonData>(postData);
            //        string message = data.message;
            //        if (!string.IsNullOrEmpty(message))
            //        {
            //            string responseMessage = "Message received: " + message;
            //            MessageReceiver(message);
            //            byte[] buffer = Encoding.UTF8.GetBytes(responseMessage);
            //            response.ContentType = "text/plain";
            //            response.ContentLength64 = buffer.Length;
            //            response.OutputStream.Write(buffer, 0, buffer.Length);
            //        }
            //        else
            //        {
            //            response.StatusCode = (int)HttpStatusCode.BadRequest;
            //            byte[] buffer = Encoding.UTF8.GetBytes("Bad request: Message missing");
            //            response.ContentType = "text/plain";
            //            response.ContentLength64 = buffer.Length;
            //            response.OutputStream.Write(buffer, 0, buffer.Length);
            //        }
            //    }
            //}
            //else
            //{
            //    response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
            //}
        }
        catch (Exception e)
        {
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            byte[] buffer = Encoding.UTF8.GetBytes("Internal server error: " + e.Message);
            response.ContentType = "text/plain";
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
        }
        finally
        {
            response.OutputStream.Close();
        }
    }

    private void OnDestroy()
    {
        if (listener != null && listener.IsListening)
        {
            listener.Stop();
            Debug.Log("Server stopped.");
        }
    }

    [Serializable]
    public class JsonData
    {
        public string message;
    }
}