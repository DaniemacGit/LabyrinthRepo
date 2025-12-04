using UnityEngine;
using UnityEngine.SceneManagement;

// Manages all scene changes
public class SceneChanger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void loadMenuPage()
    {
        SceneManager.LoadScene(0);
    }


    public void loadLevelsPage()
    {
        SceneManager.LoadScene(1);
    }

    public void loadLevelOne()
    {
        SceneManager.LoadScene(2);
    }

    public void loadSettingsPage()
    {
        SceneManager.LoadScene(3);
    }

    public void loadExitPage()
    {
        SceneManager.LoadScene(4);
    }

    public void LoadLevel1CompletePage()
    {
        PlayerPrefs.SetString("SelectedLevel", "Level1");
        PlayerPrefs.Save();
        SceneManager.LoadScene("complete_page");
    }

    public void LoadLevel2CompletePage()
    {
        PlayerPrefs.SetString("SelectedLevel", "Level2");
        PlayerPrefs.Save();
        SceneManager.LoadScene("complete_page");
    }

    public void LoadCompletePageForLevel(string levelSceneName)
    {
        PlayerPrefs.SetString("SelectedLevel", levelSceneName);
        PlayerPrefs.Save();

        SceneManager.LoadScene("complete_page");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
