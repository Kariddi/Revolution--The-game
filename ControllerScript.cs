using UnityEngine;
using System.Collections;

public class ControllerScript : MonoBehaviour {
	
	private float score = 0;
	private float lost_planet_factor = 0.5f;
	private float screen_ratio = 0.0f;
	
	/* Define a GameObject referred to the PlanetPrefab. Remember to connect this variable to PlanetPrefab
	 * object (to do this select Controller in the Hierarchy menu and edit the Planet Prefab variable shown
	 * in the Ispector window). */
	public GameObject PlanetPrefab;
	public GameObject focused_star;
	public float camera_scroll_speed;
	public float horizontalPanSpeed;
	public float verticalPanSpeed;
	public float max_camera_distance;
	public float min_camera_distance;
	public Vector2 field_rectangle_size;
	GameObject current_launching_planet;
	Camera game_camera;
	bool camera_is_max_unzoom;
	bool is_launch_mode;
	bool is_animating_camera;
	
	// Use this for initialization
	void Start () {
		is_launch_mode = true;
		is_animating_camera = false;
		this.CreateNewPlanet();
		game_camera = (Camera) GameObject.Find("Game Camera").GetComponent<Camera>();
		screen_ratio = ((float) Screen.height) / Screen.width;
		camera_is_max_unzoom = ComputeMaxCameraDistance() > game_camera.transform.position.z ? false : true;
		/*if ( field_rectangle_size.x >= field_rectangle_size.y )
		{
			max_camera_distance = field_rectangle_size.x / ((game_camera.fieldOfView/180) * Mathf.PI);
		}
		else
		{
			max_camera_distance = field_rectangle_size.y / (((game_camera.fieldOfView/180)*screen_ratio) * Mathf.PI);
		}
		game_camera.far = max_camera_distance + 100;*/
	}
	
	// Update is called once per frame
	void Update () {
		DisplayPoints();
		/* It does not work. To be fixed. */
		if (Input.GetKeyDown (KeyCode.Escape))
			Application.Quit();
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			is_launch_mode = !is_launch_mode;
			if (!is_launch_mode)
			{
				Destroy(current_launching_planet);
			}
			else
			{
				CreateNewPlanet();
			}
		}
		
		Debug.Log(screen_ratio);
		Debug.Log("Width: " + Mathf.Sin((game_camera.fieldOfView/180) * Mathf.PI) * (game_camera.transform.position.z) + "Height: " + Mathf.Sin((game_camera.fieldOfView*screen_ratio/180) * Mathf.PI) * (game_camera.transform.position.z));
		
		//CheckIfPlanetIsOutsideField
		/*
		Rotation camera implementation
		// Use Raycast to pick objects that have mesh colliders attached.
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		
		//if the user touches the buttond
		if (Input.GetMouseButtonUp (0))  //Returns true during the frame the user deselects the object
		{
			if (Physics.Raycast (ray, out hit, 300)) 
			{
				focused_star = hit.transform.gameObject;
				//game_camera.transform.LookAt(focused_star.transform.position);
				is_animating_camera = true;
				Debug.Log("Hit a planet!");
			}
		}
		*/
		float wheel_axis = Input.GetAxis("Mouse ScrollWheel")*camera_scroll_speed;
		if (wheel_axis != 0.0f)
		{
			Debug.Log("Sono qui");
			float delta = wheel_axis*camera_scroll_speed;
			float current_camera_z = game_camera.transform.position.z;
			Debug.Log(delta);
			camera_is_max_unzoom = false;
			max_camera_distance = ComputeMaxCameraDistance();
			if (current_camera_z + delta > min_camera_distance && current_camera_z + delta < max_camera_distance)
			{
				Debug.Log("Pos: " + game_camera.transform.position.z + " " + current_camera_z);
				Debug.Log("New position guessed: " + (current_camera_z + delta));
				game_camera.transform.Translate(new Vector3(0.0f, 0.0f, delta), Space.World);
				Debug.Log("New position : " + (game_camera.transform.position.z));
			}
			else
			{
				if (current_camera_z + delta <= min_camera_distance)
				{
					game_camera.transform.position = new Vector3(game_camera.transform.position.x, game_camera.transform.position.y, min_camera_distance);
				}
				else
				{
					camera_is_max_unzoom = true;
					game_camera.transform.position = new Vector3(game_camera.transform.position.x, game_camera.transform.position.y, max_camera_distance);
				}
			}
			/*Vector2 camera_offsets = ComputeCameraFieldOffset();
			if (camera_offsets != Vector2.zero)
			{
				Debug.Log("Translation factors: " + camera_offsets.x + " " + camera_offsets.y);
				game_camera.transform.Translate(new Vector3(camera_offsets.x, camera_offsets.y, 0));
			}*/
		}
		
		if (Input.GetMouseButton(1))
		{
			Debug.Log("Secondo bottone mouse");
		    float h = -horizontalPanSpeed * Input.GetAxis("Mouse X");
            float v = -verticalPanSpeed * Input.GetAxis("Mouse Y");
			Debug.Log(h + " " + v);
			game_camera.transform.Translate(new Vector3(h, v, 0));
		}
		
		Vector2 camera_offsets = ComputeCameraFieldOffset();
		if (camera_offsets != Vector2.zero)
		{
			Debug.Log("Translation factors: " + camera_offsets.x + " " + camera_offsets.y);
			game_camera.transform.Translate(new Vector3(camera_offsets.x, camera_offsets.y, 0), Space.World);
		}

	}
	
	float ComputeMaxCameraDistance()
	{
		float result;
		if ( field_rectangle_size.x >= field_rectangle_size.y )
		{
			result = field_rectangle_size.x / Mathf.Sin(((game_camera.fieldOfView*screen_ratio/180) * Mathf.PI));
		}
		else
		{
			result = field_rectangle_size.y / Mathf.Sin((((game_camera.fieldOfView*screen_ratio/180)) * Mathf.PI));
		}
		
		game_camera.far = result + 50;
		
		return result;
	}
	
	Vector2 ComputeCameraFieldOffset()
	{
		Vector2 result;
		float game_camera_x = game_camera.transform.position.x;
		float game_camera_y = game_camera.transform.position.y;
		result = new Vector2(0.0f, 0.0f);
		result.x = game_camera.transform.position.z * Mathf.Sin((game_camera.fieldOfView/180) * Mathf.PI/2);
		//Debug.Log("Camera X field: " + (result.x + game_camera_x) );
		
		/*
		if (camera_is_max_unzoom)
		{
			result.x = -game_camera.transform.position.x;
			result.y = -game_camera.transform.position.y;
		}*/
		
		//if (result.x
		if (result.x + Mathf.Abs(game_camera_x) > field_rectangle_size.x / 2 && result.x < field_rectangle_size.x/2)
		{
			int camera_sign = (game_camera_x >= 0 ? 1 : -1);
			result.x = -camera_sign*((result.x + camera_sign*game_camera_x) - (field_rectangle_size.x/2));
			//Debug.Log("Result X: " + result.x);
		}
		else
		{
			if (result.x >= field_rectangle_size.x / 2)
			{
				result.x = -game_camera_x;
			}
			else
			{
				result.x = 0.0f;
			}
		}
		
		result.y = game_camera.transform.position.z * Mathf.Sin((game_camera.fieldOfView*screen_ratio/180) * Mathf.PI/2);
		if (result.y + Mathf.Abs(game_camera_y) > field_rectangle_size.y / 2 && result.y < field_rectangle_size.y/2)
		{
			int camera_sign = (game_camera_y >= 0 ? 1 : -1);
			result.y = -camera_sign*((result.y + camera_sign*game_camera_y) - (field_rectangle_size.y/2));
		}
		else
		{
			if (result.y >= field_rectangle_size.y / 2)
			{
				result.y = -game_camera_y;
			}
			else
			{
				result.y = 0.0f;
			}
		}
		
		return result;
	}
	
	void LateUpdate()
	{
		/*
		 * Rotation camera implementation
		 * if(is_animating_camera)
			StartCoroutine(MoveCamera());
	*/}
	
	/* 
	 * Rotation camera implementation
	 * IEnumerator MoveCamera() {
        	float control = 0; //Amount along transition
			Transform lookAtTarget = focused_star.transform; //What to look at
			Transform initial = game_camera.transform; //Where to start
			Quaternion target = new Quaternion(initial.rotation.x, initial.rotation.y, initial.rotation.z, initial.rotation.w);
			Quaternion target_quat = Quaternion.LookRotation(focused_star.transform.position - game_camera.transform.position);
			float transitionTime = 2.0f; //Time to take to transition
			Debug.Log("Boom baby");
        	while(control < 1.0f) { //Continue until we reach the destination
            	control += Time.deltaTime/transitionTime; //move along transition
            	game_camera.transform.rotation = Quaternion.Lerp(target, target_quat,
                	control); //Smoothing optional
            	//if(lookAtTarget) transform.LookAt(lookAtTarget); //look at target
            	yield return null; //wait
        	}
		
		is_animating_camera = false;
	}*/
	
	/* Create a new Planet GameObject in the scene. */
	public void CreateNewPlanet()
	{
		/* Instantiate a new Moon GameObject. It is a clone of the MoonPrefab. */
		GameObject planet = (GameObject) Instantiate(PlanetPrefab, new Vector3 (0, 0, transform.position.z), Quaternion.identity);
		((PlanetScript) planet.GetComponent("PlanetScript")).ReloadState();
		current_launching_planet = planet;
	}
	
	public void DisplayPoints(){
		GameObject.Find("GUI").GetComponent<TestSceneGUI>().GUIpoints = score.ToString();
	}
	
	/* Get current score. */
	public float getScore(){
		return score;
	}
	
	/* Update current score. Sum diff to score value (if diff
	 * is negativa it will be subtracted). */
	public void updateScore(float diff){
		score += diff;
		if(score < 0)
			score = 0;
	}
	
	public float getLostPlanetFactor(){
		return lost_planet_factor;
	}
}
