using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour {

    // Use this for initialization
    private Vector3 TopLeft_Pl_W;
    private Vector3 BottomLeft_Pl_W;
    private Vector3 TopRight_Pl_W;
    private Vector3 BottomRight_Pl_W;

    public GameObject Card_Track;
    private Vector3 Center_Card;

    private float Half_W;
    private float Half_H;


    public GameObject Earth;
    public GameObject Frame;//地球外框模型

    private Vector2 TopLeft_Pl_Sc;
    private Vector2 BottomLeft_Pl_Sc;
    private Vector2 TopRight_Pl_Sc;
    private Vector2 BottomRight_Pl_Sc;

    private Vector2 TopLeft_Scan;
    private Vector2 BottomLeft_Scan;
    private Vector2 TopRight_Scan;
    private Vector2 BottomRight_Scan;

    private float X_Sc;//屏幕自适度系数

    public GameObject Plane;
    public Texture Te_Red;
    public Texture Te_Green;

    public GameObject Suc;
    public Texture Te_Tran;

    public bool B_Re = false;
    public bool B_Shot = false;
    void Start () {
        X_Sc = Screen.width / 1920f;

        Center_Card = Card_Track.transform.position;
        Half_W = Card_Track.GetComponent<MeshFilter>().mesh.bounds.size.x * 10 * 0.5f;
        Half_H = Card_Track.GetComponent<MeshFilter>().mesh.bounds.size.z * 10 * 0.5f;
        TopLeft_Pl_W = Center_Card + new Vector3(-Half_W, 0, Half_H);
        BottomLeft_Pl_W = Center_Card + new Vector3(-Half_W, 0, -Half_H);
        TopRight_Pl_W = Center_Card + new Vector3(Half_W, 0, Half_H);
        BottomRight_Pl_W = Center_Card + new Vector3(Half_W, 0, -Half_H);

        TopLeft_Scan = new Vector2(Screen.width - 1300, Screen.height + 800*X_Sc) * 0.5f;
        BottomLeft_Scan = new Vector2(Screen.width - 1300, Screen.height - 800 * X_Sc) * 0.5f;
        TopRight_Scan = new Vector2(Screen.width + 1300, Screen.height + 800 * X_Sc) * 0.5f;
        BottomRight_Scan = new Vector2(Screen.width + 1300, Screen.height - 800 * X_Sc) * 0.5f;
    }
    private void Update()
    {
        if (B_Shot == false)
        {
            TopLeft_Pl_Sc = Camera.main.WorldToScreenPoint(TopLeft_Pl_W);
            BottomLeft_Pl_Sc = Camera.main.WorldToScreenPoint(BottomLeft_Pl_W);
            TopRight_Pl_Sc = Camera.main.WorldToScreenPoint(TopRight_Pl_W);
            BottomRight_Pl_Sc = Camera.main.WorldToScreenPoint(BottomRight_Pl_W);

            if (TopLeft_Pl_Sc.x > TopLeft_Scan.x && TopLeft_Pl_Sc.y < TopLeft_Scan.y && BottomLeft_Pl_Sc.x > BottomLeft_Scan.x && BottomLeft_Pl_Sc.y > BottomLeft_Scan.y
                && TopRight_Pl_Sc.x < TopRight_Scan.x && TopRight_Pl_Sc.y < TopLeft_Scan.y && BottomRight_Pl_Sc.x < BottomRight_Scan.x && BottomRight_Pl_Sc.y > BottomRight_Scan.y)
            {
                if(B_Re==false)
                {
                    Plane.GetComponent<Renderer>().material.mainTexture = Te_Green;
                    StartCoroutine(Cancol_Occ());
                    Suc.SetActive(true);
                    B_Re = true;
                }
                
            }
            else
            {
                Plane.GetComponent<Renderer>().material.mainTexture = Te_Red;
                Suc.SetActive(false);
                B_Re = false;
            }
        }
        
    }

    public void Get_Position()
    {
        //将截图时识别图四个角的世界坐标信息传递给地球仪模型的Shader
        Earth.GetComponent<Renderer>().material.SetVector("_Uvpoint1", new Vector4(TopLeft_Pl_W.x, TopLeft_Pl_W.y, TopLeft_Pl_W.z, 1f));
        Earth.GetComponent<Renderer>().material.SetVector("_Uvpoint2", new Vector4(BottomLeft_Pl_W.x, BottomLeft_Pl_W.y, BottomLeft_Pl_W.z, 1f));
        Earth.GetComponent<Renderer>().material.SetVector("_Uvpoint3", new Vector4(TopRight_Pl_W.x, TopRight_Pl_W.y, TopRight_Pl_W.z, 1f));
        Earth.GetComponent<Renderer>().material.SetVector("_Uvpoint4", new Vector4(BottomRight_Pl_W.x, BottomRight_Pl_W.y, BottomRight_Pl_W.z, 1f));

        //将截图时识别图四个角的世界坐标信息传递给地球仪配件模型的Shader
        Frame.GetComponent<Renderer>().material.SetVector("_Uvpoint1", new Vector4(TopLeft_Pl_W.x, TopLeft_Pl_W.y, TopLeft_Pl_W.z, 1f));
        Frame.GetComponent<Renderer>().material.SetVector("_Uvpoint2", new Vector4(BottomLeft_Pl_W.x, BottomLeft_Pl_W.y, BottomLeft_Pl_W.z, 1f));
        Frame.GetComponent<Renderer>().material.SetVector("_Uvpoint3", new Vector4(TopRight_Pl_W.x, TopRight_Pl_W.y, TopRight_Pl_W.z, 1f));
        Frame.GetComponent<Renderer>().material.SetVector("_Uvpoint4", new Vector4(BottomRight_Pl_W.x, BottomRight_Pl_W.y, BottomRight_Pl_W.z, 1f));

        //确定坐标间的矩阵关系 并将信息传递给对应模型shader
        Matrix4x4 P = GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, false);
        Matrix4x4 V = Camera.main.worldToCameraMatrix;
        Matrix4x4 VP = P * V;
        Earth.GetComponent<Renderer>().material.SetMatrix("_VP", VP);
        Frame.GetComponent<Renderer>().material.SetMatrix("_VP", VP);
    }
    IEnumerator ScreenShot()
    {
        yield return new WaitForEndOfFrame();
        if (B_Re == true)
        {
            Texture2D Te = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            Te.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
            Te.Apply();
            Earth.GetComponent<Renderer>().material.mainTexture = Te;
            Frame.GetComponent<Renderer>().material.mainTexture = Te;
            B_Shot = true;
            Plane.GetComponent<Renderer>().material.mainTexture = Te_Tran;
        }
        
    }

    IEnumerator Cancol_Occ()
    {
        yield return new WaitForSeconds(2.0f);
        Suc.SetActive(false);Plane.GetComponent<Renderer>().material.mainTexture = Te_Tran;

        StartCoroutine(ScreenShot());
        Get_Position();
    }
}
