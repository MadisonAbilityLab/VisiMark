using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CombineMesh : MonoBehaviour
{
    //菜单按钮静态触发
    [MenuItem("MeshCombine/CombineChildren")]
    static void CreatMeshCombine()
    {
        //获取到当前点击的游戏物体
        Transform tSelect = (Selection.activeGameObject).transform;

        //如果当前点击的游戏物体无子物体，则无操作
        if (tSelect.childCount < 1)
        {
            return;
        }


        //确保当前点击的游戏物体身上有MeshFilter组件
        if (!tSelect.GetComponent<MeshFilter>())
        {
            tSelect.gameObject.AddComponent<MeshFilter>();
        }
        //确保当前点击的游戏物体身上有MeshRenderer组件
        if (!tSelect.GetComponent<MeshRenderer>())
        {
            tSelect.gameObject.AddComponent<MeshRenderer>();
        }
        //获取到所有子物体的MeshFilter组件
        MeshFilter[] tFilters = tSelect.GetComponentsInChildren<MeshFilter>();

        //根据所有MeshFilter组件的个数申请一个用于Mesh联合的类存储信息
        CombineInstance[] tCombiners = new CombineInstance[tFilters.Length];

        //遍历所有子物体的网格信息进行存储
        for (int i = 0; i < tFilters.Length; i++)
        {
            //记录网格
            tCombiners[i].mesh = tFilters[i].sharedMesh;
            //记录位置
            tCombiners[i].transform = tFilters[i].transform.localToWorldMatrix;
        }
        //新申请一个网格用于显示组合后的游戏物体
        Mesh tFinalMesh = new Mesh();
        //重命名Mesh
        tFinalMesh.name = "tCombineMesh";
        //调用Unity内置方法组合新Mesh网格
        tFinalMesh.CombineMeshes(tCombiners);
        //赋值组合后的Mesh网格给选中的物体
        tSelect.GetComponent<MeshFilter>().sharedMesh = tFinalMesh;
        //赋值新的材质
        tSelect.GetComponent<MeshRenderer>().material = new Material(Shader.Find("VertexLit"));
    }

}
