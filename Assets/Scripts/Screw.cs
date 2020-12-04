using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screw : MonoBehaviour
{
    public Transform drillTransform;
    public Transform drillLookRotate;
    public Transform screwModel;
    private List<Collider> _colliders = new List<Collider>();
    private List<MeshRenderer> _meshRenderers = new List<MeshRenderer>();




    private void Awake()
    {

        var meshRender = GetComponentsInChildren<MeshRenderer>();
        var colliders = GetComponents<Collider>();
        foreach (var mesh in meshRender)
        {
                _meshRenderers.Add(mesh);
        }
        DisableScrewMesh();
        foreach (var collider1 in colliders)
        {
            _colliders.Add(collider1);
        }
        DisableScrew();
    }
    public void EnableScrew()
    {
        foreach (var col in _colliders)
        {
            col.enabled = true;
        }
    }

    public void DisableScrew()
    {
        foreach (var col in _colliders)
        {
            col.enabled = false;
        }
        
    }

    public void DisableScrewMesh()
    {
        foreach (var meshRenderer in _meshRenderers)
        {
            meshRenderer.enabled = false;
        }
    }
    public void EnableScrewMesh()
    {
        foreach (var meshRenderer in _meshRenderers)
        {
            meshRenderer.enabled = true;
        }
    }

    
    
    
    
    
    
    
    
    
}
