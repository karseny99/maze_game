using UnityEngine;
using System.Collections.Generic;

public class ButtonScript : MonoBehaviour
{
    public bool isPressed => validOccupants.Count > 0;
    
    private HashSet<Collider> validOccupants = new HashSet<Collider>();

    private void OnTriggerEnter(Collider other) {
        validOccupants.Add(other);
        Debug.Log($"Pressed by {other.name}. Remaining {validOccupants.Count}");
    }

    private void OnTriggerExit(Collider other) {
        if (validOccupants.Contains(other)) {
            validOccupants.Remove(other);
            Debug.Log($"Unpressed by {other.name}. Remaining: {validOccupants.Count}");
        }
    }

    void Update() {
        validOccupants.RemoveWhere(c => c == null || !c.gameObject.activeInHierarchy);
    }
}