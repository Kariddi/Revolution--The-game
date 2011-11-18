using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour {
	
	private float	universal_z = -34;
	private int		player_score;
	private int		available_planets;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	float get_universal_z(){
		return universal_z;
	}
	
	int get_player_score(){
		return player_score;
	}
	
	void update_player_score(int delta_score){
		player_score += delta_score;
	}
	
	int get_available_planets(){
		return available_planets;
	}
	
	void decrement_available_planets(){
		available_planets--;
	}
}