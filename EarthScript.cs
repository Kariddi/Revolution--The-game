using UnityEngine;
using System.Collections;

public class EarthScript : MonoBehaviour {
	
	public float force_strength;
	
	//float G_force = 6.67e-11;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter ( Collider other) {
    	//Debug.Log("Collision");
		SphereCollider external_collider = transform.GetComponent<SphereCollider>();
		SphereCollider surface_collider = transform.Find("StarSurface").transform.GetComponent<SphereCollider>();
		Vector3 distance = this.transform.position - other.transform.position;
		float mag = Mathf.Sqrt(this.rigidbody.mass / distance.magnitude);
		float entry_angle = Mathf.Abs(Vector3.Angle(distance, other.rigidbody.velocity));
		float surface_radius = this.transform.localScale.x 
			* this.transform.Find("StarSurface").transform.localScale.x 
			* surface_collider.radius;
		Debug.Log("angle: " + entry_angle + " radius: " + surface_radius 
		          + " distance: " + distance.magnitude
		          + " tan:" + Mathf.Tan(entry_angle * Mathf.Deg2Rad)
		          + " altro: " + distance.magnitude * Mathf.Tan(entry_angle * Mathf.Deg2Rad));
		if((distance.magnitude * Mathf.Tan(entry_angle * Mathf.Deg2Rad)) > (3 * surface_radius) && entry_angle < 65.0f){
			if(other.rigidbody.velocity.magnitude < 1.4*mag){
				
			}
			/*Vector3 vel = other.rigidbody.velocity;
			vel.Normalize();
			vel *= mag*1.5f;
			other.rigidbody.velocity = vel;*/
		}
		if(entry_angle >= 65.0f){
			if(other.rigidbody.velocity.magnitude > 0.80*mag && other.rigidbody.velocity.magnitude < 1.20*mag){
				Vector3 vel = other.rigidbody.velocity;
				Vector3.OrthoNormalize(ref distance, ref vel);
				vel *= mag;
				other.rigidbody.velocity = vel;
			}
		}
	}
	
	void OnTriggerStay( Collider other ) 
	{
		/*Debug.Log("Star x: " + transform.position.x + " y: " + transform.position.y + " z: " 
		          + transform.position.z + " - Moon x: " + other.transform.position.x + " y: " + 
		          other.transform.position.y + " z: " + other.transform.position.z);*/
		
		Vector3 distance = transform.position - other.transform.position;
		SphereCollider collider = transform.GetComponent<SphereCollider>();
		//Debug.Log("Distance " + delta.magnitude);
		//Debug.Log("Radius : " + collider.radius + " - Scale: " + transform.localScale.x);
		//if(delta.magnitude > (collider.radius * transform.localScale.x / 1.5)){
		//	other.rigidbody.AddForce(delta.normalized*force_strength*rigidbody.mass/**delta.magnitude*/);
		//}
		//else
		//Vector3 direction = delta.normalized;	
		//other.rigidbody.AddForce(delta.normalized*force_strength*rigidbody.mass/Mathf.Pow(delta.magnitude,2) );
		//Debug.Log("Magnitude: " + delta.magnitude + " - Pow: " + Mathf.Pow(delta.magnitude,2));
		//float mag = other.rigidbody.velocity.magnitude;
		//Vector3.OrthoNormalize(delta, other.rigidbody.velocity);
		//other.rigidbody.velocity.magnitude = mag;
		
		/* Moto circolare intorno alla stella. */
		//other.rigidbody.AddForce(distance.normalized * Mathf.Pow(other.rigidbody.velocity.magnitude,2) * 
		//                         other.rigidbody.mass  / (collider.radius * transform.localScale.x));
		/*Debug.Log("force1: " + (distance.normalized * Mathf.Pow(other.rigidbody.velocity.magnitude,2) * 
		                         other.rigidbody.mass  / (collider.radius * transform.localScale.x)).magnitude + 
		          " force2: " + (distance.normalized * this.rigidbody.mass * other.rigidbody.mass 
		                         / distance.magnitude).magnitude +
		          " velocity: " + other.rigidbody.velocity.magnitude);*/
		/* Forza gravitazione universale. */
		other.rigidbody.AddForce(distance.normalized * this.rigidbody.mass * other.rigidbody.mass 
		                         /* * G_force */ / Mathf.Pow(distance.magnitude,2));
	}
}
