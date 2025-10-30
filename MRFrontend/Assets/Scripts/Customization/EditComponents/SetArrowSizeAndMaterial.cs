using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetArrowSizeAndMaterial : MonoBehaviour
{
    public delegate void sendMessage(String str);
    public static event sendMessage MessageSender;

    const int WRatio = 40;
    const int LRatio = 80;

    public ArrowType _arrowType;

    Dictionary<string, Color> colorDict = new Dictionary<string, Color>()
    {
        {"red",new Color(255/255f,0,0,80/255f) },
        {"yellow",new Color(255/255f,180/255f,0,80/255f) },
        {"green",new Color(0,255/255f,0,80/255f) },
        {"blue",new Color(102/255f,0,255/255f,80/255f) },
        {"transparent",new Color(80/255f,0,255/255f,0f) },
    };

    void Start()
    {

        //SetWidthAndLength((float)0.5, (float)0.3);
        //SetMaterial(GameObject.Find("Resources").GetComponent<GetResources>().getMaterial("yellow"));
    }

    public void SetWidthAndLength(float width, float length)
    {
        width = width / WRatio;
        length = length / LRatio;
        GameObject cube = gameObject.transform.Find("Cube").gameObject;
        GameObject cone = gameObject.transform.Find("Cone").gameObject;
        GameObject deadendObject = gameObject.transform.Find("DeadEnd").gameObject;
        cube.transform.localScale = new Vector3(length, width, (float)0.005);
        cube.transform.localPosition = new Vector3((float)(length / 2), 0, 0);
        cone.transform.localScale = new Vector3((float)(1.6* width), (float)(0.7 * width), (float)0.01);
        cone.transform.localPosition = new Vector3((float)(length+ 0.7 * width / 2), 0, 0);
        deadendObject.transform.localScale = new Vector3(0.015f, width*1.5f, (float)0.005);
        deadendObject.transform.localPosition = new Vector3((float)(length + 0.7 * width+0.0075f), 0, 0);
    }

    public void SetDeadEndMaterial(String color, bool if_transparent = false)
    {
        GameObject deadendObject = gameObject.transform.Find("DeadEnd").gameObject;

        if (if_transparent == false)
        {
            if (color == "transparent")
            {
                Color tmp_color = colorDict[color];
                deadendObject.GetComponent<MeshRenderer>().material.color = new Color(tmp_color.r, tmp_color.g, tmp_color.b, 0);
            }
            else
            {
                Color tmp_color = colorDict[color];
                deadendObject.GetComponent<MeshRenderer>().material.color = new Color(tmp_color.r, tmp_color.g, tmp_color.b, 1);
            }
        }
        else
        {
            Color tmp_color = colorDict[color];
            deadendObject.GetComponent<MeshRenderer>().material.color = tmp_color;
        }
    }
    public void SetMaterial(String color, bool if_transparent = false)
    {
        GameObject cube = gameObject.transform.Find("Cube").gameObject;
        GameObject cone = gameObject.transform.Find("Cone").gameObject;

        if (if_transparent == false)
        {
            if(color == "transparent")
            {
                Color tmp_color = colorDict[color];
                cube.GetComponent<MeshRenderer>().material.color = new Color(tmp_color.r, tmp_color.g, tmp_color.b, 0);
                cone.GetComponent<MeshRenderer>().material.color = new Color(tmp_color.r, tmp_color.g, tmp_color.b, 0);
            }
            else
            {
                Color tmp_color = colorDict[color];
                cube.GetComponent<MeshRenderer>().material.color = new Color(tmp_color.r, tmp_color.g, tmp_color.b, 1);
                cone.GetComponent<MeshRenderer>().material.color = new Color(tmp_color.r, tmp_color.g, tmp_color.b, 1);
            }
        }
        else
        {
            Color tmp_color = colorDict[color];
            cube.GetComponent<MeshRenderer>().material.color = tmp_color;
            cone.GetComponent<MeshRenderer>().material.color = tmp_color;
        }
    }
    public static Vector2 GetLandmarkRelativeToArrowPosition(DirectionType road_on_signboad_direction, Vector3 signBoardPos, Vector3 LandmarkPos,float path_width,float path_length,ArrowType arrow_type, float icon_size, float text_size, float signboard_size, int text_max_len,int line_num)
    {
        LandmarkPos = AxisAdapter.calculatePosion(LandmarkPos);
        signBoardPos = AxisAdapter.calculatePosion(signBoardPos);

        Vector3 posRelative = LandmarkPos - signBoardPos;
        posRelative = new Vector3(posRelative.x, 0, posRelative.z);
        float width = 0;
        float length = 0;
        //upArrow for example 2dxy coordinate (+x up, +y left)
        switch (road_on_signboad_direction)
        {
            case DirectionType.PosX:
                width = Vector3.Dot(Vector3.Project(posRelative, AxisAdapter.vectorZ), AxisAdapter.vectorZ);
                length = Vector3.Dot(Vector3.Project(posRelative, AxisAdapter.vectorX), AxisAdapter.vectorX);
                break;
            case DirectionType.NegX:
                width = Vector3.Dot(Vector3.Project(posRelative, -AxisAdapter.vectorZ), -AxisAdapter.vectorZ);
                length = Vector3.Dot(Vector3.Project(posRelative, -AxisAdapter.vectorX), -AxisAdapter.vectorX);
                break;
            case DirectionType.PosZ:
                width = Vector3.Dot(Vector3.Project(posRelative, -AxisAdapter.vectorX), -AxisAdapter.vectorX);
                length = Vector3.Dot(Vector3.Project(posRelative, AxisAdapter.vectorZ), AxisAdapter.vectorZ);
                break;
            case DirectionType.NegZ:
                width = Vector3.Dot(Vector3.Project(posRelative, AxisAdapter.vectorX), AxisAdapter.vectorX);
                length = Vector3.Dot(Vector3.Project(posRelative, -AxisAdapter.vectorZ), -AxisAdapter.vectorZ);
                break;
        }

        //Debug.Log(road_on_signboad_direction.ToString()+" w:"+width + "; l:" + length);

        float offset = 0;
        switch (arrow_type)
        {
            case ArrowType.UpArrow:
            case ArrowType.DownArrow:
                if (text_max_len > 3)
                {
                    offset = text_size / 25 * (text_max_len-1.5f);
                }
                else
                {

                    offset = text_size / 25 * 2;
                }
                break;
            case ArrowType.LeftArrow:
                if (width > 0)
                {
                    offset = icon_size / 2 + text_size / 25 - 0.01f;
                }
                else
                {
                    offset = icon_size / 2 + text_size / 20 * (line_num * 2f)+ 0.002f*(signboard_size- CustomSet.getDefaultSignBoardSize() + 1);
                }
                break;
            case ArrowType.RightArrow:
                if (width > 0)
                {
                    offset = icon_size / 2 + text_size / 20 * (line_num* 2f) + 0.002f * (signboard_size - CustomSet.getDefaultSignBoardSize() + 1);
                }
                else
                {
                    offset = icon_size / 2 + text_size / 25 - 0.01f;
                }
                break;
        }
        return new Vector2(Math.Sign(width)* (path_width / WRatio+ offset), length / LRatio);
    }
    void Update()
    {
        
    }
}
