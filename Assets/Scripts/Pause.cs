using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public enum FadeStates
    {
        FadeIn,
        Show,
        FadeOut,
        Wait
    }

    public GameObject continueButton;
    private FadeStates fadeState = FadeStates.FadeOut;
    public GameObject GameOverSign;
    public GameObject HardMode;
    private Text hintText;
    private float MaxFadeTimer = 2;
    private int MaxHintTimer = 4;
    public GameObject MenuPanel;
    public bool MenuShowing = false;
    public GameObject PausedSign;
    public GameObject SoundDown;
    public bool SoundEnabled = true;
    public GameObject SoundUp;
    private float tempHintTimer = 0;
    public GameObject VictorySign;

    public void MenuToggle(string type)
    {
        MenuPanel.SetActive(!MenuShowing);
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

    public void SoundToggle()
    {
        SoundUp.SetActive(!SoundEnabled);
        SoundDown.SetActive(SoundEnabled);
        SoundEnabled = !SoundEnabled;
    }

    public void BackToTitle()
    {
        Application.LoadLevel("TitleScreen");
    }

    public void setHardMode()
    {
        RoomManager.hardMode = HardMode.GetComponent<Toggle>().isOn;
    }

    public void Restart()
    {
        FileWrite.InitDeserialization(RoomManager.instance.levelName + ".xml");
    }

    private void Awake()
    {
        tempHintTimer = (float) MaxHintTimer;
        var obj = GameObject.Find("HintText");
        hintText = obj == null ? null : obj.GetComponent<Text>();
    }

    // Use this for initialization
    private void Start()
    {
        var hmObj = GameObject.Find("HardMode");
        var tog = hmObj.GetComponent<Toggle>();
        tog.isOn = RoomManager.hardMode;
        MenuPanel.SetActive(false);
        PausedSign.SetActive(false);
        GameOverSign.SetActive(false);
        VictorySign.SetActive(false);
        continueButton.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        tempHintTimer -= Time.deltaTime;
        if (tempHintTimer <= 0)
        {
            if (fadeState == FadeStates.FadeIn)
            {
                fadeState = FadeStates.Show;
                hintText.color = Color.black;
                tempHintTimer = MaxHintTimer;
            }
            else if (fadeState == FadeStates.Show)
            {
                fadeState = FadeStates.FadeOut;
                tempHintTimer = MaxFadeTimer;
            }
            else if (fadeState == FadeStates.FadeOut)
            {
                fadeState = FadeStates.Wait;
                hintText.color = new Color(0, 0, 0, 0);
                tempHintTimer = MaxHintTimer;
                //Hints.GetHint();
                hintText.text = Hints.GetHint();
            }
            else if (fadeState == FadeStates.Wait)
            {
                fadeState = FadeStates.FadeIn;
                tempHintTimer = MaxFadeTimer;
            }
        }

        float percent = (float) tempHintTimer/(float) MaxFadeTimer;
        if (fadeState == FadeStates.FadeOut)
        {
            hintText.color = new Color(0, 0, 0, percent);
        }
        else if (fadeState == FadeStates.FadeIn)
        {
            hintText.color = new Color(0, 0, 0, 1 - percent);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuToggle("Pause");
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }
}