using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoneHandler : MonoBehaviour
{
    private Rigidbody2D rb;
    private float direction = 0f;
    [SerializeField] private CollapsedValuesScriptable collapsedValues;
    private float jumpTime = 0;
    [SerializeField] private LayerMask ground;
    [SerializeField] private Transform referenceTransform;
    private PlayerController.State state = PlayerController.State.CanMove;
    private CollapsedManager collapsedManager;
    [SerializeField] private SOSound jumpSound;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collapsedManager = FindFirstObjectByType<CollapsedManager>();
        //ResetBone();
    }

    public void PlaySound(SOSound sound)
    {
        if (sound == null) return;
        SoundManager.Instance.Play(sound);
    }

    private void FixedUpdate()
    {
        if (state != PlayerController.State.CanMove)
        {
            rb.AddForceX(direction * collapsedValues.transitionStrength);
            rb.AddTorque(direction * collapsedValues.torqueStrength * -1);
        }
        else if (Mathf.Abs(direction) > .2f){
            rb.AddForceX(direction * collapsedValues.strength);
            rb.AddTorque(direction * collapsedValues.torqueStrength * -1);
            Vector3 attraction = collapsedManager.transform.position - transform.position;
            rb.AddForce(attraction * collapsedValues.attractionMultiplier);
            Debug.DrawLine(transform.position, transform.position + attraction);
        }
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position,
            new Vector3(2, 1, 0), 0f,
            Vector2.down, 2, ground);

        if (raycastHit.collider != null)
        {
            return true;
        }

        return false;
    }

    public void MovementInput(InputAction.CallbackContext context)
    {
        if (state != PlayerController.State.CanMove) return;
        direction = context.ReadValue<float>();
    }
    
    public void JumpInput(InputAction.CallbackContext context)
    {
        if (state != PlayerController.State.CanMove) return;
        if (context.performed && this.isActiveAndEnabled && (Time.fixedTime - jumpTime) >= collapsedValues.jumpCooldown && IsGrounded())
        {
            rb.AddForceY(collapsedValues.jumpStrength, ForceMode2D.Impulse);
            jumpTime = Time.fixedTime;
            PlaySound(jumpSound);
        }
    }

    public void MoveToStartPosition()
    {
        rb.simulated = false;
        transform.DOMove(referenceTransform.position, 1);
        transform.DORotate(referenceTransform.eulerAngles, 1);
    }

    public void ResetBone()
    {
        rb.simulated = true;
        transform.position = referenceTransform.position;
        transform.rotation = referenceTransform.rotation;
    }

    public void SimulateRigidbody(bool value)
    {
        rb.simulated = value;
    }

    public void ChangeState(PlayerController.State newState)
    {
        state = newState;
    }

    public void Move(Transition.Direction dir)
    {
        switch (dir)
        {
            case Transition.Direction.Left:
                direction = -1;
                rb.linearVelocity = Vector2.zero;
                break;

            case Transition.Direction.Right:
                direction = 1;
                rb.linearVelocity = Vector2.zero;
                break;

            default:
                direction = 0;
                break;
        }
    }

    public void TeleportToCheckpoint(Vector3 position)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = 0;
        SimulateRigidbody(false);
        transform.position = position;
        ResetBone();
    }
}
