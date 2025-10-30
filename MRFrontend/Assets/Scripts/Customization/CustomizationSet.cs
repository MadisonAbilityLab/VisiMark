using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public static class Enums
{
    public static T Next<T>(this T v) where T : struct
    {
        return Enum.GetValues(v.GetType()).Cast<T>().Concat(new[] { default(T) }).SkipWhile(e => !v.Equals(e)).Skip(1).First();
    }

    public static T Previous<T>(this T v) where T : struct
    {
        return Enum.GetValues(v.GetType()).Cast<T>().Concat(new[] { default(T) }).Reverse().SkipWhile(e => !v.Equals(e)).Skip(1).First();
    }
}
public enum CUS_STAGE
{
    FONT_SIZE,
    FONT_COLOR,
    ICON_SIZE,
    OUTLINE_COLOR,
    OUTLINE_WIDTH,
    SIGNBOARD_SIZE,
    SIGNBOARD_ICON_SIZE,
    SIGNBOARD_TEXT_COLOR,
    IF_ICON,
    SHOW_AUG_ONE_BY_ONE
}

[Serializable]
public class CustomSet{
    public float font_size { get; set; }
    //public Color font_color { get; set; }
    public int font_color_number { get; set; }
    public float font_color_r { get; set; }
    public float font_color_g { get; set; }
    public float font_color_b { get; set; }
    public float font_color_a { get; set; }
    public float outline_color_r { get; set; }
    public float outline_color_g { get; set; }
    public float outline_color_b { get; set; }
    public float outline_color_a { get; set; }
    public float icon_size { get; set; }
    public float outline_width { get; set; }

    public float signboard_icon_size { get; set; }
    public float signboard_font_size { get; set; }
    public float signboard_size { get; set; }

    public int signboard_text_color_number{ get; set; }
    public bool if_icon { get; set; }

    public static float getDefaultSignBoardSize()
    {
        return 0.13f;
    }
    public CustomSet()
    {
        reSet();
    }

    public void reSet()
    {
        font_size = 1.3f;
        icon_size = 0.6f;
        font_color_number = 3;
        setOutlineColor(Color.red);
        outline_width = 0.015f;
        signboard_font_size = 0.32f;
        signboard_icon_size = 0.08f;
        signboard_size = 1.3f;
        //signboard_text_color_number = 0;
        if_icon = true;
    }

    public string getString()
    {
        return "font_size: "+font_size.ToString()+"; "+ "icon_size: " + icon_size.ToString() + "; "
            + "signboard_size: " + signboard_size.ToString() + "; "
            +"signboard_font_size: " + signboard_font_size.ToString() + "; "
            + "signboard_icon_size: " + signboard_icon_size.ToString() + "; "
            + "font_color: " + EditText.GetColorName(font_color_number) + "; "
            //+ "signboard_font_color: " + getSignBoardTextColorNumber().ToString() + "; "
            + "outline_color: " + getOutlineColor().ToString() + "; "
            + "outline_width: " + outline_width.ToString() + "; "
            + "if_icon: " + if_icon.ToString()
            ;
    }
    //public void setAugTextColor(Color c)
    //{
    //    font_color_r = c.r;
    //    font_color_g = c.g;
    //    font_color_b = c.b;
    //    font_color_a = c.a;

    //}
    public Color getAugTextColor()
    {
        return new Color(font_color_r, font_color_g, font_color_b, font_color_a);
    }
    public void setOutlineColor(Color c)
    {
        outline_color_r = c.r;
        outline_color_g = c.g;
        outline_color_b = c.b;
        outline_color_a = c.a;

    }
    public Color getOutlineColor()
    {
        return new Color(outline_color_r, outline_color_g, outline_color_b, outline_color_a);
    }
    //public void setSignBoardTextColorNumber(int _c)
    //{
    //    signboard_text_color_number = _c;

    //}
    //public int getSignBoardTextColorNumber()
    //{
    //    return signboard_text_color_number;
    //}
}

public class CustomizationSet : MonoBehaviour
{
    public delegate void sendMessage(String str);
    public static event sendMessage MessageSender;

    public GameObject textPrefab;
    public GameObject iconPrefab;
    public GameObject outlinePrefab;
    public GameObject textSignBoardPrefab;
    public GameObject iconSignBoardPrefab;
    public GameObject signBoardPrefab;

    static CUS_STAGE stage = CUS_STAGE.FONT_SIZE;

    GameObject textObjectFireE;
    GameObject iconObjectFireE;
    GameObject outlineObjectFireE;
    GameObject textObjectDustbin;
    GameObject iconObjectDustbin;
    GameObject outlineObjectDustbin;
    //GameObject colorHallwayObject;
    GameObject signBoardObject;
    SignBoardInfomation infoSignBoard;
    List<SignBoardInfomation> SignBoardInfos = null;
    List<ColorHallwayInformation> ColorHallwayInfos = null;

    Vector3 last_headPosition = Vector3.zero;
    Vector3 last_headForward = Vector3.zero;
    Vector3 last_headLeft = Vector3.zero;

    List<string> commands = new List<string>();
    CustomSet nowSet = new CustomSet();

    private long start_time = -1;
    const int waitInitializeTime = 1;

    public float icon_text_dis = 0.08f;
    public float outline_text_dis = 0.08f;
    public static float icon_text_dis_ratio = 12f;
    const float left_right_dis = 0.55f;
    const float up_dis_text_icon = 0.2f;
    const float up_dis_signBoard = -0.1f;
    const float forward_dis = 2.5f;
    const float forward_dis_signBoard = 1.0f;
    public const float signBoard_size_increment = 0.1f;

    int currentAugNum = 0;
    GameObject[] augObjectText = { null, null, null, null, null };
    GameObject[] augObjectIcon = { null, null, null, null, null };
    const int totalAugNum = 5;
    string[] augTexts = {"Blue Wall","AED","Restroom","Elevator","Window" };
    string[] augIcons = {"Visual", "Emergency", "Info", "Accessibility","Structure" };


    void Start()
    {
        textPrefab.GetComponent<EditText>().SetSize(nowSet.font_size);
        textPrefab.GetComponent<EditText>().SetFontColor(nowSet.font_color_number);
        iconPrefab.GetComponent<EditIcon>().SetSize(nowSet.icon_size);
        outlinePrefab.GetComponent<EditOutline>().SetColor(nowSet.getOutlineColor());
        outlinePrefab.GetComponent<EditOutline>().SetWidth(nowSet.outline_width);
        textSignBoardPrefab.GetComponent<EditTextSignBoard>().SetSize(nowSet.signboard_font_size);
        iconSignBoardPrefab.GetComponent<EditIconSignBoard>().SetSize(nowSet.signboard_icon_size);
        signBoardPrefab.GetComponent<EditSignBoard>().SetSize(nowSet.signboard_size);
        //EditSignBoard.SetSignBoardTextColorNumber(nowSet.signboard_text_color_number);


        ColorHallwayInfos = HardCodedPositions.getColorHallwayList(0);
        SignBoardInfos = HardCodedPositions.getSignBoardsList(0, ColorHallwayInfos);
        CustomizationSocket.MessageReceiver += GetSetting;
        if (!InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 headPosition))
        {
            headPosition = Vector3.zero;
        }
        if (!InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion headRotation))
        {
            headRotation = Quaternion.identity;
        }
        Vector3 headForward = (headRotation * Vector3.forward).normalized;
        Vector3 headLeft = Vector3.Cross(headForward, Vector3.up);
        last_headPosition = headPosition;
        last_headForward = headForward;
        last_headLeft = headLeft;

        icon_text_dis = (iconPrefab.GetComponent<EditIcon>().GetSize() / 2 + textPrefab.GetComponent<EditText>().GetSize() / 20) / icon_text_dis_ratio;
        float tmp_pos_text_down = iconPrefab.GetComponent<EditIcon>().GetSize() / 2 + textPrefab.GetComponent<EditText>().GetSize()/ 20 + icon_text_dis;
        textObjectFireE = (GameObject)Instantiate(textPrefab, headPosition + headForward * forward_dis + headLeft * left_right_dis + Vector3.up * (up_dis_text_icon - tmp_pos_text_down), Quaternion.identity);
        textObjectFireE.GetComponent<EditText>().ChangeText("Green Wall");
        iconObjectFireE = (GameObject)Instantiate(iconPrefab, headPosition + headForward * forward_dis + headLeft * left_right_dis + Vector3.up * up_dis_text_icon, Quaternion.identity);
        iconObjectFireE.GetComponent<EditIcon>().ChangeIcon("Visual");
        textObjectDustbin = (GameObject)Instantiate(textPrefab, headPosition + headForward * forward_dis - headLeft * left_right_dis + Vector3.up * (up_dis_text_icon - tmp_pos_text_down), Quaternion.identity);
        textObjectDustbin.GetComponent<EditText>().ChangeText("Danger");
        iconObjectDustbin = (GameObject)Instantiate(iconPrefab, headPosition + headForward * forward_dis - headLeft * left_right_dis + Vector3.up * up_dis_text_icon, Quaternion.identity);
        iconObjectDustbin.GetComponent<EditIcon>().ChangeIcon("Emergency");

        outlineObjectFireE = (GameObject)Instantiate(outlinePrefab, headPosition + headForward * forward_dis + headLeft * left_right_dis + Vector3.up * up_dis_text_icon, Quaternion.identity);
        outlineObjectDustbin = (GameObject)Instantiate(outlinePrefab, headPosition + headForward * forward_dis - headLeft * left_right_dis + Vector3.up * up_dis_text_icon, Quaternion.identity);
        AxisAdapter.ChangeLayer(outlineObjectFireE.transform, false);
        AxisAdapter.ChangeLayer(outlineObjectDustbin.transform, false);

        infoSignBoard = SignBoardInfos[0];
        signBoardObject = (GameObject)Instantiate(signBoardPrefab, headPosition + headForward* forward_dis_signBoard + Vector3.up * up_dis_signBoard, Quaternion.identity);
        infoSignBoard.signBoardObject = signBoardObject;
        SetInfoSignboard();
        AxisAdapter.ChangeLayer(signBoardObject.transform, false);

        start_time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();


        for (int i = 0; i < totalAugNum; i++)
        {
            augObjectText[i] = (GameObject)Instantiate(textPrefab, headPosition + headForward * forward_dis + Vector3.up * (up_dis_text_icon - tmp_pos_text_down), Quaternion.identity);
            augObjectIcon[i] = (GameObject)Instantiate(iconPrefab, headPosition + headForward * forward_dis + Vector3.up * up_dis_text_icon, Quaternion.identity);
            augObjectText[i].GetComponent<EditText>().ChangeText(augTexts[i]);
            augObjectIcon[i].GetComponent<EditIcon>().ChangeIcon(augIcons[i]);
            AxisAdapter.ChangeLayer(augObjectText[i].transform, false);
            AxisAdapter.ChangeLayer(augObjectIcon[i].transform, false);
        }
    }

    void SetInfoSignboard()
    {
        GameObject RightArrow = signBoardObject.transform.Find("RightArrow").gameObject;
        GameObject LeftArrow = signBoardObject.transform.Find("LeftArrow").gameObject;
        GameObject UpArrow = signBoardObject.transform.Find("UpArrow").gameObject;
        GameObject DownArrow = signBoardObject.transform.Find("DownArrow").gameObject;
        infoSignBoard.position = last_headPosition + last_headForward + Vector3.up * 0.25f;

        if (infoSignBoard.posXArrow != -1)
        {
            CreateSignBoards.CreateLandmarksOnArrow(ColorHallwayInfos[infoSignBoard.posXArrow], infoSignBoard, DirectionType.PosX, textSignBoardPrefab, iconSignBoardPrefab);
            CreateSignBoards.setArrowAndLandmarks(UpArrow, ColorHallwayInfos[infoSignBoard.posXArrow], infoSignBoard, DirectionType.PosX, textPrefab, iconPrefab, signBoardPrefab);
        }
        else
        {
            UpArrow.GetComponent<SetArrowSizeAndMaterial>().SetMaterial("transparent", true);
        }
        if (infoSignBoard.posZArrow != -1)
        {
            CreateSignBoards.CreateLandmarksOnArrow(ColorHallwayInfos[infoSignBoard.posZArrow], infoSignBoard, DirectionType.PosZ, textSignBoardPrefab, iconSignBoardPrefab);
            CreateSignBoards.setArrowAndLandmarks(LeftArrow, ColorHallwayInfos[infoSignBoard.posZArrow], infoSignBoard, DirectionType.PosZ, textPrefab, iconPrefab, signBoardPrefab);
        }
        else
        {
            LeftArrow.GetComponent<SetArrowSizeAndMaterial>().SetMaterial("transparent", true);
        }
        if (infoSignBoard.negZArrow != -1)
        {
            CreateSignBoards.CreateLandmarksOnArrow(ColorHallwayInfos[infoSignBoard.negZArrow], infoSignBoard, DirectionType.NegZ, textSignBoardPrefab, iconSignBoardPrefab);
            CreateSignBoards.setArrowAndLandmarks(RightArrow, ColorHallwayInfos[infoSignBoard.negZArrow], infoSignBoard, DirectionType.NegZ, textPrefab, iconPrefab, signBoardPrefab);
        }
        else
        {
            RightArrow.GetComponent<SetArrowSizeAndMaterial>().SetMaterial("transparent", true);
        }
        if (infoSignBoard.negXArrow != -1)
        {
            CreateSignBoards.CreateLandmarksOnArrow(ColorHallwayInfos[infoSignBoard.negXArrow], infoSignBoard, DirectionType.NegX, textSignBoardPrefab, iconSignBoardPrefab);
            CreateSignBoards.setArrowAndLandmarks(DownArrow, ColorHallwayInfos[infoSignBoard.negXArrow], infoSignBoard, DirectionType.NegX, textPrefab, iconPrefab, signBoardPrefab);
        }
        else
        {
            DownArrow.GetComponent<SetArrowSizeAndMaterial>().SetMaterial("transparent", true);
        }
        
    }
    void GetSetting(String str)
    {
        Debug.Log(str);
        commands.Add(str);
    }

    void ResetPosition()
    {
        if (!InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 headPosition))
        {
            headPosition = Vector3.zero;
        }
        if (!InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion headRotation))
        {
            headRotation = Quaternion.identity;
        }
        Vector3 headForward = (headRotation * Vector3.forward).normalized;
        Vector3 headLeft = Vector3.Cross(headForward, Vector3.up);

        icon_text_dis = (iconPrefab.GetComponent<EditIcon>().GetSize() / 2 + textPrefab.GetComponent<EditText>().GetSize() / 20) / icon_text_dis_ratio;
        float tmp_pos_text_down = iconPrefab.GetComponent<EditIcon>().GetSize() / 2 + textPrefab.GetComponent<EditText>().GetSize() / 20 + icon_text_dis;
        
        last_headPosition = headPosition;
        last_headForward = headForward;
        last_headLeft = headLeft;
        textObjectFireE.transform.position = last_headPosition + last_headForward * forward_dis + last_headLeft * left_right_dis + Vector3.up * (up_dis_text_icon - tmp_pos_text_down);
        iconObjectFireE.transform.position = last_headPosition + last_headForward * forward_dis + last_headLeft * left_right_dis + Vector3.up * up_dis_text_icon;
        outlineObjectFireE.transform.position = last_headPosition + last_headForward * forward_dis + last_headLeft * left_right_dis + Vector3.up * up_dis_text_icon;
        textObjectDustbin.transform.position = last_headPosition + last_headForward * forward_dis - last_headLeft * left_right_dis + Vector3.up * (up_dis_text_icon - tmp_pos_text_down);
        iconObjectDustbin.transform.position = last_headPosition + last_headForward * forward_dis - last_headLeft * left_right_dis + Vector3.up * up_dis_text_icon;
        outlineObjectDustbin.transform.position = last_headPosition + last_headForward * forward_dis - last_headLeft * left_right_dis + Vector3.up * up_dis_text_icon;
        signBoardObject.transform.position = last_headPosition + last_headForward * forward_dis_signBoard + Vector3.up * up_dis_signBoard;
        signBoardObject.transform.rotation = Quaternion.LookRotation(headForward, Vector3.up);
        AxisAdapter.vectorX = headForward;
        AxisAdapter.vectorZ = headLeft;
        for(int i = 0; i < totalAugNum; i++)
        {
            augObjectText[i].transform.position = last_headPosition + last_headForward * forward_dis + Vector3.up * (up_dis_text_icon - tmp_pos_text_down);
            augObjectIcon[i].transform.position = last_headPosition + last_headForward * forward_dis + Vector3.up * up_dis_text_icon;
        }
    }

    void FinishSet(bool if_save=true)
    {
        //Debug.Log("finished!");
        //MessageSender("finished!");

        if (if_save)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/customize_set");
            bf.Serialize(file, nowSet);
            file.Close();
            MessageSender("Customization Saved!");
        }
        else
        {
            MessageSender("Customization not Saved!");
        }
        MessageSender("Finish customization!");
        SceneManager.LoadScene("MyMainScene");
    }
    void Update()
    {
        if (start_time != -1)
        {
            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() - start_time >= waitInitializeTime)
            {
                MessageSender("NowStage: " + stage.ToString());
                start_time = -1;
            }
        }
    }
}
