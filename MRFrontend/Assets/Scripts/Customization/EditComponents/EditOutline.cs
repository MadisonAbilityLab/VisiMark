using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class EditOutline : MonoBehaviour
{
    Color[] colorArray = { Color.white, Color.red, Color.blue, Color.green, Color.yellow, Color.magenta, Color.cyan };
    int currentColor = 1;

    void Start()
    {

    }


    void Update()
    {
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

    }

    public Color GetColor()
    {
        return this.GetComponent<MeshRenderer>().sharedMaterial.color;
    }
    public void SetColor(Color _color)
    {
        this.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(_color.r, _color.g, _color.b, 1);
    }

    public void NextColor()
    {
        currentColor += 1;
        currentColor %= colorArray.Length;
        this.GetComponent<MeshRenderer>().sharedMaterial.color = colorArray[currentColor];
    }

    public void PreviousColor()
    {
        currentColor -= 1;
        if (currentColor == -1)
        {
            currentColor = colorArray.Length - 1;
        }
        this.GetComponent<MeshRenderer>().sharedMaterial.color = colorArray[currentColor];
    }

    public float GetWidth()
    {
        return this.GetComponent<MeshRenderer>().sharedMaterial.GetFloat("_Width");
    }
    public void SetWidth(float _width)
    {
        this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Width", _width);
    }

    public void IncreaseWidth()
    {
        float _width = this.GetComponent<MeshRenderer>().sharedMaterial.GetFloat("_Width");
        this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Width", _width+0.003f);
    }

    public void DecreaseWidth()
    {
        float _width = this.GetComponent<MeshRenderer>().sharedMaterial.GetFloat("_Width");
        this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Width", _width  - 0.003f);
    }
}
