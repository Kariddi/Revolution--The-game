using UnityEngine;
using System.Collections;
using System.Collections.Generic;


class CircleSlice
{
	private float beginning_angle;
	private float end_angle;
	
	public CircleSlice(float beginning_angle, float end_angle)
	{
		this.beginning_angle = beginning_angle;
		this.end_angle = end_angle;
	}
	
	public float BeginAngle {
		get { return beginning_angle; }
	}

	public float EndAngle {
		get { return end_angle; }
	}
}

public class ControllerScript : MonoBehaviour {
	
	private float score = 0;
	private float lost_planet_factor = 0.5f;
	private float screen_ratio = 0.0f;
	
	/* Define a GameObject referred to the PlanetPrefab. Remember to connect this variable to PlanetPrefab
	 * object (to do this select Controller in the Hierarchy menu and edit the Planet Prefab variable shown
	 * in the Ispector window). */
	public float line_width = 4.0f;
	public Material line_material;
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
		StarCircleTest();
		//CreateLaunchCircles();
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
	
	private void DetermineStarSlices(StarScript star, int current_index, StarScript[] stars_array)
	{
		List<CircleSlice> circle_slice_array = new List<CircleSlice>();
		circle_slice_array.Add(new CircleSlice(0, 2*Mathf.PI));
		float star_x = star.gameObject.transform.position.x;
		float star_y = star.gameObject.transform.position.y;
		for (int i = 0; i < stars_array.Length; i++)
		{
			float second_star_x = stars_array[i].gameObject.transform.position.x;
			float second_star_y = stars_array[i].gameObject.transform.position.y;
			float dx = star_x - second_star_x;
			float dy = star_y - second_star_y;
			float d2 = dx*dx + dy*dy;
			float d = Mathf.Sqrt(d2);
			
			if ( d > star.star_launch_limit + stars_array[i].star_launch_limit ||
			     d < Mathf.Abs(star.star_launch_limit - stars_array[i].star_launch_limit) ||
			     i == current_index )
				continue;
			float radiuspow_1 = Mathf.Pow(star.star_launch_limit,2);
			float radiuspow_2 = Mathf.Pow(stars_array[i].star_launch_limit,2);
			float a = (radiuspow_1 - radiuspow_2 + d2) / (2*d);
			float h = Mathf.Sqrt( radiuspow_1 - a*a );
			float x2 = star_x + a * (second_star_x - star_x)/d;
			float y2 = star_y + a * (second_star_y - star_y)/d;
			
			float inters1_x = x2 + h*(second_star_y - star_y)/d;
			float inters1_y = y2 - h*(second_star_x - star_x)/d;
			float inters2_x = x2 - h*(second_star_y - star_y)/d;
			float inters2_y = y2 + h*(second_star_x - star_x)/d;
			float inters1_angle = Mathf.Acos((inters1_x-star_x)/star.star_launch_limit);
			float inters2_angle = Mathf.Acos((inters2_x-star_x)/star.star_launch_limit);
			//Debug.Log(inters1_x + " " + star_x + " " + second_star_x + " " + radiuspow_1);
			//Debug.Log(inters1_angle + " " + inters2_angle);
		}
	}
	
	private void CreateLaunchCircles()
	{
		StarScript[] stars = FindObjectsOfType(typeof(StarScript)) as StarScript[];
		int star_numbers = stars.Length;
		for (int i = 0; i < star_numbers; i++)
		{
			DetermineStarSlices(stars[i], i, stars);
		}
	}
	
	private void StarCircleTest()
	{
		//LineRenderer star_one_linedrawer = GameObject.Find("LineDrawer").GetComponent<LineRenderer>();
		StarScript[] stars = FindObjectsOfType(typeof(StarScript)) as StarScript[];
		foreach (StarScript star in stars)
		{
		//StarScript sc1 = GameObject.Find("Star1").GetComponent<StarScript>();
		//sc1.gameObject;
		//GameObject sc1obj = sc1.gameObject;// GameObject.Find("Star1");
			List<List<Vector3>> lines = new List<List<Vector3>>();
			List<Vector3> composing_lines = new List<Vector3>();
			int vertex_num = Mathf.FloorToInt(2*Mathf.PI /(line_width/star.star_launch_limit));
			Vector3 starting_vertex = new Vector3(star.star_launch_limit, 0, 0);
			//int discontinuities = 0;
			bool drawing_line = true;
			Vector3 previous_vertex = Vector3.zero;
			for (int i = 0; i <= vertex_num; i++)
			{
				Vector3 vertex_to_add = new Vector3(starting_vertex.x * Mathf.Cos(i * Mathf.PI*2 /vertex_num),
			    	                                starting_vertex.x * Mathf.Sin(i * Mathf.PI*2 /vertex_num),
			        	                            0);
				
				if(!TestInsideCirclePoint(vertex_to_add, star.transform.position, star))
				{
					//star_one_linedrawer.SetPosition(i, vertex_to_add);
					if (!drawing_line)
					{
						drawing_line = true;
						if (i != 0)
							composing_lines.Add((vertex_to_add + previous_vertex) / 2);
					}
					composing_lines.Add(vertex_to_add);
				} else {
					if (drawing_line)
					{
						drawing_line = false;
						if (i != 0)
							composing_lines.Add((vertex_to_add + previous_vertex) / 2);
						lines.Add(composing_lines);
						composing_lines = new List<Vector3>();
					}
				}
				
				previous_vertex = vertex_to_add;
			}
			
			lines.Add(composing_lines);
			
			foreach (List<Vector3> line in lines)
			{
				Vector3[] points = line.ToArray();
				GameObject starobj = star.gameObject;
				GameObject line_obj = new GameObject();
				line_obj.transform.position = starobj.transform.position;
				line_obj.transform.parent = starobj.transform;
				LineRenderer star_one_linedrawer = line_obj.AddComponent<LineRenderer>();
				star_one_linedrawer.SetVertexCount(line.Count);
				//int vertex_num = Mathf.FloorToInt(star.star_launch_limit*5 +5);
				//star_one_linedrawer.SetVertexCount(vertex_num+1);
				star_one_linedrawer.SetWidth(line_width,line_width);
				star_one_linedrawer.material = line_material;
				star_one_linedrawer.useWorldSpace = false;
				for (int i = 0; i < line.Count; i++)
				{
					star_one_linedrawer.SetPosition(i, points[i]);
				}
			}
		}
	}
	
	private bool TestInsideCirclePoint(Vector3 point, Vector3 star_pos, StarScript star)
	{
		StarScript[] stars = FindObjectsOfType(typeof(StarScript)) as StarScript[];
		foreach (StarScript curr_star in stars)
		{
			Vector3 point_to_test = point + star_pos;
			if (star.Equals(curr_star))
				continue;
			point_to_test = point_to_test - curr_star.gameObject.transform.position;
			float radius = Mathf.Sqrt(point_to_test.x*point_to_test.x + point_to_test.y*point_to_test.y);
			if (radius < curr_star.star_launch_limit)
				return true;
		}
		
		return false;
	}
	
	private void TestDrawLine()
	{
		LineRenderer star_one_linedrawer = GameObject.Find("Star1").GetComponent<LineRenderer>();
		if (star_one_linedrawer == null)
			return;
		Vector3 lolx1= new Vector3 (0,0,0);
		Vector3 lolx2 = new Vector3(5,0,0);
		star_one_linedrawer.SetPosition(0, lolx1);
		star_one_linedrawer.SetPosition(1, lolx2);
	}
	
	// Update is called once per frame
	void Update () {
		DisplayPoints();
		//TestDrawLine();
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
		
		//Debug.Log(screen_ratio);
		//Debug.Log("Width: " + Mathf.Sin((game_camera.fieldOfView/180) * Mathf.PI) * (game_camera.transform.position.z) + "Height: " + Mathf.Sin((game_camera.fieldOfView*screen_ratio/180) * Mathf.PI) * (game_camera.transform.position.z));
		
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
			//Debug.Log("Sono qui");
			float delta = wheel_axis*camera_scroll_speed;
			float current_camera_z = game_camera.transform.position.z;
			//Debug.Log(delta);
			camera_is_max_unzoom = false;
			max_camera_distance = ComputeMaxCameraDistance();
			if (current_camera_z + delta > min_camera_distance && current_camera_z + delta < max_camera_distance)
			{
				Vector2 plane_translation = new Vector2(0.0f, 0.0f);
				if (delta < 0)
				{
					Vector3 mouse_pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, current_camera_z));
					float x_distance = mouse_pos.x - game_camera.transform.position.x; 
					float x_distance_from_camera = Mathf.Sqrt(x_distance*x_distance + current_camera_z*current_camera_z);
					float x_sin = x_distance/x_distance_from_camera;
					float y_distance = mouse_pos.y - game_camera.transform.position.y; 
					float y_distance_from_camera = Mathf.Sqrt(y_distance*y_distance + current_camera_z*current_camera_z);
					float y_sin = y_distance/y_distance_from_camera;	
					//plane_translation = Vector3.Lerp(game_camera.transform.position, mouse_pos, -delta / (current_camera_z - min_camera_distance));
					plane_translation.x = Mathf.Abs(delta)*x_sin;
					plane_translation.y = Mathf.Abs(delta)*y_sin;
				}
				//Debug.Log("Pos: " + game_camera.transform.position.z + " " + current_camera_z);
				//Debug.Log("New position guessed: " + (current_camera_z + delta));
				game_camera.transform.Translate(new Vector3(plane_translation.x, plane_translation.y, delta), Space.World);
				//Debug.Log("New position : " + (game_camera.transform.position.z));
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
			//Debug.Log("Secondo bottone mouse");
		    float h = -horizontalPanSpeed * Input.GetAxis("Mouse X");
            float v = -verticalPanSpeed * Input.GetAxis("Mouse Y");
			//Debug.Log(h + " " + v);
			game_camera.transform.Translate(new Vector3(h, v, 0));
		}
		
		Vector2 camera_offsets = ComputeCameraFieldOffset();
		if (camera_offsets != Vector2.zero)
		{
			//Debug.Log("Translation factors: " + camera_offsets.x + " " + camera_offsets.y);
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
