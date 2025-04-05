using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CollapsedManager collapsedManager;
    private BoneHandler[] bones;
    private TRexController tRex;
    [SerializeField] Transform bonesParent;
    [SerializeField] private BoneHandler headBone;
    [SerializeField] private float playerHeight = 1;
    [SerializeField] LayerMask ground;
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
        foreach(var bone in bones)
        {
            if (bone.gameObject == headBone.gameObject)
                continue;
            bone.gameObject.SetActive(false);
        }
        tRex = FindFirstObjectByType<TRexController>();
        rb = GetComponentInChildren<Rigidbody2D>();
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
            if (bone.gameObject == headBone.gameObject)
                continue;
            bone.gameObject.SetActive(value);
        }
        collapsedManager.gameObject.SetActive(value);

    }

    public void Collapse()
    {
        SwitchToCollapsedForme.Invoke();
        foreach (var bone in bones)
        {
            bone.ResetBone();
        }
        SetBonesActive(true);
        rb.constraints = RigidbodyConstraints2D.None;
    }

    public void Construct()
    {        
        StartCoroutine(ConstructAnim());
        
        IEnumerator ConstructAnim()
        {
            headBone.SimulateRigidbody(false);
            var hit = Physics2D.Raycast(headBone.transform.position, Vector2.down, playerHeight, ground);
            if (hit.collider != null)
            {
                headBone.transform.DOMoveY(hit.point.y + playerHeight, .5f);
            }
            headBone.transform.DORotate(Vector3.zero, 1f);
            yield return new WaitForSeconds(.5f);

            foreach (var bone in bones)
            {
                if (bone.gameObject == headBone.gameObject)
                    continue;
                bone.MoveToStartPosition();
                yield return new WaitForSeconds(.1f);
            }
            SetBonesActive(false);

            SwitchToTRexForme.Invoke();
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            headBone.SimulateRigidbody(true);
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
