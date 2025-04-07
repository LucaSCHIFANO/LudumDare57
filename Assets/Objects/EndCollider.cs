using System.Collections;
using UnityEngine;

public class EndCollider : MonoBehaviour
{
    [SerializeField] private Animator fade;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Bone")
        {
            StartCoroutine(WaitForFade());
        }
    }

    IEnumerator WaitForFade()
    {
        fade.Play("FadeIn");
        yield return new WaitForSeconds(1.1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("EndScreen");
    }
}
