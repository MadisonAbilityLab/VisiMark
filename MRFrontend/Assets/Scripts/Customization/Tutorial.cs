using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.XR;

public enum TUTORIAL_STAGE
{
    FONT_SIZE,
    FONT_COLOR,
    ICON_SIZE,
    SIGNBOARD_SIZE,
    SIGNBOARD_ICON_SIZE,
    SIGNBOARD_TEXT_COLOR,
    SHOW_AUG_ONE_BY_ONE,
    NumberOfTypes
}
public class Tutorial : MonoBehaviour
{
    public delegate void sendMessage(String str);
    public static event sendMessage MessageSender;

    public static GameObject textPrefab;
    public static GameObject iconPrefab;
    public static GameObject outlinePrefab;
    public static GameObject textSignBoardPrefab;
    public static GameObject iconSignBoardPrefab;
    public static GameObject signBoardPrefab;

    List<string> commands = new List<string>();
    TUTORIAL_STAGE stage = TUTORIAL_STAGE.NumberOfTypes;
    CustomSet nowSet = new CustomSet();
    bool ifShowAugOneByOne = false;

    bool ifAugOn = true;
    bool ifCHOn = true;
    bool ifIconsOn = true;
    bool ifSignBoardOn = true;

    int currentAugNum = 0;
    static GameObject[] augObjectText = { null, null, null, null, null };
    static GameObject[] augObjectIcon = { null, null, null, null, null };
    const int totalAugNum = 5;
    static string[] augTexts = { "Blue Wall", "AED", "Restroom", "Elevator", "Window" };
    static string[] augIcons = { "Visual", "Emergency", "Info", "Accessibility", "Structure" };
    void GetSetting(String str)
    {
        if (AxisAdapter.route == AxisAdapter.tutorialRoute)
        {
            int result;
            bool success = int.TryParse(str, out result);
            if (success)
            {
                if(result>=1&&result<= (int)TUTORIAL_STAGE.NumberOfTypes+1)
                {
                    stage = (TUTORIAL_STAGE)(result-1);
                    Debug.Log("Now Stage: " + stage.ToString());
                    commands.Add("change");
                    MessageSender("Now Stage: " + stage.ToString());
                }
            }
            //a: open/close aug
            //c: open/close color hallway
            //s: openclose signboard
            //i: icon, o: outline
            else if (str == "a" || str == "c" || str == "i" || str == "s" || str == "left" || str == "right"||str=="space" || str == "ctrl")//|| str == "i"|| str == "o")
            {
                commands.Add(str);
            }
        }
    }
    void Start()
    {
        SocketClient.MessageReceiver += GetSetting;
        HttpNet.MessageReceiver += GetSetting;
        ControlSocketBehavior.MessageReceiver += GetSetting;
    }

    public void reSetCustomization(bool if_tutorial)
    {
        if (if_tutorial)
        {
            nowSet.reSet();
        }
    }
    public static void setPrefabs(GameObject _textPrefab, GameObject _iconPrefab, GameObject _outlinePrefab, GameObject _textSignBoardPrefab, GameObject _iconSignBoardPrefab, GameObject _signBoardPrefab)
    {
        textPrefab = _textPrefab;
        iconPrefab = _iconPrefab;
        outlinePrefab = _outlinePrefab;
        textSignBoardPrefab = _textSignBoardPrefab;
        iconSignBoardPrefab = _iconSignBoardPrefab;
        signBoardPrefab = _signBoardPrefab;
        if (!InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 headPosition))
        {
            headPosition = Vector3.zero;
        }
        if (!InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion headRotation))
        {
            headRotation = Quaternion.identity;
        }
        Vector3 headForward = (headRotation * Vector3.forward).normalized;
        for (int i = 0; i < totalAugNum; i++)
        {
            float icon_text_dis = (iconPrefab.GetComponent<EditIcon>().GetSize() / 2 + textPrefab.GetComponent<EditText>().GetSize() / 20) / CustomizationSet.icon_text_dis_ratio * CreateAugmentations.GetTextLinesNumber(augTexts[i]);
            augObjectText[i] = (GameObject)Instantiate(textPrefab, headPosition + headForward * 2.5f +
                Vector3.up * (0.2f - (iconPrefab.GetComponent<EditIcon>().GetSize() / 2 + textPrefab.GetComponent<EditText>().GetSize() / 20 + icon_text_dis)), Quaternion.identity);
            augObjectIcon[i] = (GameObject)Instantiate(iconPrefab, headPosition + headForward * 2.5f + Vector3.up * 0.2f, Quaternion.identity);
            augObjectText[i].GetComponent<EditText>().ChangeText(augTexts[i]);
            augObjectIcon[i].GetComponent<EditIcon>().ChangeIcon(augIcons[i]);
            AxisAdapter.ChangeLayer(augObjectText[i].transform, false);
            AxisAdapter.ChangeLayer(augObjectIcon[i].transform, false);
        }
    }


    void SaveSet(bool if_save = true)
    {
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
    }

    void Update()
    {
        if (AxisAdapter.route == AxisAdapter.tutorialRoute)
        {
            if (commands.Count > 0)
            {
                string command = commands[0];
                commands.RemoveAt(0);
                switch (command)
                {
                    case "change":
                        Debug.Log("change");
                        bool tmp_change = false;
                        if (stage == TUTORIAL_STAGE.SHOW_AUG_ONE_BY_ONE)
                        {
                            if (!ifShowAugOneByOne)
                            {
                                ifShowAugOneByOne = true;
                                tmp_change = true;
                            }
                        }
                        else
                        {
                            if (ifShowAugOneByOne)
                            {
                                ifShowAugOneByOne = false;
                                tmp_change = true;
                            }
                        }
                        Debug.Log(ifShowAugOneByOne + " " + tmp_change);
                        if (tmp_change)
                        {
                            Debug.Log("change!");
                            AxisAdapter.ChangeLayer(augObjectText[currentAugNum].transform, ifShowAugOneByOne);
                            AxisAdapter.ChangeLayer(augObjectIcon[currentAugNum].transform, ifShowAugOneByOne);
                            for (int i = 0; i < CreateSignBoards.ColorHallwayInfos.Count; i++)
                            {
                                for (int j = 0; j < CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos.Count; j++)
                                {
                                    if (CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].iconObject != null)
                                    {
                                        CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].iconObject.SetActive(!ifShowAugOneByOne);
                                    }
                                    if (CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].textObject != null)
                                    {
                                        CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].textObject.SetActive(!ifShowAugOneByOne);
                                    }
                                    if (CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].outlineOrColorObject != null)
                                    {
                                        CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].outlineOrColorObject.SetActive(!ifShowAugOneByOne);
                                    }
                                }
                            }
                            for (int i = 0; i < CreateSignBoards.ColorHallwayInfos.Count; i++)
                            {
                                if (CreateSignBoards.ColorHallwayInfos[i].colorHallwayObject != null)
                                {
                                    CreateSignBoards.ColorHallwayInfos[i].colorHallwayObject.SetActive(!ifShowAugOneByOne);
                                }
                                if (CreateSignBoards.ColorHallwayInfos[i].wallObjectLeft != null)
                                {
                                    CreateSignBoards.ColorHallwayInfos[i].wallObjectLeft.SetActive(!ifShowAugOneByOne);
                                }
                                if (CreateSignBoards.ColorHallwayInfos[i].wallObjectRight != null)
                                {
                                    CreateSignBoards.ColorHallwayInfos[i].wallObjectRight.SetActive(!ifShowAugOneByOne);
                                }
                            }
                            for (int i = 0; i < CreateSignBoards.SignBoardInfos.Count; i++)
                            {
                                if (CreateSignBoards.SignBoardInfos[i].signBoardObject != null)
                                {
                                    CreateSignBoards.SignBoardInfos[i].signBoardObject.SetActive(!ifShowAugOneByOne);
                                }
                            }
                            ifAugOn = !ifShowAugOneByOne;
                            ifCHOn = !ifShowAugOneByOne;
                            ifSignBoardOn = !ifShowAugOneByOne;
                            ifIconsOn = !ifShowAugOneByOne;
                        }
                        break;
                    case "a":
                        ifAugOn = !ifAugOn;
                        MessageSender("Set Augmentations: " + ifAugOn.ToString());
                        for (int i = 0; i < CreateSignBoards.ColorHallwayInfos.Count; i++)
                        {
                            for (int j = 0; j < CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos.Count; j++)
                            {
                                if (CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].iconObject != null)
                                {
                                    CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].iconObject.SetActive(ifAugOn);
                                }
                                if (CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].textObject != null)
                                {
                                    CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].textObject.SetActive(ifAugOn);
                                }
                                if (CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].outlineOrColorObject != null)
                                {
                                    CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].outlineOrColorObject.SetActive(ifAugOn);
                                }
                            }
                        }
                        break;
                    case "c":
                        ifCHOn = !ifCHOn;
                        MessageSender("Set ColorHallways: " + ifCHOn.ToString());
                        for (int i = 0; i < CreateSignBoards.ColorHallwayInfos.Count; i++)
                        {
                            if (CreateSignBoards.ColorHallwayInfos[i].colorHallwayObject != null)
                            {
                                CreateSignBoards.ColorHallwayInfos[i].colorHallwayObject.SetActive(ifCHOn);
                            }
                            if (CreateSignBoards.ColorHallwayInfos[i].wallObjectLeft != null)
                            {
                                CreateSignBoards.ColorHallwayInfos[i].wallObjectLeft.SetActive(ifCHOn);
                            }
                            if (CreateSignBoards.ColorHallwayInfos[i].wallObjectRight != null)
                            {
                                CreateSignBoards.ColorHallwayInfos[i].wallObjectRight.SetActive(ifCHOn);
                            }
                        }
                        break;
                    case "s":
                        ifSignBoardOn = !ifSignBoardOn;
                        MessageSender("Set SignBoards: " + ifSignBoardOn.ToString());
                        for (int i = 0; i < CreateSignBoards.SignBoardInfos.Count; i++)
                        {
                            if (CreateSignBoards.SignBoardInfos[i].signBoardObject != null)
                            {
                                CreateSignBoards.SignBoardInfos[i].signBoardObject.SetActive(ifSignBoardOn);
                            }
                        }
                        break;
                    case "i":
                        ifIconsOn = !ifIconsOn;
                        MessageSender("Set Icons on SignBoards: " + ifIconsOn.ToString());
                        for (int i = 0; i < CreateSignBoards.SignBoardInfos.Count; i++)
                        {
                            for (int j = 0; j < CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosX].Count; j++)
                            {
                                if (CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosX][j].textObjectOnSignBoard != null)
                                {
                                    AxisAdapter.ChangeLayerTempInvisible(CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosX][j].textObjectOnSignBoard.transform);
                                }
                                if (CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosX][j].iconObjectOnSignBoard != null)
                                {
                                    AxisAdapter.ChangeLayerTempInvisible(CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosX][j].iconObjectOnSignBoard.transform);
                                }
                            }
                            for (int j = 0; j < CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegX].Count; j++)
                            {
                                if (CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegX][j].textObjectOnSignBoard != null)
                                {
                                    AxisAdapter.ChangeLayerTempInvisible(CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegX][j].textObjectOnSignBoard.transform);
                                }
                                if (CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegX][j].iconObjectOnSignBoard != null)
                                {
                                    AxisAdapter.ChangeLayerTempInvisible(CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegX][j].iconObjectOnSignBoard.transform);
                                }
                            }
                            for (int j = 0; j < CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosZ].Count; j++)
                            {
                                if (CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosZ][j].textObjectOnSignBoard != null)
                                {
                                    AxisAdapter.ChangeLayerTempInvisible(CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosZ][j].textObjectOnSignBoard.transform);
                                }
                                if (CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosZ][j].iconObjectOnSignBoard != null)
                                {
                                    AxisAdapter.ChangeLayerTempInvisible(CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosZ][j].iconObjectOnSignBoard.transform);
                                }
                            }
                            for (int j = 0; j < CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegZ].Count; j++)
                            {
                                if (CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegZ][j].textObjectOnSignBoard != null)
                                {
                                    AxisAdapter.ChangeLayerTempInvisible(CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegZ][j].textObjectOnSignBoard.transform);
                                }
                                if (CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegZ][j].iconObjectOnSignBoard != null)
                                {
                                    AxisAdapter.ChangeLayerTempInvisible(CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegZ][j].iconObjectOnSignBoard.transform);
                                }
                            }
                        }
                        break;
                    case "left":
                    case "right":
                        switch (stage)
                        {
                            case TUTORIAL_STAGE.FONT_SIZE:
                                if (command.Equals("left"))
                                {
                                    textPrefab.GetComponent<EditText>().ReduceSize();
                                }
                                else
                                {
                                    textPrefab.GetComponent<EditText>().EnlargeSize();
                                }
                                nowSet.font_size = textPrefab.GetComponent<EditText>().GetSize();
                                for (int i = 0; i < CreateSignBoards.ColorHallwayInfos.Count; i++)
                                {
                                    for (int j = 0; j < CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos.Count; j++)
                                    {
                                        if (CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].textObject != null)
                                        {
                                            CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].textObject.GetComponent<EditText>().SetSize(nowSet.font_size);
                                            Vector3 tmp_pos = AxisAdapter.calculatePosion(CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].position);
                                            String tmp_type = CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].type.ToString();
                                            float icon_text_dis = (iconPrefab.GetComponent<EditIcon>().GetSize() / 2 + textPrefab.GetComponent<EditText>().GetSize() / 20) / CustomizationSet.icon_text_dis_ratio * CreateAugmentations.GetTextLinesNumber(tmp_type);
                                            Vector3 tmp_pos_text = tmp_pos - new Vector3(0, iconPrefab.GetComponent<EditIcon>().GetSize() / 2 + textPrefab.GetComponent<EditText>().GetSize() / 20 + icon_text_dis, 0);
                                            CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].textObject.GetComponent<EditText>().SetPosition(tmp_pos_text);
                                            CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].textObject.transform.position = tmp_pos_text;
                                        }
                                    }
                                }
                                MessageSender(stage.ToString() + ": " + nowSet.font_size.ToString());
                                break;
                            case TUTORIAL_STAGE.FONT_COLOR:
                                if (command.Equals("left"))
                                {
                                    textPrefab.GetComponent<EditText>().PreviousColor();
                                }
                                else
                                {
                                    textPrefab.GetComponent<EditText>().NextColor();
                                }
                                nowSet.font_color_number = textPrefab.GetComponent<EditText>().GetFontColorNumber();
                                //nowSet.setAugTextColor(textPrefab.GetComponent<EditText>().GetFontColor());
                                for (int i = 0; i < CreateSignBoards.ColorHallwayInfos.Count; i++)
                                {
                                    for (int j = 0; j < CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos.Count; j++)
                                    {
                                        if (CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].textObject != null)
                                        {
                                            CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].textObject.GetComponent<EditText>().SetFontColor(nowSet.font_color_number);
                                        }
                                    }
                                }

                                for (int i = 0; i < CreateSignBoards.SignBoardInfos.Count; i++)
                                {
                                    for (int j = 0; j < CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosX].Count; j++)
                                    {
                                        if (CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosX][j].textObjectOnSignBoard != null)
                                        {
                                            CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosX][j].textObjectOnSignBoard.GetComponent<EditTextSignBoard>().SetCurrentFontColor();
                                        }
                                    }
                                    for (int j = 0; j < CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegX].Count; j++)
                                    {
                                        if (CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegX][j].textObjectOnSignBoard != null)
                                        {
                                            CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegX][j].textObjectOnSignBoard.GetComponent<EditTextSignBoard>().SetCurrentFontColor();
                                        }
                                    }
                                    for (int j = 0; j < CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosZ].Count; j++)
                                    {
                                        if (CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosZ][j].textObjectOnSignBoard != null)
                                        {
                                            CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosZ][j].textObjectOnSignBoard.GetComponent<EditTextSignBoard>().SetCurrentFontColor();
                                        }
                                    }
                                    for (int j = 0; j < CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegZ].Count; j++)
                                    {
                                        if (CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegZ][j].textObjectOnSignBoard != null)
                                        {
                                            CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegZ][j].textObjectOnSignBoard.GetComponent<EditTextSignBoard>().SetCurrentFontColor();
                                        }
                                    }
                                }
                                MessageSender(stage.ToString() + ": " + EditText.GetColorName());
                                break;
                            case TUTORIAL_STAGE.ICON_SIZE:
                                if (!nowSet.if_icon)
                                {
                                    break;
                                }
                                if (command.Equals("left"))
                                {
                                    iconPrefab.GetComponent<EditIcon>().ReduceSize();
                                }
                                else
                                {
                                    iconPrefab.GetComponent<EditIcon>().EnlargeSize();
                                }
                                nowSet.icon_size = iconPrefab.GetComponent<EditIcon>().GetSize();
                                for (int i = 0; i < CreateSignBoards.ColorHallwayInfos.Count; i++)
                                {
                                    for (int j = 0; j < CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos.Count; j++)
                                    {
                                        if (CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].iconObject != null)
                                        {
                                            CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].iconObject.GetComponent<EditIcon>().SetSize(nowSet.icon_size);
                                            Vector3 tmp_pos = AxisAdapter.calculatePosion(CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].position);
                                            String tmp_type = CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].type.ToString();
                                            float icon_text_dis = (iconPrefab.GetComponent<EditIcon>().GetSize() / 2 + textPrefab.GetComponent<EditText>().GetSize() / 20) / CustomizationSet.icon_text_dis_ratio * CreateAugmentations.GetTextLinesNumber(tmp_type);
                                            Vector3 tmp_pos_text = tmp_pos - new Vector3(0, iconPrefab.GetComponent<EditIcon>().GetSize() / 2 + textPrefab.GetComponent<EditText>().GetSize() / 20 + icon_text_dis, 0);
                                            if (CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].textObject != null)
                                            {
                                                CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].textObject.GetComponent<EditText>().SetPosition(tmp_pos_text);
                                                CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].textObject.transform.position = tmp_pos_text;
                                            }
                                            CreateSignBoards.ColorHallwayInfos[i].arrowLandmarkInfos[j].iconObject.transform.position = tmp_pos;
                                        }
                                    }
                                }
                                MessageSender(stage.ToString() + ": " + nowSet.icon_size.ToString());
                                break;
                            case TUTORIAL_STAGE.SIGNBOARD_SIZE:
                                float tmp_size = signBoardPrefab.transform.localScale.x;
                                if (command.Equals("left"))
                                {
                                    signBoardPrefab.transform.localScale = new Vector3(tmp_size - CustomizationSet.signBoard_size_increment, tmp_size - CustomizationSet.signBoard_size_increment, tmp_size - CustomizationSet.signBoard_size_increment);
                                }
                                else { 
                                    signBoardPrefab.transform.localScale = new Vector3(tmp_size + CustomizationSet.signBoard_size_increment, tmp_size + CustomizationSet.signBoard_size_increment, tmp_size + CustomizationSet.signBoard_size_increment);
                                }
                                nowSet.signboard_size = tmp_size;
                                for (int i = 0; i < CreateSignBoards.SignBoardInfos.Count; i++)
                                {
                                    if (CreateSignBoards.SignBoardInfos[i].signBoardObject != null)
                                    {
                                        CreateSignBoards.SignBoardInfos[i].signBoardObject.transform.localScale = signBoardPrefab.transform.localScale;
                                    }
                                }
                                MessageSender(stage.ToString() + ": " + nowSet.signboard_size.ToString());
                                break;
                            case TUTORIAL_STAGE.SIGNBOARD_ICON_SIZE:
                                if (command.Equals("left"))
                                {
                                    textSignBoardPrefab.GetComponent<EditTextSignBoard>().ReduceSize();
                                    iconSignBoardPrefab.GetComponent<EditIconSignBoard>().ReduceSize();
                                }
                                else
                                {
                                    textSignBoardPrefab.GetComponent<EditTextSignBoard>().EnlargeSize();
                                    iconSignBoardPrefab.GetComponent<EditIconSignBoard>().EnlargeSize();
                                }
                                nowSet.signboard_font_size = textSignBoardPrefab.GetComponent<EditTextSignBoard>().GetSize();
                                nowSet.signboard_icon_size = iconSignBoardPrefab.GetComponent<EditIconSignBoard>().GetSize();
                                for (int i = 0; i < CreateSignBoards.SignBoardInfos.Count; i++)
                                {
                                    for(int j =0;j< CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosX].Count; j++)
                                    {
                                        if (CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosX][j].textObjectOnSignBoard != null)
                                        {
                                            CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosX][j].textObjectOnSignBoard.GetComponent<EditTextSignBoard>().SetSize(nowSet.signboard_font_size/nowSet.signboard_size);
                                        }
                                        if (CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosX][j].iconObjectOnSignBoard != null)
                                        {
                                            CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosX][j].iconObjectOnSignBoard.GetComponent<EditIconSignBoard>().SetSize(nowSet.signboard_icon_size / nowSet.signboard_size);
                                        }
                                    }
                                    for (int j = 0; j < CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegX].Count; j++)
                                    {
                                        if (CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegX][j].textObjectOnSignBoard != null)
                                        {
                                            CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegX][j].textObjectOnSignBoard.GetComponent<EditTextSignBoard>().SetSize(nowSet.signboard_font_size / nowSet.signboard_size);
                                        }
                                        if (CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegX][j].iconObjectOnSignBoard != null)
                                        {
                                            CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegX][j].iconObjectOnSignBoard.GetComponent<EditIconSignBoard>().SetSize(nowSet.signboard_icon_size / nowSet.signboard_size);
                                        }
                                    }
                                    for (int j = 0; j < CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosZ].Count; j++)
                                    {
                                        if (CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosZ][j].textObjectOnSignBoard != null)
                                        {
                                            CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosZ][j].textObjectOnSignBoard.GetComponent<EditTextSignBoard>().SetSize(nowSet.signboard_font_size / nowSet.signboard_size);
                                        }
                                        if (CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosZ][j].iconObjectOnSignBoard != null)
                                        {
                                            CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.PosZ][j].iconObjectOnSignBoard.GetComponent<EditIconSignBoard>().SetSize(nowSet.signboard_icon_size / nowSet.signboard_size);
                                        }
                                    }
                                    for (int j = 0; j < CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegZ].Count; j++)
                                    {
                                        if (CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegZ][j].textObjectOnSignBoard != null)
                                        {
                                            CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegZ][j].textObjectOnSignBoard.GetComponent<EditTextSignBoard>().SetSize(nowSet.signboard_font_size / nowSet.signboard_size);
                                        }
                                        if (CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegZ][j].iconObjectOnSignBoard != null)
                                        {
                                            CreateSignBoards.SignBoardInfos[i].arrowLandmarkInfos[DirectionType.NegZ][j].iconObjectOnSignBoard.GetComponent<EditIconSignBoard>().SetSize(nowSet.signboard_icon_size / nowSet.signboard_size);
                                        }
                                    }
                                }
                                MessageSender(stage.ToString() + ": " + nowSet.signboard_font_size.ToString() + " " + nowSet.signboard_icon_size.ToString());
                                break;
                            case TUTORIAL_STAGE.SHOW_AUG_ONE_BY_ONE:
                                if (command.Equals("left"))
                                {
                                    AxisAdapter.ChangeLayer(augObjectText[currentAugNum].transform, false);
                                    AxisAdapter.ChangeLayer(augObjectIcon[currentAugNum].transform, false);
                                    if (currentAugNum == 0)
                                    {
                                        currentAugNum = totalAugNum - 1;
                                    }
                                    else
                                    {
                                        currentAugNum -= 1;
                                    }
                                    AxisAdapter.ChangeLayer(augObjectText[currentAugNum].transform, true);
                                    AxisAdapter.ChangeLayer(augObjectIcon[currentAugNum].transform, true);
                                }
                                else
                                {
                                    AxisAdapter.ChangeLayer(augObjectText[currentAugNum].transform, false);
                                    AxisAdapter.ChangeLayer(augObjectIcon[currentAugNum].transform, false);
                                    currentAugNum = (currentAugNum + 1) % totalAugNum;
                                    AxisAdapter.ChangeLayer(augObjectText[currentAugNum].transform, true);
                                    AxisAdapter.ChangeLayer(augObjectIcon[currentAugNum].transform, true);
                                }
                                MessageSender("NowIcon: " + augTexts[currentAugNum].ToString());
                                break;
                        }
                        break;
                    case "space":
                        SaveSet();
                        break;
                    case "ctrl":
                        if(stage== TUTORIAL_STAGE.SHOW_AUG_ONE_BY_ONE)
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
                            for (int i = 0; i < totalAugNum; i++)
                            {
                                float icon_text_dis = (iconPrefab.GetComponent<EditIcon>().GetSize() / 2 + textPrefab.GetComponent<EditText>().GetSize() / 20) / CustomizationSet.icon_text_dis_ratio * CreateAugmentations.GetTextLinesNumber(augTexts[i]);
                                augObjectText[i].transform.position = headPosition + headForward * 2.5f +
                                    Vector3.up * (0.2f - (iconPrefab.GetComponent<EditIcon>().GetSize() / 2 + textPrefab.GetComponent<EditText>().GetSize() / 20 + icon_text_dis));
                                augObjectIcon[i].transform.position =  headPosition + headForward * 2.5f + Vector3.up * 0.2f;

                                augObjectText[i].GetComponent<EditText>().SetSize(nowSet.font_size);
                                augObjectText[i].GetComponent<EditText>().SetFontColor(nowSet.font_color_number);
                                augObjectIcon[i].GetComponent<EditIcon>().SetSize(nowSet.icon_size);
                            }
                        }
                        break;
                }
            }
        }
    }
}
