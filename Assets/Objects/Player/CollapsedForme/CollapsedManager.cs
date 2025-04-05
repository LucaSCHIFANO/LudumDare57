using UnityEngine;
using UnityEngine.InputSystem;

public class CollapsedManager : MonoBehaviour
{
    [SerializeField] private Collider2D triggerArea;
    int maxBones = 0;

    private void Start()
    {
        var bones = FindObjectsByType<BoneHandler>(FindObjectsSortMode.None);
        maxBones = bones.Length - 1; //minus self
    }

    private void OnEnable()
    {
        triggerArea.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bone"))
        {
            collision.GetComponent<BoneHandler>().enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Bone"))
        {
            collision.GetComponent<BoneHandler>().enabled = false;
        }
    }
}
