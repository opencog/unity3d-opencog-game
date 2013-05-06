
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
public class PhysiologicalEffect : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	
	private CostLevel _costLevel;

	private float _energyIncrease = 0.0f;

	private float _fitnessChange = 0.0f;

	private OCPhysiologicalModel.AvatarMode _newAvatarMode = OCPhysiologicalModel.AvatarMode.ACTIVE;

	private Dictionary<string, float> _changeFactors = new Dictionary<string,float>();

	private List<string> _resetFactors = new List<string>();

	private float BASE_ENERGY_COST;

	private Utility.Config config = Utility.Config.GetInstance();

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------
		

			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	public void ApplyEffect(OCPhysiologicalModel model)
	{
		// Update energy
		model.energy -= getActionCost((float)model.fitness);
		model.energy += energyIncrease;
        
		model.fitness += fitnessChange;
        
		// Set new mode
		model.currentMode = newMode;
		// Change factors...
		foreach(String factorName in changeFactors.Keys)
		{
			float changeValue = changeFactors[factorName];
			if(changeValue < 0.0f)
			{
				model.basicFactorMap[factorName].decrease(-changeValue);
			}
			else
			{
				model.basicFactorMap[factorName].increase(changeValue);
			}
		}
		// Reset factors
		foreach(String factorName in resetFactors)
		{
			model.basicFactorMap[factorName].reset();
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
	public float GetActionCost(float fitness)
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

	public PhysiologicalEffect(CostLevel level)
	{
		_costLevel = level;
		// MAX_ACTION_NUM is the number of normal actions possible on a full battery charge.
		this.BASE_ENERGY_COST = 1.0f / config.GetInt("MAX_ACTION_NUM");
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class PhysiologicalEffect

}// namespace OpenCog




