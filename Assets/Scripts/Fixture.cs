using UnityEngine;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System;
using System.IO;

public class Range
{
	public float Min {get;set;}
	public float Max {get;set;}
}

public class Fixture : MonoBehaviour
{
	private Vector3 Neutral;

	private Light Light {get;set;}
    public int DMXStart;
	public string FixtureID;

	public Range PanRange {get;set;}
	public Range TiltRange {get;set;}

	public Dictionary<string, DMXAttribute> Attributes {get;set;}

	public int DMXEnd {get;set;}

	public string fixturesLocation = @"D:\User\IntelliJ Workspace\kadmium-osc-dmx\out\artifacts\jar\data\fixtures";

    void Start()
	{
		Light = gameObject.GetComponent<Light>();

		XDocument doc = XDocument.Load(Path.Combine(fixturesLocation, FixtureID + ".xml"));

		//find the fixture definition
		XElement fixtureElement = doc.Root;

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
			Attributes["Pan"].Value = 128;
		}
		if(tiltRangeElement != null)
		{
			TiltRange = new Range();
			TiltRange.Min = float.Parse(tiltRangeElement.Attribute("min").Value);
			TiltRange.Max = float.Parse(tiltRangeElement.Attribute("max").Value);
			Attributes["Tilt"].Value = 128;
		}

		Neutral = new Vector3(gameObject.transform.eulerAngles.x,
		                      gameObject.transform.eulerAngles.y,
		                      gameObject.transform.eulerAngles.z);

		DMXEnd = max;
	}

    void Update()
    {
		if(Attributes.ContainsKey("Red") && Attributes.ContainsKey("Green") && Attributes.ContainsKey("Blue"))
		{
			Light.color = GetByteColor(Attributes["Red"].Value, Attributes["Green"].Value, Attributes["Blue"].Value);
		}

		gameObject.transform.eulerAngles = new Vector3(Neutral.x, Neutral.y, Neutral.z);

		if(Attributes.ContainsKey("Tilt"))
		{
			float tiltPercentage = ((float)Attributes["Tilt"].Value) / 255f;
			float tiltAmount = Mathf.Lerp(TiltRange.Min, TiltRange.Max, tiltPercentage);
			gameObject.transform.Rotate(Vector3.left, -tiltAmount, Space.World);
		}
		else if(Attributes.ContainsKey("TiltFine") && Attributes.ContainsKey("TiltCoarse"))
		{
			float tiltCoarsePercentage = ((float)Attributes["TiltCoarse"].Value) / 255f;
			float tiltFinePercentage = ((float)Attributes["TiltFine"].Value) / 255f / 255f;
			float tiltAmount = Mathf.Lerp(TiltRange.Min, TiltRange.Max, tiltCoarsePercentage + tiltFinePercentage);
			gameObject.transform.Rotate(Vector3.left, -tiltAmount, Space.World);
		}

		if(Attributes.ContainsKey("Pan"))
		{
			float panPercentage = ((float)Attributes["Pan"].Value) / 255f;
			float panAmount = Mathf.Lerp(PanRange.Min, PanRange.Max, panPercentage);
			gameObject.transform.Rotate(Vector3.up, panAmount, Space.World);
		}
		else if(Attributes.ContainsKey("PanFine") && Attributes.ContainsKey("PanCoarse"))
		{
			float panCoarsePercentage = ((float)Attributes["PanCoarse"].Value) / 255f;
			float panFinePercentage = ((float)Attributes["PanFine"].Value) / 255f / 255f;
			float panAmount = Mathf.Lerp(TiltRange.Min, TiltRange.Max, panCoarsePercentage + panFinePercentage);
			gameObject.transform.Rotate(Vector3.up, panAmount, Space.World);
		}
    }

    private Color GetByteColor(int red, int green, int blue)
    {
        float redFloat = ((float)red / 255.0f);
        float greenFloat = ((float)green / 255.0f);
        float blueFloat = ((float)blue / 255.0f);
        return new Color(redFloat, greenFloat, blueFloat);
    }
    	
}
