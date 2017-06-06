﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerShoot : NetworkBehaviour {

	private const string PLAYER_TAG = "Player";

	public WeaponSystemStats leftWeapon, rightWeapon;
	public Transform gunEnd;
	public GameObject leftHand;
	float currentPosition;
	float recoilPosition;
	private LineRenderer laserLine;
	public ParticleSystem muzzleFlash;
	public GameObject hitEffect;
	private WaitForSeconds shotDuration = new WaitForSeconds (.5f);

	[SerializeField]
	private Camera cam;

	[SerializeField]
	private LayerMask mask;

	// Use this for initialization
	void Start () 
	{
		if (cam == null) {
			Debug.LogError ("PlayerShoot: No camera reference!");
			this.enabled = false;
		}
		laserLine = GetComponent<LineRenderer> ();
		currentPosition = leftHand.transform.localPosition.z;
		recoilPosition = currentPosition - 0.25f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		//if it's not local player, dont do anything
		if(!isLocalPlayer){
			return;
		}
		//Local player controls below

		
		//Attack
		if (Input.GetButton ("Fire1")) {
			Debug.Log ("PressedLeft");
			WeaponAttack (leftWeapon, "ResetLeftWeaponAttack");
		}
		if(Input.GetButton ("Fire2")){
			Debug.Log ("PressedRight");
			WeaponAttack (rightWeapon, "ResetRightWeaponAttack");
		}
		if(leftHand.transform.localPosition.z != currentPosition)
		{
			leftHand.transform.localPosition = Vector3.Lerp(leftHand.transform.localPosition, new Vector3(leftHand.transform.localPosition.x, leftHand.transform.localPosition.y, currentPosition), 0.1f);
		}
	}

	void WeaponAttack (WeaponSystemStats weapon, string coroutineName){
		if(weapon.canShoot){
			if(leftWeapon.isRaycast){
				RaycastShoot(weapon);
			}
			weapon.canShoot = false;
			StartCoroutine(coroutineName,weapon.fireRate);
		}
	}

	IEnumerator ResetLeftWeaponAttack(float t){
		yield return new WaitForSeconds (t);
		leftWeapon.canShoot = true;
	}

	IEnumerator ResetRightWeaponAttack(float t){
		yield return new WaitForSeconds (t);
		rightWeapon.canShoot = true;
	}

	private IEnumerator ShotEffect()
	{
		laserLine.enabled = true;
		yield return shotDuration;
		laserLine.enabled = false;
	}

	void Recoil()
	{
		leftHand.transform.localPosition = Vector3.Lerp(leftHand.transform.localPosition, new Vector3(leftHand.transform.localPosition.x, leftHand.transform.localPosition.y, recoilPosition), 0.5f);
	}

//	[Command]
	public void RaycastShoot(WeaponSystemStats weapon){
		Debug.Log ("Shoot");
		RaycastHit _hit;
		muzzleFlash.Play ();
		StartCoroutine (ShotEffect ());
		Recoil ();
		Vector3 rayOrigin = cam.ViewportToWorldPoint (new Vector3(0.5f, 0.5f, 0));
		laserLine.SetPosition (0, gunEnd.position);

		if(Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, weapon.attackRange, mask))
		{
			Debug.Log (_hit.collider.name);
//			if (_hit.collider.tag == PLAYER_TAG)
//			{
//				CmdPlayerShot (_hit.collider.name);
//			}

			DealDamage dm = _hit.collider.gameObject.GetComponent<DealDamage>();

			if (dm != null) {
//				dm.ApplyDamage ((int)weapon.damage);
				Debug.Log("ApplyDamage");
				CmdPlayerShot (dm.playerStats.gameObject.name, dm.partsID, (int)weapon.damage);
//				Debug.Log (_hit.collider.gameObject.name + " Dmg : "+ dm.partsID + (int)weapon.damage);
			} 
		}
		else {
			laserLine.SetPosition(1, rayOrigin + (cam.transform.forward * weapon.attackRange));
		}
//		RpcShoot ();
	}

	[Client]
	public void RpcShoot()
	{
		Debug.Log ("Spawn FX");

	}

	[Command]
	void CmdPlayerShot ( string _ID, int partsID, int dmg)
	{
//		Debug.Log (_ID + " has been shot.");

		for(int i = 0; i < TeamManager.instance.players.Count; i++){
			if(TeamManager.instance.players[i].name == _ID){
				TeamManager.instance.players [i].GetComponent<PlayerStats> ().CmdApplyDamage (partsID, dmg);
				Debug.Log ("ID " + _ID + " parts: " + partsID + "Dmg " + dmg);
			}
		}
	}
}
