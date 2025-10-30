using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardCodedPositions
{

    const float ch_height = -0.1f;
    public static List<SignBoardInfomation>  getSignBoardsList(int route, List<ColorHallwayInformation> _ch_info)
    {
        List<SignBoardInfomation> SignBoardInfos = new List<SignBoardInfomation>();
        switch (route)
        {
            case 9:
                {
                    SignBoardInfos.Add(new SignBoardInfomation(
                        _ch_info[1].position + new Vector3(-_ch_info[0].width / 2, 0, _ch_info[1].width / 2-0.1f),
                        1, 0, -1, 2, _ch_info)
                    );
                    break;
                }
            default:
                {
                    SignBoardInfos.Add(new SignBoardInfomation(
                        new Vector3((float)0, (float)1.5, (float)0),
                        0, 1, 2, 3, _ch_info)
                    );
                    break;
                }
        }
        return SignBoardInfos;
    }
    public static List<ColorHallwayInformation>  getColorHallwayList(int route)
    {
        List<ColorHallwayInformation> ColorHallwayInfos = new List<ColorHallwayInformation>();
        Vector3 tmp_offset = Vector3.zero;
        switch (route)
        {
            case AxisAdapter.tutorialRoute:
                {
                    tmp_offset = -new Vector3(-0.1f, 0, 0.15f);
                    break;
                }
            default:
                {
                    tmp_offset = Vector3.zero;
                    break;
                }
        }
        if(route == AxisAdapter.tutorialRoute)
        {
            ColorHallwayInformation tmp_cHallwayInfo = new ColorHallwayInformation(
                tmp_offset + new Vector3(0, ch_height, 2.2f),
                2.2f,
                3f,
                "red",
                DirectionType.NegX,
                new List<int> { 1, 2 }
            );
            tmp_cHallwayInfo.arrowLandmarkInfos.Add(
                new LandmarkInformation(
                    tmp_offset + new Vector3(-3f, 1.8f, -1.1f),
                    new Vector3((float)5, (float)2, (float)1),
                    LandmarkType.DeadEnd,
                    LandmarkMainCategory.Structure
                )
            );
            tmp_cHallwayInfo.arrowLandmarkInfos.Add(
                new LandmarkInformation(
                    tmp_offset + new Vector3(-0.5f, 2f, 0.1f),
                    tmp_offset + new Vector3(0, 2f, -4f),
                    new Vector3((float)0.4, (float)0.4, (float)0.4),
                    LandmarkType.AED,
                    LandmarkMainCategory.Emergency
                )
            );
            ColorHallwayInfos.Add(tmp_cHallwayInfo);

            tmp_cHallwayInfo = new ColorHallwayInformation(
                tmp_offset + new Vector3(2.2f, ch_height, 0),
                2.2f,
                9.15f,
                "yellow",
                DirectionType.PosX,
                new List<int> { 0, 2 }
            );
            tmp_cHallwayInfo.arrowLandmarkInfos.Add(
                new LandmarkInformation(
                    tmp_offset + new Vector3(2.7f, 1.8f, 2.1f),
                    tmp_offset + new Vector3(tmp_cHallwayInfo.width + 2.3f, 1.8f, 2.1f),
                    new Vector3((float)1, (float)1, (float)0.4),
                    LandmarkType.Window,
                    LandmarkMainCategory.Structure
                )
            );
            ColorHallwayInfos.Add(tmp_cHallwayInfo);

            tmp_cHallwayInfo = new ColorHallwayInformation(
                tmp_offset + new Vector3(0, ch_height, 0),
                2.2f,
                5.86f,
                "blue",
                DirectionType.NegZ,
                new List<int> { 1, 0 }
            );
            tmp_cHallwayInfo.arrowLandmarkInfos.Add(
                new LandmarkInformation(
                    tmp_offset + new Vector3(0, 1.5f, -2.9f),
                    tmp_offset + new Vector3(0, 1.5f, -5.5f),
                    new Vector3((float)1.5, (float)2.5, (float)1),
                    LandmarkType.Elevator,
                    LandmarkMainCategory.Accessibility
                )
            );
            tmp_cHallwayInfo.arrowLandmarkInfos.Add(
                new LandmarkInformation(
                    tmp_offset + new Vector3(2.2f, 1.5f, -0.5f),
                    tmp_offset + new Vector3(3.5f, 1.5f, -2.5f),
                    new Vector3((float)1, (float)2.5, (float)1),
                    LandmarkType.Restroom,
                    LandmarkMainCategory.Info
                )
            );
            ColorHallwayInfos.Add(tmp_cHallwayInfo);
        }
        return ColorHallwayInfos;
    }
}
