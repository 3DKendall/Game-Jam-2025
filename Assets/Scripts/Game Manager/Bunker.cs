using UnityEngine;
using UnityEngine.SceneManagement;

public class Bunker : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            SceneManager.LoadScene(2);
        }
    }
}
