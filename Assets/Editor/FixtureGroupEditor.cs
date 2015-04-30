using UnityEditor;
using System.Xml.Linq;
using System.Collections.Generic;

 [CustomEditor(typeof(FixtureGroup))] 
public class FixtureGroupEditor : Editor 
{
	List<string> fixtureIDs;

	public override void OnInspectorGUI  () 
	{
		if(fixtureIDs == null)
		{
			LoadFixtureIDs();
		}
		FixtureGroup lightGroup = (FixtureGroup) target;
		lightGroup.DMXStart = EditorGUILayout.IntField("DMX Start", lightGroup.DMXStart);
		int selectedID = fixtureIDs.IndexOf(lightGroup.FixtureID);
		string[] options = fixtureIDs.ToArray();
		
		lightGroup.FixtureID = fixtureIDs[EditorGUILayout.Popup("Fixture", selectedID, options)];
	}
		
	private void LoadFixtureIDs()
	{
		fixtureIDs = new List<string>();
		XDocument doc = XDocument.Load("fixtures.xml");

		//find the fixture definition
		foreach(XElement fixtureElement in doc.Root.Elements("fixture"))
		{
			fixtureIDs.Add(fixtureElement.Attribute("id").Value);	
		}
		
	}
	
}
