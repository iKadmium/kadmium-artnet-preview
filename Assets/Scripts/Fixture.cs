using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using Assets.Scripts;

[Serializable]
public class PreviewFixtureJson
{
    public int address;
    public string group;
    public FixtureDefinition definition;
}

public class Range
{
	public float Min {get;set;}
	public float Max {get;set;}
}

public class Fixture : MonoBehaviour
{
	private Vector3 Neutral;

	public int Address;
    
	public Range PanRange {get;set;}
	public Range TiltRange {get;set;}

	public Dictionary<string, DMXAttribute> Attributes {get;set;}

	public int DMXEnd {get;set;}

    public Light Light;

    public Renderer ConeRenderer;

    public float VolumetricOpacity;

    private FixtureDefinition definition;

    private static float MAX_STROBE_SPEED_HZ = 15;
    private static float STROBE_TIME = 1 / MAX_STROBE_SPEED_HZ;

    void Start()
	{
        Attributes = new Dictionary<string, DMXAttribute>();
        
        Neutral = new Vector3(gameObject.transform.eulerAngles.x,
		                      gameObject.transform.eulerAngles.y,
		                      gameObject.transform.eulerAngles.z);
        
    }

    public void SetDefinition(FixtureDefinition definition, Universe universe)
    {
        int max = 0;
        foreach (ChannelDefinition channelElement in definition.channels)
        {
            string name = channelElement.name;
            int channel = channelElement.address + Address - 1;
            DMXAttribute attribute = new DMXAttribute(name);
            if (!universe.Channels.ContainsKey(channel))
            {
                universe.Channels.Add(channel, new List<DMXAttribute>());
            }
            universe.Channels[channel].Add(attribute);

            if (channel > max)
            {
                max = channel;
            }
            Attributes.Add(name, attribute);
        }

        MovementDefinition panRangeDefinition = definition.movements.SingleOrDefault(element => element.name == "Pan");
        MovementDefinition tiltRangeDefinition = definition.movements.SingleOrDefault(element => element.name == "Tilt");

        if (panRangeDefinition != null)
        {
            PanRange = new Range();
            PanRange.Min = panRangeDefinition.min;
            PanRange.Max = panRangeDefinition.max;
            Attributes["PanCoarse"].Value = 128;
        }
        if (tiltRangeDefinition != null)
        {
            TiltRange = new Range();
            TiltRange.Min = tiltRangeDefinition.min;
            TiltRange.Max = tiltRangeDefinition.max;
            Attributes["TiltCoarse"].Value = 128;
        }

        DMXEnd = max;

        this.definition = definition;
    }

    void Update()
    {
		Color color = Color.black;
		if(IsRGBW())
        {
            color = GetByteColor(Attributes["Red"].Value + Attributes["White"].Value, 
                Attributes["Green"].Value + Attributes["White"].Value, 
                Attributes["Blue"].Value + Attributes["White"].Value);
        }
        else if(IsRGB())
		{
			color = GetByteColor(Attributes["Red"].Value, Attributes["Green"].Value, Attributes["Blue"].Value);
		}
        if(Attributes.ContainsKey("Strobe") && Attributes["Strobe"].Value > 0)
        {
            float remainder = Time.time - (float)Math.Floor(Time.time);
            float modulus = remainder % STROBE_TIME;
            if(modulus > (STROBE_TIME / 2))
            {
                color = Color.black;
            }
        }
		
        UpdateMovement();

		float h, s, v;
		GetHSV(color.r, color.g, color.b, out h, out s, out v);
		Color newColor = new Color(color.r, color.g, color.b, v * VolumetricOpacity);
		ConeRenderer.material.color = newColor;
		Color emissionColor = new Color(color.r * VolumetricOpacity, color.g * VolumetricOpacity, color.b * VolumetricOpacity);
		ConeRenderer.material.SetColor("_EmissionColor", emissionColor);
		ConeRenderer.enabled = v > 0;

        if(Light != null)
        {
            Light.color = color;
        }
    }

    private bool IsRGBW()
    {
        return Attributes.ContainsKey("Red") && Attributes.ContainsKey("Green") && Attributes.ContainsKey("Blue") && Attributes.ContainsKey("White");
    }

    private bool IsRGB()
    {
        return Attributes.ContainsKey("Red") && Attributes.ContainsKey("Green") && Attributes.ContainsKey("Blue");
    }

    private void UpdateMovement()
    {
        gameObject.transform.eulerAngles = new Vector3(Neutral.x, Neutral.y, Neutral.z);

        if (Attributes.ContainsKey("Tilt"))
        {
            float tiltPercentage = ((float)Attributes["Tilt"].Value) / 255f;
            float tiltAmount = Mathf.Lerp(TiltRange.Min, TiltRange.Max, tiltPercentage);
            gameObject.transform.Rotate(Vector3.left, tiltAmount, Space.World);
        }
        else if (Attributes.ContainsKey("TiltFine") && Attributes.ContainsKey("TiltCoarse"))
        {
            float tiltCoarsePercentage = ((float)Attributes["TiltCoarse"].Value) / 255f;
            float tiltFinePercentage = ((float)Attributes["TiltFine"].Value) / 255f / 255f;
            float tiltAmount = Mathf.Lerp(TiltRange.Min, TiltRange.Max, tiltCoarsePercentage + tiltFinePercentage);
            gameObject.transform.Rotate(Vector3.left, tiltAmount, Space.World);
        }

        if (Attributes.ContainsKey("Pan"))
        {
            float panPercentage = ((float)Attributes["Pan"].Value) / 255f;
            float panAmount = Mathf.Lerp(PanRange.Min, PanRange.Max, panPercentage);
            gameObject.transform.Rotate(Vector3.up, -panAmount, Space.World);
        }
        else if (Attributes.ContainsKey("PanFine") && Attributes.ContainsKey("PanCoarse"))
        {
            float panCoarsePercentage = ((float)Attributes["PanCoarse"].Value) / 255f;
            float panFinePercentage = ((float)Attributes["PanFine"].Value) / 255f / 255f;
            float panAmount = Mathf.Lerp(PanRange.Min, PanRange.Max, panCoarsePercentage + panFinePercentage);
            gameObject.transform.Rotate(Vector3.up, -panAmount, Space.World);
        }
    }

    private Color GetByteColor(int red, int green, int blue)
    {
        float redFloat = ((float)red / 255.0f);
        float greenFloat = ((float)green / 255.0f);
        float blueFloat = ((float)blue / 255.0f);
        return new Color(redFloat, greenFloat, blueFloat);
    }

	void GetHSV( float r, float g, float b, out float h, out float s, out float v)
	{
		float min, max, delta;
		min = MIN( r, g, b );
		max = MAX( r, g, b );
		v = max;				// v
		delta = max - min;
		if( max != 0 )
			s = delta / max;		// s
		else {
			// r = g = b = 0		// s = 0, v is undefined
			s = 0;
			h = -1;
			return;
		}
		if( r == max )
			h = ( g - b ) / delta;		// between yellow & magenta
		else if( g == max )
			h = 2 + ( b - r ) / delta;	// between cyan & yellow
		else
			h = 4 + ( r - g ) / delta;	// between magenta & cyan
		h *= 60;				// degrees
		if( h < 0 )
			h += 360;
	}

	float MIN(params float[] args)
	{
		float min = args[0];
		foreach(float current in args)
		{
			if(current < min)
			{
				min = current;
			}
		}
		return min;
	}

	float MAX(params float[] args)
	{
		float max = args[0];
		foreach(float current in args)
		{
			if(current > max)
			{
				max = current;
			}
		}
		return max;
	}
	    	
}

