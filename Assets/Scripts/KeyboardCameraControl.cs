using UnityEngine;

public class KeyboardCameraControl : MonoBehaviour {
	
	public float speed = 0.1f;
	public float mouseLookSpeed = 3f;

	private bool fullscreenDeafness = false;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		this.transform.Rotate(Vector3.left, Input.GetAxis("Mouse Y") * mouseLookSpeed);
		this.transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * mouseLookSpeed);
		
		transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, this.transform.localEulerAngles.y, 0);
		
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

		if(Input.GetKey(KeyCode.F) && !fullscreenDeafness)
		{
			Screen.fullScreen = !Screen.fullScreen;
			fullscreenDeafness = true;
		}

		if(!Input.GetKey(KeyCode.F) && fullscreenDeafness)
		{
			fullscreenDeafness = false;
		}
		
		float y = this.transform.position.y;
		 
		this.transform.Translate(0, 2 - y, 0, Space.World);
	}
}
