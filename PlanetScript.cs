using UnityEngine;
using System.Collections;
//using GameScript;

public class PlanetScript : MonoBehaviour {

    public float impulse_strength;
    public bool to_launch = false;
    
    Vector3 offset;
    Vector3 screenPoint;

    Vector3 force = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 original_position;
    Quaternion original_rotation;
	
	private int planet_points = 50;
	private bool destroyed = false;
	private bool orbitant = false;
	private ControllerScript game_controller;

    // Use this for initialization
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        original_position = transform.position;
        original_rotation = transform.rotation;
        rigidbody.isKinematic = true;
		game_controller = GameObject.Find("Controller").GetComponent<ControllerScript>();
	}

    // Update is called once per frame
    void Update()
    {
        if (to_launch)
        {
            screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
            pos.z = 0;
            transform.position = pos;
        }
		else
		{
			CheckPlanetPosition();
		}
    }
	
	void CheckPlanetPosition()
	{
		if ((this.transform.position.x > game_controller.field_rectangle_size.x || 
		    this.transform.position.y > game_controller.field_rectangle_size.y) &&
		    !this.isDestroyed())
		{
				/* Show fail message. */
            	TextMesh text = GameObject.Find("InfoText").GetComponent<TextMesh>();
            	text.text = "Fail!";
            	text.animation.Play("InformationAnimation");
			
				/* Update the score. */
				GameObject.Find("Controller").GetComponent<ControllerScript>().updateScore(-rigidbody.mass  
			           * GameObject.Find("Controller").GetComponent<ControllerScript>().getLostPlanetFactor()
			           * getPlanetPoints());
				Debug.Log("Score:" + GameObject.Find("Controller").GetComponent<ControllerScript>().getScore());
			
				/* Destroy the GameObject. */
				foreach(GameObject star in GameObject.FindGameObjectsWithTag("Star")){
						star.GetComponent<StarScript>().removeOrbitantPlanet(this.GetInstanceID());
						star.GetComponent<StarScript>().updateMultiplier();
				}
            	GameObject.Destroy(this.gameObject);
		}
	}
	
  /*void OnBecameInvisible()
   // {
  //      if (!to_launch)
  //      {
//			if(!this.isDestroyed()){
//				/* Show fail message. 
 //           	TextMesh text = GameObject.Find("InfoText").GetComponent<TextMesh>();
  //          	text.text = "Fail!";
   //         	text.animation.Play("InformationAnimation");
			
//				/* Update the score.
				GameObject.Find("Controller").GetComponent<ControllerScript>().updateScore(-rigidbody.mass  
			           * GameObject.Find("Controller").GetComponent<ControllerScript>().getLostPlanetFactor()
			           * getPlanetPoints());
				Debug.Log("Score:" + GameObject.Find("Controller").GetComponent<ControllerScript>().getScore());
			
				/* Destroy the GameObject. 
				foreach(GameObject star in GameObject.FindGameObjectsWithTag("Star")){
						star.GetComponent<StarScript>().removeOrbitantPlanet(this.GetInstanceID());
						star.GetComponent<StarScript>().updateMultiplier();
				}
            	GameObject.Destroy(this.gameObject);
			}
        }
    }
	 */
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
        force = distance * impulse_strength;
        rigidbody.AddForce(force, ForceMode.Impulse);
        GameObject.Find("Controller").GetComponent<ControllerScript>().CreateNewPlanet();
    }
	
	public void setDestroyed(){
		destroyed = true;
	}
	
	public bool isDestroyed(){
		return destroyed;
	}
	
	public void orbitantEnable(){
		orbitant = true;
	}
	
	public void orbitantDisable(){
		orbitant = true;
	}
	
	public bool isOrbitant(){
		return orbitant;
	}
	
	public int getPlanetPoints(){
		return planet_points;
	}
}
