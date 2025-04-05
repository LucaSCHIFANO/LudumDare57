using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CollapsedManager collapsedManager;
    private BoneHandler[] bones;
    [SerializeField] Transform bonesParent;
    Rigidbody2D rb;

    public UnityEvent SwitchToTRexForme;
    public UnityEvent SwitchToCollapsedForme;

    enum State
    {
        CanMove,
        CannotMove
    }

    private void Start()
    {
        bones = FindObjectsByType<BoneHandler>(FindObjectsSortMode.None);
        rb = GetComponentInChildren<Rigidbody2D>();
        Construct();
    }

    public void ToRexFormeInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (collapsedManager.numberOfActiveBones < bones.Length)
                return;

            Construct();
        }
    }
    
    public void ToCollapsedFormeInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Collapse();
        }
    }

    private void SetBonesActive(bool value)
    {
        foreach (var bone in bones)
        {
            if (bone.gameObject == transform.GetChild(0).gameObject)
                continue;
            bone.gameObject.SetActive(value);
        }
    }

    public void Collapse()
    {
        bonesParent.SetParent(transform);
        SwitchToCollapsedForme.Invoke();
        SetBonesActive(true);
        rb.constraints = RigidbodyConstraints2D.None;
    }

    public void Construct()
    {
        SwitchToTRexForme.Invoke();
        SetBonesActive(false);
        bonesParent.SetParent(transform.GetChild(0));
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

    }
}
