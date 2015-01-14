
/// Unity3D OpenCog World Embodiment Program
/// Copyright (C) 2013  Novamente			
///
/// This program is free software: you can redistribute it and/or modify
/// it under the terms of the GNU Affero General Public License as
/// published by the Free Software Foundation, either version 3 of the
/// License, or (at your option) any later version.
///
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Affero General Public License for more details.
///
/// You should have received a copy of the GNU Affero General Public License
/// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections;
using System.Collections.Generic;
using Behave.Runtime;
using OpenCog.Actions;
using OpenCog.Attributes;
using OpenCog.Extensions;
using OpenCog.Map;
using ProtoBuf;
using UnityEngine;
using ContextType = BLOCBehaviours.ContextType;
using OCID = System.Guid;
using Tree = Behave.Runtime.Tree;
using TreeType = BLOCBehaviours.TreeType;
using System.Linq;
using System.Xml;
using OpenCog.Utility;

//using OpenCog.Aspects;
using OpenCog.Utilities.Logging;

namespace OpenCog
{

namespace Actions
{

/// <summary>
/// The OpenCog Action Controller.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
#pragma warning disable 0414
#endregion
public class OCActionController : OCMonoBehaviour, IAgent
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
			
	[SerializeField]
	private GameObject
		_defaultSource;
			
	[SerializeField]
	private GameObject
		_defaultStartTarget;
	
	[SerializeField]
	private GameObject
		_defaultEndTarget;
			
	[SerializeField]
	private TreeType
		_TreeType;

	private OCActionPlanStep _step = null;

	private Hashtable _idleParams;
	
	private static Dictionary<string, string> builtinActionMap = new Dictionary<string, string>();
				
	private Dictionary<string, TreeType> _ActionNameDictionary = new Dictionary<string, TreeType>()
	{ { "walk", TreeType.Character_Move }
	, { "step_forward", TreeType.Character_ForwardMove }
	, { "rotate_left", TreeType.Character_TurnLeftMove }
	, { "rotate_right", TreeType.Character_TurnRightMove }
	, { "grab", TreeType.Character_BothHandsTransfer }
	, { "eat", TreeType.Character_Destroy }
	, { "say", TreeType.Character_Tell }
	, { "jump_toward", TreeType.Character_Move }
	, { "BuildBlockAtPosition", TreeType.Character_TurnAndCreate }
	, { "MoveToCoordinate", TreeType.Character_Move }
	, { "build_block", TreeType.Character_TurnAndCreate }
	, { "idle", TreeType.Character_IdleShow }
	, { "activate", TreeType.Character_Activate }
	, { "step_backward", TreeType.Character_BackwardMove }
	, { "build_block_forward", TreeType.Character_Create }
  , { "step_down", TreeType.Character_DownMove }
	, { "emote", TreeType.Character_EmoteShow }
  , { "face_toward", TreeType.Character_FaceShow }
  , { "say_idle", TreeType.Character_IdleTell }
  , { "kick", TreeType.Character_KickActivate }
  , { "activate_left", TreeType.Character_LeftHandActivate }
	, { "eat_left", TreeType.Character_LeftHandDestroy }
  , { "step_left", TreeType.Character_LeftMove }
	, { "morph", TreeType.Character_Morph }
	, { "release", TreeType.Character_Release }
	, { "activate_right", TreeType.Character_RightHandActivate }
	, { "build_block_right", TreeType.Character_RightHandCreate }
	, { "step_right", TreeType.Character_RightMove }
	, { "show", TreeType.Character_Show }
	, { "transfer", TreeType.Character_Transfer }
  , { "destroy_block", TreeType.Character_TurnAndDestroy }
  , { "rotate_left_or_right", TreeType.Character_TurnLeftOrRight }
  , { "step_up", TreeType.Character_UpMove }
	, { "girl_behaviour", TreeType.Character_GirlBehaviour }
  , { "ghost_behaviour", TreeType.Character_GhostBehaviour }
	};
			
	// Assume that there's just one behaviour we'd like to execute at a given time
	private Dictionary<TreeType, Tree> _TreeTypeDictionary;
			
	private LinkedList< OCActionPlanStep > _ActionPlanQueue;

	//private long _LastPlanEndedAtTime;
	//public long LastPlanEndedAtTime{get{return _LastPlanEndedAtTime;}}
	private bool _PlanSucceeded = true;
	public bool PlanSucceeded{get{return _PlanSucceeded;}}
			
	private string _LastPlanID = null;

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------

	public TreeType TreeType
	{
		get { return this._TreeType;}
		set { _TreeType = value;}
	}

	public List<string> RunningActions;

	public GameObject DefaultEndTarget
	{
		get { return this._defaultEndTarget;}
		set { _defaultEndTarget = value;}
	}

	public GameObject DefaultStartTarget
	{
		get { return this._defaultStartTarget;}
		set { _defaultStartTarget = value;}
	}
			
	public OCActionPlanStep Step
	{
		get { return this._step;}
		set { _step = value;}
	}		

			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	public IEnumerator Start()
	{
		_TreeTypeDictionary = new Dictionary<TreeType, Tree>();
		_ActionPlanQueue = new LinkedList<OCActionPlanStep>();
				
		foreach(TreeType type in Enum.GetValues( typeof(TreeType) ).Cast<TreeType>())
		{
			_TreeTypeDictionary.Add(type, BLOCBehaviours.InstantiateTree(type, this));
		}
				
		OCAction[] actions = gameObject.GetComponentsInChildren<OCAction>(true);
				
		foreach(Tree tree in _TreeTypeDictionary.Values)
		{
			if(tree == null)
			{
				continue;
			}
			int index = tree.Name.LastIndexOf(".") + 1;
			if(index < 0 || index > tree.Name.Count())
			{
				index = 0;
			}
			string treeName = tree.Name.Substring(index);					
					
			foreach(OCAction action in actions)
			{
				if(((action.FullName.Contains(treeName) || treeName.Contains("Behaviour")) 
					&& !(treeName == "GhostBehaviour" && action.name.Contains("Create"))
					&& !(treeName == "GhostBehaviour" && action.name.Contains("Destroy"))
					&& !(treeName == "GhostBehaviour" && action.name.Contains("HoldBothHandsTransfer"))
					&& !(treeName == "GirlBehaviour" && action.name.Contains("HoldBothHandsTransfer")))
					|| (treeName == "TurnAndCreate" && action.FullName == "HoldRightHandCreate")
					|| (treeName == "TurnAndCreate" && action.FullName == "HoldRightHandCreateBelow")
					|| (treeName == "TurnAndCreate" && action.FullName == "StandTurnLeftMove")
					|| (treeName == "TurnAndCreate" && action.FullName == "StandTurnRightMove")
					|| (treeName == "TurnAndCreate" && action.FullName == "StandIdleShow")
					|| (treeName == "TurnAndCreate" && action.FullName == "FallIdleShow")
					|| (treeName == "TurnLeftOrRight" && action.FullName == "StandTurnLeftMove")
					|| (treeName == "TurnLeftOrRight" && action.FullName == "StandTurnRightMove")
					|| (treeName == "TurnLeftOrRight" && action.FullName == "StandIdleShow")
					|| (treeName == "TurnLeftOrRight" && action.FullName == "FallIdleShow")
					|| (treeName == "BothHandsTransfer" && action.FullName == "HoldBothHandsTransfer")
				  )
				{
					// whatever, this is awful... I evidently fail at logic...
					if(!(treeName == "LeftMove" && action.FullName == "StandTurnLeftMove")
						&& !(treeName == "RightMove" && action.FullName == "StandTurnRightMove"))
					{
						int actionTypeID = (int)Enum.Parse(typeof(BLOCBehaviours.ActionType), action.FullName);
							
						tree.SetTickForward(actionTypeID, action.ExecuteBehave);
					}

				}
			}
		}
				
		OCActionPlanStep firstStep = OCScriptableObject.CreateInstance<OCActionPlanStep>();
		firstStep.Behaviour = _TreeTypeDictionary[_TreeType];
		firstStep.Arguments = new OCAction.OCActionArgs(_defaultSource, _defaultStartTarget, _defaultEndTarget);
		KeyValuePair<string, TreeType> keyValuePair = _ActionNameDictionary.First(s => s.Value == _TreeType);
		firstStep.Arguments.ActionName = keyValuePair.Key;

		_ActionPlanQueue.AddLast(firstStep);		

		RunningActions = new List<string>();
		//RunningActions.Add("StandIdleShow");

		// vvvv Testing Nil's action for the SantaFe problem vvvv
		/////////////////////////////////////////////////////////////////////////////////

//		XmlDocument doc = new XmlDocument();
//		doc.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\" ?>\n<oc:action-plan xmlns:oc=\"http://www.opencog.org/brain\" demand=\"\" entity-id=\"OAC_AGI_Robot\" id=\"0\">\n<action name=\"step_forward\" sequence=\"1\"/>\n</oc:action-plan>");
//		
//		XmlNodeList list = doc.GetElementsByTagName(OCEmbodimentXMLTags.ACTION_PLAN_ELEMENT);
//		XmlElement root = (XmlElement)list.Item(0);//MakeXMLElementRoot(doc);
//
//		CharacterController charControl = gameObject.GetComponent<CharacterController>();
//		bool tryParse = true;

		/////////////////////////////////////////////////////////////////////////////////

		while(Application.isPlaying)
		{
			yield return new WaitForSeconds(1.0f / 100.0f);
			UpdateAI();

//			if(tryParse && charControl.isGrounded)
//			{
//				OCConnectorSingleton.Instance.ParseActionPlanElement(root);
//				tryParse = false;
//			}
		}
	}
	
//			private void TestProprioception()
//			{
//				Debug.Log ("RobotAgent's Transform is at position [" + this.transform.position.x + ", " + this.transform.position.y + ", " + this.transform.position.z + "]");
//		
//				Map map = (Map)GameObject.FindObjectOfType (typeof(Map));
//				
//				float characterHeight = this.GetComponent<CharacterController>().height;
//				
//				//map.IsPathOpen(this.transform, characterHeight, Map.PathDirection.ForwardWalk);
//				//map.IsPathOpen(this.transform, characterHeight, Map.PathDirection.ForwardRun);
//				//map.IsPathOpen(this.transform, characterHeight, Map.PathDirection.ForwardRun);
//				//map.IsPathOpen(this.transform, characterHeight, Map.PathDirection.ForwardJump);
//				//map.IsPathOpen(this.transform, characterHeight, Map.PathDirection.ForwardClimb);
//				//map.IsPathOpen(this.transform, characterHeight, Map.PathDirection.ForwardDrop);
//			}

	public void Update()
	{
//		if(Time.frameCount%120 == 0)
//		{
//			var bestWeight = -1.0;
//			String playing = "";
//			foreach (AnimationState s in animation)
//			{
//    		if (s.enabled && s.weight > bestWeight)
//				{
//       		playing += s.name + " ";
//        	bestWeight = s.weight;
//    		}
//			}
//			Debug.Log("Animation State: " + playing);
//		}
		
//				this.TestProprioception ();
	}

	public BehaveResult	 Tick(Tree sender, bool init)
	{
//			Debug.Log
//			(
//				"Got ticked by unhandled " + (BLOpenCogCharacterBehaviours.IsAction( sender.ActiveID ) ? "action" : "decorator")
//			+ ( BLOpenCogCharacterBehaviours.IsAction( sender.ActiveID )
//				? ((BLOpenCogCharacterBehaviours.ActionType)sender.ActiveID).ToString()
//				: ((BLOpenCogCharacterBehaviours.DecoratorType)sender.ActiveID).ToString()
//				)
//			);

		return BehaveResult.Failure;
	}

//	public BehaveResult TickIdleAction(Tree sender, string stringParameter, float floatParameter, IAgent agent, object data)
//	{
//			Debug.Log("In Robot Idle...");
//
//			return BehaveResult.Success;
//	}

//	public BehaveResult FallAction 
//	{
//		get {
//			OCFallForwardAction action = gameObject.GetComponent<OCFallForwardAction> ();
//
//			BehaveResult ret = DefaultActionTickHandler (action);
//
//			OpenCog.Map.OCMap map = (OpenCog.Map.OCMap)GameObject.FindObjectOfType (typeof(OpenCog.Map.OCMap));
//
//			if (ret != BehaveResult.Success)
//				return ret;
//
//			CharacterController charController = gameObject.GetComponent<CharacterController> ();
//
//			Vector3 robotPos = gameObject.transform.position;
//			Vector3 distanceVec = ((Vector3)TargetBlockPos) - robotPos;
//			float robotForwardDistance = Vector3.Dot (distanceVec, gameObject.transform.forward);
//			float robotUpDistance = Vector3.Dot (distanceVec, gameObject.transform.up);
//			float robotRightDistance = Vector3.Dot (distanceVec, gameObject.transform.right);
//			float robotLeftDistance = Vector3.Dot (distanceVec, -gameObject.transform.right);
//
//			if (TargetBlockPos != Vector3i.zero
//				&& map.IsPathOpen(transform, charController.height, OpenCog.Map.OCMap.PathDirection.ForwardDrop)
////					&& robotForwardDistance <= 2.5f
//			&& robotForwardDistance >= 0.5f
////					&& robotRightDistance < 0.5f
////					&& robotLeftDistance < 0.5f
////					&& charController.isGrounded
//			) {
//				action.Execute ();
//				//Debug.Log("In OCRobotAgent.FallAction, " + action.GetType() + " Success");
//				return BehaveResult.Success;
//			}
//
//			//Debug.Log("In OCRobotAgent.FallAction, " + action.GetType() + " Failure");
//			return BehaveResult.Failure;
//		}
//
//		set {
//		}
//	}
//
//	public BehaveResult IdleAction 
//	{
//	// tick handler
//		get {
//			OCIdleAction action = gameObject.GetComponent<OCIdleAction> ();
//			CharacterController charController = gameObject.GetComponent<CharacterController> ();
//
//			BehaveResult ret = DefaultActionTickHandler (action);
//
//			if (ret != BehaveResult.Success)
//				return ret;
//
//			if (TargetBlockPos == Vector3i.zero && charController.isGrounded) {
//				action.Execute ();
//				//Debug.Log("In OCRobotAgent.IdleAction, " + action.GetType() + " Success");
//				return BehaveResult.Success;
//			}
//
//			//Debug.Log("In OCRobotAgent.IdleAction, " + action.GetType() + " Failure");
//			return BehaveResult.Failure;
//		}
//
//	// reset handler
//		set {
//		}
//	}
//
//	public BehaveResult ClimbAction 
//	{
//	// tick handler
//		get {
//			OCClimbUpAction action = gameObject.GetComponent<OCClimbUpAction> ();
//
//			BehaveResult ret = DefaultActionTickHandler (action);
//
//			OpenCog.Map.OCMap map = (OpenCog.Map.OCMap)GameObject.FindObjectOfType (typeof(OpenCog.Map.OCMap));
//
//			if (ret != BehaveResult.Success)
//				return ret;
//
//			CharacterController charController = gameObject.GetComponent<CharacterController> ();
//
//			Vector3 robotPos = gameObject.transform.position;
//			Vector3 distanceVec = ((Vector3)TargetBlockPos) - robotPos;
//			float robotUpDistance = Vector3.Dot (distanceVec, gameObject.transform.up);
//			float robotForwardDistance = Vector3.Dot (distanceVec, gameObject.transform.forward);
//
//			if (TargetBlockPos != Vector3i.zero
//			//&& robotUpDistance >= 1.5f
//			&& robotForwardDistance >= 0.5f
//			&& map.IsPathOpen(transform, charController.height, OpenCog.Map.OCMap.PathDirection.ForwardClimb)
//			&& charController.isGrounded) {
//				action.Execute ();
//				//Debug.Log("In OCRobotAgent.ClimbAction, " + action.GetType() + " Success");
//				return BehaveResult.Success;
//			}
//
//			//Debug.Log("In OCRobotAgent.ClimbAction, " + action.GetType() + " Failure");
//			return BehaveResult.Failure;
//		}
//
//	// reset handler
//		set {
//		}
//	}
//
//	public BehaveResult RunAction 
//	{
//	// tick handler
//		get {
//			OCRunForwardAction action = gameObject.GetComponent<OCRunForwardAction> ();
//
//			BehaveResult ret = DefaultActionTickHandler (action);
//
//			OpenCog.Map.OCMap map = (OpenCog.Map.OCMap)GameObject.FindObjectOfType (typeof(OpenCog.Map.OCMap));
//
//			if (ret != BehaveResult.Success)
//				return ret;
//
//			CharacterController charController = gameObject.GetComponent<CharacterController> ();
//
//			Vector3 robotPos = gameObject.transform.position;
//			Vector3 distanceVec = ((Vector3)TargetBlockPos) - robotPos;
//			float robotForwardDistance = Vector3.Dot (distanceVec, gameObject.transform.forward);
//			float robotUpDistance = Vector3.Dot (distanceVec, gameObject.transform.up);
//			float robotRightDistance = Vector3.Dot (distanceVec, gameObject.transform.right);
//			float robotLeftDistance = Vector3.Dot (distanceVec, -gameObject.transform.right);
//
//			if (TargetBlockPos != Vector3i.zero
//				&& map.IsPathOpen(transform, charController.height, OpenCog.Map.OCMap.PathDirection.ForwardRun)
//			&& robotForwardDistance > 3.5f
////					&& robotRightDistance < 1.5f
////					&& robotLeftDistance < 1.5f
//			&& charController.isGrounded
//			) {
//				action.Execute ();
//				//Debug.Log ("In OCRobotAgent.RunAction, " + action.GetType () + " Success");
//				return BehaveResult.Success;
//			}
//
//			//Debug.Log ("In OCRobotAgent.RunAction, " + action.GetType () + " Failure");
//			return BehaveResult.Failure;
//		}
//
//	// reset handler
//		set {
//		}
//	}
//
//	public BehaveResult JumpAction 
//	{
//	// tick handler
//		get {
//			OCJumpForwardAction action = gameObject.GetComponent<OCJumpForwardAction> ();
//
//			BehaveResult ret = DefaultActionTickHandler (action);
//
//			OpenCog.Map.OCMap map = (OpenCog.Map.OCMap)GameObject.FindObjectOfType (typeof(OpenCog.Map.OCMap));
//
//			if (ret != BehaveResult.Success)
//				return ret;
//
//			CharacterController charController = gameObject.GetComponent<CharacterController> ();
//
//			Vector3 robotPos = gameObject.transform.position;
//			Vector3 distanceVec = ((Vector3)TargetBlockPos) - robotPos;
//			float robotUpDistance = Vector3.Dot (distanceVec, gameObject.transform.up);
//			float robotForwardDistance = Vector3.Dot (distanceVec, gameObject.transform.forward);
//
//			if (TargetBlockPos != Vector3i.zero
//				&& map.IsPathOpen(transform, charController.height, OpenCog.Map.OCMap.PathDirection.ForwardJump)
//			&& robotUpDistance >= 1.5f
//			&& robotForwardDistance >= 0.0f
//			&& charController.isGrounded) {
//				action.Execute ();
//				//Debug.Log("In OCRobotAgent.JumpAction, " + action.GetType() + " Success");
//				return BehaveResult.Success;
//			}
//
//			//Debug.Log("In OCRobotAgent.JumpAction, " + action.GetType() + " Failure");
//			return BehaveResult.Failure;
//		}
//
//	// reset handler
//		set {
//		}
//	}
//
//	public BehaveResult TurnLeftAction 
//	{
//	// tick handler
//		get {
//			OCTurnLeftAction action = gameObject.GetComponent<OCTurnLeftAction> ();
//
//			BehaveResult ret = DefaultActionTickHandler (action);
//
//			if (ret != BehaveResult.Success)
//				return ret;
//
//			CharacterController charController = gameObject.GetComponent<CharacterController> ();
//
//			Vector3 robotPos = gameObject.transform.position;
//			Vector3 distanceVec = ((Vector3)TargetBlockPos) - robotPos;
//			float robotForwardDistance = Vector3.Dot (distanceVec, gameObject.transform.forward);
//			float robotRightDistance = Vector3.Dot (distanceVec, gameObject.transform.right);
//			float robotLeftDistance = Vector3.Dot (distanceVec, -gameObject.transform.right);
//
//			if (TargetBlockPos != Vector3i.zero
//				&& ( robotLeftDistance >= 0.5f || robotForwardDistance < 0.0f)
//				&& charController.isGrounded) {
//				action.Execute ();
//				//Debug.Log("In OCRobotAgent.TurnLeftAction, " + action.GetType() + " Success");
//				return BehaveResult.Success;
//			}
//
//			//Debug.Log("In OCRobotAgent.TurnLeftAction, " + action.GetType() + " Failure");
//			return BehaveResult.Failure;
//		}
//
//	// reset handler
//		set {
//		}
//	}
//
//	public BehaveResult TurnRightAction 
//	{
//	// tick handler
//		get {
//			OCTurnRightAction action = gameObject.GetComponent<OCTurnRightAction> ();
//
//			BehaveResult ret = DefaultActionTickHandler (action);
//
//			if (ret != BehaveResult.Success)
//				return ret;
//
//			CharacterController charController = gameObject.GetComponent<CharacterController> ();
//
//			Vector3 robotPos = gameObject.transform.position;
//			Vector3 distanceVec = ((Vector3)TargetBlockPos) - robotPos;
//			float robotForwardDistance = Vector3.Dot (distanceVec, gameObject.transform.forward);
//			float robotRightDistance = Vector3.Dot (distanceVec, gameObject.transform.right);
//			float robotLeftDistance = Vector3.Dot (distanceVec, -gameObject.transform.right);
//
//			if (TargetBlockPos != Vector3i.zero
//				&& robotRightDistance >= 0.5f
//				&& charController.isGrounded) {
//				action.Execute ();
//				//Debug.Log("In OCRobotAgent.TurnRightAction, " + action.GetType() + " Success");
//				return BehaveResult.Success;
//			}
//
//			//Debug.Log("In OCRobotAgent.TurnRightAction, " + action.GetType() + " Failure");
//			return BehaveResult.Failure;
//		}
//
//	// reset handler
//		set {
//		}
//	}
//
//	public BehaveResult WalkAction 
//	{
//	// tick handler
//		get {
//			OCWalkForwardAction action = gameObject.GetComponent<OCWalkForwardAction> ();
//
//			BehaveResult ret = DefaultActionTickHandler (action);
//
//			OpenCog.Map.OCMap map = (OpenCog.Map.OCMap)GameObject.FindObjectOfType (typeof(OpenCog.Map.OCMap));
//
//			if (ret != BehaveResult.Success)
//				return ret;
//
//			CharacterController charController = gameObject.GetComponent<CharacterController> ();
//
//			Vector3 robotPos = gameObject.transform.position;
//			Vector3 distanceVec = ((Vector3)TargetBlockPos) - robotPos;
//			float robotForwardDistance = Vector3.Dot (distanceVec, gameObject.transform.forward);
//			float robotUpDistance = Vector3.Dot (distanceVec, gameObject.transform.up);
//			float robotRightDistance = Vector3.Dot (distanceVec, gameObject.transform.right);
//			float robotLeftDistance = Vector3.Dot (distanceVec, -gameObject.transform.right);
//
//			if (TargetBlockPos != Vector3i.zero
//				&& map.IsPathOpen(transform, charController.height, OpenCog.Map.OCMap.PathDirection.ForwardWalk)
//			&& robotForwardDistance <= 3.5f
//			&& robotForwardDistance >= 0.5f
//			&& robotUpDistance <= 1.5f
////					&& robotRightDistance < 0.5f
////					&& robotLeftDistance < 0.5f
//			&& charController.isGrounded
//			) {
//				action.Execute ();
//				//Debug.Log("In OCRobotAgent.WalkAction, " + action.GetType() + " Success");
//				return BehaveResult.Success;
//			}
//
//			//Debug.Log("In OCRobotAgent.WalkAction, " + action.GetType() + " Failure");
//			return BehaveResult.Failure;
//		}
//
//	// reset handler
//		set {
//		}
//	}
			
	// Map XML elements to high-, mid-, or low-behaviour trees.
	// Then queue each element in the plan in the behaviour queue.
	public void ReceiveActionPlan(List<XmlElement> actionPlan)
	{
//		Debug.Log("In ReceiveActionPlan...");
//				
//		string actionName = GetAttribute(element, OCEmbodimentXMLTags.NAME_ATTRIBUTE);
//
//	  int sequence = int.Parse(GetAttribute(element, OCEmbodimentXMLTags.SEQUENCE_ATTRIBUTE));
//	  ArrayList paramList = new ArrayList();
//	
//	  XmlNodeList list = GetChildren(element, OCEmbodimentXMLTags.PARAMETER_ELEMENT);
//	  // Extract parameters from the xml element.
//	  for (int i = 0; i < list.Count; i++)
//	  {
//      XmlElement parameterElement = (XmlElement)list.Item(i);
//      ActionParamType parameterType = ActionParamType.getFromName(parameterElement.GetAttribute(OCEmbodimentXMLTags.TYPE_ATTRIBUTE));
//
//      switch (parameterType.getCode())
//      {
//        case ActionParamTypeCode.VECTOR_CODE:
//          XmlElement vectorElement = ((XmlElement)(GetChildren(parameterElement, OCEmbodimentXMLTags.VECTOR_ELEMENT)).Item(0));
//          float x = float.Parse(GetAttribute(vectorElement, OCEmbodimentXMLTags.X_ATTRIBUTE), CultureInfo.InvariantCulture.NumberFormat);
//          float y = float.Parse(GetAttribute(vectorElement, OCEmbodimentXMLTags.Y_ATTRIBUTE), CultureInfo.InvariantCulture.NumberFormat);
//          float z = float.Parse(GetAttribute(vectorElement, OCEmbodimentXMLTags.Z_ATTRIBUTE), CultureInfo.InvariantCulture.NumberFormat);
//
//					if (adjustCoordinate)
//					{
//						x += 0.5f;
//						y += 0.5f;
//						z += 0.5f;
//					}
//
//          // swap z and y
//          paramList.Add(new Vector3(x, z, y)); 
//          break;
//        case ActionParamTypeCode.BOOLEAN_CODE:
//          paramList.Add(Boolean.Parse(GetAttribute(parameterElement, OCEmbodimentXMLTags.VALUE_ATTRIBUTE)));
//          break;
//        case ActionParamTypeCode.INT_CODE:
//          paramList.Add(int.Parse(GetAttribute(parameterElement, OCEmbodimentXMLTags.VALUE_ATTRIBUTE)));
//          break;
//        case ActionParamTypeCode.FLOAT_CODE:
//          paramList.Add(float.Parse(GetAttribute(parameterElement, OCEmbodimentXMLTags.VALUE_ATTRIBUTE)));
//          break;
//        case ActionParamTypeCode.ROTATION_CODE:
//          //!! This is a hacky trick. For currently, we do not use rotation
//          // in rotate method, so just convert it to vector type. What's more,
//          // "RotateTo" needs an angle parameter.
//
//          // Trick... add an angle...
//          XmlElement rotationElement = ((XmlElement)(GetChildren(parameterElement, OCEmbodimentXMLTags.ROTATION_ELEMENT)).Item(0));
//          float pitch = float.Parse(GetAttribute(rotationElement, OCEmbodimentXMLTags.PITCH_ATTRIBUTE), CultureInfo.InvariantCulture.NumberFormat);
//          float roll = float.Parse(GetAttribute(rotationElement, OCEmbodimentXMLTags.ROLL_ATTRIBUTE), CultureInfo.InvariantCulture.NumberFormat);
//          float yaw = float.Parse(GetAttribute(rotationElement, OCEmbodimentXMLTags.YAW_ATTRIBUTE), CultureInfo.InvariantCulture.NumberFormat);
//
//          Rotation rot = new Rotation(pitch, roll, yaw);
//          Vector3 rot3 = new Vector3(rot.Pitch, rot.Roll, rot.Yaw);
//
//          paramList.Add(0.0f);
//          paramList.Add(rot3);
//          break;
//        case ActionParamTypeCode.ENTITY_CODE:
//          // This action is supposed to act on certain entity.
//          XmlElement entityElement = ((XmlElement)(GetChildren(parameterElement, OCEmbodimentXMLTags.ENTITY_ELEMENT)).Item(0));
//
//          int id = int.Parse(GetAttribute(entityElement, OCEmbodimentXMLTags.ID_ATTRIBUTE));
//          string type = GetAttribute(entityElement, OCEmbodimentXMLTags.TYPE_ATTRIBUTE);
//          ActionTarget target = new ActionTarget(id, type);
//
//          paramList.Add(target);
//          break;
//        default:
//          paramList.Add(GetAttribute(parameterElement, OCEmbodimentXMLTags.VALUE_ATTRIBUTE));
//          break;
//      }
//	  } 
	}
			
	public void LoadActionPlanStep(string actionName, OCAction.OCActionArgs arguments)
	{
		//Debug.Log("OCActionController::LoadActionPlanStep: " + actionName);
		TreeType treeType = _ActionNameDictionary[actionName];
		Tree tree = _TreeTypeDictionary[treeType];
		OCActionPlanStep actionPlanStep = OCScriptableObject.CreateInstance<OCActionPlanStep>();
		actionPlanStep.Behaviour = tree;
		actionPlanStep.Arguments = arguments;
		_ActionPlanQueue.AddLast(actionPlanStep);
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "Enqueued Action Step: " + actionPlanStep.Arguments.ActionName);
	}
			
	public void CancelActionPlan()
	{
		_step.Behaviour.Reset();
		_ActionPlanQueue.Clear();
	}
	
	public void UpdateAI()
	{
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "In OCActionController.UpdateAI, _step = " + _step);
		
		if(_step == null)
		{
			if(_ActionPlanQueue.Count != 0) {
				_step = _ActionPlanQueue.First();
				_ActionPlanQueue.RemoveFirst();
				System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "In OCActionController.UpdateAI, starting action step: " + _step.Arguments.ActionName + ", retry: " + _step.Retry);
            } else {
				_PlanSucceeded = true;
				//_LastPlanEndedAtTime = System.DateTime.Now.Ticks;

				OCActionPlanStep step = OCScriptableObject.CreateInstance<OCActionPlanStep>();
				step.Behaviour = _TreeTypeDictionary[_TreeType];
				step.Arguments = new OCAction.OCActionArgs(_defaultSource, _defaultStartTarget, _defaultEndTarget);
				KeyValuePair<string, TreeType> keyValuePair = _ActionNameDictionary.First(s => s.Value == _TreeType);
				step.Arguments.ActionName = keyValuePair.Key;
				_step = step;
			}
		}
				
		BehaveResult result = _step.Behaviour.Tick();

        System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "In OCActionController.UpdateAI, result = " + result);
				
//		if((_step.Behaviour.Name == _TreeTypeDictionary[TreeType.Character_IdleShow].Name) && result == BehaveResult.Success)
//		{
//			//iTween itween = _step.Arguments.Source.GetComponent<iTween>();
//			iTween.Stop(_step.Arguments.Source);
//		}		
				
		if(result != BehaveResult.Running)
		{
			OCAction.OCActionArgs args = _step.Arguments; 	
					
//			if((_step.Behaviour.Name != _TreeTypeDictionary[_TreeType].Name) 
//				|| ((_step.Behaviour.Name == _TreeTypeDictionary[_TreeType].Name) 
//					&& (GameObject.Find("EndPointStub").transform.position != Vector3.zero)))
			{
				// if we have a goal...
				if(_step.Arguments.EndTarget != null)
				{
					if(_step.Arguments.EndTarget.transform.position != Vector3.zero)
					{
						_PlanSucceeded &= result == BehaveResult.Success;
						//_LastPlanEndedAtTime = System.DateTime.Now.Ticks;
					}
							
					//Vector3 startPosition = _step.Arguments.StartTarget.transform.position;
					Vector3 endPosition = _step.Arguments.EndTarget.transform.position;
					Vector3 sourcePosition = _step.Arguments.Source.transform.position;
					sourcePosition.y = sourcePosition.y - 0.5f;
								
					//Vector3 startToEnd = endPosition - startPosition;
					//Vector3 sourceToEnd = endPosition - sourcePosition;		
							
					//float startToEndManDist = Math.Abs(endPosition.x - startPosition.x) + Math.Abs(endPosition.y - startPosition.y) + Math.Abs(endPosition.z - startPosition.z);
					//float sourceToEndManDist = Math.Abs(endPosition.x - sourcePosition.x) + Math.Abs(endPosition.y - sourcePosition.y) + Math.Abs(endPosition.z - sourcePosition.z);
							
					if(_step.Behaviour.Name == "Character.Move" || _step.Arguments.ActionName == "walk" || _step.Arguments.ActionName == "jump_toward")
					{
						// don't use euclideon distance
						//_PlanSucceeded |= sourceToEnd.sqrMagnitude < startToEnd.sqrMagnitude;
								
						// use manhattan distance
						//_PlanSucceeded = sourceToEndManDist <= startToEndManDist;

						if(VectorUtil.AreVectorsEqual(sourcePosition, endPosition))
						{
							_PlanSucceeded = true;
						} else
						{
							_PlanSucceeded = false;
						}
						//_LastPlanEndedAtTime = System.DateTime.Now.Ticks;
					}
							
					if(_step.Behaviour.Name == "Character.Destroy" || _step.Arguments.ActionName == "eat")
					{
						_PlanSucceeded = (endPosition == Vector3.zero || _step.Arguments.EndTarget == null);
						//_LastPlanEndedAtTime = System.DateTime.Now.Ticks;
					}

					if(_step.Arguments.ActionName == "grab")
					{
						_PlanSucceeded = OCAction.IsEndTargetCloseForward(null, _step.Arguments);
						//_LastPlanEndedAtTime = System.DateTime.Now.Ticks;
					}
							
//					if(_step.Arguments.ActionName == "grab")
//					{
//						_PlanSucceeded = endPosition != startPosition && endPosition != null;
//					}
							
					if(_step.Arguments.ActionPlanID != null && (_PlanSucceeded || _step.Retry > OCActionPlanStep.MaxRetries))
					{
						OCConnectorSingleton.Instance.SendActionStatus(args.ActionPlanID, args.SequenceID, args.ActionName, _PlanSucceeded);

						if(_step.Behaviour.Name != "Character.IdleShow" && !_step.Behaviour.Name.Contains("Behaviour"))
						{
								System.Console.WriteLine(OCLogSymbol.RUNNING + "In OCActionController.UpdateAI, Result: " + (_PlanSucceeded ? "Success" : "Failure") + " for Action: " + (_step.Arguments.ActionName == null ? _step.Behaviour.Name : (_step.Arguments.ActionName + " & Sequence: " + _step.Arguments.SequenceID)));
						}		
					}
//							else if(_step.Arguments.ActionPlanID == null && (_PlanSucceeded || _step.Retry > OCActionPlanStep.MaxRetries) && OCConnectorSingleton.Instance.IsEstablished )
//					{
//								OCConnectorSingleton.Instance.HandleOtherAgentActionResult(_step, _PlanSucceeded);
//					}
				}
						
//				if(!_PlanSucceeded)
//					Debug.LogWarning(" -- Step Failed: " + (_step.Arguments.ActionName == null ? _step.Behaviour.Name : _step.Arguments.ActionName));						
					

				
				_step.Behaviour.Reset();

				System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "In OCActionController.UpdateAI, _step (after reset) = " + _step);
                System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "In OCActionController.UpdateAI, _PlanSucceeded = " + _PlanSucceeded);

				// if we failed, retry last step
				if(_PlanSucceeded == false && OCActionPlanStep.MaxRetries > _step.Retry)
				{
					_ActionPlanQueue.AddFirst(_step);
					_step.Retry += 1;
				} 
				else if(_PlanSucceeded == false && OCActionPlanStep.MaxRetries <= _step.Retry)
				{
					_ActionPlanQueue.Clear();
					_step = null;
				} 
				else if(_step.Arguments.EndTarget)
				{
					OCFadeOutGameObject fadeOut = _step.Arguments.EndTarget.GetComponent<OCFadeOutGameObject>();
					
					if(fadeOut != null)
					{
						fadeOut.enabled = true;
					}
				}
				
				System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "In OCActionController.UpdateAI, _ActionPlanQueue.Count = " + _ActionPlanQueue.Count);
				System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "In OCActionController.UpdateAI, _LastPlanID = " + _LastPlanID);
				
				if(_ActionPlanQueue.Count == 0)
				{
					if(_LastPlanID != null)
					{
							
								
//						if(result == BehaveResult.Failure)
//							OCConnectorSingleton.Instance.SendActionStatus(args.ActionPlanID, args.SequenceID, args.ActionName, true);			
								
						OCConnectorSingleton.Instance.SendActionPlanStatus(_LastPlanID, _PlanSucceeded /*, _LastPlanEndedAtTime*/);

						if(_step != null && _step.Arguments.EndTarget != null)
						{
							OCFadeOutGameObject fadeOut = _step.Arguments.EndTarget.GetComponent<OCFadeOutGameObject>();
						
							if(fadeOut != null)
							{
								fadeOut.enabled = true;
							}
						}

						_LastPlanID = null;
					}
					_step = null;	
				} else
				{
					_step = _ActionPlanQueue.First();
					_ActionPlanQueue.RemoveFirst();
					System.Console.WriteLine(OCLogSymbol.RUNNING + "In OCActionController.UpdateAI, re-starting action step: " + _step.Arguments.ActionName + ", retry: " + _step.Retry);

					if(_LastPlanID != null)
					{
						if(_LastPlanID != _step.Arguments.ActionPlanID)
						{
							Debug.LogError(OCLogSymbol.ERROR + "We've changed plans without reporting back to OpenCog!");
						}
					} else
					{
						_LastPlanID = _step.Arguments.ActionPlanID;
					}
				}
			}
		}
	}

	public void	 Reset(Tree sender)
	{
	}

	public int	 SelectTopPriority(Tree sender, params int[] IDs)
	{
		return IDs[0];
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------

//	private BehaveResult DefaultActionTickHandler (OCAction action)
//	{
//		Vector3 robotPos = gameObject.transform.position;
//		Vector3 distanceVec = ((Vector3)TargetBlockPos) - robotPos;
//		
//		if (action == null)
//			return BehaveResult.Failure;
//
//		if (action.ShouldTerminate ()) {
//			//action.Terminate();
//			//Debug.Log ("In OCRobotAgent.DefaultActionTickHandler, " + action.GetType () + " Failure");
//			return BehaveResult.Failure;
//		}
//
//		if (action.IsExecuting ()) {
//			//Debug.Log("In OCRobotAgent.DefaultActionTickHandler, " + action.GetType() + " Running");
//			return BehaveResult.Running;
//		}
//
//		if (TargetBlockPos != Vector3i.zero) {
//			//Debug.Log("In OCRobotAgent.DefaultActionTickHandler, Distance to TNT block is: " + distanceVec.magnitude + ", Vector is: " + distanceVec);
//		}
//
//		return BehaveResult.Success;
//	}
	
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------
			
	
			
	// TODO [Dynamic Behavior Tree]: This code is just a set of stubs to get rid of an error.
	//public static event ActionCompleteHandler globalActionCompleteEvent;
	//public delegate void ActionCompleteHandler(OCAction action);
	// Removed...due to the fact that OCConnector will be polling this class for action status updates.

	// TODO [Dynamic Behavior Tree]: Implement / remove build block function which can be called by OCConnector.ParseSingleActionElement. Will probably have to be amended with material / blockdata.
	public void BuildBlockAtPosition(Vector3i desiredBlockLocation)
	{
		OCAction.OCActionArgs args = new OCAction.OCActionArgs();
		args.Source = _defaultSource;
		args.StartTarget = _defaultStartTarget;
		args.EndTarget = _defaultEndTarget;
		
		args.StartTarget.transform.position = args.Source.transform.position;
		args.EndTarget.transform.position = desiredBlockLocation;
				
		LoadActionPlanStep("BuildBlockAtPosition", args);
	}

	// TODO [Dynamic Behavior Tree]: Implement / remove move to location function which can be called by OCConnector.ParseSingleActionElement.
	public void MoveToCoordinate(Vector3 desiredLocation)
	{
		OCAction.OCActionArgs args = new OCAction.OCActionArgs();
		args.Source = _defaultSource;
		args.StartTarget = _defaultStartTarget;
		args.EndTarget = _defaultEndTarget;
		
		args.StartTarget.transform.position = args.Source.transform.position;
		args.EndTarget.transform.position = desiredLocation;
				
		LoadActionPlanStep("MoveToCoordinate", args);
	}

	// TODO [Dynamic Behavior Tree]: Implement function below properly.
	public static string GetOCActionNameFromMap(string methodName)
	{
		if(builtinActionMap.ContainsValue(methodName))
		{
			foreach(KeyValuePair<string, string> pair in builtinActionMap)
			{
				if(pair.Value == methodName)
				{
					return pair.Key;
				}
			}
		}

		return null;
	}
			
	// TODO [Dynamic Behavior Tree]: Implement dynamic behaviour tree loading to execute actions
	public void StartAction(OCAction action, OCID sourceID, OCID targetStartID, OCID targetEndID)
	{
		Debug.LogError("OCActionController.StartAction is unimplemented...");
	}
			
	public Dictionary<string, OCAction> GetCurrentActions()
	{
		return new Dictionary<string, OCAction>();
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCRobotAgent

}// namespace Character

}// namespace OpenCog




