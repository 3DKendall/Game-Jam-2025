using UnityEngine;
using UnityEngine.SceneManagement;

public class Hatchet : MonoBehaviour
{
    public void ExitBunker()
    {
        SceneManager.LoadScene(2);
    }
}
