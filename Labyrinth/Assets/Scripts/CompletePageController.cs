using UnityEngine;
using UnityEngine.SceneManagement;

public class CompletePageController : MonoBehaviour
{
    [Header("Star GameObjects")]
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;

    [Header("Scene Names")]
    public string levelsPageSceneName = "levelsPage";

    private string currentLevelName;

    void Start()
    {
        // Which level is this page about?
        currentLevelName = PlayerPrefs.GetString("SelectedLevel", "");

        // Default: no stars
        int stars = 0;

        if (!string.IsNullOrEmpty(currentLevelName))
        {
            // Will be 0 if the level has never been completed
            stars = PlayerPrefs.GetInt(currentLevelName + "_Stars", 0);
        }

        UpdateStars(stars);
    }

    // Turn stars on/off
    void UpdateStars(int stars)
    {
        // Hide all first
        star1.SetActive(false);
        star2.SetActive(false);
        star3.SetActive(false);

        if (stars >= 1) star1.SetActive(true);
        if (stars >= 2) star2.SetActive(true);
        if (stars >= 3) star3.SetActive(true);
    }

    // Back button → Levels page
    public void OnBackPressed()
    {
        SceneManager.LoadScene(levelsPageSceneName);
    }

    // Play button → the actual level scene
    public void OnPlayPressed()
    {
        if (!string.IsNullOrEmpty(currentLevelName))
        {
            SceneManager.LoadScene(currentLevelName);
        }
    }
}
