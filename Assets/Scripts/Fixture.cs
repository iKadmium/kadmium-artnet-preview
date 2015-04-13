using UnityEngine;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System;

public class Fixture : MonoBehaviour
{
	Light light;
    bool strobeOn = false;
	public int DMXStart;
	public string FixtureID;

	public DMXAttribute Red {get;set;}
	public DMXAttribute Green {get;set;}
	public DMXAttribute Blue {get;set;}

	public int DMXEnd {get;set;}

    void Start()
	{
		light = gameObject.GetComponent<Light>();

		XDocument doc = XDocument.Load("fixtures.xml");

		//find the fixture definition
		XElement fixtureElement = doc.Root.Elements("fixture").Single(element => element.Attribute("id").Value == FixtureID);

		DMXLightControl control = GameObject.FindObjectOfType<DMXLightControl>();
		if(control.Channels == null)
		{
			control.Channels = new Dictionary<int, DMXAttribute>();
		}

		int max = 0;

		foreach(XElement channelElement in fixtureElement.Elements("channel"))
		{
			string name = channelElement.Attribute("name").Value;
			int channel = Int32.Parse(channelElement.Attribute("dmx").Value) + DMXStart - 1;
			DMXAttribute attribute = new DMXAttribute(name);
			control.Channels.Add(channel, attribute);
			if(channel > max)
			{
				max = channel;
			}
			switch(name)
			{
			case "Red":
				Red = attribute;
				break;
			case "Green":
				Green = attribute;
				break;
			case "Blue":
				Blue = attribute;
				break;
			}
		}

		DMXEnd = max;
	}

    void Update()
    {
		if(Red != null && Green != null && Blue != null)
		{
			light.color = GetByteColor(Red.Value, Green.Value, Blue.Value);
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
