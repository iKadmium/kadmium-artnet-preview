using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFixture : MonoBehaviour {

    private ParticleSystem Particles;
    private DMXFixture Fixture;

    // Use this for initialization
    void Start () {
        Particles = GetComponentInChildren<ParticleSystem>();
        Fixture = GetComponent<DMXFixture>();
    }
	
	// Update is called once per frame
	void Update () {
        var em = Particles.emission;
        if (Fixture.Attributes.ContainsKey("Fire"))
        {
            em.rateOverTimeMultiplier = (float)Fixture.Attributes["Fire"].Value;
        }
        else
        {
            em.rateOverTimeMultiplier = 0;
        }
		
	}
}
