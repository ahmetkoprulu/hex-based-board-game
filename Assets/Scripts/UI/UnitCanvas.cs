using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCanvas : MonoBehaviour
{
    [field: SerializeField] public Transform Cam { get; set; }

    private HealthBar HealthBar;

    private void Awake()
    {
        HealthBar = GetComponentInChildren<HealthBar>();
    }

    public void SetMaxHealth(int health)
    {
        HealthBar.SetMaxHealth(health);
    }

    public void SetHealth(int health)
    {
        HealthBar.SetHealth(health);
    }

    void LateUpdate()
    {
        var cam = Cam ? Cam : Camera.main.transform;
        transform.LookAt(transform.position + cam.forward);
    }
}
