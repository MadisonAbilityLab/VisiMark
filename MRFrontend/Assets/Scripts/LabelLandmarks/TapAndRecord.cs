using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.XR;

public class TapAndRecord : MonoBehaviour
{
    public delegate void sendMessage(String str);
    public static event sendMessage MessageSender;

    private float[] _tappingTimer = { 0, 0 };
    public GameObject textPrefab;

    bool ifInitialized = false;

    StreamWriter writer;

    int[] _count = { 0, 0, 0 };
    long now_time = -1;

    void Start()
    {
        now_time =  new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
    }

    public void SetInitialized()
    {
        ifInitialized = true;
        using (writer = new StreamWriter(Application.persistentDataPath + "/" + now_time + "_AxisAdapter", true))
        {
            writer.WriteLine("route " + AxisAdapter.route);
            writer.WriteLine("anchor0 " + AxisAdapter.anchor0.x + " " + AxisAdapter.anchor0.y + " " + AxisAdapter.anchor0.z);
            writer.WriteLine("anchorX " + AxisAdapter.anchorX.x + " " + AxisAdapter.anchorX.y + " " + AxisAdapter.anchorX.z);
            writer.WriteLine("vectorX " + AxisAdapter.vectorX.x + " " + AxisAdapter.vectorX.y + " " + AxisAdapter.vectorX.z);
            writer.WriteLine("vectorY " + AxisAdapter.vectorY.x + " " + AxisAdapter.vectorY.y + " " + AxisAdapter.vectorY.z);
            writer.WriteLine("vectorZ " + AxisAdapter.vectorZ.x + " " + AxisAdapter.vectorZ.y + " " + AxisAdapter.vectorZ.z);
        }
        MessageSender("!!AxisAdapter: Initialized!");

    }
    void ShortTap(Vector3 handPosition, int hand)
    {
        if (hand == 0)
        {
            using (writer = new StreamWriter(Application.persistentDataPath + "/" + now_time + "_LandmarkPositions", true))
            {
                writer.WriteLine(handPosition.x + " " + handPosition.y + " " + handPosition.z);
            }

            MessageSender("!!Landmarks: " + handPosition.ToString() +" "+ _count[0].ToString());//right hand
            _count[0] += 1;
            if (!InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 headPosition))
            {
                headPosition = Vector3.zero;
            }
            Quaternion orientationTowardsHead = Quaternion.LookRotation(handPosition - headPosition, Vector3.up);
            GameObject anchorGameObject = (GameObject)Instantiate(textPrefab, handPosition, orientationTowardsHead);
            anchorGameObject.GetComponent<EditTextPrefab>().ChangeText("Landmark " + _count[0].ToString());
        }
        else
        {
            using (writer = new StreamWriter(Application.persistentDataPath + "/" + now_time + "_SignBoardPositions", true))
            {
                writer.WriteLine(handPosition.x + " " + handPosition.y + " " + handPosition.z);
            }
            MessageSender("!!Signboards: " + handPosition.ToString() + " " + _count[1].ToString());//left hand
            _count[1] += 1;
            if (!InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 headPosition))
            {
                headPosition = Vector3.zero;
            }
            Quaternion orientationTowardsHead = Quaternion.LookRotation(handPosition - headPosition, Vector3.up);
            GameObject anchorGameObject = (GameObject)Instantiate(textPrefab, handPosition, orientationTowardsHead);
            anchorGameObject.GetComponent<EditTextPrefab>().ChangeText("Signboard " + _count[1].ToString());
        }
    }
    void LongTap(Vector3 handPosition, int hand)
    {
        using (writer = new StreamWriter(Application.persistentDataPath + "/" + now_time + "_ColorHallwayPositions", true))
        {
            writer.WriteLine(handPosition.x + " " + handPosition.y + " " + handPosition.z);
        }
        MessageSender("!!ColorHallways: " + handPosition.ToString() + " " + _count[2].ToString());
        _count[2] += 1;
        if (!InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 headPosition))
        {
            headPosition = Vector3.zero;
        }
        Quaternion orientationTowardsHead = Quaternion.LookRotation(handPosition - headPosition, Vector3.up);
        GameObject anchorGameObject = (GameObject)Instantiate(textPrefab, handPosition, orientationTowardsHead);
        anchorGameObject.GetComponent<EditTextPrefab>().ChangeText("ColorHallway " + _count[2].ToString());
    }

    void Update()
    {
        if (!ifInitialized)
        {
            return;
        }
        for (int i = 0; i < 2; i++)
        {
            InputDevice device = InputDevices.GetDeviceAtXRNode((i == 0) ? XRNode.RightHand : XRNode.LeftHand);
            if (device.TryGetFeatureValue(CommonUsages.primaryButton, out bool isTapping))
            {

                if (!isTapping)
                {
                    //Stopped Tapping or wasn't tapping
                    if (0f < _tappingTimer[i] && _tappingTimer[i] < 1f)
                    {
                        //User has been tapping for less than 1 sec. Get hand position and call ShortTap
                        if (device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 handPosition))
                        {
                            ShortTap(handPosition,i);
                        }
                    }
                    _tappingTimer[i] = 0;
                }
                else
                {
                    _tappingTimer[i] += Time.deltaTime;
                    if (_tappingTimer[i] >= 2f)
                    {
                        //User has been air tapping for at least 2sec. Get hand position and call LongTap
                        if (device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 handPosition))
                        {
                            LongTap(handPosition,i);
                        }
                        _tappingTimer[i] = -float.MaxValue; // reset the timer, to avoid retriggering if user is still holding tap
                    }
                }
            }

        }
    }
}
