using UnityEngine;
using System.Collections;

public class StarGravitationalFieldScript : MonoBehaviour {
	
	/* This is a sort of approximation for the gravitational costant G. */
	float G_const = 6.67f;
		
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	/* When the planet enter in the gravitational field of the star, its velocity and 
	 * entry angle is evaluated: if these parameters fall in specific ranges, then 
	 * they are "adapted" in order to obtain circular or elliptical orbits. */
	void OnTriggerEnter ( Collider other) {
		if(other.transform.CompareTag("Planet")){
			/* If the planet has been already destroyed do nothing. */
			if(((PlanetScript) other.GetComponent("PlanetScript")).isDestroyed())
				return;
		
			/* Get the sphere colliders for the gravitational field and for the surface of the star. */
			SphereCollider external_collider = transform.GetComponent<SphereCollider>();
			SphereCollider surface_collider = transform.parent.transform.GetComponent<SphereCollider>();
			/* Distance vector between the planet and the star. */
			Vector3 distance = this.transform.position - other.transform.position;
		
			float entry_angle = Mathf.Abs(Vector3.Angle(distance, other.rigidbody.velocity));
			float surface_radius = this.transform.parent.transform.localScale.x 
				* surface_collider.radius;
	
			/* If the entry angle is to small, there will probably be an impact on the star.
			 * If the entry angle is not to large (less than 65 degrees) than the planet can be
			 * evalueted to obtain an elliptical orbit.
			 * If the entry angle is greater than 65 degrees, the possibility for the circular
			 * orbit is evalueted.
			 * The further evaluations (both for elliptical and circular orbits) are done considering
			 * the planet velocity. */
		
			float C = 2*G_const*transform.parent.rigidbody.mass/(distance.magnitude*Mathf.Pow(other.rigidbody.velocity.magnitude,2));
			float a = (distance.magnitude / 2)*(C*(-1)/(1-C));
			float ra = distance.magnitude*(Mathf.Sqrt(Mathf.Pow(C,2) + 4*(1-C)*(Mathf.Pow(Mathf.Sin((180-entry_angle) * Mathf.Deg2Rad),2))) - C)/(2*(1-C));
			Debug.Log("Pos: " + other.transform.position + " vel: " + other.rigidbody.velocity.normalized
			          + " a: " + a + " radius: " + distance.magnitude + " ra: " + ra
			          + " e: " + ((Mathf.Pow(other.rigidbody.velocity.magnitude,2)/2) - (G_const*transform.parent.rigidbody.mass/distance.magnitude)));
		
			if((distance.magnitude * Mathf.Tan(entry_angle * Mathf.Deg2Rad)) > (3 * surface_radius) && entry_angle < 65.0f){
				/* Make an approximation for the eccentricity of the elliptical orbit. The eccentricity is
				 * in inverse proportion to the entry_angle*/
				float eccentricity = 1 - (entry_angle / 90)/2;
				/* TODO...explanation for velocity calculus...*/
				float velocity_magnitude = Mathf.Sqrt(transform.parent.rigidbody.mass * G_const 
				                       * (1 - eccentricity)/ distance.magnitude );
				/* If the planet has a velocity similar to the one needed for an elliptical orbit,
				 * its velocity is adapted to fit the correct trajectory. */
				if(other.rigidbody.velocity.magnitude > 0.80 * velocity_magnitude 
				   && other.rigidbody.velocity.magnitude < 1.20 * velocity_magnitude){
					Debug.Log("elliptical");
					/* Orthonormalize the velocity respect to the distance vector and then
					 * assign to it the correct magnitude. */
					Vector3 vel = other.rigidbody.velocity;
					Vector3.OrthoNormalize(ref distance, ref vel);
					vel *= velocity_magnitude;
					other.rigidbody.velocity = vel;
				}
			}
		
			/* Circular orbit. */
			if(entry_angle >= 65.0f){
				/* Calculate the velocity for the circular trajectory. */
				float mag = Mathf.Sqrt(transform.parent.rigidbody.mass * G_const / distance.magnitude);
				/* If the planet has a velocity similar to the one needed for a circular orbit,
				 * its velocity is adapted to fit the correct trajectory. */
				if(other.rigidbody.velocity.magnitude > 0.80*mag && other.rigidbody.velocity.magnitude < 1.20*mag){
					Debug.Log("circular");
					Vector3 vel = other.rigidbody.velocity;
					Vector3.OrthoNormalize(ref distance, ref vel);
					vel *= mag;
					other.rigidbody.velocity = vel;
				}
			}
		}
	}
	
	void OnTriggerStay( Collider other ) 
	{
		if(other.transform.CompareTag("Planet")){
			/* If the planet has been already destroyed do nothing. */
			if(!((PlanetScript) other.GetComponent("PlanetScript")).isDestroyed()){
				/* Distance vector of the planet from the star. */	
				Vector3 distance = transform.position - other.transform.position;

				/* Approximation for the universal gravitation force. */
				other.rigidbody.AddForce(distance.normalized * transform.parent.rigidbody.mass * other.rigidbody.mass
				                         * G_const / Mathf.Pow(distance.magnitude,2));
			}
		}

	}
}
