using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using System.IO;
using kadmium_opc_dotnet;

public class OPCListener : MonoBehaviour {

    OPCServer server;
    private Color[] PixelColors;
    public GameObject originalPixelObject;
    private Renderer[] pixelRenderers;
    public int PixelCount;
    
    // Use this for initialization
    void Start ()
    {
        PixelColors = new Color[PixelCount];
        
        for (int i = 0; i < PixelCount; i++)
        {
            PixelColors[i] = new Color();
        }

        pixelRenderers = new Renderer[PixelCount];

        for (int i = 0; i < PixelCount; i++)
        {
            GameObject obj = Instantiate(originalPixelObject);
            obj.transform.parent = originalPixelObject.transform.parent;
            float x = (float)Math.Sin(Math.PI * 2 / (float)PixelCount * i) / 2;
            float z = (float)Math.Cos(Math.PI * 2 / (float)PixelCount * i) / 2;
            obj.transform.localPosition = new Vector3(x, obj.transform.localPosition.y, z);
            pixelRenderers[i] = obj.GetComponentInChildren<Renderer>();
        }

        server = new OPCServer(7890);
        server.PacketReceived += Server_PacketReceived;

        
        originalPixelObject.GetComponentInChildren<MeshRenderer>().enabled = false;
    }

    private void Server_PacketReceived(object sender, OPCPacketEventArgs e)
    {
        for(int i = 0; i < PixelCount && i < e.Packet.Data.Length / 3; i++)
        {
            int offset = i * 3;
            float red = (float)e.Packet.Data[offset + 0] / 255.0f;
            float green = (float)e.Packet.Data[offset + 1] / 255.0f;
            float blue = (float)e.Packet.Data[offset + 2] / 255.0f;
            PixelColors[i] = new Color(red, green, blue);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        for(int i = 0; i < PixelCount; i++)
        {
            Renderer targetRenderer = pixelRenderers[i];
            Color color = PixelColors[i];
            float VolumetricOpacity = 1.0f;
            Color newColor = new Color(color.r, color.g, color.b, VolumetricOpacity);
            targetRenderer.material.color = newColor;
            Color emissionColor = new Color(color.r * VolumetricOpacity, color.g * VolumetricOpacity, color.b * VolumetricOpacity);
            targetRenderer.material.SetColor("_EmissionColor", emissionColor);
        }
    }
}
