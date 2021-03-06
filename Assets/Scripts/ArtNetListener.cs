﻿using UnityEngine;
using System.Net.Sockets;

public class ArtNetListener : MonoBehaviour {

	public int port = 6454;

	private UdpClient client;

	Venue control;

	byte[] header;
	int dataLength;

	// Use this for initialization
	void Start () 
	{
		client = new UdpClient(port);
		control = gameObject.GetComponentInParent<Venue>();
	}
	
	// Update is called once per frame
	void Update () {
		ArtNetPacket packet = null;
		while(client.Available > 0)
		{
			System.Net.IPEndPoint point = null;
			byte[] packetData = client.Receive(ref point);
			ArtNetPacket newPacket = new ArtNetPacket(packetData);
			if(packet == null)
			{
				packet = newPacket;
			}
		}
		if(packet != null)
		{
			control.ProcessDMX((short)packet.Universe, packet.Data);
		}
	}
}
