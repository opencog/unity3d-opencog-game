﻿using UnityEngine;
using System.Collections;
using OpenCog.Actions;

public class NPCScript : MonoBehaviour 
{

	public string colorToFind;
	public int NPCCurPlanId = 0;
	public bool hasStartNPCScript =false;
	Vector3 moveToKeyPos;
	Vector3 walkToChectPos;
	GameObject keyObj;
	GameObject chestObj;


	OCActionController actionController;

	// Use this for initialization
	void Start () 
	{
		actionController = GetComponent<OCActionController>();
	}
	
	public void startNPCScript()
	{
		if (hasStartNPCScript)
			return;
		
		hasStartNPCScript = true;
		NPC_WalkToKey();
	}

	Vector3 getNearestAjacentLocation(Vector3 from, Vector3 to) // consider 8 directions
	{
		Vector3 nearestPos = Vector3.zero;
		float minD = 9999999.999f;

		for (int x = -1; x <= 1; x ++)
			for (int z = -1; z <= 1; z ++)
		{
			Vector3 ajacentPos = to + new Vector3(x*0.5f,0.0f,z*0.5f);
			float d = Vector3.Distance(from,ajacentPos);
			if (d < minD)
			{
				minD = d;
				nearestPos = ajacentPos;
			}
		}

		return nearestPos;

	}

	public void NPC_WalkToKey()
	{
		// walk to the key
		GameObject keyObjs = GameObject.Find ("keys");
		keyObj = null;
		foreach (Transform key in keyObjs.transform)
		{
			if (key.GetComponent<OCColor>().color == colorToFind)
				keyObj = key.gameObject;
		}
		
		if (keyObj == null)
			return;
		
		NPCCurPlanId ++;
		
		Vector3 keyPos = keyObj.transform.position;
		
		
		OCAction.OCActionArgs actionArguments1 = new OCAction.OCActionArgs();
		
		actionArguments1.StartTarget = actionController.DefaultStartTarget;
		actionArguments1.StartTarget.transform.position = transform.position;		
		
		
		actionArguments1.EndTarget = actionController.DefaultEndTarget;
		//actionArguments.EndTarget = GameObject.Find("EndPointStub");
		
		
		moveToKeyPos = getNearestAjacentLocation(gameObject.transform.position, keyPos);

		actionArguments1.EndTarget.transform.position = moveToKeyPos;
		
		actionArguments1.ActionName = "walk";
		actionArguments1.ActionPlanID = NPCCurPlanId.ToString();
		actionArguments1.SequenceID = 1;
		actionArguments1.Source = gameObject;
		
		
		actionController.LoadActionPlanStep("walk", actionArguments1);
		
		OCAction.OCActionArgs originalArgs1 =  new OCAction.OCActionArgs(actionArguments1);
		originalArgs1.EndTarget = keyObj;
		
		actionController.originalActionPlanQueue.Add(originalArgs1);
	}
	
	public void NPC_GrabTheKey()
	{
		// grap the key
		NPCCurPlanId ++;
		OCAction.OCActionArgs actionArguments2 = new OCAction.OCActionArgs();
		
		actionArguments2.StartTarget = actionController.DefaultStartTarget;
		actionArguments2.StartTarget.transform.position = moveToKeyPos;		
		
		
		actionArguments2.EndTarget = keyObj;
		//actionArguments.EndTarget = GameObject.Find("EndPointStub");
		
		
		actionArguments2.ActionName = "grab";
		actionArguments2.ActionPlanID = NPCCurPlanId.ToString();
		actionArguments2.SequenceID = 2;
		actionArguments2.Source = gameObject;
		
		
		actionController.LoadActionPlanStep("grab", actionArguments2);
		actionController.originalActionPlanQueue.Add(actionArguments2);
		
		
	}
	
	public void NPC_WalkToChest()
	{
		// walk to the chest
		GameObject chestObjs = GameObject.Find ("chests");
		chestObj = null;
		foreach (Transform chest in chestObjs.transform)
		{
			if (chest.GetComponent<OCColor>().color == colorToFind)
				chestObj = chest.gameObject;
		}
		
		if (chestObj == null)
			return;
		
		NPCCurPlanId ++;
		
		Vector3 chestPos = chestObj.transform.position;
		
		
		OCAction.OCActionArgs actionArguments3 = new OCAction.OCActionArgs();
		
		actionArguments3.StartTarget = actionController.DefaultStartTarget;
		actionArguments3.StartTarget.transform.position = transform.position;;		
		
		
		actionArguments3.EndTarget = actionController.DefaultEndTarget;
		
		walkToChectPos = getNearestAjacentLocation(moveToKeyPos, chestPos); 
		
		actionArguments3.EndTarget.transform.position = walkToChectPos;
		
		actionArguments3.ActionName = "walk";
		actionArguments3.ActionPlanID = NPCCurPlanId.ToString();
		actionArguments3.SequenceID = 1;
		actionArguments3.Source = gameObject;
		
		actionController.LoadActionPlanStep("walk", actionArguments3);
		OCAction.OCActionArgs originalArgs3 =  new OCAction.OCActionArgs(actionArguments3);
		originalArgs3.EndTarget = chestObj;

		actionController.originalActionPlanQueue.Add(originalArgs3);


	}

	public void NPC_OpenTheChest()
	{
		// open the chest
		NPCCurPlanId ++;
		OCAction.OCActionArgs actionArguments2 = new OCAction.OCActionArgs();
		
		actionArguments2.StartTarget = actionController.DefaultStartTarget;
		actionArguments2.StartTarget.transform.position = walkToChectPos;		
		
		
		actionArguments2.EndTarget = chestObj;
		//actionArguments.EndTarget = GameObject.Find("EndPointStub");
		
		
		actionArguments2.ActionName = "open";
		actionArguments2.ActionPlanID = NPCCurPlanId.ToString();
		actionArguments2.SequenceID = 2;
		actionArguments2.Source = gameObject;
		
		
		actionController.LoadActionPlanStep("open", actionArguments2);
		actionController.originalActionPlanQueue.Add(actionArguments2);
	}

}
