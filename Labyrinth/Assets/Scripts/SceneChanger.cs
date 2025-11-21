using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void loadLevelsPage()
    {
        SceneManager.LoadScene(1);
    }

    public void loadLevelOne()
    {
        SceneManager.LoadScene(2);
    }

    public void loadExitPage()
    {
        SceneManager.LoadScene(3);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
