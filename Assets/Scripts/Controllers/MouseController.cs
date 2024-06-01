using System;
using UnityEngine;

public class MouseController : Singleton<MouseController>
{
    public Action<RaycastHit> OnLeftMouseClick { get; set; }
    public Action<RaycastHit> OnRightMouseClick { get; set; }
    public Action<RaycastHit> OnMiddleMouseClick { get; set; }
    public Action<RaycastHit> OnHover { get; set; }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckMouseClick(0);
        }
        if (Input.GetMouseButtonDown(1))
        {
            CheckMouseClick(1);
        }
        if (Input.GetMouseButtonDown(2))
        {
            CheckMouseClick(2);
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity))
        {
            Debug.DrawLine(hit.point, hit.point + Vector3.up * 10, Color.green);
            OnHover?.Invoke(hit);
        }
    }

    public void CheckMouseClick(int button)
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity))
        {
            switch (button)
            {
                case 0:
                    OnLeftMouseClick?.Invoke(hit);
                    break;
                case 1:
                    OnRightMouseClick?.Invoke(hit);
                    break;
                case 2:
                    OnMiddleMouseClick?.Invoke(hit);
                    break;
            }
        }
    }
}