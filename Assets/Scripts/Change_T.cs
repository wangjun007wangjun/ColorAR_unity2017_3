using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Change_T : MonoBehaviour {

    // Use this for initialization
    public GameObject Earth;
    public Texture Card_01;

    public Material Mat_Model;

	void Start () {
        
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Button_T()
    {
        //Earth.GetComponent<Renderer>().material.mainTexture = Card_01;
        StartCoroutine(GetScren());
    }

     IEnumerator GetScren()
    {
        yield return new WaitForEndOfFrame();
        Texture2D Te = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        Te.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
        Te.Apply();
        Earth.GetComponent<Renderer>().material.mainTexture = Te;
    }
}
