using UnityEngine;
using System.Collections;

public class DoorScript : MonoBehaviour
{
    public GameObject[] logicButtons;
    private bool isOpen = false; 
    private MeshRenderer doorRenderer;
    private Collider doorCollider;

    void Start() {
        doorRenderer = GetComponent<MeshRenderer>();
        doorCollider = GetComponent<Collider>();
    }

    void Update() {
        bool allPressed = true;
        foreach (GameObject btnObject in logicButtons)
        {
            ButtonScript btn = btnObject.GetComponent<ButtonScript>();
            if (btn != null && !btn.isPressed)
            {
                allPressed = false;
                break;
            }
        }

        if (allPressed && !isOpen) {
            SetDoorState(true);
        } else if (!allPressed && isOpen) {
            SetDoorState(false);
        }
    }

    void SetDoorState(bool open) {
        isOpen = open;
        
        if (doorRenderer) doorRenderer.enabled = !open;
        if (doorCollider) doorCollider.enabled = !open;
    }
}