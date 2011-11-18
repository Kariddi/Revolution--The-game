using UnityEngine;
using System.Collections;

public class ExternalGravitationalFieldScript : MonoBehaviour {
	
	float G_const = 6.67f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay( Collider other ) 
	{
		if(other.transform.CompareTag("Planet")){
			/* If the planet has been already destroyed do nothing. */
			if(!((PlanetScript) other.GetComponent("PlanetScript")).isDestroyed() 
			   && ((PlanetScript) other.GetComponent("PlanetScript")).isOrbitant()){
				/* Distance vector of the planet from the star. */	
				Vector3 distance = transform.position - other.transform.position;
				
				/* Approximation for the universal gravitation force. */
				other.rigidbody.AddForce(distance.normalized 
				                         * transform.parent.rigidbody.mass 
				                         * other.rigidbody.mass
				                         * G_const / Mathf.Pow(distance.magnitude,2));
			}
		}

	}
}
