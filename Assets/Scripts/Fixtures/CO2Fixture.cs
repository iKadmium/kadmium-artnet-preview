using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CO2Fixture : MonoBehaviour {

    private ParticleSystem Particles;
    private DMXFixture Fixture;
    private LightFixture LightFixture;

    // Use this for initialization
    void Start () {
        Particles = GetComponentInChildren<ParticleSystem>();
        Fixture = GetComponent<DMXFixture>();
        LightFixture = GetComponent<LightFixture>();
    }
	
	// Update is called once per frame
	void Update () {
        var emission = Particles.emission;
        var main = Particles.main;

        main.startColor = LightFixture.GetPrimaryColor();

        if (Fixture.Attributes.ContainsKey("CO2"))
        {
            emission.rateOverTimeMultiplier = (float)Fixture.Attributes["CO2"].Value;
        }
        else
        {
            emission.rateOverTimeMultiplier = 0;
        }
		
	}
}
