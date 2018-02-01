using UnityEngine;
using System;

public class KeyboardCameraControl : MonoBehaviour {
	
	public float walkSpeed = 0.1f;
	public float mouseLookSpeed = 3f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		this.transform.Rotate(Vector3.left, Input.GetAxis("Mouse Y") * mouseLookSpeed);
		this.transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * mouseLookSpeed);
		
		transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, this.transform.localEulerAngles.y, 0);

        this.transform.Translate(Input.GetAxis("Horizontal") * walkSpeed, 0, 0);
        this.transform.Translate(0, 0, Input.GetAxis("Vertical") * walkSpeed);
        
        if(Input.GetButtonDown("Full Screen"))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
        
        if(Input.GetButtonDown("Next Resolution"))
        {
            var currentResolution = Screen.currentResolution;
            var resolutions = Screen.resolutions;
            var currentResolutionIndex = Array.IndexOf(resolutions, currentResolution);
            if(currentResolutionIndex < (resolutions.Length - 1))
            {
                var nextResolution = resolutions[currentResolutionIndex + 1];
                Screen.SetResolution(nextResolution.width, nextResolution.height, Screen.fullScreen);
            }
        }

        if (Input.GetButtonDown("Previous Resolution"))
        {
            var currentResolution = Screen.currentResolution;
            var resolutions = Screen.resolutions;
            var currentResolutionIndex = Array.IndexOf(resolutions, currentResolution);
            if (currentResolutionIndex > 0)
            {
                var nextResolution = resolutions[currentResolutionIndex - 1];
                Screen.SetResolution(nextResolution.width, nextResolution.height, Screen.fullScreen);
            }
        }

        float y = this.transform.position.y;
		 
		this.transform.Translate(0, 2 - y, 0, Space.World);
	}
}
