using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.InputSystem;

public class TRexController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private BoxCollider2D bc;

    [Header("Movements")]
    [SerializeField] private float speed;
    private float currentJoystickPosition;

    [Header("Jump")]
    [SerializeField] private float jumpForce;
    private bool isJumping;

    [SerializeField] private float jumpDuration;
    private float currentJumpDuration;

    [SerializeField] private float jumpGravity;
    [SerializeField] private float fallGravity;

    [SerializeField] float extraHeightBelow;
    [SerializeField] private LayerMask ground;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate()
    {
        Movement();
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
    }

    private void Movement()
    {
        float trueMovement = currentJoystickPosition == 0 ? 0 : Mathf.Sign(currentJoystickPosition);
        rb.linearVelocity = new Vector2(trueMovement * speed, rb.linearVelocityY);
    }

    private void Jump()
    {   
        isJumping = true;
        currentJumpDuration = 0;

        rb.gravityScale = jumpGravity;
        rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
    }

    private void StopJump()
    {   isJumping = false;
        rb.gravityScale = fallGravity;
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(bc.bounds.center,
            bc.bounds.size + new Vector3(Physics2D.defaultContactOffset, 0, 0), 0f,
            Vector2.down, extraHeightBelow, ground);

        if (raycastHit.collider != null)
        {
            return true;
        }

        return false;
    }


    public void MovementInput(InputAction.CallbackContext context)
    {
        currentJoystickPosition = context.ReadValue<float>();
    }

    public void JumpInput(InputAction.CallbackContext context)
    {
        if(context.performed && IsGrounded()) Jump();

        if(context.canceled) StopJump();
    }
}
