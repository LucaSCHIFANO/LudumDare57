using UnityEngine;

public class EndScreen : MonoBehaviour
{
    public void BackToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
