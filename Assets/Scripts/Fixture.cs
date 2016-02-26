using UnityEngine;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System;
using System.IO;
using Assets.Scripts;

public class Range
{
	public float Min {get;set;}
	public float Max {get;set;}
}

public class Fixture : MonoBehaviour
{
	private Vector3 Neutral;

	public int DMXStart;
	public string FixtureID;

	public Range PanRange {get;set;}
	public Range TiltRange {get;set;}

	public Dictionary<string, DMXAttribute> Attributes {get;set;}

	public int DMXEnd {get;set;}

	public static string fixturesLocation = @"D:\User\Documents\GitHubVisualStudio\kadmium-osc-dmx-dotnet\kadmium-osc-dmx-dotnet\bin\Debug\data\fixtures";

    public Material LightConeMaterial;

	private GameObject Cone {get;set;}
    private Light Light { get; set; }

    public float VolumetricOpacity;

    void Start()
	{
		string path = (Path.Combine(fixturesLocation, FixtureID + ".xml"));
        
		//find the fixture definition
		XElement fixtureElement = FileAccessor.LoadXML(path);

		DMXLightControl control = GameObject.FindObjectOfType<DMXLightControl>();
		if(control.Channels == null)
		{
			control.Channels = new Dictionary<int, List<DMXAttribute>>();
		}

		int max = 0;

		Attributes = new Dictionary<string, DMXAttribute>();

		foreach(XElement channelElement in fixtureElement.Elements("channel"))
		{
			string name = channelElement.Attribute("name").Value;
			int channel = Int32.Parse(channelElement.Attribute("dmx").Value) + DMXStart - 1;
			DMXAttribute attribute = new DMXAttribute(name);
			if(!control.Channels.ContainsKey(channel))
			{
				control.Channels.Add(channel, new List<DMXAttribute>());
			}
			control.Channels[channel].Add(attribute);

			if(channel > max)
			{
				max = channel;
			}
			Attributes.Add(name, attribute);
		}

		XElement panRangeElement = fixtureElement.Elements("movement").SingleOrDefault(element => element.Attribute("type").Value == "Pan");
		XElement tiltRangeElement = fixtureElement.Elements("movement").SingleOrDefault(element => element.Attribute("type").Value == "Tilt");

		if(panRangeElement != null)
		{
			PanRange = new Range();
			PanRange.Min = float.Parse(panRangeElement.Attribute("min").Value);
			PanRange.Max = float.Parse(panRangeElement.Attribute("max").Value);
			Attributes["PanCoarse"].Value = 128;
		}
		if(tiltRangeElement != null)
		{
			TiltRange = new Range();
			TiltRange.Min = float.Parse(tiltRangeElement.Attribute("min").Value);
			TiltRange.Max = float.Parse(tiltRangeElement.Attribute("max").Value);
			Attributes["TiltCoarse"].Value = 128;
		}

		Neutral = new Vector3(gameObject.transform.eulerAngles.x,
		                      gameObject.transform.eulerAngles.y,
		                      gameObject.transform.eulerAngles.z);

		DMXEnd = max;

		Cone = transform.FindChild("Cone").gameObject;
        Transform spotlightTransform = transform.FindChild("Spotlight");
        if(spotlightTransform != null)
        {
            Light = spotlightTransform.GetComponent("Light") as Light;
        }
        Transform trans = transform.FindChild("mshParFront2");
        if (trans != null)
        {
            trans.gameObject.SetActive(false);
        }
        
    }

    void Update()
    {
		Color color = Color.black;
		if(Attributes.ContainsKey("Red") && Attributes.ContainsKey("Green") && Attributes.ContainsKey("Blue"))
		{
			color = GetByteColor(Attributes["Red"].Value, Attributes["Green"].Value, Attributes["Blue"].Value);
		}
		gameObject.transform.eulerAngles = new Vector3(Neutral.x, Neutral.y, Neutral.z);

		if(Attributes.ContainsKey("Tilt"))
		{
			float tiltPercentage = ((float)Attributes["Tilt"].Value) / 255f;
			float tiltAmount = Mathf.Lerp(TiltRange.Min, TiltRange.Max, tiltPercentage);
			gameObject.transform.Rotate(Vector3.left, tiltAmount, Space.World);
		}
		else if(Attributes.ContainsKey("TiltFine") && Attributes.ContainsKey("TiltCoarse"))
		{
			float tiltCoarsePercentage = ((float)Attributes["TiltCoarse"].Value) / 255f;
			float tiltFinePercentage = ((float)Attributes["TiltFine"].Value) / 255f / 255f;
			float tiltAmount = Mathf.Lerp(TiltRange.Min, TiltRange.Max, tiltCoarsePercentage + tiltFinePercentage);
			gameObject.transform.Rotate(Vector3.left, tiltAmount, Space.World);
		}

		if(Attributes.ContainsKey("Pan"))
		{
			float panPercentage = ((float)Attributes["Pan"].Value) / 255f;
			float panAmount = Mathf.Lerp(PanRange.Min, PanRange.Max, panPercentage);
			gameObject.transform.Rotate(Vector3.up, -panAmount, Space.World);
		}
		else if(Attributes.ContainsKey("PanFine") && Attributes.ContainsKey("PanCoarse"))
		{
			float panCoarsePercentage = ((float)Attributes["PanCoarse"].Value) / 255f;
			float panFinePercentage = ((float)Attributes["PanFine"].Value) / 255f / 255f;
			float panAmount = Mathf.Lerp(PanRange.Min, PanRange.Max, panCoarsePercentage + panFinePercentage);
			gameObject.transform.Rotate(Vector3.up, -panAmount, Space.World);
		}

		float h, s, v;
		GetHSV(color.r, color.g, color.b, out h, out s, out v);
		Color newColor = new Color(color.r, color.g, color.b, v * VolumetricOpacity);
		Cone.GetComponent<Renderer>().material.color = newColor;
		Color emissionColor = new Color(color.r * VolumetricOpacity, color.g * VolumetricOpacity, color.b * VolumetricOpacity);
		Cone.GetComponent<Renderer>().material.SetColor("_EmissionColor", emissionColor);
		Cone.GetComponent<MeshRenderer>().enabled = v > 0;

        if(Light != null)
        {
            Light.color = color;
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
