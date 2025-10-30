using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DirectionType
{
    PosX,
    NegX,
    PosZ,
    NegZ
};


[Serializable]
public class ColorHallwayInformation
{
    public Vector3 position { get; private set; }
    public float width { get; private set; }
    public float length { get; private set; }
    public float true_length { get; private set; }
    public string color { get; private set; }
    public DirectionType direction { get; private set; }
    public Material colorMaterial { get; set; }

    public GameObject colorHallwayObject { get; set; }
    public GameObject wallObjectLeft { get; set; }
    public GameObject wallObjectRight { get; set; }
    public List<LandmarkInformation> arrowLandmarkInfos { get; set; }
    public List<int> relatedColorHallway { get; set; }
    public ColorHallwayInformation(Vector3 _position, float _width, float _length, string _color, DirectionType _direction, List<int> _relatedColorHallway)
    {
        position = _position;
        width = _width;
        length = _length;
        true_length = -1;
        color = _color;
        direction = _direction;
        colorMaterial = Resources.Load<Material>("Materials/" + _color);
        Color tmp_color = colorMaterial.color;
        colorMaterial.color = new Color(tmp_color.r, tmp_color.g, tmp_color.b, (float)80/255);
        arrowLandmarkInfos = new List<LandmarkInformation>();
        colorHallwayObject = null;
        wallObjectLeft = null;
        wallObjectRight = null;
        relatedColorHallway = _relatedColorHallway;
    }
    public ColorHallwayInformation(Vector3 _position, float _width, float _length, float _true_length, string _color, DirectionType _direction, List<int> _relatedColorHallway)
    {
        position = _position;
        width = _width;
        length = _length;
        true_length = _true_length;
        color = _color;
        direction = _direction;
        colorMaterial = Resources.Load<Material>("Materials/" + _color);
        Color tmp_color = colorMaterial.color;
        colorMaterial.color = new Color(tmp_color.r, tmp_color.g, tmp_color.b, (float)80 / 255);
        arrowLandmarkInfos = new List<LandmarkInformation>();
        colorHallwayObject = null;
        wallObjectLeft = null;
        wallObjectRight = null;
        relatedColorHallway = _relatedColorHallway;
    }
}

public class CreateColorHallways : MonoBehaviour
{
    public delegate void sendMessage(String str);
    public static event sendMessage MessageSender;


    public GameObject colorHallwayPrefab;
    public GameObject wallPrefab;

    void Start()
    {
    }

    void Update()
    {

    }
    public static void setColorHallwayAndAugIfVisible(ColorHallwayInformation ColorHallwayInfo, bool _visible)
    {
        AxisAdapter.ChangeLayer(ColorHallwayInfo.colorHallwayObject.transform, _visible);
        AxisAdapter.ChangeLayer(ColorHallwayInfo.wallObjectLeft.transform, _visible);
        AxisAdapter.ChangeLayer(ColorHallwayInfo.wallObjectRight.transform, _visible);
        for (int i = 0; i < ColorHallwayInfo.arrowLandmarkInfos.Count; i++)
        {
            if (ColorHallwayInfo.arrowLandmarkInfos[i].iconObject != null && CreateAugmentations.ifIcon)
            {
                AxisAdapter.ChangeLayer(ColorHallwayInfo.arrowLandmarkInfos[i].iconObject.transform, _visible);
            }
            if (ColorHallwayInfo.arrowLandmarkInfos[i].textObject != null)
            {
                AxisAdapter.ChangeLayer(ColorHallwayInfo.arrowLandmarkInfos[i].textObject.transform, _visible);
            }
            if (ColorHallwayInfo.arrowLandmarkInfos[i].outlineOrColorObject != null && !CreateAugmentations.ifIcon)
            {
                AxisAdapter.ChangeLayer(ColorHallwayInfo.arrowLandmarkInfos[i].outlineOrColorObject.transform, _visible);
            }
        }
    }
    public static void setColorHallwayAndAugIfVisibleWithoutWall(ColorHallwayInformation ColorHallwayInfo, bool _visible)
    {
        AxisAdapter.ChangeLayer(ColorHallwayInfo.colorHallwayObject.transform, _visible);
        AxisAdapter.ChangeLayer(ColorHallwayInfo.wallObjectLeft.transform, false);
        AxisAdapter.ChangeLayer(ColorHallwayInfo.wallObjectRight.transform, false);
        for (int i = 0; i < ColorHallwayInfo.arrowLandmarkInfos.Count; i++)
        {
            if (ColorHallwayInfo.arrowLandmarkInfos[i].iconObject != null && CreateAugmentations.ifIcon)
            {
                AxisAdapter.ChangeLayer(ColorHallwayInfo.arrowLandmarkInfos[i].iconObject.transform, _visible);
            }
            if (ColorHallwayInfo.arrowLandmarkInfos[i].textObject != null)
            {
                AxisAdapter.ChangeLayer(ColorHallwayInfo.arrowLandmarkInfos[i].textObject.transform, _visible);
            }
            if (ColorHallwayInfo.arrowLandmarkInfos[i].outlineOrColorObject != null && !CreateAugmentations.ifIcon)
            {
                AxisAdapter.ChangeLayer(ColorHallwayInfo.arrowLandmarkInfos[i].outlineOrColorObject.transform, _visible);
            }
        }
    }


    public void TryCreateColorHallways(ColorHallwayInformation ColorHallwayInfo, bool _visible,int _num)
    {
        Vector3 tmp_pos = AxisAdapter.calculatePosion(ColorHallwayInfo.position);
        const float length_offset = 0.05f;
        const float width_offset = 0.05f;

        if (ColorHallwayInfo.colorHallwayObject == null)
        {
            ColorHallwayInfo.colorHallwayObject = (GameObject)Instantiate(colorHallwayPrefab, tmp_pos, Quaternion.identity);
            ColorHallwayInfo.colorHallwayObject.transform.Find("ColorHallway").gameObject.GetComponent<PrefabName>().objectName = "colorHallway " + _num.ToString();
            GameObject colorHallwayObject = ColorHallwayInfo.colorHallwayObject;
            colorHallwayObject.transform.localScale = new Vector3(ColorHallwayInfo.length, (float)1, ColorHallwayInfo.width);

            switch (ColorHallwayInfo.direction)
            {
                case DirectionType.PosX:
                    Quaternion orientationTowards = Quaternion.LookRotation(AxisAdapter.vectorZ, Vector3.up);
                    colorHallwayObject.transform.rotation = orientationTowards;
                    ColorHallwayInfo.wallObjectLeft = (GameObject)Instantiate(wallPrefab, AxisAdapter.calculatePosion(ColorHallwayInfo.position + new Vector3(length_offset, 0, -width_offset)), Quaternion.identity);
                    ColorHallwayInfo.wallObjectLeft.transform.localScale = new Vector3(ColorHallwayInfo.length - length_offset*2, 10, 1);
                    ColorHallwayInfo.wallObjectLeft.transform.rotation = orientationTowards;
                    ColorHallwayInfo.wallObjectRight = (GameObject)Instantiate(wallPrefab, AxisAdapter.calculatePosion(ColorHallwayInfo.position + new Vector3(length_offset, 0, ColorHallwayInfo.width + width_offset)), Quaternion.identity);
                    ColorHallwayInfo.wallObjectRight.transform.localScale = new Vector3(ColorHallwayInfo.length - length_offset * 2, 10, 1);
                    ColorHallwayInfo.wallObjectRight.transform.rotation = orientationTowards;
                    break;
                case DirectionType.NegX:
                    orientationTowards = Quaternion.LookRotation(-AxisAdapter.vectorZ, Vector3.up);
                    colorHallwayObject.transform.rotation = orientationTowards; 
                     ColorHallwayInfo.wallObjectLeft = (GameObject)Instantiate(wallPrefab, AxisAdapter.calculatePosion(ColorHallwayInfo.position + new Vector3(-length_offset, 0, width_offset)), Quaternion.identity);
                    ColorHallwayInfo.wallObjectLeft.transform.localScale = new Vector3(ColorHallwayInfo.length - length_offset * 2, 10, 1);
                    ColorHallwayInfo.wallObjectLeft.transform.rotation = orientationTowards; 
                    ColorHallwayInfo.wallObjectRight = (GameObject)Instantiate(wallPrefab, AxisAdapter.calculatePosion(ColorHallwayInfo.position + new Vector3(-length_offset, 0, -ColorHallwayInfo.width - width_offset)), Quaternion.identity);
                    ColorHallwayInfo.wallObjectRight.transform.localScale = new Vector3(ColorHallwayInfo.length - length_offset * 2, 10, 1);
                    ColorHallwayInfo.wallObjectRight.transform.rotation = orientationTowards;
                    break;
                case DirectionType.PosZ:
                    orientationTowards = Quaternion.LookRotation(-AxisAdapter.vectorX, Vector3.up);
                    colorHallwayObject.transform.rotation = orientationTowards; 
                    ColorHallwayInfo.wallObjectLeft = (GameObject)Instantiate(wallPrefab, AxisAdapter.calculatePosion(ColorHallwayInfo.position + new Vector3(width_offset, 0, length_offset)), Quaternion.identity);
                    ColorHallwayInfo.wallObjectLeft.transform.localScale = new Vector3(ColorHallwayInfo.length - length_offset * 2, 10, 1);
                    ColorHallwayInfo.wallObjectLeft.transform.localScale = new Vector3(ColorHallwayInfo.length - length_offset * 2, 10, 1);
                    ColorHallwayInfo.wallObjectLeft.transform.rotation = orientationTowards; 
                    ColorHallwayInfo.wallObjectRight = (GameObject)Instantiate(wallPrefab, AxisAdapter.calculatePosion(ColorHallwayInfo.position + new Vector3(-(ColorHallwayInfo.width + width_offset), 0, length_offset)), Quaternion.identity);
                    ColorHallwayInfo.wallObjectRight.transform.localScale = new Vector3(ColorHallwayInfo.length - length_offset * 2, 10, 1);
                    ColorHallwayInfo.wallObjectRight.transform.rotation = orientationTowards;
                    break;
                case DirectionType.NegZ:
                    orientationTowards = Quaternion.LookRotation(AxisAdapter.vectorX, Vector3.up);
                    colorHallwayObject.transform.rotation = orientationTowards; 
                    ColorHallwayInfo.wallObjectLeft = (GameObject)Instantiate(wallPrefab, AxisAdapter.calculatePosion(ColorHallwayInfo.position + new Vector3(-width_offset, 0, -length_offset)), Quaternion.identity);
                    ColorHallwayInfo.wallObjectLeft.transform.localScale = new Vector3(ColorHallwayInfo.length - length_offset * 2, 10, 1);
                    ColorHallwayInfo.wallObjectLeft.transform.rotation = orientationTowards; 
                    ColorHallwayInfo.wallObjectRight = (GameObject)Instantiate(wallPrefab, AxisAdapter.calculatePosion(ColorHallwayInfo.position + new Vector3(ColorHallwayInfo.width + width_offset, 0, -length_offset)), Quaternion.identity);
                    ColorHallwayInfo.wallObjectRight.transform.localScale = new Vector3(ColorHallwayInfo.length - length_offset * 2, 10, 1);
                    ColorHallwayInfo.wallObjectRight.transform.rotation = orientationTowards;
                    break;
            }
            ColorHallwayInfo.colorHallwayObject.transform.Find("ColorHallway").gameObject.GetComponent<MeshRenderer>().material = ColorHallwayInfo.colorMaterial;
            ColorHallwayInfo.wallObjectLeft.transform.Find("ColorHallway").gameObject.GetComponent<PrefabName>().objectName = "ColorHallwayLeftWall " + _num.ToString();
            ColorHallwayInfo.wallObjectRight.transform.Find("ColorHallway").gameObject.GetComponent<PrefabName>().objectName = "ColorHallwayRightWall " + _num.ToString();
        }
        else
        {
            ColorHallwayInfo.colorHallwayObject.transform.position = tmp_pos;
            switch (ColorHallwayInfo.direction)
            {
                case DirectionType.PosX:
                    Quaternion orientationTowards = Quaternion.LookRotation(AxisAdapter.vectorZ, Vector3.up);
                    ColorHallwayInfo.colorHallwayObject.transform.rotation = orientationTowards;
                    ColorHallwayInfo.wallObjectLeft.transform.rotation = orientationTowards;
                    ColorHallwayInfo.wallObjectRight.transform.rotation = orientationTowards;
                    ColorHallwayInfo.wallObjectLeft.transform.position = AxisAdapter.calculatePosion(ColorHallwayInfo.position + new Vector3(length_offset, 0, -width_offset));
                    ColorHallwayInfo.wallObjectRight.transform.position = AxisAdapter.calculatePosion(ColorHallwayInfo.position + new Vector3(length_offset, 0, ColorHallwayInfo.width + width_offset));
                    break;
                case DirectionType.NegX:
                    orientationTowards = Quaternion.LookRotation(-AxisAdapter.vectorZ, Vector3.up);
                    ColorHallwayInfo.colorHallwayObject.transform.rotation = orientationTowards;
                    ColorHallwayInfo.wallObjectLeft.transform.rotation = orientationTowards;
                    ColorHallwayInfo.wallObjectRight.transform.rotation = orientationTowards;
                    ColorHallwayInfo.wallObjectLeft.transform.position = AxisAdapter.calculatePosion(ColorHallwayInfo.position + new Vector3(-length_offset, 0, width_offset));
                    ColorHallwayInfo.wallObjectRight.transform.position = AxisAdapter.calculatePosion(ColorHallwayInfo.position + new Vector3(-length_offset, 0, -ColorHallwayInfo.width - width_offset));
                    break;
                case DirectionType.PosZ:
                    orientationTowards = Quaternion.LookRotation(-AxisAdapter.vectorX, Vector3.up);
                    ColorHallwayInfo.colorHallwayObject.transform.rotation = orientationTowards;
                    ColorHallwayInfo.wallObjectLeft.transform.rotation = orientationTowards;
                    ColorHallwayInfo.wallObjectRight.transform.rotation = orientationTowards;
                    ColorHallwayInfo.wallObjectLeft.transform.position = AxisAdapter.calculatePosion(ColorHallwayInfo.position + new Vector3(width_offset, 0, length_offset));
                    ColorHallwayInfo.wallObjectRight.transform.position = AxisAdapter.calculatePosion(ColorHallwayInfo.position + new Vector3(-(ColorHallwayInfo.width + width_offset), 0, length_offset));
                    break;
                case DirectionType.NegZ:
                    orientationTowards = Quaternion.LookRotation(AxisAdapter.vectorX, Vector3.up);
                    ColorHallwayInfo.colorHallwayObject.transform.rotation = orientationTowards;
                    ColorHallwayInfo.wallObjectLeft.transform.rotation = orientationTowards;
                    ColorHallwayInfo.wallObjectRight.transform.rotation = orientationTowards;
                    ColorHallwayInfo.wallObjectLeft.transform.position = AxisAdapter.calculatePosion(ColorHallwayInfo.position + new Vector3(-width_offset, 0, -length_offset));
                    ColorHallwayInfo.wallObjectRight.transform.position = AxisAdapter.calculatePosion(ColorHallwayInfo.position + new Vector3(ColorHallwayInfo.width + width_offset, 0, -length_offset));
                    break;
            }
        }
        ColorHallwayInfo.colorHallwayObject.transform.Find("ColorHallway").gameObject.SetActive(_visible);
        ColorHallwayInfo.wallObjectLeft.transform.Find("ColorHallway").gameObject.SetActive(_visible);
        ColorHallwayInfo.wallObjectRight.transform.Find("ColorHallway").gameObject.SetActive(_visible);
    }
}
