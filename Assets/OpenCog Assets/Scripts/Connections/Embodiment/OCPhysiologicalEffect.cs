
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
using UnityEngine;

#region Usings, Namespaces, and Pragmas

using System.Collections;
using System.Collections.Generic;
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
/// The OpenCog PhysiologicalEffect.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCPhysiologicalEffect : OCScriptableObject
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	
	private CostLevel _costLevel;

	private float _energyIncrease;

	private float _fitnessChange;

	private OCPhysiologicalModel.AvatarMode _newAvatarMode;
		
	private Dictionary<string, float> _changeFactors;

	private List<string> _resetFactors;

	private float BASE_ENERGY_COST;

	private OCConfig config;

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------
		
	public float EnergyIncrease 
	{
		get { return _energyIncrease; }
		set { _energyIncrease = value; }
	}
		
	public float FitnessChange
	{
		get { return _fitnessChange; }
		set { _fitnessChange = value; }
	}

	public CostLevel CostLevelProp 
	{
		get { return _costLevel; }
		set { _costLevel = value; }
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	public void OnEnable()
	{
			_energyIncrease = 0.0f;
			_fitnessChange = 0.0f;
			_newAvatarMode = OCPhysiologicalModel.AvatarMode.ACTIVE;
			_changeFactors = new Dictionary<string,float>();
			_changeFactors["hunger"] = 0.5f;
			_changeFactors["thirst"] = 0.5f;
			//_changeFactors["energy"] = 0.5f;
			//_changeFactors["fitness"] = 0.5f;
			_changeFactors["pee_urgency"]	= 0.5f;
			_changeFactors["poo_urgency"]	= 0.5f;
			_resetFactors = new List<string>();
			config = OCConfig.Instance;
	}

	public void ApplyEffect(OCPhysiologicalModel model)
	{
		// Update energy
		model.Energy -= getActionCost((float)model.Fitness);
		model.Energy += _energyIncrease;
        
		model.Fitness += _fitnessChange;
			
		model.Fitness = OpenCog.Utility.NumberUtil.zeroOneCut(model.Fitness);
		model.Energy = OpenCog.Utility.NumberUtil.zeroOneCut(model.Energy);
        
		// Set new mode
		model.CurrentMode = _newAvatarMode;
		// Change factors...
		foreach(string factorName in _changeFactors.Keys)
		{
			float changeValue = _changeFactors[factorName];
			if(changeValue < 0.0f)
			{
				model.BasicFactorMap[factorName].Decrease(-changeValue);
			}
			else
			{
				model.BasicFactorMap[factorName].Increase(changeValue);
			}
		}
		// Reset factors
		foreach(string factorName in _resetFactors)
		{
			model.BasicFactorMap[factorName].Reset();
		}

		// Deal with the action which has effects on the physiological factors.
        
		// For reference, these are the old physiological effect of actions.
		/*switch (effect.actionName)
        {
            case "sleep":
                this.currentMode = AvatarMode.SLEEP;
                break;
            case "eat":
                // increase the energy
                this.energy += EAT_ENERGY_INCREASE;
                // decrease the hunger
                basicFactorMap["hunger"].decrease(this.energy);
                // increase the poo urgency
                basicFactorMap["poo_urgency"].increase(EAT_POO_INCREASE);
                break;
            case "drink":
                // decrease the thirst
                basicFactorMap["thirst"].decrease(DRINK_THIRST_DECREASE);
                // increase the pee urgency
                basicFactorMap["pee_urgency"].increase(DRINK_PEE_INCREASE);
                break;
            case "pee":
                // reset the pee urgency
                basicFactorMap["pee_urgency"].reset();
                break;
            case "poo":
                // reset the poo urgency
                basicFactorMap["poo_urgency"].reset();
                break;
            default:
                break;
        }*/
        
        
	}
    
	/**
     * Calculate the actual energy cost according to BASE_ENERGY_COST, fitness and level.
     * Higher energy cost is produced with lower fitness, higher BASE_ENERGY_COST and level.
     */
	public float getActionCost(float fitness)
	{
		return (float)(1.5 - fitness) * ((int)_costLevel * BASE_ENERGY_COST);
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------		

	public enum CostLevel
	{
		NONE = 0,
		LOW = 1,
		MEDIUM = 2,
		HIGH = 3,
		STRONG = 4
	};

	public OCPhysiologicalEffect(CostLevel level)
	{
		_costLevel = level;
		// MAX_ACTION_NUM is the number of normal actions possible on a full battery charge.
		this.BASE_ENERGY_COST = 1.0f / config.getInt("MAX_ACTION_NUM");
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class PhysiologicalEffect

}// namespace OpenCog




