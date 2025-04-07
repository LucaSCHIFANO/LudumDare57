using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CollapsedManager : MonoBehaviour
{
    public int numberOfActiveBones = 1;
    [SerializeField] private TextMeshProUGUI bonesAmountText;

    private void Start()
    {
        UpdateText();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bone"))
        {
            collision.GetComponent<BoneHandler>().enabled = true;
            numberOfActiveBones++;
            UpdateText();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Bone"))
        {
            collision.GetComponent<BoneHandler>().enabled = false;
            numberOfActiveBones--;
            UpdateText();
        }
    }

    private void UpdateText()
    {
        bonesAmountText.text = numberOfActiveBones + " / 8";
            }
}
