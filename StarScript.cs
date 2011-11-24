using UnityEngine;
using System.Collections;


public class StarScript : MonoBehaviour {
	
	
	public float star_launch_limit = 20.0f;
	/* List containing planets orbitating around the star. */
	private ArrayList orbitantPlanetsList = null;
	
	/* Point multiplier. The value of the multiplier correspond to 
	 * the number of orbitant planets. */
	private int multiplier = 0;
	
	private float star_mass_limit = 5000;
	
	// Use this for initialization
	void Start () {
		orbitantPlanetsList = new ArrayList();
		multiplier = orbitantPlanetsList.Count + 1;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.transform.CompareTag("Planet")){
			/* If the planet has been already destroyed do nothing. */
			if(((PlanetScript) other.GetComponent("PlanetScript")).isDestroyed())
				return;
			/* Set the status of the planet to 0 => Planet destroyed. */
			((PlanetScript) other.GetComponent("PlanetScript")).setDestroyed();
			
			/* Update the score. */
			GameObject.Find("Controller").GetComponent<ControllerScript>().updateScore(-other.rigidbody.mass 
			           * other.GetComponent<PlanetScript>().getPlanetPoints());
			foreach(GameObject star in GameObject.FindGameObjectsWithTag("Star")){
					star.GetComponent<StarScript>().removeOrbitantPlanet(other.GetInstanceID());
					star.GetComponent<StarScript>().updateMultiplier();
			}
			Debug.Log("Score:" + GameObject.Find("Controller").GetComponent<ControllerScript>().getScore());
			
			/* Get the Detonator script component */
			Detonator det = (Detonator) other.GetComponent("Detonator");
			/* Call Explode method defined in Detonator script. Simply show an explosion. */
			det.Explode();
			/* Play an explosion sound. */
			other.GetComponent<AudioSource>().Play();
			/*Adds the planet mass to the star mass*/
			rigidbody.mass += other.rigidbody.mass;
			/* Destroy the rigidboy of the collided planet. In this way it is no more subject to forces
			   and has no speed. */
			Destroy(other.rigidbody);
			/* Hide the collided planet. */
			other.transform.position = transform.position;
			/* Destroy the game object of the planet. */
			Destroy(other.gameObject, 3);
		}
	}

	
	/* Add planet to orbitant planet list. */
	public void addOrbitantPlanet(int planet_ID){
		orbitantPlanetsList.Add(planet_ID);
	}
	
	/* Remove planet to orbitant planet list. */
	public void removeOrbitantPlanet(int planet_ID){
		orbitantPlanetsList.Remove(planet_ID);
	}
	
	public int getMultiplier(){
		return multiplier;
	}
	
	public void updateMultiplier(){
		multiplier = orbitantPlanetsList.Count + 1;
	}
	
}
