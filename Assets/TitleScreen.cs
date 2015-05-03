using UnityEngine;
using System.Collections;

public class TitleScreen : MonoBehaviour
{
    public string firstLevel;
	public GameObject StartButton;
	public GameObject WhatDoButton;
	public GameObject Logo;
	public GameObject Credits;
	public GameObject WhatDoPanel;
	public GameObject Xbutton;
	public GameObject RulesText;


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
	public void HowTo() {
		StartButton.SetActive (false);
		WhatDoButton.SetActive (false);
		Logo.SetActive (false);
		Credits.SetActive (false);
		WhatDoPanel.SetActive (true);

	}
	public void ExitRules() {
		WhatDoPanel.SetActive (false);
		StartButton.SetActive (true);
		WhatDoButton.SetActive (true);
		Logo.SetActive (true);
		Credits.SetActive (true);

	}
}
