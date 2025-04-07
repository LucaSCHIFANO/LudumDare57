using UnityEngine;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private Animator fade;

    private void Start()
    {
        fade.Play("FadeOut");
    }
    public void BackToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
