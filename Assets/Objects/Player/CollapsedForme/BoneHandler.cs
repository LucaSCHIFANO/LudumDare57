using UnityEngine;
using UnityEngine.InputSystem;

public class BoneHandler : MonoBehaviour
{
    private Rigidbody2D rb;
    private float direction = 0f;
    [SerializeField] private CollapsedValuesScriptable collapsedValues;
    private float jumpTime = 0;

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

    public void MovementInput(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<float>();
    }
    
    public void JumpInput(InputAction.CallbackContext context)
    {
        if (context.performed && this.isActiveAndEnabled && (Time.fixedTime - jumpTime) >= collapsedValues.jumpCooldown)
        {
            rb.AddForceY(collapsedValues.jumpStrength, ForceMode2D.Impulse);
            jumpTime = Time.fixedTime;
        }
    }
}
