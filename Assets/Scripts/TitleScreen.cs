using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    public string firstLevel;
    public GameObject HardMode;
    public GameObject HelpWindow;
    private void Start()
    {
        HardMode.GetComponent<Toggle>().isOn = RoomManager.hardMode;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnClick();
        }
    }

    public void SetHardMode()
    {
        RoomManager.hardMode = HardMode.GetComponent<Toggle>().isOn;
    }

    public void OnClick()
    {
        FileWrite.InitDeserialization(firstLevel + ".xml");
        Hints.Level = 0;
    }

    public void HelpPanel(bool show)
    {
        HelpWindow.SetActive(show);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}