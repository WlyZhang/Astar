/// <summary>
/// 公司：东方天鹤
/// 张王磊玉（编写）
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using tcom.tools;

public class AstarCenter : MonoBehaviour
{
    private static AstarCenter _instance;
    public static AstarCenter GetInstance()
    {
        return _instance;
    }

    private AStart star;
    private Transform nodeParent;

    void Awake()
    {
        _instance = this;

        GameObject go = new GameObject("NodeList");
        nodeParent = go.transform;
        nodeParent.position = Vector3.zero;
    }
    
    /// <summary>
    /// 初始化Astar
    /// </summary>
    public void Init(AStart star, float yHeight=0f,bool isTest=false)
    {
        this.star = star;
        for (int i = 0; i < star.indexs.Length; i++)
        {
            int _x = i % star.width;
            int _z = i / star.height;

            int centerZ = (i % star.width) - star.width / 2;
            int centerX = (i / star.height) - star.height / 2;

            Vector3 pos = new Vector3(centerX, yHeight, centerZ);

            //Debug.Log(pos);

            bool isWalk = Physics.CheckSphere(pos, 1f, LayerMask.GetMask("Astar"));
            if (isWalk)
            {
                star.SetBlock(_z, _x, 1);
            }
        }

        if (isTest)
        {
            for (var i = 0; i < star.indexs.Length; i++)
            {
                int _x = i % star.width;
                int _z = i / star.height;

                int centerZ = (i % star.width) - star.width / 2;
                int centerX = (i / star.height) - star.height / 2;

                if (star.indexs[i] == 0)
                {
                    GameObject b = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    b.transform.parent = nodeParent;
                    MeshRenderer render = b.GetComponent<MeshRenderer>();
                    render.material.color = Color.red;
                    b.transform.position = new Vector3(centerX, yHeight, centerZ);
                }
                else
                {
                    GameObject b = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    b.transform.parent = nodeParent;
                    MeshRenderer render = b.GetComponent<MeshRenderer>();
                    render.material.color = Color.white;
                    b.transform.position = new Vector3(centerX, yHeight, centerZ);
                }
            }
        }
    }

    /// <summary>
    /// 获取路点列表
    /// </summary>
    /// <param name="startPos">开始点</param>
    /// <param name="endPos">终点</param>
    /// <param name="yHeight">Y轴高度</param>
    /// <param name="isTest">是否测试</param>
    /// <returns></returns>
    public List<Vector3> GetNodeList(Vector3 startPos, Vector3 endPos,float yHeight = 0f, bool isTest = false)
    {

        Vector3 _star = new Vector3(startPos.x + star.width / 2, yHeight, startPos.z + star.height / 2);
        Vector3 _end = new Vector3(endPos.x + star.width / 2, yHeight, endPos.z + star.height / 2);

        List<Node> list = star.FindPath(_star, _end);

        List<Vector3> nodeList = new List<Vector3>();

        for (int i = 0; i < list.Count; i++)
        {

            Vector3 pos = new Vector3(list[i].X / 10 - star.width / 2, yHeight, list[i].Y / 10 - star.height / 2);
            nodeList.Add(pos);

            if(isTest)
            {
                //测试
                GameObject b = GameObject.CreatePrimitive(PrimitiveType.Cube);
                b.transform.parent = nodeParent;
                MeshRenderer render = b.GetComponent<MeshRenderer>();
                render.material.color = Color.blue;
                b.transform.position = pos;
            }
        }

        return nodeList;
    }
}
