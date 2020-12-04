using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence : MonoBehaviour
{
    
    public Transform fenceModel;
    public Transform hammerTransform;
    private List<Collider> _colliders = new List<Collider>();

    private void Awake()
    {
        var colliders = GetComponents<Collider>();
        foreach (var col in colliders)
        {
            _colliders.Add(col);
        }
        DisableFence();
    }

    public void EnableFence()
    {
        foreach (var col in _colliders)
        {
            col.enabled = true;
        }
    }

    public void DisableFence()
    {
        foreach (var col in _colliders)
        {
            col.enabled = false;
        }
        
    }
    
}
