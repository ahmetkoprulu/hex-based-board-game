using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [field: SerializeField] public Slider Slider { get; set; }

    public void SetMaxHealth(int health)
    {
        Slider.maxValue = health;
        Slider.value += health;
    }

    public void SetHealth(int health)
    {
        Slider.value = health;
    }
}
