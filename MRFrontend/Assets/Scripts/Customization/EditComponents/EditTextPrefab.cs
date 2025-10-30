using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class EditTextPrefab : MonoBehaviour
{
    public GameObject tmp_instance;
    void Start()
    {
        
    }

    void Update()
    {
        //ChangeTextFacing();
    }

    void ChangeTextFacing()
    {
        if (!InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 headPosition))
        {
            headPosition = Vector3.zero;
        }
        Vector3 vectorTowardsHead = tmp_instance.transform.position - headPosition;
        Quaternion orientationTowardsHead = Quaternion.LookRotation(vectorTowardsHead, Vector3.up);
        tmp_instance.transform.rotation = orientationTowardsHead;
    }
    public void ChangeText(string str)
    {
        tmp_instance.GetComponent<TMP_Text>().text = str;
    }
}
