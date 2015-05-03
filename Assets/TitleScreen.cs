using UnityEngine;
using System.Collections;

public class TitleScreen : MonoBehaviour
{
    public string firstLevel;
	public GameObject StartButton;
	public GameObject WhatDoButton;
	public GameObject Logo;
	public GameObject Credits;
	public GameObject WhatDoPanel1;
	public GameObject WhatDoPanel2;
	public GameObject WhatDoPanel3;
	public GameObject WhatDoPanel4;
	public GameObject WhatDoPanel5;
	public GameObject RulesText;
	public GameObject Title;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	GameObject lastOne = null;
	public void PanelChange(int ScreenName) {
		var panel = GameObject.Find ("whatdo panel "+ ScreenName);
		if (lastOne != null) {
			lastOne.SetActive (false);
		}
		panel.SetActive (false);
		lastOne = panel;

	}


    public void OnClick() {
        Debug.Log("Click!");
        FileWrite.InitDeserialization(firstLevel + ".xml");
        Hints.Level = 0;
    }
	public void HowTo() {
		Logo.SetActive (false);
		Credits.SetActive (false);
		WhatDoPanel1.SetActive (true);
		WhatDoPanel2.SetActive (true);
		WhatDoPanel3.SetActive (true);
		WhatDoPanel4.SetActive (true);
		WhatDoPanel5.SetActive (true);

	}
	public void ExitRules() {
		StartButton.SetActive (true);
		WhatDoButton.SetActive (true);


	}
}


