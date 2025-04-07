using System;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.InputSystem;

public class TRexController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    [SerializeField] private CapsuleCollider2D cc;
    [SerializeField] GameObject rotatingPart;
    private PlayerController.State state = PlayerController.State.CanMove;

    [Header("Movements")]
    [SerializeField] private float speed;
    [SerializeField] private float transitionSpeed;
    private float currentSpeed;
    private float currentJoystickPosition;

    [Header("Jump")]
    [SerializeField] private float jumpForce;
    private bool isJumping;
    private bool isInAir;

    [SerializeField] private float jumpDuration;
    private float currentJumpDuration;

    [Space, SerializeField] private float jumpGravity;
    [SerializeField] private float fallGravity;

    [Space, SerializeField] float extraHeightBelow;
    [SerializeField] private LayerMask ground;

    [SerializeField] private SOSound jumpSound;

    [Header("Attack")]
    [SerializeField] private Transform attackPosition;
    [SerializeField] private float attackDamage;
    [SerializeField] private float attackRange;
    [SerializeField] private SOSound attackSound;

    [Header("Debug")]
    [SerializeField] private bool showDebug;

    [Header("Visuals")]
    [SerializeField] private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = speed;
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        Movement();
    }

    public void PlaySound(SOSound sound)
    {
        if (sound == null) return;
        SoundManager.Instance.Play(sound);
    }

    private void Update()
    {
        if (isJumping) 
        {
            if (currentJumpDuration > jumpDuration)
            {
                StopJump();
            }
            else currentJumpDuration += Time.deltaTime;
        }

        if (isInAir && IsGrounded())
        {
            animator.SetBool("IsInAir", false);
            isInAir = false;
        }else if (!isInAir && !IsGrounded())
        {
            animator.SetBool("IsInAir", true);
            isInAir = true;

        }
    }

    private void Movement()
    {
        float trueMovement = currentJoystickPosition == 0 ? 0 : Mathf.Sign(currentJoystickPosition);
        rb.linearVelocity = new Vector2(trueMovement * currentSpeed, rb.linearVelocityY);
        if (Mathf.Abs(trueMovement) > .2f)
            animator.SetBool("IsWalking", true);
        else 
            animator.SetBool("IsWalking", false);


        if (trueMovement < 0)
            rotatingPart.transform.localScale = new Vector2(-1, 1);
        else if (trueMovement > 0) rotatingPart.transform.localScale = new Vector2(1, 1);
    }

    private void Jump()
    {   
        isJumping = true;
        currentJumpDuration = 0;

        rb.gravityScale = jumpGravity;
        rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
        animator.SetTrigger("Jump");
        PlaySound(jumpSound);
    }

    private void StopJump()
    {   isJumping = false;
        rb.gravityScale = fallGravity;
    }

    private void Attack()
    {
        RaycastHit2D[] objects = Physics2D.CircleCastAll(attackPosition.position, attackRange, Vector2.right, attackRange);
        
        foreach (RaycastHit2D obj in objects)
        {
            if (obj.collider.GetComponent<IDestroyable>() != null)
                obj.collider.GetComponent<IDestroyable>().TakeDamage(attackDamage);
        }
        PlaySound(attackSound);
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(cc.bounds.center, new Vector2(cc.bounds.size.x /2, cc.bounds.size.y) /*+ new Vector3(Physics2D.defaultContactOffset, 0, 0)*/, 0f,
            Vector2.down, extraHeightBelow, ground);

        if (raycastHit.collider != null)
        {
            return true;
        }

        return false;
    }


    public void MovementInput(InputAction.CallbackContext context)
    {
        if (state != PlayerController.State.CanMove) return;

        currentJoystickPosition = context.ReadValue<float>();
    }

    public void JumpInput(InputAction.CallbackContext context)
    {
        if (state != PlayerController.State.CanMove) return;

        if (context.performed && IsGrounded()) Jump();

        if(context.canceled) StopJump();
    }

    public void AttackInput(InputAction.CallbackContext context)
    {
        if (state != PlayerController.State.CanMove) return;

        if (context.performed) Attack();
    }

    private void OnDrawGizmosSelected()
    {
        if (!showDebug) return;
        Gizmos.DrawSphere(attackPosition.position, attackRange);
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
                currentJoystickPosition = -1;
                currentSpeed = transitionSpeed;

                break;

            case Transition.Direction.Right:
                currentJoystickPosition = 1;
                currentSpeed = transitionSpeed;
                break;

            default:
                currentJoystickPosition = 0;
                currentSpeed = speed;
                break;
        }
    }

    public void Restart(Vector2 position)
    {
        transform.position = position;
        rb.linearVelocity = Vector2.zero;
    }
}
