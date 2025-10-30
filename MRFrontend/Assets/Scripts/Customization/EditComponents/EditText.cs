using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class EditText : MonoBehaviour
{
    public GameObject tmp_instance;
    public static Color[] colorArray = {Color.white,Color.red,Color.blue,Color.green,Color.yellow,Color.magenta,Color.cyan };
    public static string[] colorNameArray = { "white", "red", "blue", "green", "yellow", "magenta", "cyan" };
    public static int currentColor = 3;
    Vector3 position = Vector3.zero;

    public static string GetColorName()
    {
        return colorNameArray[currentColor];
    }
    public static string GetColorName(int _number)
    {
        return colorNameArray[_number];
    }
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

        //this.transform.position = position - vectorTowardsHead.normalized * (float)0.6;
    }
    public void ChangeText(string str)
    {
        tmp_instance.GetComponent<TMP_Text>().text = str;
    }

    public void EnlargeSize()
    {
        //tmp_instance.GetComponent<TMP_Text>().fontSize = tmp_instance.GetComponent<TMP_Text>().fontSize+0.05f;
        float tmp_size = this.transform.localScale.x;
        this.transform.localScale = new Vector3(tmp_size+0.1f, tmp_size+0.1f, tmp_size+0.1f);
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
    public Color GetFontColor()
    {
        return tmp_instance.GetComponent<TMP_Text>().color;
    }
    public int GetFontColorNumber()
    {
        return currentColor;
    }
    public static Color GetCurrentFontColor()
    {
        return colorArray[currentColor];
    }
    //public void SetFontColor(Color _color)
    //{
    //    tmp_instance.GetComponent<TMP_Text>().color = _color;
    //}
    public void SetFontColor(int _current_number)
    {
        currentColor = _current_number;
        tmp_instance.GetComponent<TMP_Text>().color = colorArray[currentColor];
    }

    public void NextColor()
    {
        currentColor += 1;
        currentColor %= colorArray.Length;
        tmp_instance.GetComponent<TMP_Text>().color = colorArray[currentColor];
    }

    public void PreviousColor()
    {
        currentColor -= 1;
        if (currentColor == -1)
        {
            currentColor = colorArray.Length-1;
        }
        tmp_instance.GetComponent<TMP_Text>().color = colorArray[currentColor];
    }
}
