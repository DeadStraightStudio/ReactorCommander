using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class AIWalkPoint : NetworkBehaviour {

	[SyncVar]
	public Vector3 destinationPosition;

	//use this to Move the AI
	public void CmdMove(){
		//Insert Move here
	}
}
