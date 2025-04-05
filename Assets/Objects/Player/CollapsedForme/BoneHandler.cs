using UnityEngine;
using UnityEngine.InputSystem;

public class BoneHandler : MonoBehaviour
{
    private Rigidbody2D rb;
    private float direction = 0f;
    [SerializeField] private CollapsedValuesScriptable collapsedValues;
    private float jumpTime = 0;
    [SerializeField] private LayerMask ground;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(direction) > .2f){
            rb.AddForceX(direction * collapsedValues.strength);
            rb.AddTorque(direction * collapsedValues.torqueStrength * -1);
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
        direction = context.ReadValue<float>();
    }
    
    public void JumpInput(InputAction.CallbackContext context)
    {
        if (context.performed && this.isActiveAndEnabled && (Time.fixedTime - jumpTime) >= collapsedValues.jumpCooldown && IsGrounded())
        {
            rb.AddForceY(collapsedValues.jumpStrength, ForceMode2D.Impulse);
            jumpTime = Time.fixedTime;
        }
    }
}
