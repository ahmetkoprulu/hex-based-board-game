using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCanvas : MonoBehaviour
{
    [field: SerializeField] public Transform Cam { get; set; }

    void LateUpdate()
    {
        var cam = Cam ? Cam : Camera.main.transform;
        transform.LookAt(transform.position + cam.forward);
    }
}
