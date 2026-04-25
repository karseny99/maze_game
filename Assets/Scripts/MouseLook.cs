using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 0.1f;
    public Transform playerBody;
    float xRotation = 0f;
    
    [HideInInspector] public float yRotation = 0f;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        yRotation = playerBody.eulerAngles.y;
    }

    void Update() {
        if (Cursor.lockState != CursorLockMode.Locked) return;
        
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity;
        float mouseY = mouseDelta.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        yRotation += mouseX;
    }
}