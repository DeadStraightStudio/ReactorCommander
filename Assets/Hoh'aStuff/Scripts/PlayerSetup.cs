﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

	[SerializeField]
	Behaviour[] componentsToDisable;

	Camera sceneCamera;

	void Start()
	{
		if (!isLocalPlayer) 
		{
			for (int i = 0; i < componentsToDisable.Length; i++) {
				componentsToDisable [i].enabled = false;
			}
		} 
		else 
		{
			sceneCamera = Camera.main;
			if (sceneCamera != null)
			{
				sceneCamera.gameObject.SetActive (false);
			}
		}
	}

	void OnDisable()
	{
		if (sceneCamera != null) 
		{
			sceneCamera.gameObject.SetActive (true);
		}
	}
		/*
	[SyncVar] // Only sync Float int string etc normal variable - not GameObject
	public int health;//Testing

	[SerializeField]
	private GameObject Bullet;

	// Use this for initialization
	void Start () 
	{
		if(!isLocalPlayer)
		{
			
			return;
		}
		//		playerTransform.parent = transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!isLocalPlayer){
			return;
		}
	}

	[Command]// Only works in Server
	void CmdDebugLog(string log){
		Debug.Log (log);
	}

	//Left Weapon
	void LeftWeapon_Attack(){ 
		//Do a local Raycast to a position

		RpcSpawnBullet (Bullet);
	}

	//Right Weapon
	void RightWeapon_Attack (){
		//Do a local Raycast to a position || enable a collider || do something || Rpc to spawn Bullet
	}

	[Client] // means Calling functions to other client
	void RpcSpawnBullet(GameObject _bulletObject){//Use Rpc Only to spawn Particles/bullets (not for collision detection)
		Debug.Log ("Shoot");
	
		//Below are references

		GameObject go = Instantiate (_bulletObject);
		go.transform.position = transform.position;
		go.transform.rotation = transform.rotation;

		NetworkServer.Spawn (go);//Only can spawn an instantiated game object //Spawn go to other client as a network object
	*/
}
