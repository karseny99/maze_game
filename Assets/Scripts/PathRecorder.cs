using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct PathPoint {
    public Vector3 position;
    public Quaternion rotation;
    public Quaternion cameraRotation;
}

public class PathRecorder : MonoBehaviour
{
    public Transform playerCamera;
    private List<List<PathPoint>> recordings; 
    public bool IsRecording { get; private set; } = false;
    private int currentSlot = 0;

    void FixedUpdate()
    {
        if (IsRecording)
        {
            RecordFrame();
        }
    }

    void RecordFrame()
    {
        PathPoint point = new PathPoint();
        point.position = transform.position;
        point.rotation = transform.rotation;
        
        if (playerCamera != null)
        {
            point.cameraRotation = playerCamera.localRotation;
        }

        recordings[currentSlot].Add(point);
    }

    public void InitializeSlots(int count)
    {
        recordings = new List<List<PathPoint>>(count);
        for (int i = 0; i < count; i++)
        {
            recordings.Add(new List<PathPoint>());
        }
    }

    public void StartRecording(int slot)
    {
        if (recordings == null || slot >= recordings.Count) {
            return;
        }
        currentSlot = slot;
        recordings[currentSlot].Clear();
        IsRecording = true;
        Debug.Log($"Slot {slot} is recording");
    }
    
    public void StopRecording() => IsRecording = false;

    public List<PathPoint> GetPath(int slot) 
    {
        if (slot >= 0 && slot < recordings.Count) return recordings[slot];
        return null;
    }
    
    public int TotalSlots => recordings?.Count ?? 0;
}