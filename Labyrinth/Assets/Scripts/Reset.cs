using UnityEngine;
using UnityEngine.SceneManagement;

public class Reset : MonoBehaviour
{
    [Header("Scene / Level Info")]
    public string currentLevelSceneName = "Level1";     // set per scene
    public string completePageSceneName = "complete_page";

    private void OnCollisionEnter(Collision collision)
    {
        // Fall / hazard reset
        if (collision.gameObject.layer == 6)
        {
            transform.position = new Vector3(0, 2, 0);
            var rb = GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Goal reached
        if (collision.gameObject.layer == 7)
        {
            float time = Time.timeSinceLevelLoad;
            int stars = CalculateStars(time);

            PlayerPrefs.SetFloat(currentLevelSceneName + "_LastTime", time);
            PlayerPrefs.SetInt(currentLevelSceneName + "_Stars", stars);
            PlayerPrefs.SetString("SelectedLevel", currentLevelSceneName);
            PlayerPrefs.Save();

            SceneManager.LoadScene(completePageSceneName);
        }
    }

    private int CalculateStars(float timeSeconds)
    {
        if (timeSeconds <= 15f) return 3; // 3 stars
        if (timeSeconds <= 20f) return 2; // 2 stars
        return 1;                         // >20s → 1 star
    }
}
