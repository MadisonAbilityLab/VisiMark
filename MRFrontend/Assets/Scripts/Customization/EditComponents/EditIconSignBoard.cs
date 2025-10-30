using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class EditIconSignBoard : MonoBehaviour
{
    public GameObject tmp_instance;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //ChangeTextFacing();
    }

    void ChangeTextFacing()
    {
        if (!InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion headRotation))
        {
            headRotation = Quaternion.identity;
        }
        Vector3 headForward = (headRotation * Vector3.forward).normalized;
        float angleWithX = Math.Abs(Vector3.Angle(headForward, AxisAdapter.vectorX));
        float angleWithZ = Math.Abs(Vector3.Angle(headForward, AxisAdapter.vectorZ));
        if (angleWithX < 45)
        {
            //PosX
            this.transform.rotation = Quaternion.LookRotation(AxisAdapter.vectorX, Vector3.up);
            //tmp_instance.transform.localPosition = -AxisAdapter.vectorX * 0.009f;
        }
        else if (angleWithZ < 45)
        {
            //PosZ
            this.transform.rotation = Quaternion.LookRotation(AxisAdapter.vectorZ, Vector3.up);
            //tmp_instance.transform.localPosition = -AxisAdapter.vectorZ * 0.009f;
        }
        else if (angleWithX > 135)
        {
            //NegX
            this.transform.rotation = Quaternion.LookRotation(-AxisAdapter.vectorX, Vector3.up);
            //tmp_instance.transform.localPosition = AxisAdapter.vectorX * 0.009f;

        }
        else
        {
            //NegZ
            this.transform.rotation = Quaternion.LookRotation(-AxisAdapter.vectorZ, Vector3.up);
            //tmp_instance.transform.localPosition = AxisAdapter.vectorZ * 0.009f;
        }
    }
    public void ChangeIcon(string _icon, bool _if_abstract = true)
    {
        if (!_if_abstract)
        {
            tmp_instance.GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/" + _icon);
        }
        else
        {
            tmp_instance.GetComponent<Image>().sprite = Resources.Load<Sprite>("AbstractIcons/" + _icon);
        }
    }


    public void EnlargeSize()
    {
        //tmp_instance.GetComponent<TMP_Text>().fontSize = tmp_instance.GetComponent<TMP_Text>().fontSize+0.05f;
        float tmp_size = this.transform.localScale.x;
        this.transform.localScale = new Vector3(tmp_size + 0.01f, tmp_size + 0.01f, tmp_size + 0.01f);
    }
    public void ReduceSize()
    {
        //tmp_instance.GetComponent<TMP_Text>().fontSize = tmp_instance.GetComponent<TMP_Text>().fontSize - 0.05f;
        float tmp_size = this.transform.localScale.x;
        this.transform.localScale = new Vector3(tmp_size - 0.01f, tmp_size - 0.01f, tmp_size - 0.01f);
    }

    public float GetSize()
    {
        return this.transform.localScale.x;
    }

    public void SetSize(float _size)
    {
        this.transform.localScale = new Vector3(_size, _size, _size);
    }
}
