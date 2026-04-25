using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody rb;
    private Vector2 moveInput;
    private MouseLook mouseLook; 

    void Start() {
        rb = GetComponent<Rigidbody>();
        mouseLook = GetComponentInChildren<MouseLook>();
    }

    public void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();
    }

    void FixedUpdate() 
    {
        Quaternion targetRotation = Quaternion.Euler(0, mouseLook.yRotation, 0);
        rb.MoveRotation(targetRotation);

        Vector3 cleanForward = targetRotation * Vector3.forward;
        Vector3 cleanRight = targetRotation * Vector3.right;
        Vector3 moveDir = (cleanForward * moveInput.y + cleanRight * moveInput.x);

        if (moveDir.sqrMagnitude > 0.01f)
        {
            rb.linearVelocity = new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed);
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }

        rb.angularVelocity = Vector3.zero;
    }

    public void TeleportTo(Transform target)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.position = target.position;
        rb.rotation = target.rotation;

        transform.position = target.position;
        transform.rotation = target.rotation;
        
        if (mouseLook != null)
        {
            mouseLook.yRotation = target.eulerAngles.y;
        }
    }
}