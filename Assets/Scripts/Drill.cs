using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

public class Drill : MonoBehaviour
{
    
    
    public void OnTriggerEnter(Collider other)
    {
        var collision = other.GetComponent<Screw>();
        if (collision)
        {
            DrillManager.Manager.selectedScrew = collision;
            DrillManager.Manager.colliderCollision = true;
            DrillManager.Manager.entryScrew = false;
        }
    }
}
