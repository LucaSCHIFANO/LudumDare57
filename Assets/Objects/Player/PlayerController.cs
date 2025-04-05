using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CollapsedManager collapsedManager;
    private BoneHandler[] bones;
    private TRexController tRex;
    [SerializeField] Transform bonesParent;
    Rigidbody2D rb;
    private State state = State.CanMove;

    public UnityEvent SwitchToTRexForme;
    public UnityEvent SwitchToCollapsedForme;

    public TRexController TRex { get => tRex; }

    public enum State
    {
        CanMove,
        CannotMove
    }

    private void Start()
    {
        bones = FindObjectsByType<BoneHandler>(FindObjectsSortMode.None);
        tRex = FindFirstObjectByType<TRexController>();
        rb = GetComponentInChildren<Rigidbody2D>();
        Construct();
    }

    public void ToRexFormeInput(InputAction.CallbackContext context)
    {
        if (state != PlayerController.State.CanMove) return;

        if (context.performed)
        {
            if (collapsedManager.numberOfActiveBones < bones.Length)
                return;

            Construct();
        }
    }
    
    public void ToCollapsedFormeInput(InputAction.CallbackContext context)
    {
        if (state != PlayerController.State.CanMove) return;

        if (context.performed)
        {
            Collapse();
        }
    }

    public void RestartInput(InputAction.CallbackContext context)
    {
        if (state != PlayerController.State.CanMove) return;

        if (context.performed)
        {
            CameraMovement.Instance.Restart();
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

    public void ChangeState(State newState, Transition.Direction dir)
    {
        state = newState;
        tRex.ChangeState(newState);
        tRex.Move(dir);

        foreach (var bone in bones)
        {
            bone.ChangeState(newState);
            bone.Move(dir);
        }
    }
}
