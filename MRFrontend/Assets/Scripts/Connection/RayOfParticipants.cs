using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class RayOfParticipants : MonoBehaviour
{
    public delegate void sendMessage(String str);
    public static event sendMessage MessageSender;
    void Start()
    {
        
    }

    void Update()
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



        RaycastHit hit;
        if (Physics.Raycast(headPosition, headForward, out hit))
        {
            if (hit.collider.gameObject.GetComponent<PrefabName>() != null)
            {
                MessageSender("Hit object: " + hit.collider.gameObject.GetComponent<PrefabName>().objectName);
                //Debug.Log("Hit object: " + hit.collider.gameObject.GetComponent<PrefabName>().objectName);
            }
            else
            {
                //MessageSender("Hit object: " + hit.collider.gameObject.name);
                //Debug.Log("Hit object: " + hit.collider.gameObject.name);
            }
        }
        else
        {
            //MessageSender("Do not hit object!");
            Debug.Log("Do not hit object!");
        }
    }
}
