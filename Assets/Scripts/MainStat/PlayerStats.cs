﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerStats : NetworkBehaviour {
	[SyncVar]
	public int teamID;

	public MeshRenderer[] meshRenderer;
//	public Transform[] _childObject;
	public PlayerLoadout playerLoadoutScript;

	[SyncVar]
	public bool isAlive;

	public GameObject explodeVFX;

//	[SerializeField]
	public TorsoStats frontTorsoStats, backTorsoStats, leftTorsoStats, rightTorsoStats;

//	[SerializeField]
	public LegStats leftLegStats, rightLegStats;

//	[SerializeField]
	public WeaponSystemStats leftWeaponSystemStats, rightWeaponSystemStats;

	[SyncVar]
	public int frontTorso_Health, backTorso_Health, leftTorso_Health, rightTorso_Health,
	leftLeg_Health, rightLeg_Health , leftWeaponSystem_Health, rightWeaponSystem_Health;

	[SyncVar]
	public bool canMove;

	//Test
	public bool setColor;

//	public void respawn(int id){
//		GameManager.GM.RespawnPlayer ();
//	}

	void Start(){
		meshRenderer = GetComponentsInChildren<MeshRenderer> ();


		//Add player to TeamManager
		if (isServer) {
			TeamManager.instance.AddPlayerToList (this.gameObject);
			StartStuff ();
		}

		//		CmdResetStats ();


		if(isLocalPlayer){
			//			Debug.Log ("AA");
			GameManager.GM.localPlayer = this.gameObject;
			//			GameManager.GM.localPlayerStatsScript = GameManager.GM.localPlayer.GetComponent<PlayerStats>();
			GameManager.GM.localPlayerStatsScript = this;
			GameManager.GM.RespawnPlayer ();


			//			RpcRespawnPlayer ();
		}
		else if(!isLocalPlayer){
			Debug.Log ("Not Local Player");
		}



		if(!setColor){

			Debug.Log ("Set Team Color");
			//MeshRenderer[] meshRenderer = GetComponentsInChildren<MeshRenderer> ();
			for(int i = 0; i < meshRenderer.Length; i++){
				if(teamID == 2){
					meshRenderer [i].material.color = Color.blue;
				}
				setColor = true;
			}

		}

		//TestingPurpose
//		GameManager.GM.RespawnPlayer();
	}

	void Update(){
		//Test
//		if(!setColor){

//			Debug.Log ("Set Team Color");
			for(int i = 0; i < meshRenderer.Length; i++){
				if(teamID == 1){
					meshRenderer [i].material.color = Color.yellow;
				}
				if(teamID == 2){
					meshRenderer [i].material.color = Color.blue;
				}
				//					setColor = true;
			}

//		}
//		if(!isLocalPlayer){
//			return;
//		}

		if(!isAlive){
			return;
		}
		//Test
		if(isLocalPlayer){

			//Eject
			if(Input.GetKeyDown(KeyCode.R)){
				Debug.Log ("Eject");
				GameManager.GM.RespawnPlayer ();
				CmdSetMatchKills ();
			}
		}

		if(!isServer){
			return;
		}
//		if(isServer){
//			Debug.Log (this.gameObject.name);
//		}
//

		if(leftLeg_Health <= 0 && rightLeg_Health <= 0){
			canMove = false;
		}

		if(frontTorso_Health <= 0 || backTorso_Health <=0){
			isAlive = false;
			CmdEnablePlayer (false);
			if(isLocalPlayer){
				CmdSetMatchKills ();
			}
			CmdResetStats ();
			CmdEnablePlayer (true);
			GameManager.GM.RespawnPlayer ();
		}

		if(leftWeaponSystem_Health <= 0 || leftTorso_Health <=0){
			RpcDisableLeftWeaponSystem (true);
		}

		if(rightWeaponSystem_Health <= 0 || rightTorso_Health <=0){
			RpcDisableRightWeaponSystem (true);
		}

		if(leftLeg_Health <= 0){
			RpcDisableLeftLeg (true);
		}

		if(rightLeg_Health <= 0){
			RpcDisableRightLeg (true);
		}
	}

	public void StartStuff(){
		//Set Stats Reference
		//		playerLoadoutScript.LoadParts ();
		playerLoadoutScript.Display();
		bool setLeft = true;
		for(int i = 0; i < playerLoadoutScript.enabledObject.Count; i++){
			if(playerLoadoutScript.enabledObject[i].name == playerLoadoutScript.torsoName){

//				frontTorsoStats = playerLoadoutScript.enabledObject [i].GetComponent<TorsoStats> ();

				TorsoStats[] childTorsoStats = playerLoadoutScript.enabledObject [i].GetComponentsInChildren<TorsoStats> ();

				for(int i2 = 0; i2 < childTorsoStats.Length; i2++){
					if(childTorsoStats[i2].gameObject.name == "FrontTorso"){
//						Debug.Log (childTorsoStats[i2].gameObject.name);
						frontTorsoStats = childTorsoStats [i2];
					}
					else if(childTorsoStats[i2].gameObject.name == "BackTorso"){
						backTorsoStats = childTorsoStats [i2];
					}
					else if(childTorsoStats[i2].gameObject.name == "LeftTorso"){
						leftTorsoStats = childTorsoStats [i2];
					}
					else if(childTorsoStats[i2].gameObject.name == "RightTorso"){
						rightTorsoStats = childTorsoStats [i2];
					}
				}
			}
			else if(playerLoadoutScript.enabledObject[i].name == playerLoadoutScript.leftWeaponSystemName && setLeft){
				leftWeaponSystemStats = playerLoadoutScript.enabledObject [i].GetComponent<WeaponSystemStats> ();
				setLeft = false;
			}
			else if(playerLoadoutScript.enabledObject[i].name == playerLoadoutScript.rightWeaponSystemName && !setLeft){
				rightWeaponSystemStats = playerLoadoutScript.enabledObject [i].GetComponent<WeaponSystemStats> ();
			}
			else if(playerLoadoutScript.enabledObject[i].name == playerLoadoutScript.legName){
				LegStats[] childgo = GetComponentsInChildren<LegStats> ();
				for(int i2 = 0; i2 < childgo.Length; i2++){
					if(childgo[i2].gameObject.name == "LeftLeg"){
						leftLegStats = childgo [i2];
					}
					else if(childgo[i2].gameObject.name == "RightLeg"){
						rightLegStats = childgo [i2];
					}
				}
			}
		}


		GetComponent<PlayerShoot> ().SetWeapon();
//		CmdResetStats ();
		if(isServer){
			RpcStartStuff ();
		}
	}

	[ClientRpc]
	public void RpcStartStuff(){
		//Set Stats Reference
		//		playerLoadoutScript.LoadParts ();
		playerLoadoutScript.Display();
		bool setLeft = true;
		for(int i = 0; i < playerLoadoutScript.enabledObject.Count; i++){
			if(playerLoadoutScript.enabledObject[i].name == playerLoadoutScript.torsoName){
//				frontTorsoStats = playerLoadoutScript.enabledObject [i].GetComponent<TorsoStats> ();

				TorsoStats[] childTorsoStats = playerLoadoutScript.enabledObject [i].GetComponentsInChildren<TorsoStats> ();

				for(int i2 = 0; i2 < childTorsoStats.Length; i2++){
					if(childTorsoStats[i2].gameObject.name == "FrontTorso"){
//						Debug.Log (childTorsoStats[i2].gameObject.name);
						frontTorsoStats = childTorsoStats [i2];
					}
					else if(childTorsoStats[i2].gameObject.name == "BackTorso"){
						backTorsoStats = childTorsoStats [i2];
					}
					else if(childTorsoStats[i2].gameObject.name == "LeftTorso"){
						leftTorsoStats = childTorsoStats [i2];
					}
					else if(childTorsoStats[i2].gameObject.name == "RightTorso"){
						rightTorsoStats = childTorsoStats [i2];
					}
				}
			}
			else if(playerLoadoutScript.enabledObject[i].name == playerLoadoutScript.leftWeaponSystemName && setLeft){
				leftWeaponSystemStats = playerLoadoutScript.enabledObject [i].GetComponent<WeaponSystemStats> ();
				setLeft = false;
			}
			else if(playerLoadoutScript.enabledObject[i].name == playerLoadoutScript.rightWeaponSystemName && !setLeft){
				rightWeaponSystemStats = playerLoadoutScript.enabledObject [i].GetComponent<WeaponSystemStats> ();
			}
			else if(playerLoadoutScript.enabledObject[i].name == playerLoadoutScript.legName){
				LegStats[] childgo = GetComponentsInChildren<LegStats> ();
				for(int i2 = 0; i2 < childgo.Length; i2++){
					if(childgo[i2].gameObject.name == "LeftLeg"){
						leftLegStats = childgo [i2];
					}
					else if(childgo[i2].gameObject.name == "RightLeg"){
						rightLegStats = childgo [i2];
					}
				}
			}
		}


		GetComponent<PlayerShoot> ().SetWeapon();
		CmdResetStats ();
	}


	void OnDestroy(){
		TeamManager.instance.RemovePlayer(this.gameObject, teamID);
	}

	[Command]
	void CmdSetMatchKills(){
		if(teamID == 1){
			MatchManager.instance.team1DeathCount += 1;
		}
		if(teamID == 2){
			MatchManager.instance.team2DeathCount += 2;
		}
	}

	//Disable in the Server
	[Command]
	public void CmdEnablePlayer(bool temp){
		RpcEnablePlayer (temp);
	}
	[ClientRpc]
	public void RpcEnablePlayer(bool temp){
		this.gameObject.SetActive (temp);
	}
	//End of Disable

	[Command]
	public void CmdResetStats(){
		frontTorso_Health = frontTorsoStats.maxHealth;
		backTorso_Health = backTorsoStats.maxHealth;
		leftTorso_Health = leftTorsoStats.maxHealth;
		rightTorso_Health = rightTorsoStats.maxHealth;
		leftLeg_Health = leftLegStats.maxHealth;
		rightLeg_Health = rightLegStats.maxHealth;
		leftWeaponSystem_Health = leftWeaponSystemStats.maxHealth;
		rightWeaponSystem_Health = rightWeaponSystemStats.maxHealth;
		isAlive = true;
		canMove = true;

		//Enable Parts
		RpcDisableLeftWeaponSystem(false);
		RpcDisableRightWeaponSystem(false);
		RpcDisableLeftLeg(false);
		RpcDisableRightLeg(false);
	}

	[ClientRpc]
	public void RpcDisableLeftWeaponSystem(bool t){
		leftWeaponSystemStats.gameObject.SetActive (!t);
	}

	[ClientRpc]
	public void RpcDisableRightWeaponSystem(bool t){
		rightWeaponSystemStats.gameObject.SetActive (!t);
	}

	[ClientRpc]
	public void RpcDisableLeftLeg(bool t){
		leftLegStats.gameObject.SetActive (!t);
	}

	[ClientRpc]
	public void RpcDisableRightLeg(bool t){
		rightLegStats.gameObject.SetActive (!t);
	}

	[ClientRpc]
	public void RpcRespawnPlayer(){
		GameManager.GM.RespawnPlayer ();

	}

	// 0 = torso, 1 = leftWeapon, 2 = RightWeapon, 3 = LeftLeg, 4 = RightLeg
	[Command]
	public void CmdApplyDamage(int partsID, int dmg){
		if(partsID == 0){
			frontTorso_Health -= dmg;
		}
		else if(partsID == 1){
			backTorso_Health -= dmg;
		}
		else if(partsID == 2){
			leftTorso_Health -= dmg;
		}
		else if(partsID == 3){
			rightTorso_Health -= dmg;
		}
		else if(partsID == 4){
			leftWeaponSystem_Health -= dmg;
		}
		else if(partsID == 5){
			rightWeaponSystem_Health -= dmg;
		}
		else if(partsID == 6){
			leftLeg_Health -= dmg;
		}
		else if(partsID == 7){
			rightLeg_Health -= dmg;
		}
	}


}
