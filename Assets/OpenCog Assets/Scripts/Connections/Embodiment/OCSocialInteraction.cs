
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

#region Usings, Namespaces, and Pragmas

using System.Collections;
using OpenCog.Attributes;
using OpenCog.Extensions;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Embodiment
{

/// <summary>
/// The OpenCog SocialInteraction.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCSocialInteraction : OCMonoBehaviour
{
//	DEPRECATED: This has not worked since the ancient dayyyyssss
//	//---------------------------------------------------------------------------
//
//	#region Private Member Data
//
//	//---------------------------------------------------------------------------
//		
//	private ActionSummary _touchAction;  // touch me, depend on the force you use, the npc would consider as pat, push or hit.
//	private ActionSummary _kissAction; // kiss me
//	private ActionSummary _hugAction;  // hug me
//		
//	//---------------------------------------------------------------------------
//
//	#endregion
//
//	//---------------------------------------------------------------------------
//
//	#region Accessors and Mutators
//
//	//---------------------------------------------------------------------------
//		
//	//---------------------------------------------------------------------------
//
//	#endregion
//
//	//---------------------------------------------------------------------------
//
//	#region Public Member Functions
//
//	//---------------------------------------------------------------------------
//
//	/// <summary>
//	/// Called when the script instance is being loaded.
//	/// </summary>
//	public void Awake()
//	{
//		Initialize();
//		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is awake.");
//	}
//
//	/// <summary>
//	/// Use this for initialization
//	/// </summary>
//	public void Start()
//	{
//		AnimSummary animS = new AnimSummary();
//		OCPhysiologicalEffect effect = new OCPhysiologicalEffect(OCPhysiologicalEffect.CostLevel.LOW);
//		_touchAction = new ActionSummary(this, "Touch", animS, effect, true);
//		_touchAction.usesCallback = true;
//		myActionList.Add("Touch");
//		
//		_kissAction = new ActionSummary(this, "Kiss", animS, effect, true);
//		_kissAction.usesCallback = true;
//		myActionList.Add("Kiss");
//		
//		_hugAction = new ActionSummary(this, "Hug", animS, effect, true);
//		_hugAction.usesCallback = true;
//		myActionList.Add("Hug");
//
//		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is started.");
//	}
//
//	/// <summary>
//	/// Update is called once per frame.
//	/// </summary>
//	public void Update()
//	{
//		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is updated.");	
//	}
//		
//	/// <summary>
//	/// Reset this instance to its default values.
//	/// </summary>
//	public void Reset()
//	{
//		Uninitialize();
//		Initialize();
//		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is reset.");	
//	}
//
//	/// <summary>
//	/// Raises the enable event when SocialInteraction is loaded.
//	/// </summary>
//	public void OnEnable()
//	{
//		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is enabled.");
//	}
//
//	/// <summary>
//	/// Raises the disable event when SocialInteraction goes out of scope.
//	/// </summary>
//	public void OnDisable()
//	{
//		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is disabled.");
//	}
//
//	/// <summary>
//	/// Raises the destroy event when SocialInteraction is about to be destroyed.
//	/// </summary>
//	public void OnDestroy()
//	{
//		Uninitialize();
//		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is about to be destroyed.");
//	}
//
//	public void AddAction(Avatar avatar)
//	{
//		ActionManager AM = avatar.GetComponent<ActionManager>() as ActionManager;
//		AM.addAction(_touchAction);
//		AM.addAction(_kissAction);
//		AM.addAction(_hugAction);
//
//
//	}
//    
//	public void RemoveAction(Avatar avatar)
//	{
//		ActionManager AM = avatar.GetComponent<ActionManager>() as ActionManager;
//		AM.removeAction(gameObject.GetInstanceID(), "Touch");
//		AM.removeAction(gameObject.GetInstanceID(), "Kiss");
//		AM.removeAction(gameObject.GetInstanceID(), "Hug");		
//	}
//
//	public void @Touch(Avatar a, float force =300.0f, ActionCompleteHandler completionCallback=null)
//	{
//		StartCoroutine(ApplyTouch(a, force, completionCallback));
//	}
//	
//	public void Kiss(Avatar a, ActionCompleteHandler completionCallback=null)
//	{
//		
//		// todo: we need a kiss animation
//		Debug.Log(a.gameObject.name + "kiss " + gameObject.name);
//		
//	}
//	
//	public void Hug(Avatar a, ActionCompleteHandler completionCallback=null)
//	{
//
//		// todo: we need a hug animation
//		Debug.Log(a.gameObject.name + "hug " + gameObject.name);
//		
//	}
//
//	//---------------------------------------------------------------------------
//
//	#endregion
//
//	//---------------------------------------------------------------------------
//
//	#region Private Member Functions
//
//	//---------------------------------------------------------------------------
//	
//	/// <summary>
//	/// Initializes this instance.  Set default values here.
//	/// </summary>
//	private void Initialize()
//	{
//	}
//	
//	/// <summary>
//	/// Uninitializes this instance.  Cleanup refernces here.
//	/// </summary>
//	private void Uninitialize()
//	{
//	}
//
//	private IEnumerator ApplyTouch(Avatar a, float force, ActionCompleteHandler completionCallback=null)
//	{
//		// if the avatar is a player, we get the force from the HUG force Panel
//		if(a.gameObject.tag == "Player")
//		{
//			Player player = a as Player;
//			OCHeadUpDisplay theHud = player.GetTheHUD();
//			if(theHud == null)
//			{
//				Debug.LogError("The player's HUD is null! --in touch action");
//				yield break;
//			}
//			yield return StartCoroutine(theHud.gettingForceFromHud(this));
//			
//			force = theHud.GetCurrentForceVal();
//		}
//			
//		// Touch an avatar will add a force to it, and it maybe push on the forward direction of the toucher
//		// if the avatar will be pushed forward , depends on the force and the mass of avatar
//		
//		Rigidbody avatarRigid = gameObject.GetComponent<Rigidbody>() as Rigidbody;
//		float mass = avatarRigid.mass;
//		
//		// caculate the relative force
//		float relforce = force / 20.0f / mass;
//		Vector3 direction = a.transform.forward;
//		
//		if(relforce > 1.0f)
//		{
//			Vector3 dest = gameObject.transform.position;
//	   	    
//			dest.x += direction.x * relforce;
//			dest.y += 0.0f;
//			dest.z += direction.z * relforce;	
//			iTween.MoveTo(gameObject, iTween.Hash("position", dest,
//		                                  "speed", relforce,
//		                                  "easetype", iTween.EaseType.easeInOutCubic));
//			
//		}
//		
//		Debug.Log(a.gameObject.name + "touch " + gameObject.name + " with force = " + force + " direction=" + direction);
//		
//		if(completionCallback != null)
//		{
//			ArrayList pp = new ArrayList();
//			pp.Add(force);
//
//			ActionResult ar = new ActionResult(_touchAction, ActionResult.Status.SUCCESS, a, pp, a.gameObject.name + " touched " + gameObject.name);
//			completionCallback(ar);
//		}
//	}
//			
//	//---------------------------------------------------------------------------
//
//	#endregion
//
//	//---------------------------------------------------------------------------
//
//	#region Other Members
//
//	//---------------------------------------------------------------------------		
//
//	/// <summary>
//	/// Initializes a new instance of the <see cref="OpenCog.SocialInteraction"/> class.  
//	/// Generally, intitialization should occur in the Start or Awake
//	/// functions, not here.
//	/// </summary>
//	public OCSocialInteraction()
//	{
//	}
//
//	//---------------------------------------------------------------------------
//
//	#endregion
//
//	//---------------------------------------------------------------------------

}// class SocialInteraction

}// namespace OpenCog




