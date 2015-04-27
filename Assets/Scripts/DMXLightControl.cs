using UnityEngine;
using System.Collections.Generic;
using System.Xml.Linq;
using Bespoke.Common.Osc;
using System.Linq;

public class DMXLightControl : MonoBehaviour {

    OscServer server;
    int fixtureStartIndex;

	public Dictionary<int, DMXAttribute> Channels {get;set;}

	// Use this for initialization
	public void Start () {
        
	}

	public void ProcessDMX(IDMXPacket packet)
	{
		for(int i = 0; i < packet.Data.Length; i++)
		{
			if(Channels.ContainsKey(i+1))
			{
				Channels[i+1].Value = packet.Data[i];
			}
		}
	}

    // Update is called once per frame
    public void Update()
    {
	}
}
