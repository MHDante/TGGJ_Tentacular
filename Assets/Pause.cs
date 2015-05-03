using UnityEngine;
using System.Collections;

public class Pause : MonoBehaviour {
	public bool MenuShowing = false;
	public GameObject SoundUp;
	public GameObject SoundDown;
	public bool SoundEnabled = true;
	public GameObject MenuPanel;

	public GameObject GameOverSign;
	public GameObject VictorySign;
	public GameObject PausedSign;


	public void MenuToggle (string type){
		MenuPanel.SetActive (!MenuShowing);
		MenuShowing = !MenuShowing;
	}
	public void SoundToggle () {
			SoundUp.SetActive (!SoundEnabled);
			SoundDown.SetActive (SoundEnabled);
			SoundEnabled = !SoundEnabled;
		}
	public void BackToTitle () {
		Application.LoadLevel ("TitleScreen");
	}

	public void Restart () {
		FileWrite.InitDeserialization (RoomManager.roomManager.levelName + ".xml");
	}




	// Use this for initialization
	void Start () {
		MenuPanel.SetActive(false);
		PausedSign.SetActive (false);
		GameOverSign.SetActive (false);
		VictorySign.SetActive (false);


	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
