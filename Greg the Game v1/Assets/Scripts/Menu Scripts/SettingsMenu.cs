using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("References")]
    public TMP_InputField xText, yText;
    public Slider xSlider, ySlider;

    public static float xSen = 0.7f, ySen = 0.7f; //Value Between 0 and 1

    //Set Fill input to match Sliders
    public void SetXSense(float sense)
    {
        xSen = sense;
        Debug.Log("Slider X: " + xSen);
        xText.text = (xSen * 100).ToString("0.0");
    }

    public void SetYSense(float sense)
    {
        ySen = sense;
        Debug.Log("Slider Y: " + ySen);
        yText.text = (ySen * 100).ToString("0.0");
    }

    //Set Sliders to match fill input
    public void ChangeSlider_X()
    {
        float sensInput;
        float.TryParse(xText.text,out sensInput);
        float value = sensInput / 100;
        
        xSen = value;
        if (xSen < 0)
            xSlider.value = 0;
        else if (xSen > 1)
        {
            xSlider.value = 1;
            xSen = value;
            xText.text = (xSen * 100).ToString("0.0");
        }
        else
            xSlider.value = xSen;

        Debug.Log("Set X sense: " + xSen);
    }

    public void ChangeSlider_Y()
    {
        float sensInput;
        float.TryParse(yText.text, out sensInput);
        float value = sensInput / 100;

        ySen = value;
        if (xSen < 0)
            ySlider.value = 0;
        else if (xSen > 1)
        {
            ySlider.value = 1;
            ySen = value;
            yText.text = (ySen * 100).ToString("0.0");
        }
        else
            ySlider.value = ySen;

        Debug.Log("Set Y sense: " + ySen);
    }

}
