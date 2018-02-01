using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFixture : MonoBehaviour {

    private static float MAX_STROBE_SPEED_HZ = 15;
    private static float STROBE_TIME = 1 / MAX_STROBE_SPEED_HZ;

    private Light Light;

    private Renderer ConeRenderer;
    private DMXFixture Fixture;
    public float VolumetricOpacity;

    // Use this for initialization
    void Start () {
        Fixture = GetComponent<DMXFixture>();
        ConeRenderer = GetComponentInChildren<Renderer>();
        Light = GetComponentInChildren<Light>();
	}
	
	// Update is called once per frame
	void Update () {

        Color color = GetPrimaryColor();
        
        if (Fixture.Attributes.ContainsKey("Strobe") && Fixture.Attributes["Strobe"].Value > 0)
        {
            float remainder = Time.time - (float)Math.Floor(Time.time);
            float modulus = remainder % STROBE_TIME;
            if (modulus > (STROBE_TIME / 2))
            {
                color = Color.black;
            }
        }

        float h, s, v;
        GetHSV(color.r, color.g, color.b, out h, out s, out v);
        Color newColor = new Color(color.r, color.g, color.b, v * VolumetricOpacity);
        ConeRenderer.material.color = newColor;
        Color emissionColor = new Color(color.r * VolumetricOpacity, color.g * VolumetricOpacity, color.b * VolumetricOpacity);
        ConeRenderer.material.SetColor("_EmissionColor", emissionColor);
        ConeRenderer.enabled = v > 0;

        if (Light != null)
        {
            Light.color = color;
        }
    }

    public Color GetPrimaryColor()
    {
        Color color = Color.black;
        if (IsRGBW())
        {
            color = GetByteColor(Fixture.Attributes["Red"].Value + Fixture.Attributes["White"].Value,
                Fixture.Attributes["Green"].Value + Fixture.Attributes["White"].Value,
                Fixture.Attributes["Blue"].Value + Fixture.Attributes["White"].Value);
        }
        else if (IsRGB())
        {
            color = GetByteColor(Fixture.Attributes["Red"].Value, Fixture.Attributes["Green"].Value, Fixture.Attributes["Blue"].Value);
        }

        return color;
    }

    private Color GetByteColor(int red, int green, int blue)
    {
        float redFloat = ((float)red / 255.0f);
        float greenFloat = ((float)green / 255.0f);
        float blueFloat = ((float)blue / 255.0f);
        return new Color(redFloat, greenFloat, blueFloat);
    }

    void GetHSV(float r, float g, float b, out float h, out float s, out float v)
    {
        float min, max, delta;
        min = MIN(r, g, b);
        max = MAX(r, g, b);
        v = max;                // v
        delta = max - min;
        if (max != 0)
            s = delta / max;        // s
        else
        {
            // r = g = b = 0		// s = 0, v is undefined
            s = 0;
            h = -1;
            return;
        }
        if (r == max)
            h = (g - b) / delta;        // between yellow & magenta
        else if (g == max)
            h = 2 + (b - r) / delta;    // between cyan & yellow
        else
            h = 4 + (r - g) / delta;    // between magenta & cyan
        h *= 60;                // degrees
        if (h < 0)
            h += 360;
    }

    float MIN(params float[] args)
    {
        float min = args[0];
        foreach (float current in args)
        {
            if (current < min)
            {
                min = current;
            }
        }
        return min;
    }

    float MAX(params float[] args)
    {
        float max = args[0];
        foreach (float current in args)
        {
            if (current > max)
            {
                max = current;
            }
        }
        return max;
    }

    private bool IsRGBW()
    {
        return Fixture.Attributes.ContainsKey("Red") && Fixture.Attributes.ContainsKey("Green") 
            && Fixture.Attributes.ContainsKey("Blue") && Fixture.Attributes.ContainsKey("White");
    }

    private bool IsRGB()
    {
        return Fixture.Attributes.ContainsKey("Red") && Fixture.Attributes.ContainsKey("Green") 
            && Fixture.Attributes.ContainsKey("Blue");
    }
}
