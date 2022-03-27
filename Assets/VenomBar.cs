using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VenomBar : MonoBehaviour
{
    public Slider slider;
    public Slider sliderv2;

    public void SetMaxTime(float time)
    {
        sliderv2.maxValue = time;
        sliderv2.value = time;
    }

    public void SetTime(float time)
    {
        sliderv2.value = time;
    }

    public void SetMaxVenom(float venom)
    {
        slider.maxValue = venom;
        slider.value = venom;
    }

    public void SetVenom(float venom)
    {
        slider.value = venom;
    }

}
