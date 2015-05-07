using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    public string firstLevel;
	public GameObject StartButton;
	public GameObject WhatDoButton;
	public GameObject Logo;
	public GameObject Credits;
	public GameObject WhatDoPanel1;
	public GameObject RulesText;
	public GameObject Title;
	public GameObject GarbagePanel;
	public GameObject QuitButton;
	public GameObject Xbutton;

    public GameObject HardMode;
	// Use this for initialization
	void Start () {
        HardMode.GetComponent<Toggle>().isOn= RoomManager.hardMode;

        //FileWrite.WriteFile("rummy123.txt", "haha");
    }
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.Return))
        {
            OnClick();
        }
	}

    public void setHardMode()
    {
        RoomManager.hardMode = HardMode.GetComponent<Toggle>().isOn;
    }

    public void OnClick() {
        //Debug.Log("Click!");
        FileWrite.InitDeserialization(firstLevel + ".xml");
        Hints.Level = 0;
    }
	public void HowTo() {
		WhatDoPanel1.SetActive (true);
		Logo.SetActive (false);
		Credits.SetActive (false);
		GarbagePanel.SetActive (false);
		QuitButton.SetActive (false);
	}
	public void ExitRules() {
		WhatDoPanel1.SetActive (false);
		Logo.SetActive (true);
		Credits.SetActive (true);
		GarbagePanel.SetActive (true);
		QuitButton.SetActive (true);

	}
	public void EndAll() {
		Application.Quit();
	}

}


