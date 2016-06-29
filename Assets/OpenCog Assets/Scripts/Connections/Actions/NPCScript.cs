using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OpenCog.Actions;
using OpenCog.Map;
using OpenCog.Utility;

public enum NPCScriptPlanCode
{
	PickUpAKey,
	FindAndOpenTheChest,
	PickUpAnFruit,
	DropTheFruitAtARandomPlace,
	SaveAnAnimal
}

public class NPCScript : MonoBehaviour 
{

	public string colorToFind;
	public int NPCCurPlanId = 0;
	public bool hasStartNPCScript =false;
	public GameObject animalToSave;
	public GameObject fruitToEat;
	public Vector3 locationToDropFruit;
	Vector3 moveToKeyPos;
	Vector3 walkToChectPos;
	GameObject keyObj;
	GameObject chestObj;
	public List<NPCScriptPlanCode> scriptPlanList; // assign this in unity editor
		 
	OCMap _Map;

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
		generateNextScriptedPlan();

	}

	public void generateNextScriptedPlan()
	{
		if (! hasStartNPCScript)
			return;

		if (scriptPlanList.Count == 0)
			return;

		NPCScriptPlanCode curPlanCode = scriptPlanList[0];
		scriptPlanList.RemoveAt(0);

		switch (curPlanCode)
		{
		case NPCScriptPlanCode.PickUpAKey:
			NPC_PickUpAKey();
			break;
		case NPCScriptPlanCode.FindAndOpenTheChest:
			NPC_FindAndOpenTheChest();
			break;
		case NPCScriptPlanCode.PickUpAnFruit:
			NPC_PickUpAnFruit();
			break;
		case NPCScriptPlanCode.DropTheFruitAtARandomPlace:
			NPC_DropTheFruitAtARandomPlace();
			break;
		case NPCScriptPlanCode.SaveAnAnimal:
			NPC_SaveAnAnimal();
			break;
		}

	}

	Vector3 getNearestAjacentLocation(Vector3 from, Vector3 to) // consider 8 directions
	{
		Vector3 nearestPos = Vector3.zero;
		float minD = 9999999.999f;

		for (int x = -1; x <= 1; x ++)
			for (int z = -1; z <= 1; z ++)
		{
			Vector3 ajacentPos = to + new Vector3(x*0.6f,0.0f,z*0.6f);
			float d = Vector3.Distance(from,ajacentPos);
			if (d < minD)
			{
				minD = d;
				nearestPos = ajacentPos;
			}
		}

		return nearestPos;

	}

	public void NPC_PickUpAKey()
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

		moveToKeyPos = getNearestAjacentLocation(gameObject.transform.position, keyPos);

		actionArguments1.EndTarget.transform.position = moveToKeyPos;
		
		actionArguments1.ActionName = "walk";
		actionArguments1.ActionPlanID = NPCCurPlanId.ToString();
		actionArguments1.SequenceID = 1;
		actionArguments1.Source = gameObject;
		actionArguments1.EndTargetObject = keyObj;

		actionController.LoadActionPlanStep("walk", actionArguments1);

		// grap the key
		OCAction.OCActionArgs actionArguments2 = new OCAction.OCActionArgs();
		
		actionArguments2.StartTarget = actionController.DefaultStartTarget;
		actionArguments2.StartTarget.transform.position = moveToKeyPos;		

		actionArguments2.EndTarget = keyObj;

		actionArguments2.ActionName = "grab";
		actionArguments2.ActionPlanID = NPCCurPlanId.ToString();
		actionArguments2.SequenceID = 2;
		actionArguments2.Source = gameObject;

		actionController.LoadActionPlanStep("grab", actionArguments2);


	}

	
	public void NPC_FindAndOpenTheChest()
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
		
		walkToChectPos = getNearestAjacentLocation(transform.position, chestPos); 
		
		actionArguments3.EndTarget.transform.position = walkToChectPos;
		
		actionArguments3.ActionName = "walk";
		actionArguments3.ActionPlanID = NPCCurPlanId.ToString();
		actionArguments3.SequenceID = 1;
		actionArguments3.Source = gameObject;
		actionArguments3.EndTargetObject = chestObj;
		
		actionController.LoadActionPlanStep("walk", actionArguments3);

		// open the chest
		OCAction.OCActionArgs actionArguments2 = new OCAction.OCActionArgs();
		
		actionArguments2.StartTarget = actionController.DefaultStartTarget;
		actionArguments2.StartTarget.transform.position = walkToChectPos;		

		actionArguments2.EndTarget = chestObj;		
		
		actionArguments2.ActionName = "open";
		actionArguments2.ActionPlanID = NPCCurPlanId.ToString();
		actionArguments2.SequenceID = 2;
		actionArguments2.Source = gameObject;

		actionController.LoadActionPlanStep("open", actionArguments2);

	}

	public void NPC_PickUpAnFruit()
	{
		
		NPCCurPlanId ++;
		
		Vector3 fruitPos = fruitToEat.transform.position;
		
		OCAction.OCActionArgs actionArguments1 = new OCAction.OCActionArgs();
		
		actionArguments1.StartTarget = actionController.DefaultStartTarget;
		actionArguments1.StartTarget.transform.position = transform.position;;		
		
		actionArguments1.EndTarget = actionController.DefaultEndTarget;
		
		Vector3 walkToPos = getNearestAjacentLocation(transform.position, fruitPos); 
		
		actionArguments1.EndTarget.transform.position = walkToPos;
		
		actionArguments1.ActionName = "walk";
		actionArguments1.ActionPlanID = NPCCurPlanId.ToString();
		actionArguments1.SequenceID = 1;
		actionArguments1.Source = gameObject;
		actionArguments1.EndTargetObject = fruitToEat;
		
		actionController.LoadActionPlanStep("walk", actionArguments1);
		
		// pick up the fruit
		OCAction.OCActionArgs actionArguments2 = new OCAction.OCActionArgs();
		
		actionArguments2.StartTarget = actionController.DefaultStartTarget;
		actionArguments2.StartTarget.transform.position = walkToPos;		
		
		actionArguments2.EndTarget = fruitToEat;
		
		
		actionArguments2.ActionName = "grab";
		actionArguments2.ActionPlanID = NPCCurPlanId.ToString();
		actionArguments2.SequenceID = 2;
		actionArguments2.Source = gameObject;	
		
		actionController.LoadActionPlanStep("grab", actionArguments2);

	}

	public void NPC_DropTheFruitAtARandomPlace()
	{
		NPCCurPlanId ++;
//		_Map = OCMap.Instance;
//		// Find a free random loaction
//		Vector3 walkToPos;
//		while (true)
//		{
//			int x,z;
//			x = Random.Range(-5,5);
//			z = Random.Range(-5,5);
//
//			walkToPos = new Vector3(transform.position.x + x, transform.position.y ,transform.position.z);
//			Vector3i blockPos = VectorUtil.Vector3ToVector3i(walkToPos);
//			Vector3i blockBelowPos = new Vector3i(blockPos.x, blockPos.y - 1, blockPos.z);
//
//			if ((_Map.GetBlock(blockPos).block == null) && (_Map.GetBlock(blockBelowPos).block != null))
//			{
//				break;
//			}
//
//		}

		Vector3 walkToPos = new Vector3(locationToDropFruit.x, transform.position.y ,locationToDropFruit.z);
		OCAction.OCActionArgs actionArguments1 = new OCAction.OCActionArgs();
		
		actionArguments1.StartTarget = actionController.DefaultStartTarget;
		actionArguments1.StartTarget.transform.position = transform.position;;		
		
		actionArguments1.EndTarget = actionController.DefaultEndTarget;
		actionArguments1.EndTarget.transform.position = walkToPos;
		
		actionArguments1.ActionName = "walk";
		actionArguments1.ActionPlanID = NPCCurPlanId.ToString();
		actionArguments1.SequenceID = 1;
		actionArguments1.Source = gameObject;
		actionArguments1.EndTargetObject = fruitToEat;
		
		actionController.LoadActionPlanStep("walk", actionArguments1);

		OCAction.OCActionArgs actionArguments3 = new OCAction.OCActionArgs();
		
		actionArguments3.StartTarget = actionController.DefaultStartTarget;
		actionArguments3.StartTarget.transform.position = walkToPos;		
		
		actionArguments3.EndTarget = fruitToEat;

		actionArguments3.ActionName = "drop";
		actionArguments3.ActionPlanID = NPCCurPlanId.ToString();
		actionArguments3.SequenceID = 2;
		actionArguments3.Source = gameObject;	
		
		actionController.LoadActionPlanStep("drop", actionArguments3);
	}

	public void NPC_SaveAnAnimal()
	{
		
		NPCCurPlanId ++;
		
		Vector3 animalPos = animalToSave.transform.position;

		OCAction.OCActionArgs actionArguments3 = new OCAction.OCActionArgs();
		
		actionArguments3.StartTarget = actionController.DefaultStartTarget;
		actionArguments3.StartTarget.transform.position = transform.position;;		

		actionArguments3.EndTarget = actionController.DefaultEndTarget;
		
		Vector3 walkToPos = getNearestAjacentLocation(transform.position, animalPos); 
		
		actionArguments3.EndTarget.transform.position = walkToPos;
		
		actionArguments3.ActionName = "walk";
		actionArguments3.ActionPlanID = NPCCurPlanId.ToString();
		actionArguments3.SequenceID = 1;
		actionArguments3.Source = gameObject;
		actionArguments3.EndTargetObject = animalToSave;
		
		actionController.LoadActionPlanStep("walk", actionArguments3);

		// heal the animal
		OCAction.OCActionArgs actionArguments2 = new OCAction.OCActionArgs();
		
		actionArguments2.StartTarget = actionController.DefaultStartTarget;
		actionArguments2.StartTarget.transform.position = walkToPos;		

		actionArguments2.EndTarget = animalToSave;


		actionArguments2.ActionName = "heal";
		actionArguments2.ActionPlanID = NPCCurPlanId.ToString();
		actionArguments2.SequenceID = 2;
		actionArguments2.Source = gameObject;	
		
		actionController.LoadActionPlanStep("heal", actionArguments2);


	}




}
