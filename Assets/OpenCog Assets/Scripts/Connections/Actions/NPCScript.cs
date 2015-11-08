using UnityEngine;
using System.Collections;
using OpenCog.Actions;

public class NPCScript : MonoBehaviour 
{

	public string colorToFind;
	public int NPCCurPlanId = 0;
	public bool hasStartNPCScript =false;

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
		NPC_FindAKey();
		
	}
	

	
	public void NPC_FindAKey()
	{
		
		// walk to the key
		GameObject keyObj = GameObject.Find ("key_" + colorToFind);
		
		if (keyObj == null)
			return;
		
		NPCCurPlanId ++;
		
		Vector3 keyPos = keyObj.transform.position;
		
		
		OCAction.OCActionArgs actionArguments1 = new OCAction.OCActionArgs();
		
		actionArguments1.StartTarget = actionController.DefaultStartTarget;
		actionArguments1.StartTarget.transform.position = transform.position;		
		
		
		actionArguments1.EndTarget = actionController.DefaultEndTarget;
		//actionArguments.EndTarget = GameObject.Find("EndPointStub");
		
		Vector3 walkToKeyPos = keyPos + new Vector3(1, 0, 0);
		
		actionArguments1.EndTarget.transform.position = walkToKeyPos;
		
		actionArguments1.ActionName = "walk";
		actionArguments1.ActionPlanID = NPCCurPlanId.ToString();
		actionArguments1.SequenceID = 1;
		actionArguments1.Source = gameObject;
		
		
		actionController.LoadActionPlanStep("walk", actionArguments1);

		OCAction.OCActionArgs originalArgs1 =  new OCAction.OCActionArgs(actionArguments1);
		originalArgs1.EndTarget = keyObj;

		actionController.originalActionPlanQueue.Add(originalArgs1);
		
		// grap the key
		
		OCAction.OCActionArgs actionArguments2 = new OCAction.OCActionArgs();
		
		actionArguments2.StartTarget = actionController.DefaultStartTarget;
		actionArguments2.StartTarget.transform.position = walkToKeyPos;		
		
		
		actionArguments2.EndTarget = keyObj;
		//actionArguments.EndTarget = GameObject.Find("EndPointStub");
		
		
		actionArguments2.ActionName = "grab";
		actionArguments2.ActionPlanID = NPCCurPlanId.ToString();
		actionArguments2.SequenceID = 2;
		actionArguments2.Source = gameObject;
		
		
		actionController.LoadActionPlanStep("grab", actionArguments2);
		actionController.originalActionPlanQueue.Add(actionArguments2);
		
		
	}
	
	public void NPC_FindAndOpenAChest()
	{
		// walk to the chest
		GameObject chestObj = GameObject.Find ("chest_" + colorToFind);
		
		if (chestObj == null)
			return;
		
		NPCCurPlanId ++;
		
		Vector3 chestPos = chestObj.transform.position;
		
		
		OCAction.OCActionArgs actionArguments3 = new OCAction.OCActionArgs();
		
		actionArguments3.StartTarget = actionController.DefaultStartTarget;
		actionArguments3.StartTarget.transform.position = transform.position;;		
		
		
		actionArguments3.EndTarget = actionController.DefaultEndTarget;
		
		Vector3 walkToChectPos = chestPos + new Vector3(1, 0, 0);
		
		actionArguments3.EndTarget.transform.position = walkToChectPos;
		
		actionArguments3.ActionName = "walk";
		actionArguments3.ActionPlanID = NPCCurPlanId.ToString();
		actionArguments3.SequenceID = 1;
		actionArguments3.Source = gameObject;
		
		actionController.LoadActionPlanStep("walk", actionArguments3);
		OCAction.OCActionArgs originalArgs3 =  new OCAction.OCActionArgs(actionArguments3);
		originalArgs3.EndTarget = chestObj;
		actionController.originalActionPlanQueue.Add(originalArgs3);

		
		// open the chest
		
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
