using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Manages all scene changes
public class SceneChanger : MonoBehaviour
{
    public static float sens = 0;
    public static float volume = 0.5f;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Settings")
        {
            GameObject.Find("Sensitivity Slider").GetComponent<Slider>().value = sens;
            GameObject.Find("Sound Slider").GetComponent<Slider>().value = volume;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitializeSettings()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            volume = PlayerPrefs.GetFloat("Volume");
        }
        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            sens = PlayerPrefs.GetFloat("Sensitivity");
        }
    }

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
    
    public void sensChanger(float val)
    {
        sens = val;
        PlayerPrefs.SetFloat("Sensitivity", sens);
        PlayerPrefs.Save();
    }

    public void volumeChanger(float val)
    {
        volume = val;
        GameObject.Find("Sound").GetComponent<AudioSource>().volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.Save();
    }
}
