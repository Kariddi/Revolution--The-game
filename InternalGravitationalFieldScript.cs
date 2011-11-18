using UnityEngine;
using System.Collections;

public class InternalGravitationalFieldScript : MonoBehaviour {
		
	float G_const = 6.67f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter ( Collider other) {
		if(other.transform.CompareTag("Planet")){
			if(((PlanetScript) other.GetComponent("PlanetScript")).isDestroyed() ||
			   ((PlanetScript) other.GetComponent("PlanetScript")).isOrbitant())
				return;
			
			((PlanetScript) other.GetComponent("PlanetScript")).orbitantEnable();
			
			/* Get the sphere colliders for the gravitational field and for the surface of the star. */
			SphereCollider internal_collider = transform.GetComponent<SphereCollider>();
			SphereCollider external_collider = transform.parent.FindChild("ExternalStarGravitationalField").transform.GetComponent<SphereCollider>();
			SphereCollider surface_collider = transform.parent.transform.GetComponent<SphereCollider>();
			SphereCollider planet_collider = other.transform.GetComponent<SphereCollider>();
			
			/* Calculate sphere collider radius. */
			float int_radius = internal_collider.radius 
					           * this.transform.localScale.x 
					           * transform.parent.transform.localScale.x;
			float ext_radius = external_collider.radius 
							   * transform.parent.transform.localScale.x 
							   * transform.parent.FindChild("ExternalStarGravitationalField").transform.localScale.x;
			float surface_radius = surface_collider.radius * transform.parent.transform.localScale.x;
			float planet_radius = other.transform.localScale.x * planet_collider.radius;
			
			/* Distance vector between the planet and the star. */
			Vector3 distance = this.transform.position - other.transform.position;
			
			/* Entry angle. */
			float entry_angle = Mathf.Abs(Vector3.Angle(distance, other.rigidbody.velocity));
			float gamma = (180 - entry_angle) * Mathf.Deg2Rad;

			float C = (2 * G_const * transform.parent.rigidbody.mass) 
					  / (distance.magnitude * Mathf.Pow(other.rigidbody.velocity.magnitude, 2));
			
			/* Calculate specific orbital energy. */
			float energy = ((Mathf.Pow(other.rigidbody.velocity.magnitude, 2) / 2)
			               - (G_const * transform.parent.rigidbody.mass / distance.magnitude));
			
			/* Calculate the specific angular momentum. */
			Vector3 specific_angular_momentum = Vector3.Cross(-distance, (other.rigidbody.velocity * other.rigidbody.mass)) / other.rigidbody.mass;
						
			/* If the specific orbital energy is negative, the planet will execute an elliptical orbit. */
			if(energy < 0){
				/* Calculate the periapsis and apoapsis distance to check if the planet orbit
				 * will remain inside the gravitational field of the star with an elliptical orbit. */
				float apoapsis_dis = distance.magnitude * (Mathf.Sqrt(Mathf.Pow(C, 2) + 4 * (1 - C) 
				                     * (Mathf.Pow(Mathf.Sin(gamma), 2))) - C) / (2 * (1 - C));
				float periapsis_dis = distance.magnitude * (-C - Mathf.Sqrt((Mathf.Pow(C, 2) + 4 * (1 - C) 
				                      * (Mathf.Pow(Mathf.Sin(gamma), 2))))) / (2 * (1 - C));
				float major_semiaxis = apoapsis_dis + periapsis_dis;
				float eccentricity = Mathf.Sqrt(1 - (Mathf.Pow(specific_angular_momentum.magnitude,2)
			                         / (G_const * transform.parent.rigidbody.mass * major_semiaxis)));
				if(apoapsis_dis < periapsis_dis){
					float tmp = apoapsis_dis;
					apoapsis_dis = periapsis_dis;
					periapsis_dis = tmp;
				}

				if(apoapsis_dis < ext_radius && periapsis_dis > (surface_radius + planet_radius)){
					/* The planet has a closed elliptical orbit around the star. 
					 * Update the Score. */
					GameObject.Find("Controller").GetComponent<ControllerScript>().updateScore(other.rigidbody.mass 
					           * other.GetComponent<PlanetScript>().getPlanetPoints()
					           * transform.parent.GetComponent<StarScript>().getMultiplier());
					/* Add the planet to the orbitant planet list of the star. */
					transform.parent.GetComponent<StarScript>().addOrbitantPlanet(other.GetInstanceID());
					/* Increase multiplier. */
					transform.parent.GetComponent<StarScript>().updateMultiplier();
					
					Debug.Log("Score:" + GameObject.Find("Controller").GetComponent<ControllerScript>().getScore());
					
					Debug.Log("Elliptical orbit - major-semiaxis: " + major_semiaxis 
				          + " periapsis_dis: " + periapsis_dis 
				          + " apoapsis_dis: " + apoapsis_dis
					      + " eccentricity: " + eccentricity);
				}
				else if(periapsis_dis < (surface_radius + planet_radius))
					Debug.Log("BOOOOOOOOOOOM!!!!");
				
				else if(apoapsis_dis > ext_radius)
					Debug.Log("Adios - elliptical orbit too large");
			}

			/* If the specific orbital energy is positive, the planet will execute an hyperbolic orbit. */
			else if(energy > 0){
				/* Calculate hyperbola parameters. */
				float major_semiaxis = G_const * transform.parent.rigidbody.mass / (-2 * energy);
				float eccentricity = Mathf.Sqrt(1 - (Mathf.Pow(specific_angular_momentum.magnitude, 2)
			                         / (G_const * transform.parent.rigidbody.mass * major_semiaxis)));
				float focal_semidistance = major_semiaxis * eccentricity;
				float minor_semiaxis = Mathf.Sqrt(Mathf.Pow(focal_semidistance, 2) 
				                       - Mathf.Pow(major_semiaxis, 2));
				float periapsis_dis = major_semiaxis * (1 - eccentricity);
				
				/* If the periapsis is not too small, the planet won't impact with the star. */
				if(periapsis_dis > (surface_radius + planet_radius)){
					Debug.Log("Adios - hyperbolic orbit - major_semiaxis: " + major_semiaxis 
				          + " eccentricity: " + eccentricity 
				          + " focal_semidistance: " + focal_semidistance 
				          + " minor_semiaxis: " + minor_semiaxis
				          + " periapsis: " + periapsis_dis);
				}
				else
					Debug.Log("BOOOOOOOOOOOM!!!!");
			}
			
			/* If the specific orbital energy is null, the planet will execute a parabolic orbit. */
			else if(energy > 0){
				/* TODO... */
			}
		}
	}
}
