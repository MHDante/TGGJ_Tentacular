using UnityEngine;
using System.Collections;

public class TitleScreen : MonoBehaviour
{
    public string firstLevel;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnClick() {
        Debug.Log("Click!");
        FileWrite.InitDeserialization(firstLevel + ".xml");
    }
}
