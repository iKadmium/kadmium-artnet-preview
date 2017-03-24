using UnityEditor;
using UnityEngine;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO;

 /*[CustomEditor(typeof(Fixture))] 
public class FixtureEditor : Editor 
{
    List<string> fixtureIDs;
    List<string> manufacturers;

	public override void OnInspectorGUI  () 
	{
        Fixture fixture = (Fixture) target;
		fixture.DMXStart = EditorGUILayout.IntField("DMX Start", fixture.DMXStart);
        fixture.VolumetricOpacity = EditorGUILayout.FloatField("Volumetric Opacity", fixture.VolumetricOpacity);
        fixture.Light = (Light)EditorGUILayout.ObjectField("Light Object", fixture.Light, typeof(Light), true);
        fixture.ConeRenderer = (Renderer)EditorGUILayout.ObjectField("Cone renderer", fixture.ConeRenderer, typeof(Renderer), true);        
    }
	
}
*/