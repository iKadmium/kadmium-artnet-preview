using UnityEditor;
using UnityEngine;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO;

 [CustomEditor(typeof(Fixture))] 
public class FixtureEditor : Editor 
{
    public static string fixturesLocation = Fixture.FixturesLocation;
	List<string> fixtureIDs;

	public override void OnInspectorGUI  () 
	{
		if(fixtureIDs == null)
		{
			LoadFixtureIDs();
		}
		Fixture fixture = (Fixture) target;
		fixture.DMXStart = EditorGUILayout.IntField("DMX Start", fixture.DMXStart);
        fixture.VolumetricOpacity = EditorGUILayout.FloatField("Volumetric Opacity", fixture.VolumetricOpacity);
        fixture.Light = (Light)EditorGUILayout.ObjectField("Light Object", fixture.Light, typeof(Light), true);
        fixture.ConeRenderer = (Renderer)EditorGUILayout.ObjectField("Cone renderer", fixture.ConeRenderer, typeof(Renderer), true);
		int selectedID = fixtureIDs.IndexOf(fixture.FixtureID);
		string[] options = fixtureIDs.ToArray();
		
		fixture.FixtureID = fixtureIDs[EditorGUILayout.Popup("Fixture", selectedID, options)];
	}
		
	private void LoadFixtureIDs()
	{
		fixtureIDs = new List<string>();
		foreach(string file in Directory.GetFiles(fixturesLocation, "*.json"))
		{
			fixtureIDs.Add(Path.GetFileNameWithoutExtension(file));
		}
	}
	
}
