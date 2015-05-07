using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pause : MonoBehaviour {
	public bool MenuShowing = false;
	public GameObject SoundUp;
	public GameObject SoundDown;
	public bool SoundEnabled = true;
	public GameObject MenuPanel;

	public GameObject GameOverSign;
	public GameObject VictorySign;
	public GameObject PausedSign;

    public GameObject continueButton;
   
    public GameObject HardMode;
	public void MenuToggle (string type){
		MenuPanel.SetActive (!MenuShowing);
		MenuShowing = !MenuShowing;

        if (type == "Pause")
        {
            PausedSign.SetActive(MenuShowing);
            continueButton.SetActive(MenuShowing);
        }
        else if (type == "Victory")
        {
            VictorySign.SetActive(MenuShowing);
        }
        else if (type == "GameOver")
        {
            GameOverSign.SetActive(MenuShowing);

        }
    }
	public void SoundToggle () {
			SoundUp.SetActive (!SoundEnabled);
			SoundDown.SetActive (SoundEnabled);
			SoundEnabled = !SoundEnabled;
		}
	public void BackToTitle () {
		Application.LoadLevel ("TitleScreen");
	}

    public void setHardMode()
    {
        RoomManager.hardMode = HardMode.GetComponent<Toggle>().isOn;
    }

    public void Restart () {
		FileWrite.InitDeserialization (RoomManager.roomManager.levelName + ".xml");
	}




	// Use this for initialization
	void Start () {
        var hmObj = GameObject.Find("HardMode");
        var tog = hmObj.GetComponent<Toggle>();
        tog.isOn = RoomManager.hardMode;

        MenuPanel.SetActive(false);
		PausedSign.SetActive (false);
		GameOverSign.SetActive (false);
		VictorySign.SetActive (false);
        continueButton.SetActive(false);

    }
	
	// Update is called once per frame
	void Update () {
	
	}

}
