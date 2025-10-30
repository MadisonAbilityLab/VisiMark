using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public enum ArrowType
{
    UpArrow,
    LeftArrow,
    RightArrow,
    DownArrow
};


[Serializable]
public class SignBoardInfomation
{
    public Vector3 position { get; set; }
    //public Vector3 UnityPosition { get; private set; }
    public int posXArrow { get; private set; }
    public int posZArrow { get; private set; }
    public int negXArrow { get; private set; }
    public int negZArrow { get; private set; }

    public GameObject signBoardObject { get; set; }
    public Dictionary<DirectionType, List<LandmarkOnSignBoardInformation>> arrowLandmarkInfos = new Dictionary<DirectionType, List<LandmarkOnSignBoardInformation>>();

    public SignBoardInfomation(Vector3 _position, int _posXArrow, int _negXArrow,
         int _posZArrow, int _negZArrow,List<ColorHallwayInformation> _ch_info)
    {
        position = _position;
        posXArrow = _posXArrow;
        negXArrow = _negXArrow;
        posZArrow = _posZArrow;
        negZArrow = _negZArrow;
        //UnityPosition = AxisAdapter.calculatePosion(_position);
        signBoardObject = null;
        arrowLandmarkInfos[DirectionType.PosX] = new List<LandmarkOnSignBoardInformation>();
        arrowLandmarkInfos[DirectionType.PosZ] = new List<LandmarkOnSignBoardInformation>();
        arrowLandmarkInfos[DirectionType.NegX] = new List<LandmarkOnSignBoardInformation>();
        arrowLandmarkInfos[DirectionType.NegZ] = new List<LandmarkOnSignBoardInformation>();
        if (posXArrow != -1)
        {
            for (int i = 0; i < _ch_info[posXArrow].arrowLandmarkInfos.Count; i++)
            {
                arrowLandmarkInfos[DirectionType.PosX].Add(new LandmarkOnSignBoardInformation());
            }
        }
        if (negXArrow != -1)
        {
            for (int i = 0; i < _ch_info[negXArrow].arrowLandmarkInfos.Count; i++)
            {
                arrowLandmarkInfos[DirectionType.NegX].Add(new LandmarkOnSignBoardInformation());
            }
        }
        if (posZArrow != -1)
        {
            for (int i = 0; i < _ch_info[posZArrow].arrowLandmarkInfos.Count; i++)
            {
                arrowLandmarkInfos[DirectionType.PosZ].Add(new LandmarkOnSignBoardInformation());
            }
        }
        if (negZArrow != -1)
        {
            for (int i = 0; i < _ch_info[negZArrow].arrowLandmarkInfos.Count; i++)
            {
                arrowLandmarkInfos[DirectionType.NegZ].Add(new LandmarkOnSignBoardInformation());
            }
        }
    }
}

public class CreateSignBoards : MonoBehaviour
{
    public delegate void sendMessage(String str);
    public static event sendMessage MessageSender;

    public const float SignBoardShowingDistance = 1;

    static GameObject textPrefab = null;
    static GameObject iconPrefab = null;
    static GameObject signBoardPrefab = null;

    public LoadCustomization loadCustomization;

    bool ifInitialized = false;


    public static List<SignBoardInfomation> SignBoardInfos = null;
    public static List<ColorHallwayInformation> ColorHallwayInfos = null;


    public CreateAugmentations augSetter;
    public CreateColorHallways cHallwaySetter;
    public const float icon_text_dis = 0.015f;

    int test_forward = 0;
    long test_time = 0;

    const float signBoardDisappearDistance = 0.25f;
    public static float signBoardHeight = 1f;

    List<string> commands = new List<string>();
    void GetSetting(String str)
    {
        if (str == "ctrl")
        {
            commands.Add(str);
        }
    }
    public static void SetPrefab(GameObject _textPrefab, GameObject _iconPrefab, GameObject _signBoardPrefab)
    {
        textPrefab = _textPrefab;
        iconPrefab = _iconPrefab;
        signBoardPrefab = _signBoardPrefab;
    }

    void Start()
    {
        test_time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        SocketClient.MessageReceiver += GetSetting;
        HttpNet.MessageReceiver += GetSetting;
        ControlSocketBehavior.MessageReceiver += GetSetting;
    }
    public static void setArrowAndLandmarks(GameObject arrow, ColorHallwayInformation infoArrow, SignBoardInfomation _signBoardInfo, DirectionType road_on_signboad_direction,GameObject textPrefab,GameObject iconPrefab, GameObject signBoardPrefab)
    {
        Vector3 _signBoardPos = _signBoardInfo.position;
        arrow.GetComponent<SetArrowSizeAndMaterial>().SetMaterial(infoArrow.color); 
        if (infoArrow.true_length != -1)
        {
            arrow.GetComponent<SetArrowSizeAndMaterial>().SetWidthAndLength(infoArrow.width, infoArrow.true_length);
        }
        else
        {
            arrow.GetComponent<SetArrowSizeAndMaterial>().SetWidthAndLength(infoArrow.width, infoArrow.length);
        }
        //return;
        bool ifDeadEnd = false;
        for (int i = 0; i < infoArrow.arrowLandmarkInfos.Count; i++)
        {
            if (infoArrow.arrowLandmarkInfos[i].ifShowOnSignBoard == false)
            {
                continue;
            }
            if (infoArrow.arrowLandmarkInfos[i].type == LandmarkType.DeadEnd)
            {
                ifDeadEnd = true;
                continue;
            }
            _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].textObjectOnSignBoard.transform.SetParent(arrow.transform);
            _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].textObjectOnSignBoard.SetActive(true);
            //AxisAdapter.ChangeLayer(_signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].textObjectOnSignBoard.transform, 0);
            _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].textObjectOnSignBoard.transform.rotation = _signBoardInfo.signBoardObject.transform.rotation;

            _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].iconObjectOnSignBoard.transform.SetParent(arrow.transform);
            _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].iconObjectOnSignBoard.SetActive(true);
            //AxisAdapter.ChangeLayer(_signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].iconObjectOnSignBoard.transform, 0);
            _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].iconObjectOnSignBoard.transform.rotation = _signBoardInfo.signBoardObject.transform.rotation;

            float icon_size = _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].iconObjectOnSignBoard.GetComponent<EditIconSignBoard>().GetSize();
            float text_size = _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].textObjectOnSignBoard.GetComponent<EditTextSignBoard>().GetSize();
            float signboard_size = signBoardPrefab.GetComponent<EditSignBoard>().GetSize();
            ArrowType _arrowType = arrow.GetComponent<SetArrowSizeAndMaterial>()._arrowType;
            Vector3 tmp_pos_text = new Vector3(0, icon_size / 2 + text_size / 20 + icon_text_dis * CreateAugmentations.GetTextLinesNumber(infoArrow.arrowLandmarkInfos[i].type.ToString()), 0);
            
            Vector2 WAndL = SetArrowSizeAndMaterial.GetLandmarkRelativeToArrowPosition(road_on_signboad_direction, _signBoardPos, infoArrow.arrowLandmarkInfos[i].position, infoArrow.width, infoArrow.length, _arrowType,icon_size,text_size, signboard_size
                , CreateAugmentations.GetMaxTextLineLength(infoArrow.arrowLandmarkInfos[i].type.ToString()), CreateAugmentations.GetTextLinesNumber(infoArrow.arrowLandmarkInfos[i].type.ToString()));
            if(infoArrow.arrowLandmarkInfos[i].positionOnSignBoard!=Vector3.zero)
            {
                WAndL = SetArrowSizeAndMaterial.GetLandmarkRelativeToArrowPosition(road_on_signboad_direction, _signBoardPos, infoArrow.arrowLandmarkInfos[i].positionOnSignBoard, infoArrow.width, infoArrow.length, _arrowType, icon_size, text_size, signboard_size
                   , CreateAugmentations.GetMaxTextLineLength(infoArrow.arrowLandmarkInfos[i].type.ToString()), CreateAugmentations.GetTextLinesNumber(infoArrow.arrowLandmarkInfos[i].type.ToString()));
            }
            else if (infoArrow.true_length != -1)
            {
                WAndL = SetArrowSizeAndMaterial.GetLandmarkRelativeToArrowPosition(road_on_signboad_direction, _signBoardPos, infoArrow.arrowLandmarkInfos[i].position, infoArrow.width, infoArrow.true_length, _arrowType, icon_size, text_size, signboard_size
                , CreateAugmentations.GetMaxTextLineLength(infoArrow.arrowLandmarkInfos[i].type.ToString()), CreateAugmentations.GetTextLinesNumber(infoArrow.arrowLandmarkInfos[i].type.ToString()));
            }

            
            _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].textObjectOnSignBoard.transform.position = arrow.transform.right * WAndL.y + arrow.transform.up * WAndL.x
                         + arrow.transform.forward * -0.03f
                         + arrow.transform.position- tmp_pos_text;
            _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].iconObjectOnSignBoard.transform.position = arrow.transform.right * WAndL.y + arrow.transform.up * WAndL.x
                + arrow.transform.forward * -0.03f
                + arrow.transform.position;
        }

        for (int i = 0; i < infoArrow.arrowLandmarkInfos.Count; i++)
        {
            if (infoArrow.arrowLandmarkInfos[i].ifShowOnSignBoard == false|| _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].textObjectOnSignBoard == null)
            {
                continue;
            }
            for (int j = i + 1; j < infoArrow.arrowLandmarkInfos.Count; j++)
            {
                if (infoArrow.arrowLandmarkInfos[j].ifShowOnSignBoard == false|| _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][j].textObjectOnSignBoard == null)
                {
                    continue;
                }
                float icon_size = _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].iconObjectOnSignBoard.GetComponent<EditIconSignBoard>().GetSize();
                float text_size = _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].textObjectOnSignBoard.GetComponent<EditTextSignBoard>().GetSize();
                float signboard_size = signBoardPrefab.GetComponent<EditSignBoard>().GetSize();
                //Vector3 tmp_pos_text = new Vector3(0, icon_size / 2 + text_size / 20 + icon_text_dis * CreateAugmentations.GetTextLinesNumber(infoArrow.arrowLandmarkInfos[j].type.ToString()), 0);
                int tmp_len = Math.Max(CreateAugmentations.GetTextLinesNumber(infoArrow.arrowLandmarkInfos[j].type.ToString()), CreateAugmentations.GetTextLinesNumber(infoArrow.arrowLandmarkInfos[i].type.ToString()));
                Vector3 tmp_pos_text = new Vector3(0, icon_size / 2 + text_size / 20 + icon_text_dis * tmp_len, 0);
                ArrowType _arrowType = arrow.GetComponent<SetArrowSizeAndMaterial>()._arrowType;
                double distance = Math.Abs(_signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].iconObjectOnSignBoard.transform.localPosition.x- _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][j].iconObjectOnSignBoard.transform.localPosition.x);
                float minDistance = icon_size + text_size / 10 + icon_text_dis+ 0.02f;
                if(_arrowType==ArrowType.LeftArrow || _arrowType == ArrowType.RightArrow)
                {
                    
                    minDistance = icon_size  + text_size / 25 *(-8+CreateAugmentations.GetMaxTextLineLength(infoArrow.arrowLandmarkInfos[i].type.ToString()) + CreateAugmentations.GetMaxTextLineLength(infoArrow.arrowLandmarkInfos[j].type.ToString()));
                }
                Vector2 WAndL1 = SetArrowSizeAndMaterial.GetLandmarkRelativeToArrowPosition(road_on_signboad_direction, _signBoardPos, infoArrow.arrowLandmarkInfos[i].position, infoArrow.width, infoArrow.length, _arrowType, icon_size, text_size, signboard_size
                , CreateAugmentations.GetMaxTextLineLength(infoArrow.arrowLandmarkInfos[i].type.ToString()), CreateAugmentations.GetTextLinesNumber(infoArrow.arrowLandmarkInfos[i].type.ToString()));
                Vector2 WAndL2 = SetArrowSizeAndMaterial.GetLandmarkRelativeToArrowPosition(road_on_signboad_direction, _signBoardPos, infoArrow.arrowLandmarkInfos[j].position, infoArrow.width, infoArrow.length, _arrowType, icon_size, text_size, signboard_size
                , CreateAugmentations.GetMaxTextLineLength(infoArrow.arrowLandmarkInfos[j].type.ToString()), CreateAugmentations.GetTextLinesNumber(infoArrow.arrowLandmarkInfos[j].type.ToString()));

                if (distance < minDistance&& WAndL1.x*WAndL2.x>0)
                {
                    Vector3 newPosition = _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].iconObjectOnSignBoard.transform.localPosition +
                        new Vector3((_signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][j].iconObjectOnSignBoard.transform.localPosition.x - _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].iconObjectOnSignBoard.transform.localPosition.x),0,0).normalized * (minDistance+ j*0.0001f);
                    
                    _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][j].iconObjectOnSignBoard.transform.localPosition = newPosition;
                    _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][j].textObjectOnSignBoard.transform.position = _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][j].iconObjectOnSignBoard.transform.position - tmp_pos_text;
                }
                
            }
        }
        if (ifDeadEnd)
        {
            arrow.GetComponent<SetArrowSizeAndMaterial>().SetDeadEndMaterial(infoArrow.color);
        }
        else
        {
            arrow.GetComponent<SetArrowSizeAndMaterial>().SetDeadEndMaterial("transparent", true);
        }
    }

    public static void CreateLandmarksOnArrow(ColorHallwayInformation infoArrow, SignBoardInfomation _signBoardInfo, DirectionType road_on_signboad_direction, GameObject textPrefab, GameObject iconPrefab)
    {
        for (int i = 0; i < infoArrow.arrowLandmarkInfos.Count; i++)
        {
            if (infoArrow.arrowLandmarkInfos[i].ifShowOnSignBoard == false)
            {
                continue;
            }
            if (infoArrow.arrowLandmarkInfos[i].type == LandmarkType.DeadEnd)
            {
                continue;
            }
            if (_signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].textObjectOnSignBoard == null)
            {
                _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].textObjectOnSignBoard = (GameObject)Instantiate(textPrefab);
                _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].textObjectOnSignBoard.GetComponent<EditTextSignBoard>().ChangeText(CreateAugmentations.AddLineBeforeSecondAndSubsequentUppercase(infoArrow.arrowLandmarkInfos[i].type.ToString()));
                _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].textObjectOnSignBoard.GetComponent<EditTextSignBoard>().SetCurrentFontColor();
            }

            if (_signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].iconObjectOnSignBoard == null)
            {
                _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].iconObjectOnSignBoard = (GameObject)Instantiate(iconPrefab);
                if (infoArrow.arrowLandmarkInfos[i].category != LandmarkMainCategory.None)
                {
                    _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].iconObjectOnSignBoard.GetComponent<EditIconSignBoard>().ChangeIcon(infoArrow.arrowLandmarkInfos[i].category.ToString());
                }
                else
                {
                    _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].iconObjectOnSignBoard.GetComponent<EditIconSignBoard>().ChangeIcon(infoArrow.arrowLandmarkInfos[i].type.ToString(), false);
                }
            }
        }
    }

    public static void setLandmarksOnArrowInvisible(GameObject arrow, ColorHallwayInformation infoArrow, SignBoardInfomation _signBoardInfo, DirectionType road_on_signboad_direction, GameObject textPrefab, GameObject iconPrefab, GameObject signBoardPrefab)
    {
        for (int i = 0; i < infoArrow.arrowLandmarkInfos.Count; i++)
        {
            if (infoArrow.arrowLandmarkInfos[i].ifShowOnSignBoard == false)
            {
                continue;
            }
            if (_signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].textObjectOnSignBoard != null)
            {
                _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].textObjectOnSignBoard.SetActive(false);
                _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].textObjectOnSignBoard.transform.SetParent(arrow.transform);
            }
            if (_signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].iconObjectOnSignBoard != null)
            {
                _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].iconObjectOnSignBoard.SetActive(false);
                _signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].iconObjectOnSignBoard.transform.SetParent(arrow.transform);
            }
            //AxisAdapter.ChangeLayer(_signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].textObjectOnSignBoard.transform, 3);
            //AxisAdapter.ChangeLayer(_signBoardInfo.arrowLandmarkInfos[road_on_signboad_direction][i].iconObjectOnSignBoard.transform, 3);
        }
    }
    void Update()
    {
        if (ifInitialized==false)
        {
            return;
        }
        if(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() - test_time >= 3)
        {
            test_time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            test_forward = (test_forward + 1)% 4;
        }
        if (!InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 headPosition))
        {
            headPosition = Vector3.zero;
        }
        if (!InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion headRotation))
        {
            headRotation = Quaternion.identity;
        }
        Vector3 headForward = headRotation * Vector3.forward;
        if(headPosition == Vector3.zero)
        {
            switch (test_forward)
            {
                case 1:
                    headForward = headRotation * Vector3.right;
                    break;
                case 2:
                    headForward = headRotation * Vector3.back;
                    break;
                case 3:
                    headForward = headRotation * Vector3.left;
                    break;
            }
        }
        headForward = new Vector3(headForward.x, 0, headForward.z);

        bool resetHeadPosition = false;
        if (commands.Count > 0)
        {
            commands.RemoveAt(0);
            resetHeadPosition = true;
        }
        for (int i = 0; i < SignBoardInfos.Count; i++)
        {
            SignBoardInfomation infoSignBoard = SignBoardInfos[i];
            if (resetHeadPosition)
            //else if (Math.Abs(headPosition.y - last_head_position.y) < 0.5f)
            {
                signBoardHeight = headPosition.y - 0.05f;
                infoSignBoard.signBoardObject.transform.position = new Vector3(infoSignBoard.signBoardObject.transform.position.x, signBoardHeight, infoSignBoard.signBoardObject.transform.position.z);
            }
            GameObject RightArrow = infoSignBoard.signBoardObject.transform.Find("RightArrow").gameObject;
            GameObject LeftArrow = infoSignBoard.signBoardObject.transform.Find("LeftArrow").gameObject;
            GameObject UpArrow = infoSignBoard.signBoardObject.transform.Find("UpArrow").gameObject;
            GameObject DownArrow = infoSignBoard.signBoardObject.transform.Find("DownArrow").gameObject;

            float angleWithX = Math.Abs(Vector3.Angle(headForward, AxisAdapter.vectorX));
            float angleWithZ = Math.Abs(Vector3.Angle(headForward, AxisAdapter.vectorZ));
            if (angleWithX < 45)
            {
                //PosX
                //MessageSender("PosX!!");
                Quaternion orientationTowardsHead = Quaternion.LookRotation(AxisAdapter.vectorX, Vector3.up);
                infoSignBoard.signBoardObject.transform.rotation = orientationTowardsHead;
                if (infoSignBoard.posXArrow != -1)
                {
                    setArrowAndLandmarks(UpArrow, ColorHallwayInfos[infoSignBoard.posXArrow], infoSignBoard, DirectionType.PosX, textPrefab, iconPrefab, signBoardPrefab);
                }
                else
                {
                    UpArrow.GetComponent<SetArrowSizeAndMaterial>().SetMaterial("transparent", true);
                    UpArrow.GetComponent<SetArrowSizeAndMaterial>().SetDeadEndMaterial("transparent", true);
                }
                if (infoSignBoard.posZArrow != -1)
                {
                    setArrowAndLandmarks(LeftArrow, ColorHallwayInfos[infoSignBoard.posZArrow], infoSignBoard, DirectionType.PosZ, textPrefab, iconPrefab, signBoardPrefab);
                }
                else
                {
                    LeftArrow.GetComponent<SetArrowSizeAndMaterial>().SetMaterial("transparent", true);
                    LeftArrow.GetComponent<SetArrowSizeAndMaterial>().SetDeadEndMaterial("transparent", true);
                }
                if (infoSignBoard.negZArrow != -1)
                {
                    setArrowAndLandmarks(RightArrow, ColorHallwayInfos[infoSignBoard.negZArrow], infoSignBoard, DirectionType.NegZ, textPrefab, iconPrefab, signBoardPrefab);
                }
                else
                {
                    RightArrow.GetComponent<SetArrowSizeAndMaterial>().SetMaterial("transparent", true);
                    RightArrow.GetComponent<SetArrowSizeAndMaterial>().SetDeadEndMaterial("transparent", true);
                }
                if (infoSignBoard.negXArrow != -1)
                {
                    setLandmarksOnArrowInvisible(DownArrow, ColorHallwayInfos[infoSignBoard.negXArrow], infoSignBoard, DirectionType.NegX, textPrefab, iconPrefab, signBoardPrefab);
                }
            }
            else if (angleWithZ < 45)
            {
                //PosZ
                //MessageSender("PosZ!!");
                Quaternion orientationTowardsHead = Quaternion.LookRotation(AxisAdapter.vectorZ, Vector3.up);
                infoSignBoard.signBoardObject.transform.rotation = orientationTowardsHead;
                if (infoSignBoard.posZArrow != -1)
                {
                    setArrowAndLandmarks(UpArrow, ColorHallwayInfos[infoSignBoard.posZArrow], infoSignBoard, DirectionType.PosZ, textPrefab, iconPrefab, signBoardPrefab);
                }
                else
                {
                    UpArrow.GetComponent<SetArrowSizeAndMaterial>().SetMaterial("transparent", true);
                    UpArrow.GetComponent<SetArrowSizeAndMaterial>().SetDeadEndMaterial("transparent", true);
                }
                if (infoSignBoard.negXArrow != -1)
                {
                    setArrowAndLandmarks(LeftArrow, ColorHallwayInfos[infoSignBoard.negXArrow], infoSignBoard, DirectionType.NegX, textPrefab, iconPrefab, signBoardPrefab);
                }
                else
                {
                    LeftArrow.GetComponent<SetArrowSizeAndMaterial>().SetMaterial("transparent", true);
                    LeftArrow.GetComponent<SetArrowSizeAndMaterial>().SetDeadEndMaterial("transparent", true);
                }
                if (infoSignBoard.posXArrow != -1)
                {
                    setArrowAndLandmarks(RightArrow, ColorHallwayInfos[infoSignBoard.posXArrow], infoSignBoard, DirectionType.PosX, textPrefab, iconPrefab, signBoardPrefab);
                }
                else
                {
                    RightArrow.GetComponent<SetArrowSizeAndMaterial>().SetMaterial("transparent", true);
                    RightArrow.GetComponent<SetArrowSizeAndMaterial>().SetDeadEndMaterial("transparent", true);
                }
                if (infoSignBoard.negZArrow != -1)
                {
                    setLandmarksOnArrowInvisible(DownArrow, ColorHallwayInfos[infoSignBoard.negZArrow], infoSignBoard, DirectionType.NegZ, textPrefab, iconPrefab, signBoardPrefab);
                }
            }
            else if (angleWithX > 135)
            {
                //NegX
                //MessageSender("NegX!!");
                Quaternion orientationTowardsHead = Quaternion.LookRotation(-AxisAdapter.vectorX, Vector3.up);
                infoSignBoard.signBoardObject.transform.rotation = orientationTowardsHead;
                if (infoSignBoard.negXArrow != -1)
                {
                    setArrowAndLandmarks(UpArrow, ColorHallwayInfos[infoSignBoard.negXArrow], infoSignBoard, DirectionType.NegX, textPrefab, iconPrefab, signBoardPrefab);
                }
                else
                {
                    UpArrow.GetComponent<SetArrowSizeAndMaterial>().SetMaterial("transparent", true);
                    UpArrow.GetComponent<SetArrowSizeAndMaterial>().SetDeadEndMaterial("transparent", true);
                }
                if (infoSignBoard.negZArrow != -1)
                {
                    setArrowAndLandmarks(LeftArrow, ColorHallwayInfos[infoSignBoard.negZArrow], infoSignBoard, DirectionType.NegZ, textPrefab, iconPrefab, signBoardPrefab);
                }
                else
                {
                    LeftArrow.GetComponent<SetArrowSizeAndMaterial>().SetMaterial("transparent", true);
                    LeftArrow.GetComponent<SetArrowSizeAndMaterial>().SetDeadEndMaterial("transparent", true);
                }
                if (infoSignBoard.posZArrow != -1)
                {
                    setArrowAndLandmarks(RightArrow, ColorHallwayInfos[infoSignBoard.posZArrow], infoSignBoard, DirectionType.PosZ, textPrefab, iconPrefab, signBoardPrefab);
                }
                else
                {
                    RightArrow.GetComponent<SetArrowSizeAndMaterial>().SetMaterial("transparent", true);
                    RightArrow.GetComponent<SetArrowSizeAndMaterial>().SetDeadEndMaterial("transparent", true);
                }
                if (infoSignBoard.posXArrow != -1)
                {
                    setLandmarksOnArrowInvisible(DownArrow,ColorHallwayInfos[infoSignBoard.posXArrow], infoSignBoard, DirectionType.PosX, textPrefab, iconPrefab, signBoardPrefab);
                }
            }
            else
            {
                //NegZ
                //MessageSender("NegZ!!");
                Quaternion orientationTowardsHead = Quaternion.LookRotation(-AxisAdapter.vectorZ, Vector3.up);
                infoSignBoard.signBoardObject.transform.rotation = orientationTowardsHead;
                if (infoSignBoard.negZArrow != -1)
                {
                    setArrowAndLandmarks(UpArrow, ColorHallwayInfos[infoSignBoard.negZArrow], infoSignBoard, DirectionType.NegZ, textPrefab, iconPrefab, signBoardPrefab);
                }
                else
                {
                    UpArrow.GetComponent<SetArrowSizeAndMaterial>().SetMaterial("transparent", true);
                    UpArrow.GetComponent<SetArrowSizeAndMaterial>().SetDeadEndMaterial("transparent", true);
                }
                if (infoSignBoard.posXArrow != -1)
                {
                    setArrowAndLandmarks(LeftArrow, ColorHallwayInfos[infoSignBoard.posXArrow], infoSignBoard, DirectionType.PosX, textPrefab, iconPrefab, signBoardPrefab);
                }
                else
                {
                    LeftArrow.GetComponent<SetArrowSizeAndMaterial>().SetMaterial("transparent", true);
                    LeftArrow.GetComponent<SetArrowSizeAndMaterial>().SetDeadEndMaterial("transparent", true);
                }
                if (infoSignBoard.negXArrow != -1)
                {
                    setArrowAndLandmarks(RightArrow, ColorHallwayInfos[infoSignBoard.negXArrow], infoSignBoard, DirectionType.NegX, textPrefab, iconPrefab, signBoardPrefab);
                }
                else
                {
                    RightArrow.GetComponent<SetArrowSizeAndMaterial>().SetMaterial("transparent", true);
                    RightArrow.GetComponent<SetArrowSizeAndMaterial>().SetDeadEndMaterial("transparent", true);
                }
                if (infoSignBoard.posZArrow != -1)
                {
                    setLandmarksOnArrowInvisible(DownArrow, ColorHallwayInfos[infoSignBoard.posZArrow], infoSignBoard, DirectionType.PosZ, textPrefab, iconPrefab, signBoardPrefab);
                }
            }
        }

        int onCL = -1;
        for (int i = 0; i < ColorHallwayInfos.Count; i++)
        {
            if (AxisAdapter.calculateIfOnPath(ColorHallwayInfos[i], headPosition))
            {
                onCL = i;
                break;
            }
        }
        if (onCL != -1)
        {
            //showCh.Add(onCL);
            for (int i = 0; i < SignBoardInfos.Count; i++)
            {
                if (SignBoardInfos[i].negXArrow == onCL || SignBoardInfos[i].posXArrow == onCL || SignBoardInfos[i].posZArrow == onCL || SignBoardInfos[i].negZArrow == onCL)
                {
                    if (Math.Abs((SignBoardInfos[i].signBoardObject.transform.position - headPosition).x) < signBoardDisappearDistance
                        && Math.Abs((SignBoardInfos[i].signBoardObject.transform.position - headPosition).z) < signBoardDisappearDistance)
                        //if (new Vector3((SignBoardInfos[i].signBoardObject.transform.position - headPosition).x, 0, (SignBoardInfos[i].signBoardObject.transform.position - headPosition).z).magnitude < signBoardDisappearDistance)
                    {
                        AxisAdapter.ChangeLayer(SignBoardInfos[i].signBoardObject.transform, false);
                    }
                    else
                    {
                        AxisAdapter.ChangeLayer(SignBoardInfos[i].signBoardObject.transform, true, true);
                    }
                }
                else
                {
                    AxisAdapter.ChangeLayer(SignBoardInfos[i].signBoardObject.transform, false);
                }
            }
            for (int i = 0; i < ColorHallwayInfos.Count; i++)
            {
                if (i == onCL)
                {
                    if (i == 19)
                    {
                        CreateColorHallways.setColorHallwayAndAugIfVisible(ColorHallwayInfos[i], true);
                    }
                    else
                    {
                        CreateColorHallways.setColorHallwayAndAugIfVisibleWithoutWall(ColorHallwayInfos[i], true);
                    }
                }
                else
                {
                    if (ColorHallwayInfos[onCL].relatedColorHallway.Contains(i))
                    {
                        if(AxisAdapter.route==AxisAdapter.tutorialRoute||AxisAdapter.IfSameDirection(ColorHallwayInfos[onCL].direction, ColorHallwayInfos[i].direction))
                        {
                            CreateColorHallways.setColorHallwayAndAugIfVisibleWithoutWall(ColorHallwayInfos[i], true);
                        }
                        else
                        {
                            CreateColorHallways.setColorHallwayAndAugIfVisible(ColorHallwayInfos[i], true);
                        }
                    }
                    else
                    {
                        CreateColorHallways.setColorHallwayAndAugIfVisible(ColorHallwayInfos[i], false);
                    }
                }
            }
        }
        else
        {
            List<int> showCh = new List<int>();
            for (int i = 0; i < SignBoardInfos.Count; i++)
            {
                if ((SignBoardInfos[i].signBoardObject.transform.position - headPosition).magnitude < 5)
                {
                    if (SignBoardInfos[i].posXArrow != -1)
                    {
                        showCh.Add(SignBoardInfos[i].posXArrow);
                    }
                    if (SignBoardInfos[i].posZArrow != -1)
                    {
                        showCh.Add(SignBoardInfos[i].posZArrow);
                    }
                    if (SignBoardInfos[i].negXArrow != -1)
                    {
                        showCh.Add(SignBoardInfos[i].negXArrow);
                    }
                    if (SignBoardInfos[i].negZArrow != -1)
                    {
                        showCh.Add(SignBoardInfos[i].negZArrow);
                    }
                    if (Math.Abs((SignBoardInfos[i].signBoardObject.transform.position - headPosition).x)< signBoardDisappearDistance
                        && Math.Abs((SignBoardInfos[i].signBoardObject.transform.position - headPosition).z)< signBoardDisappearDistance)
                    {
                        AxisAdapter.ChangeLayer(SignBoardInfos[i].signBoardObject.transform, false);
                    }
                    else
                    {
                        AxisAdapter.ChangeLayer(SignBoardInfos[i].signBoardObject.transform, true, true);
                    }
                }
                else
                {
                    AxisAdapter.ChangeLayer(SignBoardInfos[i].signBoardObject.transform, false);
                }
                for (int j = 0; j < ColorHallwayInfos.Count; j++)
                {
                    CreateColorHallways.setColorHallwayAndAugIfVisibleWithoutWall(ColorHallwayInfos[j], showCh.Contains(j));
                }
            }
        }


    }
    public void TryCreateSignBoards(bool ifRouteChanged)
    {
        ///*
        if (loadCustomization != null)
        {
            loadCustomization.SetPrefabs(AxisAdapter.route==AxisAdapter.tutorialRoute);
            Debug.Log("setPrefabs");
        }
        if (ifRouteChanged && ColorHallwayInfos != null)
        {
            for (int i = 0; i < SignBoardInfos.Count; i++)
            {
                if (SignBoardInfos[i].signBoardObject != null)
                {
                    Destroy(SignBoardInfos[i].signBoardObject);
                }
            }

            for (int i = 0; i < ColorHallwayInfos.Count; i++)
            {
                if (ColorHallwayInfos[i].colorHallwayObject != null)
                {
                    Destroy(ColorHallwayInfos[i].colorHallwayObject);
                }
                if (ColorHallwayInfos[i].wallObjectLeft!=null)
                {
                    Destroy(ColorHallwayInfos[i].wallObjectLeft);
                }
                if (ColorHallwayInfos[i].wallObjectRight != null)
                {
                    Destroy(ColorHallwayInfos[i].wallObjectRight);
                }
                for (int j = 0; j < ColorHallwayInfos[i].arrowLandmarkInfos.Count; j++)
                {

                    if (ColorHallwayInfos[i].arrowLandmarkInfos[j].textObject != null)
                    {
                        Destroy(ColorHallwayInfos[i].arrowLandmarkInfos[j].textObject);
                    }
                    if (ColorHallwayInfos[i].arrowLandmarkInfos[j].iconObject != null)
                    {
                        Destroy(ColorHallwayInfos[i].arrowLandmarkInfos[j].iconObject);
                    }
                    if (ColorHallwayInfos[i].arrowLandmarkInfos[j].outlineOrColorObject != null)
                    {
                        Destroy(ColorHallwayInfos[i].arrowLandmarkInfos[j].outlineOrColorObject);
                    }
                    if (ColorHallwayInfos[i].arrowLandmarkInfos[j].leftWallObject != null)
                    {
                        Destroy(ColorHallwayInfos[i].arrowLandmarkInfos[j].leftWallObject);
                    }
                    if (ColorHallwayInfos[i].arrowLandmarkInfos[j].rightWallObject != null)
                    {
                        Destroy(ColorHallwayInfos[i].arrowLandmarkInfos[j].rightWallObject);
                    }
                }
            }
            ColorHallwayInfos.Clear();
            SignBoardInfos.Clear();
            ColorHallwayInfos = null;
            ColorHallwayInfos = null;
        }
        //*/
        if (ColorHallwayInfos == null)
        {
            ColorHallwayInfos = HardCodedPositions.getColorHallwayList(AxisAdapter.route);
            SignBoardInfos = HardCodedPositions.getSignBoardsList(AxisAdapter.route, ColorHallwayInfos);
            MessageSender("Get Infos!");
            Debug.Log("Get Infos!");
        }
        else
        {
            Debug.Log("Infos Existed!");
        }
        if(signBoardPrefab == null)
        {
            MessageSender("No SignBoard Prefab!");
            return;
        }
        for (int i = 0; i < SignBoardInfos.Count; i++)
        {
            if (SignBoardInfos[i].signBoardObject == null)
            {
                Vector3 tmp_pos = AxisAdapter.calculatePosion(SignBoardInfos[i].position);
                GameObject gameObjectSignBoard = (GameObject)Instantiate(signBoardPrefab, new Vector3(tmp_pos.x, signBoardHeight, tmp_pos.z), Quaternion.identity);
                gameObjectSignBoard.GetComponent<PrefabName>().objectName = "SignBoard " + i.ToString();
                GameObject RightArrow = gameObjectSignBoard.transform.Find("RightArrow").gameObject;
                GameObject LeftArrow = gameObjectSignBoard.transform.Find("LeftArrow").gameObject;
                GameObject UpArrow = gameObjectSignBoard.transform.Find("UpArrow").gameObject;
                RightArrow.GetComponent<SetArrowSizeAndMaterial>().SetMaterial("transparent", true);
                LeftArrow.GetComponent<SetArrowSizeAndMaterial>().SetMaterial("transparent", true);
                UpArrow.GetComponent<SetArrowSizeAndMaterial>().SetMaterial("transparent", true);
                SignBoardInfos[i].signBoardObject = gameObjectSignBoard;
            }
            else
            {
                Vector3 tmp_pos = AxisAdapter.calculatePosion(SignBoardInfos[i].position);
                SignBoardInfos[i].signBoardObject.transform.position = new Vector3(tmp_pos.x,signBoardHeight,tmp_pos.z);
            }
            SignBoardInfomation infoSignBoard = SignBoardInfos[i];
            if (infoSignBoard.posXArrow != -1)
            {
                CreateLandmarksOnArrow(ColorHallwayInfos[infoSignBoard.posXArrow], infoSignBoard, DirectionType.PosX, textPrefab, iconPrefab);
            }
            if (infoSignBoard.posZArrow != -1)
            {
                CreateLandmarksOnArrow(ColorHallwayInfos[infoSignBoard.posZArrow], infoSignBoard, DirectionType.PosZ, textPrefab, iconPrefab);
            }
            if (infoSignBoard.negZArrow != -1)
            {
                CreateLandmarksOnArrow(ColorHallwayInfos[infoSignBoard.negZArrow], infoSignBoard, DirectionType.NegZ, textPrefab, iconPrefab);
            }
            if (infoSignBoard.negXArrow != -1)
            {
                CreateLandmarksOnArrow(ColorHallwayInfos[infoSignBoard.negXArrow], infoSignBoard, DirectionType.NegX, textPrefab, iconPrefab);
            }
        }
        for (int i = 0; i < ColorHallwayInfos.Count; i++)
        {
            cHallwaySetter.TryCreateColorHallways(ColorHallwayInfos[i], true,i);
            augSetter.TryCreateAugmentations(ColorHallwayInfos[i], true);
        }
        ifInitialized = true;
        MessageSender("CreateSignBoards!!! "+ifRouteChanged.ToString()+"! SignBoardsCount: " + SignBoardInfos.Count.ToString());
    }
}
