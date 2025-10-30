using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LoadCustomization : MonoBehaviour
{
    public delegate void sendMessage(String str);
    public static event sendMessage MessageSender;

    public GameObject textPrefab;
    public GameObject iconPrefab;
    public GameObject outlinePrefab;
    public GameObject textSignBoardPrefab;
    public GameObject iconSignBoardPrefab;
    public GameObject signBoardPrefab;

    CustomSet p_CustomSet = null;

    private long start_time = -1;
    const int waitInitializeTime = 1;
    void Start()
    {
        start_time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
    }


    public bool LoadSet(bool if_tutorial = false)
    {
        if (if_tutorial)
        {
            return false;
        }
        if (File.Exists(Application.persistentDataPath + "/customize_set"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/customize_set", FileMode.Open);
            p_CustomSet = (CustomSet)bf.Deserialize(file);
            file.Close();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetPrefabs(bool if_tutorial = false)
    {
        if (LoadSet(if_tutorial))
        {
            textPrefab.GetComponent<EditText>().SetSize(p_CustomSet.font_size);
            textPrefab.GetComponent<EditText>().SetFontColor(p_CustomSet.font_color_number);
            iconPrefab.GetComponent<EditIcon>().SetSize(p_CustomSet.icon_size);
            outlinePrefab.GetComponent<EditOutline>().SetColor(p_CustomSet.getOutlineColor());
            outlinePrefab.GetComponent<EditOutline>().SetWidth(p_CustomSet.outline_width);
            textSignBoardPrefab.GetComponent<EditTextSignBoard>().SetSize(p_CustomSet.signboard_font_size);
            iconSignBoardPrefab.GetComponent<EditIconSignBoard>().SetSize(p_CustomSet.signboard_icon_size);
            signBoardPrefab.GetComponent<EditSignBoard>().SetSize(p_CustomSet.signboard_size);
            textSignBoardPrefab.GetComponent<EditTextSignBoard>().SetCurrentFontColor();
            CreateAugmentations.ifIcon = p_CustomSet.if_icon;
            CreateAugmentations.SetPrefab(textPrefab, iconPrefab, outlinePrefab);
            CreateSignBoards.SetPrefab(textSignBoardPrefab, iconSignBoardPrefab, signBoardPrefab);
            Tutorial.setPrefabs(textPrefab, iconPrefab, outlinePrefab, textSignBoardPrefab, iconSignBoardPrefab, signBoardPrefab);
            MessageSender("Find Customization! " + p_CustomSet.getString());
            Debug.Log("Find Customization! " + p_CustomSet.getString());
        }
        else
        {
            CustomSet nowSet = new CustomSet();
            textPrefab.GetComponent<EditText>().SetSize(nowSet.font_size);
            textPrefab.GetComponent<EditText>().SetFontColor(nowSet.font_color_number);
            iconPrefab.GetComponent<EditIcon>().SetSize(nowSet.icon_size);
            outlinePrefab.GetComponent<EditOutline>().SetColor(nowSet.getOutlineColor());
            outlinePrefab.GetComponent<EditOutline>().SetWidth(nowSet.outline_width);
            textSignBoardPrefab.GetComponent<EditTextSignBoard>().SetSize(nowSet.signboard_font_size);
            iconSignBoardPrefab.GetComponent<EditIconSignBoard>().SetSize(nowSet.signboard_icon_size);
            signBoardPrefab.GetComponent<EditSignBoard>().SetSize(nowSet.signboard_size);
            //EditSignBoard.SetSignBoardTextColorNumber(nowSet.signboard_text_color_number);
            textSignBoardPrefab.GetComponent<EditTextSignBoard>().SetCurrentFontColor();
            CreateAugmentations.ifIcon = nowSet.if_icon;
            CreateAugmentations.SetPrefab(textPrefab, iconPrefab, outlinePrefab);
            CreateSignBoards.SetPrefab(textSignBoardPrefab, iconSignBoardPrefab, signBoardPrefab);
            Tutorial.setPrefabs(textPrefab, iconPrefab, outlinePrefab, textSignBoardPrefab, iconSignBoardPrefab, signBoardPrefab);
            MessageSender("Do Not Find Customization!");
            Debug.Log("Do Not Find Customization!" + nowSet.getString());
        }
    }

    void Update()
    {
    }
}
