using Shapes2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowHexagon : HexagonCtrl
{
    Color destination = Color.magenta;
    Shape rainbow;

    private void Awake()
    {
        rainbow = GetComponent<Shape>();
        colors[0] = colors[1] = colors[2] = Color.white;
        rainbow.settings.fillColor2 = Color.red;
    }

    void FixedUpdate()
    {
        rainbow.settings.fillColor2 = Color.Lerp(rainbow.settings.fillColor2, destination, Time.deltaTime * 1.5f);
        Vector3 temp = new Vector3(rainbow.settings.fillColor2.r - destination.r,
                                    rainbow.settings.fillColor2.g - destination.g,
                                    rainbow.settings.fillColor2.b - destination.b);
        
        if(temp.magnitude<.1f)
        {
            if (destination == Color.red) destination = Color.magenta;
            else if (destination == Color.magenta) destination = Color.blue;
            else if (destination == Color.blue) destination = Color.cyan;
            else if (destination == Color.cyan) destination = Color.green;
            else if (destination == Color.green) destination = Color.yellow;
            else destination = Color.red;
        }

        rainbow.settings.gradientStart = (Mathf.Sin(Time.time*5) + 1) / 15f;
    }
}
