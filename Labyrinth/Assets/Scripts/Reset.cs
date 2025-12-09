using UnityEngine;
using UnityEngine.SceneManagement;

public class Reset : MonoBehaviour
{
    Vector3 startPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // reset board and ball on fall
        if (collision.gameObject.layer == 6)
        {
            transform.position = startPos;
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            GameObject.Find("Grid").GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(new Vector3(0, 0, 0)));
        }

        // move back to levels screen after level completion
        if (collision.gameObject.layer == 7)
        {
            if(SceneManager.GetActiveScene().name == "Level1" && LevelCompletion.level < 2)
            {
                LevelCompletion.level = 2;
            }
            if (SceneManager.GetActiveScene().name == "Level2" && LevelCompletion.level < 3)
            {
                LevelCompletion.level = 3;
            }
            if (SceneManager.GetActiveScene().name == "Level3" && LevelCompletion.level < 4)
            {
                LevelCompletion.level = 4;
            }
            if (SceneManager.GetActiveScene().name == "Level4" && LevelCompletion.level < 5)
            {
                LevelCompletion.level = 5;
            }
            SceneManager.LoadScene(2);
        }
    }
}
