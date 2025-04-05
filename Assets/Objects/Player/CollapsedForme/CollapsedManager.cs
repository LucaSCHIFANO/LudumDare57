using UnityEngine;
using UnityEngine.InputSystem;

public class CollapsedManager : MonoBehaviour
{
    [SerializeField] private Collider2D triggerArea;
    public int numberOfActiveBones = 1;

    private void OnEnable()
    {
        triggerArea.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bone"))
        {
            collision.GetComponent<BoneHandler>().enabled = true;
            numberOfActiveBones++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Bone"))
        {
            collision.GetComponent<BoneHandler>().enabled = false;
            numberOfActiveBones--;
        }
    }
}
