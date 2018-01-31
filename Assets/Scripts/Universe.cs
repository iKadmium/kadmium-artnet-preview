using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PreviewFixtureJson
{
    public int address;
    public string group;
    public FixtureDefinition definition;
}

[Serializable]
public class PreviewUniverseJson
{
    public string name;
    public int universeID;
    public PreviewFixtureJson[] fixtures;
}

public class Universe : MonoBehaviour {

    public int UniverseID;
    private DMXFixture[] Fixtures;

    public Dictionary<int, List<DMXAttribute>> Channels { get; set; }
    private Dictionary<int, DMXFixture> fixtureMap;

    // Use this for initialization
    void Start () {
        Channels = new Dictionary<int, List<DMXAttribute>>();
        fixtureMap = new Dictionary<int, DMXFixture>();
        Fixtures = GetComponentsInChildren<DMXFixture>();
        foreach (var fixture in Fixtures)
        {
            fixtureMap.Add(fixture.Address, fixture);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetFixtureDefinitions(PreviewFixtureJson[] fixtures)
    {
        foreach(var fixtureJson in fixtures)
        {
            DMXFixture fixture = fixtureMap[fixtureJson.address];
            fixture.SetDefinition(fixtureJson.definition, this);
        }
    }
    
    public void ProcessDMX(byte[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            if (Channels.ContainsKey(i + 1))
            {
                foreach (DMXAttribute attribute in Channels[i + 1])
                {
                    attribute.Value = data[i];
                }
            }
        }
    }
}
