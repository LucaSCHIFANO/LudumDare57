using UnityEngine;
using UnityEngine.InputSystem;

public class BoneHandler : MonoBehaviour
{
    private Rigidbody2D rb;
    private float direction = 0f;
    [SerializeField] private float strength = 10f;
    [SerializeField] private float jumpStrength = 1;
    [SerializeField] private float torqueStrength = 1;
    private float jumpTime = 0;
    [SerializeField] private float jumpCooldown = 3;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        if (Mathf.Abs(direction) > .2f){
            rb.AddForceX(direction * strength);
            rb.AddTorque(direction * torqueStrength * -1);
        }
    }

    public void MovementInput(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<float>();
    }
    
    public void JumpInput(InputAction.CallbackContext context)
    {
        if (context.performed && this.isActiveAndEnabled && (Time.fixedTime - jumpTime) >= jumpCooldown)
        {
            rb.AddForceY(jumpStrength, ForceMode2D.Impulse);
            jumpTime = Time.fixedTime;
        }
    }
}
