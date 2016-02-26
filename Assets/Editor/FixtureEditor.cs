using UnityEditor;
using UnityEngine;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO;

 [CustomEditor(typeof(Fixture))] 
public class FixtureEditor : Editor 
{
    public static string fixturesLocation = Fixture.fixturesLocation;
	List<string> fixtureIDs;

	public override void OnInspectorGUI  () 
	{
		if(fixtureIDs == null)
		{
			LoadFixtureIDs();
		}
		Fixture lightGroup = (Fixture) target;
		lightGroup.DMXStart = EditorGUILayout.IntField("DMX Start", lightGroup.DMXStart);
        lightGroup.VolumetricOpacity = EditorGUILayout.FloatField("Volumetric Opacity", lightGroup.VolumetricOpacity);
        lightGroup.LightConeMaterial = (Material)EditorGUILayout.ObjectField("Light Cone Material", lightGroup.LightConeMaterial, typeof(Material), false);
		int selectedID = fixtureIDs.IndexOf(lightGroup.FixtureID);
		string[] options = fixtureIDs.ToArray();
		
		lightGroup.FixtureID = fixtureIDs[EditorGUILayout.Popup("Fixture", selectedID, options)];
	}
		
	private void LoadFixtureIDs()
	{
		fixtureIDs = new List<string>();
		foreach(string file in Directory.GetFiles(fixturesLocation, "*.xml"))
		{
			fixtureIDs.Add(Path.GetFileNameWithoutExtension(file));
		}
	}
	
}
