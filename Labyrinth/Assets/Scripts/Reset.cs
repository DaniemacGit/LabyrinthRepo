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

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6)
        {
            transform.position = startPos;
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            GameObject.Find("Grid").GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(new Vector3(0, 0, 0)));
        }

        if (collision.gameObject.layer == 7)
        {
            SceneManager.LoadScene(0);
        }
    }
}
