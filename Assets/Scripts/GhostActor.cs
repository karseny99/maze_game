using UnityEngine;
using System.Collections.Generic;

public class GhostActor : MonoBehaviour
{
    public Transform ghostCamera;
    private List<PathPoint> _path;
    private int _index = 0;
    private bool _active = false;

    public void Init(List<PathPoint> points)
    {
        _path = points;
        if (_path != null && _path.Count > 0)
        {
            _index = 0;
            _active = true;
        }
    }

    void FixedUpdate()
    {
        if (!_active || _path == null) return;

        if (_index < _path.Count)
        {
            transform.position = _path[_index].position;
            transform.rotation = _path[_index].rotation;

            if (ghostCamera != null) {
                ghostCamera.localRotation = _path[_index].cameraRotation;
            }

            _index++;
        }
        else
        {
            Destroy(gameObject); 
        }
    }
}