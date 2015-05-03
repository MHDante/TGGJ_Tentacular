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
	public GameObject RulesText;
	public GameObject Title;
	public GameObject GarbagePanel;
	public GameObject QuitButton;
	public GameObject Xbutton;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnClick() {
        Debug.Log("Click!");
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


