using UnityEngine;
using System.Collections.Generic;
using System.Net;
using System;

public class PreviewJson
{
    public string[] groups;
    public PreviewUniverseJson[] universes;
}



public class Venue : MonoBehaviour
{
    WebClient client;
    
    public Universe[] Universes;
    private Dictionary<int, Universe> universeMap;

    // Use this for initialization
    public void Start()
    {
        universeMap = new Dictionary<int, Universe>();
        foreach(Universe universe in Universes)
        {
            universeMap.Add(universe.UniverseID, universe);
        }

        client = new WebClient();
        client.DownloadStringCompleted += Client_DownloadStringCompleted;
        client.DownloadStringAsync(new System.Uri(Settings.previewURL));
    }

    private void Client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
    {
        PreviewJson previewData = JsonUtility.FromJson<PreviewJson>(e.Result);
        foreach(var universeJson in previewData.universes)
        {
            Universe universe = universeMap[universeJson.universeID];
            universe.SetFixtureDefinitions(universeJson.fixtures);
        }
    }
    
    // Update is called once per frame
    public void Update()
    {
    }

    public void ProcessDMX(short universeID, byte[] data)
    {
        if (universeMap.ContainsKey(universeID))
        {
            Universe universe = universeMap[universeID];
            universe.ProcessDMX(data);
        }
    }
}
