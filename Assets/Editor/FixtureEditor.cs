using UnityEditor;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO;

 [CustomEditor(typeof(Fixture))] 
public class FixtureEditor : Editor 
{
	public string fixturesLocation = @"D:\User\IntelliJ Workspace\kadmium-osc-dmx\out\artifacts\jar\data\fixtures";

	List<string> fixtureIDs;

	public override void OnInspectorGUI  () 
	{
		if(fixtureIDs == null)
		{
			LoadFixtureIDs();
		}
		Fixture lightGroup = (Fixture) target;
		lightGroup.DMXStart = EditorGUILayout.IntField("DMX Start", lightGroup.DMXStart);
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
