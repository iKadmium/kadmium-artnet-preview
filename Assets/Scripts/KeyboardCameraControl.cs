using UnityEngine;
using System.Collections;

public class KeyboardCameraControl : MonoBehaviour {
	
	float speed = 0.1f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.W))
		{
			this.transform.Translate(0.0f, 0.0f, speed);
		}
		if(Input.GetKey(KeyCode.S))
		{
			this.transform.Translate(0.0f, 0.0f, -speed);
		}
		if(Input.GetKey(KeyCode.A))
		{
			this.transform.Translate(-speed, 0.0f, 0.0f);
		}
		if(Input.GetKey(KeyCode.D))
		{
			this.transform.Translate(speed, 0.0f, 0.0f);
		}
	}
}
