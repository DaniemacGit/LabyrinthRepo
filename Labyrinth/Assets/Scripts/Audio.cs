using UnityEngine;
using UnityEngine.Rendering;

public class Audio : MonoBehaviour
{

    private static Audio instance;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        GetComponent<AudioSource>().volume = SceneChanger.volume;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }
}
