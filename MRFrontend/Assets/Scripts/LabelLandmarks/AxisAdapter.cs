using Microsoft.MixedReality.SampleQRCodes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisAdapter : Singleton<QRCodesManager>
{
    public delegate void sendMessage(String str);
    public static event sendMessage MessageSender;

    public static Vector3 anchor0 = Vector3.zero;
    public static Vector3 anchorX = Vector3.zero;
    public static int route = 0;
    public static Vector3 vectorX = new Vector3(1, 0, 0);
    public static Vector3 vectorY = new Vector3(0, 1, 0);
    public static Vector3 vectorZ = new Vector3(0, 0, 1);

    public const int tutorialRoute = 9;

    bool ifUpadated = false;
    bool ifRouteChanged = false;
    bool ifRouteChangedZero = false;
    public MyQRVis qrVisualizer;


    private long start_time = -1;
    private long start_time_2 = -1;
    const int waitInitializeTime = 2;

    public CreateSignBoards signBoardSetter;
    public MonitoringLogs monirotingLogSocketSetter;
    public WSocketServer monirotingLogWebSocketSetter;
    void Start()
    {
        start_time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        start_time_2 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
    }

    void Update()
    {
        if (qrVisualizer.getQRCodesList().Count > 0)
        {
            getAnchorPositions();
        }
    }

    void getAnchorPositions()
    {
        foreach (KeyValuePair<System.Guid, GameObject> kvp in qrVisualizer.getQRCodesList())
        {
            string text = kvp.Value.GetComponent<MyQRCode>().CodeText;
            int text_to_num = int.Parse(text.Substring(text.Length-1,1));
            if (text_to_num == 0 && kvp.Value.GetComponent<SpatialGraphNodeTracker>().pose.position!=Vector3.zero)
            {
                if(ifRouteChanged == true||anchor0 == Vector3.zero||((anchor0- kvp.Value.GetComponent<SpatialGraphNodeTracker>().pose.position).magnitude > 0.2f))
                {
                    anchor0 = kvp.Value.GetComponent<SpatialGraphNodeTracker>().pose.position;
                    MessageSender("Anchor 0 set! " + anchor0.ToString() + "!");
                    ifUpadated = true;
                    if (ifRouteChanged)
                    {
                        ifRouteChangedZero = true;
                    }
                }
            }
            else if(text_to_num<=9 && text_to_num>=1 && kvp.Value.GetComponent<SpatialGraphNodeTracker>().pose.position != Vector3.zero)
            {
                if (text_to_num != route && route != 0)
                {
                    anchorX = kvp.Value.GetComponent<SpatialGraphNodeTracker>().pose.position;
                    anchor0 = Vector3.zero;
                    MyQRVis.setClear();
                    ifRouteChanged = true;
                    MessageSender("Route " + route + " changed to " + text_to_num + "!");
                    route = text_to_num;
                    ifUpadated = true;
                }
                else if (anchorX == Vector3.zero || ((anchorX - kvp.Value.GetComponent<SpatialGraphNodeTracker>().pose.position).magnitude > 0.2f))
                {
                    anchorX = kvp.Value.GetComponent<SpatialGraphNodeTracker>().pose.position;
                    route = text_to_num;
                    MessageSender("Anchor " + route + " set! " + anchorX.ToString() + "!");
                    ifUpadated = true;
                }
            }
        }
        if(ifRouteChanged && !ifRouteChangedZero)
        {
            return;
        }
        if (anchor0 != Vector3.zero && anchorX != Vector3.zero && route!=0&&ifUpadated==true)
        {
            ifUpadated = false;
            vectorX = new Vector3((anchorX - anchor0).x,0, (anchorX - anchor0).z);
            vectorX = vectorX.normalized;
            vectorY = Vector3.up;
            Quaternion rot = Quaternion.FromToRotation(Vector3.right, Vector3.forward);
            vectorZ = (rot * vectorX).normalized;
            MessageSender("Axis initialized: X: " + vectorX.ToString() + "; Y: " + vectorY.ToString() + "; Z: " + vectorZ.ToString() + "!");

            if (CreateSignBoards.signBoardHeight == 1)
            {
                CreateSignBoards.signBoardHeight = 1.55f + anchor0.y;
            }
            if (signBoardSetter != null)
            {
                signBoardSetter.TryCreateSignBoards(ifRouteChangedZero);
            }
            ifRouteChanged = false;
            ifRouteChangedZero = false;
            if (monirotingLogSocketSetter != null)
            {
                monirotingLogSocketSetter.initializeLog();
            }
            if (monirotingLogWebSocketSetter != null)
            {
                monirotingLogWebSocketSetter.initializeLog();
            }
        }
    }

    public static Vector3 calculatePosion(Vector3 posRelatedOrigin)
    {
        return posRelatedOrigin.x * vectorX + posRelatedOrigin.y * vectorY + posRelatedOrigin.z * vectorZ + anchor0;
    }

    

    public static bool calculateIfOnPath(Vector3 posUnity, float _width, float _length, DirectionType _direction, Vector3 headPosition)
    {
        Vector3 posRelative = headPosition - posUnity;
        posRelative = new Vector3(posRelative.x, 0, posRelative.z);
        float head_width = 0;
        float head_length = 0;
        switch (_direction)
        {
            case DirectionType.PosX:
                head_width = Vector3.Dot(Vector3.Project(posRelative, AxisAdapter.vectorZ), AxisAdapter.vectorZ);
                head_length = Vector3.Dot(Vector3.Project(posRelative, AxisAdapter.vectorX), AxisAdapter.vectorX);
                break;
            case DirectionType.NegX:
                head_width = Vector3.Dot(Vector3.Project(posRelative, -AxisAdapter.vectorZ), -AxisAdapter.vectorZ);
                head_length = Vector3.Dot(Vector3.Project(posRelative, -AxisAdapter.vectorX), -AxisAdapter.vectorX);
                break;
            case DirectionType.PosZ:
                head_width = Vector3.Dot(Vector3.Project(posRelative, -AxisAdapter.vectorX), -AxisAdapter.vectorX);
                head_length = Vector3.Dot(Vector3.Project(posRelative, AxisAdapter.vectorZ), AxisAdapter.vectorZ);
                break;
            case DirectionType.NegZ:
                head_width = Vector3.Dot(Vector3.Project(posRelative, AxisAdapter.vectorX), AxisAdapter.vectorX);
                head_length = Vector3.Dot(Vector3.Project(posRelative, -AxisAdapter.vectorZ), -AxisAdapter.vectorZ);
                break;
        }

        if (head_width<=_width && head_width >=0&& head_length <= _length && head_length >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool calculateIfOnPath(ColorHallwayInformation ch_info, Vector3 headPosition)
    {
        
        Vector3 posRelative = headPosition - ch_info.colorHallwayObject.transform.position;
        posRelative = new Vector3(posRelative.x, 0, posRelative.z);
        float head_width = 0;
        float head_length = 0;
        switch (ch_info.direction)
        {
            case DirectionType.PosX:
                head_width = Vector3.Dot(Vector3.Project(posRelative, AxisAdapter.vectorZ), AxisAdapter.vectorZ);
                head_length = Vector3.Dot(Vector3.Project(posRelative, AxisAdapter.vectorX), AxisAdapter.vectorX);
                break;
            case DirectionType.NegX:
                head_width = Vector3.Dot(Vector3.Project(posRelative, -AxisAdapter.vectorZ), -AxisAdapter.vectorZ);
                head_length = Vector3.Dot(Vector3.Project(posRelative, -AxisAdapter.vectorX), -AxisAdapter.vectorX);
                break;
            case DirectionType.PosZ:
                head_width = Vector3.Dot(Vector3.Project(posRelative, -AxisAdapter.vectorX), -AxisAdapter.vectorX);
                head_length = Vector3.Dot(Vector3.Project(posRelative, AxisAdapter.vectorZ), AxisAdapter.vectorZ);
                break;
            case DirectionType.NegZ:
                head_width = Vector3.Dot(Vector3.Project(posRelative, AxisAdapter.vectorX), AxisAdapter.vectorX);
                head_length = Vector3.Dot(Vector3.Project(posRelative, -AxisAdapter.vectorZ), -AxisAdapter.vectorZ);
                break;
        }

        if (head_width <= ch_info.width && head_width >= 0 && head_length <= ch_info.length && head_length >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void ChangeLayerTempInvisible(Transform transform)
    {
        int layer_visible = 0;
        int layer_invisible = 6;
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                ChangeLayerTempInvisible(transform.GetChild(i));
            }
            if(transform.gameObject.layer == layer_visible)
            {
                transform.gameObject.layer = layer_invisible;
            }
            else if(transform.gameObject.layer == layer_invisible)
            {
                transform.gameObject.layer = layer_visible;
            }
        }
        else
        {
            if (transform.gameObject.layer == layer_visible)
            {
                transform.gameObject.layer = layer_invisible;
            }
            else if (transform.gameObject.layer == layer_invisible)
            {
                transform.gameObject.layer = layer_visible;
            }
        }
    }


    public static void ChangeLayer(Transform transform, bool _visible)
    {
        int layer = 0;
        if (!_visible)
        {
            layer = 3;
        }
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                ChangeLayer(transform.GetChild(i), _visible);
            }
            if (transform.gameObject.layer != 6)
            {
                transform.gameObject.layer = layer;
            }
        }
        else
        {
            if (transform.gameObject.layer != 6)
            {
                transform.gameObject.layer = layer;
            }
        }
    }

    public static void ChangeLayer(Transform transform, bool _visible, bool if_down)
    {
        int layer = 0;
        if (!_visible)
        {
            layer = 3;
        }
        if (transform.name == "DownArrow" && layer !=3)
        {
            return;
        }
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                ChangeLayer(transform.GetChild(i), _visible, if_down);
            }
            if (transform.gameObject.layer != 6)
            {
                transform.gameObject.layer = layer;
            }
        }
        else
        {
            if (transform.gameObject.layer != 6)
            {
                transform.gameObject.layer = layer;
            }
        }
    }

    public static bool IfSameDirection(DirectionType dt_a,DirectionType dt_b)
    {
        bool result = false;
        switch (dt_a){
            case DirectionType.PosX:
            case DirectionType.NegX:
                if(dt_b == DirectionType.PosX || dt_b == DirectionType.NegX)
                {
                    result = true;
                }
                break;
            case DirectionType.PosZ:
            case DirectionType.NegZ:
                if (dt_b == DirectionType.PosZ || dt_b == DirectionType.NegZ)
                {
                    result = true;
                }
                break;
        }
        return result;
    }
}
