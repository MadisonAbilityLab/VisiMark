using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CombineMesh : MonoBehaviour
{
    //�˵���ť��̬����
    [MenuItem("MeshCombine/CombineChildren")]
    static void CreatMeshCombine()
    {
        //��ȡ����ǰ�������Ϸ����
        Transform tSelect = (Selection.activeGameObject).transform;

        //�����ǰ�������Ϸ�����������壬���޲���
        if (tSelect.childCount < 1)
        {
            return;
        }


        //ȷ����ǰ�������Ϸ����������MeshFilter���
        if (!tSelect.GetComponent<MeshFilter>())
        {
            tSelect.gameObject.AddComponent<MeshFilter>();
        }
        //ȷ����ǰ�������Ϸ����������MeshRenderer���
        if (!tSelect.GetComponent<MeshRenderer>())
        {
            tSelect.gameObject.AddComponent<MeshRenderer>();
        }
        //��ȡ�������������MeshFilter���
        MeshFilter[] tFilters = tSelect.GetComponentsInChildren<MeshFilter>();

        //��������MeshFilter����ĸ�������һ������Mesh���ϵ���洢��Ϣ
        CombineInstance[] tCombiners = new CombineInstance[tFilters.Length];

        //���������������������Ϣ���д洢
        for (int i = 0; i < tFilters.Length; i++)
        {
            //��¼����
            tCombiners[i].mesh = tFilters[i].sharedMesh;
            //��¼λ��
            tCombiners[i].transform = tFilters[i].transform.localToWorldMatrix;
        }
        //������һ������������ʾ��Ϻ����Ϸ����
        Mesh tFinalMesh = new Mesh();
        //������Mesh
        tFinalMesh.name = "tCombineMesh";
        //����Unity���÷��������Mesh����
        tFinalMesh.CombineMeshes(tCombiners);
        //��ֵ��Ϻ��Mesh�����ѡ�е�����
        tSelect.GetComponent<MeshFilter>().sharedMesh = tFinalMesh;
        //��ֵ�µĲ���
        tSelect.GetComponent<MeshRenderer>().material = new Material(Shader.Find("VertexLit"));
    }

}
