using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class EditIcon : MonoBehaviour
{
    public GameObject tmp_instance;
    Vector3 position = Vector3.zero;

    public void SetPosition(Vector3 _position)
    {
        position = _position;
    }
    void Start()
    {

    }


    void Update()
    {
        ChangeTextFacing();
    }

    void ChangeTextFacing()
    {
        if (!InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 headPosition))
        {
            headPosition = Vector3.zero;
        }
        Vector3 vectorTowardsHead = this.transform.position - headPosition;
        Quaternion orientationTowardsHead = Quaternion.LookRotation(vectorTowardsHead, Vector3.up);
        this.transform.rotation = orientationTowardsHead;

        //this.transform.position = position-vectorTowardsHead.normalized * (float)0.6;
    }
    public void ChangeIcon(string _icon,bool _if_abstract=true)
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
        this.transform.localScale = new Vector3(tmp_size + 0.1f, tmp_size + 0.1f, tmp_size + 0.1f);
    }
    public void ReduceSize()
    {
        //tmp_instance.GetComponent<TMP_Text>().fontSize = tmp_instance.GetComponent<TMP_Text>().fontSize - 0.05f;
        float tmp_size = this.transform.localScale.x;
        this.transform.localScale = new Vector3(tmp_size - 0.1f, tmp_size - 0.1f, tmp_size - 0.1f);
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
