using UnityEngine;
using System.Xml.Linq;
using System.Linq;

public class LightGroup : MonoBehaviour {

	public int DMXStart;
	public string FixtureID;
	
	// Use this for initialization
	void Start () 
	{
		int dmx = DMXStart;
		int channelCount = GetChannelCount();
		foreach(Transform child in transform)
		{
			Fixture fixture = child.gameObject.AddComponent<Fixture>();
			fixture.FixtureID = FixtureID;
			fixture.DMXStart = dmx;
			dmx += channelCount;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private int  GetChannelCount()
	{
		XDocument doc = XDocument.Load("fixtures.xml");

		//find the fixture definition
		XElement fixtureElement = doc.Root.Elements("fixture").Single(element => element.Attribute("id").Value == FixtureID);
		var dmxElements = fixtureElement.Elements("channel").Attributes("dmx");
		int maxDMX = dmxElements.Max(element => int.Parse(element.Value));
		
		return maxDMX;
		
	}
}
