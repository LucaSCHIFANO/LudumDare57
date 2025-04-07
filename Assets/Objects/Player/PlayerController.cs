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
    [SerializeField] private BoneHandler headBone;
    [SerializeField] private Transform headVisual;
    [SerializeField] private float playerHeight = 1;
    [SerializeField] LayerMask ground;
    Rigidbody2D rb;
    private State state = State.CanMove;

    [SerializeField] private bool startAsTrex = true;

    public UnityEvent SwitchToTRexForme;
    public UnityEvent SwitchToCollapsedForme;

    [Header("Sound")]
    [SerializeField] private SOSound deconstructSound;
    [SerializeField] private SOSound reconstructSound;
    public TRexController TRex { get => tRex; }

    public enum State
    {
        CanMove,
        CannotMove
    }

    public void PlaySound(SOSound sound)
    {
        if (sound == null) return;
        SoundManager.Instance.Play(sound);
    }

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody2D>();
        tRex = FindFirstObjectByType<TRexController>();
    }

    private void Start()
    {
        bones = FindObjectsByType<BoneHandler>(FindObjectsSortMode.None);
        if (startAsTrex)
        {
            foreach (var bone in bones)
            {
                if (bone.gameObject == headBone.gameObject)
                    continue;
                bone.gameObject.SetActive(false);
            }
            headVisual.gameObject.SetActive(false);
        }
        else
        {
            SwitchToCollapsedForme.Invoke();
        }
    }

    public void ToRexFormeInput(InputAction.CallbackContext context)
    {
        if (state != PlayerController.State.CanMove) return;

        if (context.performed)
        {
            if (collapsedManager.numberOfActiveBones < bones.Length)
                return;          
            
            var wall = Physics2D.RaycastAll(headVisual.transform.position, Vector2.up, 1f, ground);
            
            if (wall.Length > 0) return;

            Construct();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(headVisual.transform.position, Vector2.up * 1f);
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
            var checkpoint = CheckPointManager.Instance.GetRestartPoint();
            if(checkpoint == Vector3.zero)
            {
                Debug.Log("No checkpoint set");
                return;
            }
            CameraMovement.Instance.Restart();
            ResetToCheckpoint(checkpoint);
        }
    }

    private void SetBonesActive(bool value, bool isLeft)
    {
        foreach (var bone in bones)
        {
            if (bone.gameObject == headBone.gameObject)
                continue;
            bone.gameObject.SetActive(value);
            bone.Sr.flipX = isLeft;
        }
        headVisual.gameObject.SetActive(value);
        headBone.Sr.flipX = isLeft;

    }

    public void Collapse()
    {
        SwitchToCollapsedForme.Invoke();
        foreach (var bone in bones)
        {
            bone.ResetBone();
        }
        SetBonesActive(true, TRex.RotatingPart.transform.localScale.x < 0);
        rb.constraints = RigidbodyConstraints2D.None;
        PlaySound(deconstructSound);
    }

    public void Construct()
    {
        PlaySound(reconstructSound);

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
                yield return new WaitForSeconds(.5f);
            SetBonesActive(false, TRex.RotatingPart.transform.localScale.x < 0);

            SwitchToTRexForme.Invoke();
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            headBone.SimulateRigidbody(true);
        }
    }
    public void ChangeState(State newState, Transition.Direction dir)
    {
        state = newState;

        tRex.ChangeState(newState);
        tRex.Move(dir);

        foreach (var bone in bones)
        {
            if (!bone.isActiveAndEnabled)
                continue;
            bone.ChangeState(newState);
            bone.Move(dir);
        }
    }

    public void ResetToCheckpoint(Vector3 checkpointPosition)
    {
        ChangeState(PlayerController.State.CanMove, Transition.Direction.NONE);

        TRex.Restart(checkpointPosition);

        foreach (var bone in bones)
        {
            bone.enabled = true;
            bone.TeleportToCheckpoint(checkpointPosition);
        }
    }
}
