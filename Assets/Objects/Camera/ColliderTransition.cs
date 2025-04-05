using UnityEditor.AssetImporters;
using UnityEditorInternal;
using UnityEngine;

public class ColliderTransition : MonoBehaviour
{
    private BoxCollider2D bc;
    private Transition transition;

    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
    }

    public void Init(bool isTransition, Vector2 size, Transition _transition)
    {
        bc.size = size;
        transition = _transition;

        if (!isTransition) return;
        bc.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            CameraMovement.Instance.Transition(transition);
        }
    }
}
