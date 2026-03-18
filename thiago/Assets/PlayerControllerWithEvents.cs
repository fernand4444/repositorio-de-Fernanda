using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerWithEvents : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Multiplier applied to input when applying torque/force")]
    public float moveSpeed = 6f;
    [Tooltip("If true, applies torque to roll the ball. Otherwise applies linear force.")]
    public bool useTorque = true;

    [Header("Jump")]
    public float jumpForce = 5f;
    [Tooltip("Distance to check beneath the ball to consider it grounded")]
    public float groundCheckDistance = 0.55f;
    [Tooltip("Layer mask used for ground checks")]
    public LayerMask groundLayers = ~0; // default to everything

    Rigidbody _rb;
    Vector2 _moveInput = Vector2.zero;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        // Ensure reasonable angular velocity for rolling responsiveness
        _rb.maxAngularVelocity = 20f;
    }

    // This method is called by the Input System (PlayerInput -> Invoke Unity Events)
    // Bind the "Move" action to this method in the PlayerInput inspector.
    public void OnMove(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();
    }

    // Bind the "Jump" action to this method in the PlayerInput inspector.
    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (IsGrounded())
            {
                _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 move = new Vector3(_moveInput.x, 0f, _moveInput.y);

        if (move.sqrMagnitude < 1e-6f)
            return;

        if (useTorque)
        {
            // Apply torque perpendicular to movement to make the sphere roll naturally.
            Vector3 axis = Vector3.Cross(Vector3.up, move.normalized);
            float factor = move.magnitude * moveSpeed;
            Vector3 torque = axis * factor;
            _rb.AddTorque(torque, ForceMode.Acceleration);
        }
        else
        {
            _rb.AddForce(move * moveSpeed, ForceMode.Acceleration);
        }
    }

    bool IsGrounded()
    {
        // Raycast down from the center of the sphere
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayers);
    }

    // Optional: draw debug ray in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
