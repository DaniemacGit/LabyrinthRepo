using UnityEngine;
using UnityEngine.UI;

public class LevelCompletion : MonoBehaviour
{
    public static int level = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (level >= 1)
        {
            GameObject.Find("lvl1").GetComponent<Image>().enabled = true;
            GameObject.Find("lvl1").GetComponent<Button>().enabled = true;
        }
        if (level >= 2)
        {
            GameObject.Find("lvl2").GetComponent<Image>().enabled = true;
            GameObject.Find("lvl2").GetComponent<Button>().enabled = true;
        }
        if (level >= 3)
        {
            GameObject.Find("lvl3").GetComponent<Image>().enabled = true;
            GameObject.Find("lvl3").GetComponent<Button>().enabled = true;
        }
        if (level >= 4)
        {
            GameObject.Find("lvl4").GetComponent<Image>().enabled = true;
            GameObject.Find("lvl4").GetComponent<Button>().enabled = true;
        }
        if (level == 5)
        {
            GameObject.Find("lvl5").GetComponent<Image>().enabled = true;
            GameObject.Find("lvl5").GetComponent<Button>().enabled = true;
        }
    }
}
