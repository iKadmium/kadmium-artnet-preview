using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using Assets.Scripts;

public class Range
{
	public float Min {get;set;}
	public float Max {get;set;}
}

public class DMXFixture : MonoBehaviour
{
	private Vector3 Neutral;

	public int Address;
    
	public Range PanRange {get;set;}
	public Range TiltRange {get;set;}

	public Dictionary<string, DMXAttribute> Attributes {get;set;}

	public int DMXEnd {get;set;}
    
    private FixtureDefinition definition;
    
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
        UpdateMovement();
    }
    
    private bool IsFire()
    {
        return Attributes.ContainsKey("Fire");
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
    	
}

