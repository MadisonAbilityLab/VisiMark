using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.SampleQRCodes;

public enum LandmarkMainCategory
{
    Accessibility,
    Emergency,
    Info,
    Structure,
    Visual,
    None

}

public enum LandmarkType
{
    AED,
    Alcove,
    BioLab,
    Blackboard,
    BlueWall,
    DesignLab,
    BioDanger,
    Boards,
    Chair,
    Clock,
    Clutter,
    ComputerLab,
    Corkboard,
    CurvedWall,
    Danger,
    DeadEnd,
    Display,
    Door,
    DoubleDoor,
    DoubleDoorLeft,
    DoubleDoorRight,
    Dustbin,
    Elevator,
    EyeWasher,
    FireExtinguisher,
    Fountain,
    GlassWindow,
    GreenDoor,
    GreenDoubleDoor,
    GreenWall,
    Intersection,
    Locker,
    Map,
    MetalTubes,
    MixedPaperBin,
    PathWidthChange,
    Pipe,
    Poster,
    Ramp,
    Railing,
    RecycleBin,
    RedCorkboard,
    RestArea,
    Restroom,
    Stairs,
    Stairs2,
    Stuff,
    TVScreen,
    WastePaperContainer,
    WhiteTile,
    WhiteSockets,
    Window,
    WoodenDoubleDoor,
    WoodenCabinet,
    WoodenFloor,
};

[Serializable]
public class LandmarkInformation
{
    public Vector3 position { get; private set; }
    public Vector3 positionOnSignBoard { get; private set; }
    //public Vector3 UnityPosition { get; private set; }
    public Vector3 scale { get; private set; }
    public LandmarkType type { get; private set; }
    public LandmarkMainCategory category { get; private set; }
    public GameObject textObject { get; set; }
    public GameObject iconObject { get; set; }
    public GameObject outlineOrColorObject { get; set; }
    public GameObject textObjectOnSignBoard { get; set; }
    public GameObject iconObjectOnSignBoard { get; set; }
    public GameObject leftWallObject { get; set; }
    public GameObject rightWallObject { get; set; }
    public bool ifShowOnSignBoard { get; set; }
    public int number { get; set; }
    
    public LandmarkInformation(Vector3 _position, Vector3 _scale, LandmarkType _type, LandmarkMainCategory _category)
    {
        position = _position;
        positionOnSignBoard = Vector3.zero;
        scale = _scale;
        type = _type;
        category = _category;
        //UnityPosition = AxisAdapter.calculatePosion(_position);
        textObject = null;
        iconObject = null;
        outlineOrColorObject = null;
        textObjectOnSignBoard = null;
        iconObjectOnSignBoard = null;
        leftWallObject = null;
        rightWallObject = null;
        ifShowOnSignBoard = true;
        number = -1;
    }
    public LandmarkInformation(Vector3 _position,Vector3 _positionOnSignBoard, Vector3 _scale, LandmarkType _type, LandmarkMainCategory _category)
    {
        position = _position;
        positionOnSignBoard = _positionOnSignBoard;
        scale = _scale;
        type = _type;
        category = _category;
        //UnityPosition = AxisAdapter.calculatePosion(_position);
        textObject = null;
        iconObject = null;
        outlineOrColorObject = null;
        textObjectOnSignBoard = null;
        iconObjectOnSignBoard = null;
        leftWallObject = null;
        rightWallObject = null;
        ifShowOnSignBoard = true;
        number = -1;
    }
    public LandmarkInformation(Vector3 _position, Vector3 _scale, LandmarkType _type, LandmarkMainCategory _category, bool _ifshow)
    {
        position = _position;
        positionOnSignBoard = Vector3.zero;
        scale = _scale;
        type = _type;
        category = _category;
        textObject = null;
        iconObject = null;
        outlineOrColorObject = null;
        textObjectOnSignBoard = null;
        iconObjectOnSignBoard = null;
        leftWallObject = null;
        rightWallObject = null;
        ifShowOnSignBoard = _ifshow;
        number = -1;
    }
    public LandmarkInformation(Vector3 _position, Vector3 _scale, LandmarkType _type, LandmarkMainCategory _category, bool _ifshow,int _number)
    {
        position = _position;
        positionOnSignBoard = Vector3.zero;
        scale = _scale;
        type = _type;
        category = _category;
        textObject = null;
        iconObject = null;
        outlineOrColorObject = null;
        textObjectOnSignBoard = null;
        iconObjectOnSignBoard = null;
        leftWallObject = null;
        rightWallObject = null;
        ifShowOnSignBoard = _ifshow;
        number = _number;
    }
    public LandmarkInformation(Vector3 _position, Vector3 _scale, LandmarkType _type, LandmarkMainCategory _category, int _number)
    {
        position = _position;
        positionOnSignBoard = Vector3.zero;
        scale = _scale;
        type = _type;
        category = _category;
        textObject = null;
        iconObject = null;
        outlineOrColorObject = null;
        textObjectOnSignBoard = null;
        iconObjectOnSignBoard = null;
        leftWallObject = null;
        rightWallObject = null;
        ifShowOnSignBoard = true;
        number = _number;
    }
    public LandmarkInformation(Vector3 _position, Vector3 _positionOnSignBoard, Vector3 _scale, LandmarkType _type, LandmarkMainCategory _category, int _number)
    {
        position = _position;
        positionOnSignBoard = _positionOnSignBoard;
        scale = _scale;
        type = _type;
        category = _category;
        textObject = null;
        iconObject = null;
        outlineOrColorObject = null;
        textObjectOnSignBoard = null;
        iconObjectOnSignBoard = null;
        leftWallObject = null;
        rightWallObject = null;
        ifShowOnSignBoard = true;
        number = _number;
    }
}

[Serializable]
public class LandmarkOnSignBoardInformation
{
    public GameObject textObjectOnSignBoard { get; set; }
    public GameObject iconObjectOnSignBoard { get; set; }
    public LandmarkOnSignBoardInformation()
    {
        textObjectOnSignBoard = null;
        iconObjectOnSignBoard = null;
    }
}

public class CreateAugmentations : MonoBehaviour
{
    // To do: block augmentation when not achieved; set for different route

    public delegate void sendMessage(String str);
    public static event sendMessage MessageSender;



    static GameObject textPrefab = null;
    static GameObject iconPrefab = null;

    static GameObject outlinePrefab = null;
    public GameObject wallPrefab = null;
    //public GameObject colorCoatingPrefab;

    public static bool ifIcon;
    //static int OutlineOrColor; // none:0; outline: 1; color coating: 2

    void Start()
    {

    }

    public static void SetPrefab(GameObject _textPrefab, GameObject _iconPrefab,GameObject _outlinePrefab)
    {
        textPrefab = _textPrefab;
        iconPrefab = _iconPrefab;
        outlinePrefab = _outlinePrefab;
    }

    void Update()
    {

    }
    public static int GetTextLinesNumber(string input)
    {
        string result = "";
        int uppercaseCount = 0;

        foreach (char c in input)
        {
            if (char.IsUpper(c))
            {
                uppercaseCount++;

                if (uppercaseCount >= 2)
                {
                    result += "\n" + c;
                    continue;
                }
            }

            result += c;
        }
        return uppercaseCount;
    }

    public static int GetMaxTextLineLength(string input)
    {
        string result = "";
        int uppercaseCount = 0;
        int maxNum = 0;
        int nowNum = 0;
        foreach (char c in input)
        {
            nowNum += 1;
            if (char.IsUpper(c))
            {
                uppercaseCount++;

                if (uppercaseCount >= 2)
                {
                    result += "\n" + c;
                    if (nowNum > maxNum)
                    {
                        maxNum = nowNum;
                    }
                    nowNum = 0;
                    continue;
                }
            }

            result += c;
        }

        if (nowNum > maxNum)
        {
            maxNum = nowNum;
        }
        return maxNum;
    }
    public static string AddSpaceBeforeSecondAndSubsequentUppercase(string input)
    {
        string result = "";
        int uppercaseCount = 0;
        foreach (char c in input)
        {
            if (char.IsUpper(c))
            {
                uppercaseCount++;

                if (uppercaseCount >= 2)
                {
                    result += " " + c;
                    continue;
                }
            }

            result += c;
        }

        return result;
    }

    public static string AddLineBeforeSecondAndSubsequentUppercase(string input)
    {
        string result = "";
        int uppercaseCount = 0;

        foreach (char c in input)
        {
            if (char.IsUpper(c))
            {
                uppercaseCount++;

                if (uppercaseCount >= 2)
                {
                    result += "\n" + c;
                    continue;
                }
            }

            result += c;
        }

        return result;
    }

    public void TryCreateAugmentations(ColorHallwayInformation ColorHallwayInfo,bool _visible)
    {
        List<LandmarkInformation> LandmarkInfos = ColorHallwayInfo.arrowLandmarkInfos;
        if (textPrefab == null)
        {
            MessageSender("No Text or Icon Prefab!");
            return;
        }
        for (int i =0;i< LandmarkInfos.Count; i++)
        {
            if (LandmarkInfos[i].type == LandmarkType.DeadEnd)
            {
                continue;
            }
            Vector3 tmp_pos = AxisAdapter.calculatePosion(LandmarkInfos[i].position);
            String tmp_type = LandmarkInfos[i].type.ToString();
            float icon_text_dis = (iconPrefab.GetComponent<EditIcon>().GetSize() / 2 + textPrefab.GetComponent<EditText>().GetSize() / 20) / CustomizationSet.icon_text_dis_ratio*GetTextLinesNumber(tmp_type);
            Vector3 tmp_pos_text = tmp_pos - new Vector3(0, iconPrefab.GetComponent<EditIcon>().GetSize() / 2+ textPrefab.GetComponent<EditText>().GetSize()/20 + icon_text_dis, 0);
            Vector3 tmp_pos_icon = tmp_pos;

            if (LandmarkInfos[i].textObject == null)
            {
                LandmarkInfos[i].textObject = (GameObject)Instantiate(textPrefab, tmp_pos_text, Quaternion.identity);
                LandmarkInfos[i].textObject.GetComponent<EditText>().SetPosition(tmp_pos_text);
                String tmp_type_str = tmp_type;
                if (LandmarkInfos[i].number != -1)
                {
                    tmp_type_str += " " + LandmarkInfos[i].number.ToString();
                }
                LandmarkInfos[i].textObject.GetComponent<EditText>().ChangeText(AddSpaceBeforeSecondAndSubsequentUppercase(tmp_type_str));
                LandmarkInfos[i].textObject.GetComponent<PrefabName>().objectName = "text " + tmp_type;
            }
            else
            {
                LandmarkInfos[i].textObject.transform.position = tmp_pos_text;
                LandmarkInfos[i].textObject.GetComponent<EditText>().SetPosition(tmp_pos_text);
            }

            if (LandmarkInfos[i].iconObject == null)
            {
                LandmarkInfos[i].iconObject = (GameObject)Instantiate(iconPrefab, tmp_pos_icon, Quaternion.identity);
                LandmarkInfos[i].iconObject.GetComponent<PrefabName>().objectName = "icon " + tmp_type;
                LandmarkInfos[i].iconObject.GetComponent<EditIcon>().SetPosition(tmp_pos_icon);
                if (LandmarkInfos[i].category != LandmarkMainCategory.None)
                {
                    LandmarkInfos[i].iconObject.GetComponent<EditIcon>().ChangeIcon(LandmarkInfos[i].category.ToString());
                }
                else
                {
                    LandmarkInfos[i].iconObject.GetComponent<EditIcon>().ChangeIcon(tmp_type, false);
                }
                AxisAdapter.ChangeLayer(LandmarkInfos[i].iconObject.transform, ifIcon);
            }
            else
            {
                LandmarkInfos[i].iconObject.transform.position = tmp_pos_icon;
                LandmarkInfos[i].iconObject.GetComponent<EditIcon>().SetPosition(tmp_pos_icon);
            }


            if (LandmarkInfos[i].outlineOrColorObject == null)
            {
                LandmarkInfos[i].outlineOrColorObject = (GameObject)Instantiate(outlinePrefab, tmp_pos_icon, Quaternion.identity);
                LandmarkInfos[i].outlineOrColorObject.transform.localScale = LandmarkInfos[i].scale;
                if (ColorHallwayInfo.direction == DirectionType.PosX || ColorHallwayInfo.direction == DirectionType.NegX)
                {
                    LandmarkInfos[i].outlineOrColorObject.transform.rotation = Quaternion.LookRotation(AxisAdapter.vectorZ, Vector3.up);
                }
                else
                {
                    LandmarkInfos[i].outlineOrColorObject.transform.rotation = Quaternion.LookRotation(AxisAdapter.vectorX, Vector3.up);
                }
                AxisAdapter.ChangeLayer(LandmarkInfos[i].outlineOrColorObject.transform, !ifIcon);
            }
            else
            {
                LandmarkInfos[i].outlineOrColorObject.transform.position = tmp_pos_icon;
            }

            if (LandmarkInfos[i].textObject != null)
            {
                LandmarkInfos[i].textObject.SetActive(_visible);
            }

            if (LandmarkInfos[i].iconObject != null)
            {
                LandmarkInfos[i].iconObject.SetActive(_visible);
            }

            if (LandmarkInfos[i].outlineOrColorObject != null)
            {
                LandmarkInfos[i].outlineOrColorObject.SetActive(_visible);
            }

        }
        //MessageSender("CreateAugmentations!!! AugmentationsCount: " + LandmarkInfos.Count.ToString());
    }

}
