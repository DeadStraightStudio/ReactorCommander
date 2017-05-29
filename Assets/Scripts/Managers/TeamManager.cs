﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour {

	public static TeamManager instance;

	// Use this for initialization
	void Awake () {
		instance = this;
	}

	public List<GameObject> players;

	public List<GameObject> team1,team2;

	//TestPurpose
	public bool otherTeam;

	public void AddPlayerToList (GameObject go){
		players.Add (go);

		//Testing
		SetPlayerTeam();
	}

	public void DeletePlayerFromList(GameObject go){
		players.Remove (go);
	}

	public void SetPlayerTeam(){
		Debug.Log ("SetPlayer");

		//Prototype Purpose (Delete after testing)
		team1.Clear();
		team2.Clear();
		for(int i = 0; i<players.Count; i++){

			if (otherTeam) {
				otherTeam = false;
				team1.Add (players[i]);
			} 
			else {
				otherTeam = true;
				team2.Add (players[i]);
			}
		}
		//End of Test



		if(team1.Count > 0){
			//Setting team 1
			Debug.Log ("Set 1");

			for(int i = 0; i < team1.Count; i++){
				team1 [i].GetComponent<PlayerStats> ().teamID = 1;
			}
		}
		if(team2.Count >0){

			Debug.Log ("Set 2");

			//Setting team 2
			for(int i = 0; i < team2.Count; i++){
				team2 [i].GetComponent<PlayerStats> ().teamID = 2;
			}
		}


	}


}
