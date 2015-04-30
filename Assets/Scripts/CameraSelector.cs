using UnityEngine;

public class CameraSelector : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.BackQuote))
		{
			SelectCamera(GameObject.Find("CameraPerspective").GetComponent<Camera>());
		}
		else if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			SelectCamera(GameObject.Find("Camera1").GetComponent<Camera>());
		}
		else if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			SelectCamera(GameObject.Find("Camera2").GetComponent<Camera>());
		}
		else if(Input.GetKeyDown(KeyCode.Alpha3))
		{
			SelectCamera(GameObject.Find("Camera3").GetComponent<Camera>());
		}
	}
	
	void SelectCamera(Camera selected)
	{
		foreach(Camera camera in Camera.allCameras)
		{
			camera.enabled = false;
		}
		selected.enabled = true;
	}
}
