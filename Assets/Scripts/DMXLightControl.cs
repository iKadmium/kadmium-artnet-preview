using UnityEngine;
using System.Collections.Generic;

public class DMXLightControl : MonoBehaviour {

    int fixtureStartIndex;

	public Dictionary<int, List<DMXAttribute>> Channels {get;set;}

	// Use this for initialization
	public void Start () 
	{
        
	}

	public void ProcessDMX(IDMXPacket packet)
	{
		for(int i = 0; i < packet.Data.Length; i++)
		{
			if(Channels.ContainsKey(i+1))
			{
				foreach(DMXAttribute attribute in Channels[i+1])
				{
					attribute.Value = packet.Data[i];
				}
			}
		}
	}

    // Update is called once per frame
    public void Update()
    {
	}
}
