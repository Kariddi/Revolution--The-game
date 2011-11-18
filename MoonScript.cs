using UnityEngine;
using System.Collections;

public class MoonScript : MonoBehaviour {
	
	public float impulse_strength;
	public bool to_launch = false;
	
	Vector3 offset;
	Vector3 screenPoint;
	
	Vector3 force = new Vector3(0.0f, 0.0f, 0.0f);
	Vector3 original_position;
	Quaternion original_rotation;
	
	/* 0 - none
	 * 1 - circular 
	 * 2 - elliptical 
	 * 3 - impact */
	private int current_status;
	
	// Use this for initialization
	void Start () {
		//transform.position = new Vector3(transform.position.x, transform.position.y, GameObject.Find("Controller").GetComponent<ControllerScript>().universal_z);
		original_position = transform.position;
		original_rotation = transform.rotation;
		rigidbody.isKinematic = true;
		current_status = 0;
	}
	
	// Update is called once per frame
	void Update () {

		if (to_launch)
		{	
			screenPoint = Camera.main.WorldToScreenPoint(transform.position);
			Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
			//pos.z = GameObject.Find("Controller").GetComponent<ControllerScript>().universal_z;
			transform.position = pos;
		}
	}
	
	void OnBecameInvisible()
	{
		if(!to_launch)
		{
			TextMesh text = GameObject.Find("InfoText").GetComponent<TextMesh>();
			text.text = "Fail!";
			updatePoints();
			text.animation.Play("InformationAnimation");
			GameObject.Destroy(this.gameObject);
		}
	}
	
	IEnumerator WaitForAnimation(float wait_time)
	{
		yield return new WaitForSeconds(wait_time);
		ReloadState();
	}
	
	public void ReloadState()
	{
		to_launch = true;
		transform.position = original_position;
		transform.rotation = original_rotation;
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		rigidbody.isKinematic = true;
	}
	
	void OnMouseDown()
	{
		to_launch = false;
		rigidbody.isKinematic = false;	
	}
	
	void OnMouseUp()
	{
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
		Vector3 distance = transform.position - mousePosition;
		distance.z = 0;
		force = distance*impulse_strength;
		rigidbody.AddForce(force, ForceMode.Impulse);
		GameObject.Find("Controller").GetComponent<ControllerScript>().CreateNewPlanet();
	}
	
	/*
	void OnMouseDrag()
	{
    	Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

    	Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint); //+ offset;
		
    	transform.position = curPosition;
	}
	*/
	/*
		void OnTriggerEnter ( Collider other) {
    Debug.Log("Collision");
}*/
	
	void setStatus(int new_status){
		current_status = new_status;
	}
	
	void updatePoints(){
		//GameObject.Find("Controller").GetComponent<ControllerScript>().points -= (rigidbody.mass*GameObject.Find("Controller").GetComponent<ControllerScript>().planet_points*GameObject.Find("Controller").GetComponent<ControllerScript>().lost_planet_factor);
		//	if (GameObject.Find("Controller").GetComponent<ControllerScript>().points < 0)
		//		GameObject.Find("Controller").GetComponent<ControllerScript>().points = 0;
		//	Debug.Log("points:" + GameObject.Find("Controller").GetComponent<ControllerScript>().points);
	}

}
