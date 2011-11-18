using UnityEngine;
using System.Collections;

public class PlanetSurfaceScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other)
	{
		/* Get the Detonator script component */
		Detonator det = (Detonator) other.GetComponent("Detonator");
		/* Calla Explode method defined in Detonator script. Simply show an explosion. */
		det.Explode();
		/* Destroy the rigidboy of the collided planet. In this way it is no more subject to forces
		   and has no speed. */
		Destroy(other.rigidbody);
		/* Hide the collided planet. */
		other.transform.position = transform.position;
		/* Destroy the game object of the planet. */
		Destroy(other.gameObject, 3);
	}
}
