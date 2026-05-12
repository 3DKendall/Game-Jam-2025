using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEditor;

public class GameMenu : MonoBehaviour
{
    public bool opened;
    public enum MenuMode { Main, Pause }
    public MenuMode menuMode;

    [Header("General")]
    public Transform UI;
    public GameObject newGameButton;
    [Space]
    [Space]
    public GameObject settingsMenu;

    [Header("Main Menu")]
    public GameObject mainBackground;

    [Header("Pause Menu")]
    public GameObject pauseBackground;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (menuMode == MenuMode.Main)
        {
            UI.transform.localPosition = new Vector3(0, 0, 0);
            Time.timeScale = 1;
        }

        else if (menuMode == MenuMode.Pause)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                opened = !opened;

            if (opened)
            { 
                UI.transform.localPosition = new Vector3(0, 0, 0);
                Time.timeScale = 0;
            }
        else
            {
                UI.transform.localPosition = new Vector3(-10000, 0, 0);
                Time.timeScale = 1;
            }
        }
    }

    public void NewGame()
    {
        if(mainBackground != null)
            mainBackground.gameObject.SetActive(false);

        if(pauseBackground != null)
            pauseBackground.gameObject.SetActive(true);

        newGameButton.gameObject.SetActive(false);

        menuMode = MenuMode.Pause;

        SaveHandler.load = false;

        SceneManager.LoadScene(1);
    }

    public void LoadGame()
    {
        // CHECK THAT DIRECTORY & FILE HAVE BEEN CREATED
        if (!Directory.Exists(Application.dataPath + "/saves"))
        {
            return;
        }

        if (!File.Exists(Application.dataPath + "/saves/GameSave.save"))
        {
            return;
        }

        SaveHandler.load = true;

        menuMode = MenuMode.Pause;

        if (mainBackground != null)
            mainBackground.gameObject.SetActive(false);

        if (pauseBackground != null)
            pauseBackground.gameObject.SetActive(true);

        newGameButton.gameObject.SetActive(false);

        menuMode = MenuMode.Pause;

        SceneManager.LoadScene(1);
    }

    public void BackToMain()
    {
        menuMode = MenuMode.Main;

        SceneManager.LoadScene(0);

        Destroy(gameObject);
    }

    public void SaveGame()
    {
        FindObjectOfType<SaveHandler>().Save();
    }

    public void Settings()
    {
        settingsMenu.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

}
