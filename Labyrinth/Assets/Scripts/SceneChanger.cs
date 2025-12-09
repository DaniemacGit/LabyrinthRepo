using UnityEngine;
using UnityEngine.SceneManagement;

// Manages all scene changes
public class SceneChanger : MonoBehaviour
{
    public void loadMenuPage()
    {
        SceneManager.LoadScene(0);
    }

    public void loadSettingsPage()
    {
        SceneManager.LoadScene(1);
    }

    public void loadLevelsPage()
    {
        SceneManager.LoadScene(2);
    }

    public void loadLevelOne()
    {
        SceneManager.LoadScene(3);
    }

    public void loadLevelTwo()
    {
        SceneManager.LoadScene(4);
    }

    public void loadLevelThree()
    {
        SceneManager.LoadScene(5);
    }

    public void loadLevelFour()
    {
        SceneManager.LoadScene(6);
    }

    public void loadLevelFive()
    {
        SceneManager.LoadScene(7);
    }

    public void LoadLevelOneCompletePage()
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
}
