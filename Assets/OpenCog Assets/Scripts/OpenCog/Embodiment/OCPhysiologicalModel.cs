
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
using OpenCog.Utility;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Embodiment
{

/// <summary>
/// The OpenCog OCPhysiologicalModel.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCPhysiologicalModel : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	/**
     * Constants the model needs.
     */
	private long MILLISECONDS_PER_DAY = 24 * 60 * 60 * 1000;

	// Actually, the following parameters should be decided by the amount
    // of eating and drinking.
	private double EAT_ENERGY_INCREASE;
	private double EAT_POO_INCREASE;
	private double DRINK_THIRST_DECREASE;
	private double DRINK_PEE_INCREASE;
	private double FITNESS_DECREASE_OUTSIDE_HOME;
	private double IDLE_ENERGY_DECREASE_RATE;
	private double SLEEP_ENERGY_INCREASE_RATE;
	private double STARVING_ENERGY_DECREASE_RATE;

	//	private double AT_HOME_DISTANCE;
	//	private double FITNESS_INCREASE_AT_HOME;
	private double m_energy;
	private int[] m_modeCounts;
	private double _m_fitness; // Currently equivalent to the "integrity" level.

	/**
	 * Create a system parameters instance.
	 */
	private Config m_config = Config.GetInstance();

	/**
     * Update the physiological model every 0.5 second.
     */
	private readonly float m_updateInterval = 0.5f;
	/**
     * The timer accumulates during the update, after the value of timer exceeding
     * the interval, some actions would be triggered.
     */
	private float m_updateTimer = 0.0f;

	private float m_millisecondsPerTick;

//	private bool at_home_flag; 
    #endregion
	
    #region Private Variables
	// Private variables:


	/**
     * Map of basic physiological factors: hunger, thirst, pee urgency, poo urgency etc.
     */
	private Dictionary<string, BasicPhysiologicalFactor> m_basicFactorMap = new Dictionary<string,BasicPhysiologicalFactor>();
	// A quick way to traverse the dictionary "basicFactorMap" by recording all its keys, C# does not allow 
	// modifying the value when traversing a dictionary.
	private List<string> m_basicFactorList = null;
	/**
     * Summary for the value of all physiological factors, including hunger, thirst, pee urgency, poo urgency,
     * energy, fitness. The summary is used to send to OAC in a tick message.
     */
	private Dictionary<string, double> m_factorSummaryMap = new Dictionary<string, double>();
			
//	private OCConnector m_connector;

	private AvatarMode m_currentMode;


	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------

	public AvatarMode CurrentMode
	{
		get { return m_currentMode;}
		set { m_currentMode = value;}
	}

	public double Fitness
	{
		get { return m_fitness; }
		set { m_fitness = value; }
	}

	public Dictionary<string, BasicPhysiologicalFactor> BasicFactorMap
		{
			get {return m_basicFactorMap;}
			set {m_basicFactorMap = value;}
		}

			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	void Awake()
	{
		// Initialize parameters below.
		m_modeCounts = new int[3];
		m_modeCounts[(int)AvatarMode.IDLE] = 0;
		m_modeCounts[(int)AvatarMode.SLEEP] = 0;
		m_modeCounts[(int)AvatarMode.ACTIVE] = 0;

		m_millisecondsPerTick = m_config.getLong("MILLISECONDS_PER_TICK");
		
		this.IDLE_ENERGY_DECREASE_RATE = - m_millisecondsPerTick / (MILLISECONDS_PER_DAY / m_config.getInt("EAT_STOPS_PER_DAY"));
		this.SLEEP_ENERGY_INCREASE_RATE = - IDLE_ENERGY_DECREASE_RATE * 5;
		this.STARVING_ENERGY_DECREASE_RATE = IDLE_ENERGY_DECREASE_RATE * 2;
		this.FITNESS_DECREASE_OUTSIDE_HOME = m_config.getFloat("FITNESS_DECREASE_OUTSIDE_HOME");
		this.EAT_ENERGY_INCREASE = m_config.getFloat("EAT_ENERGY_INCREASE");
		this.EAT_POO_INCREASE = m_config.getFloat("EAT_POO_INCREASE");
		this.DRINK_THIRST_DECREASE = m_config.getFloat("EAT_THIRST_DECREASE");
		this.DRINK_PEE_INCREASE = m_config.getFloat("DRINK_PEE_INCREASE");

		m_energy = m_config.getFloat("INIT_ENERGY");
		m_fitness = m_config.getFloat("INIT_FITNESS");
		m_currentMode = AvatarMode.IDLE;

		//		this.AT_HOME_DISTANCE = config.getFloat("AT_HOME_DISTANCE");
		//		this.FITNESS_INCREASE_AT_HOME = config.getFloat("FITNESS_INCREASE_AT_HOME");
		//		this.at_home_flag = false;

		SetupBasicFactors();

		m_connector = gameObject.GetComponent<OCConnector>() as OCConnector;
	}

	void Update()
	{

	}

	void FixedUpdate()
	{
		m_updateTimer += UnityEngine.Time.fixedDeltaTime;
		if(m_updateTimer >= m_updateInterval)
		{
//			CheckAtHomeStatus();
			
			// Some actions here
			TimeTick();

			// Reset the timer.
			m_updateTimer = 0.0f;
		}
	}

	public void SetupBasicFactors()
	{
		m_basicFactorMap["hunger"] = new BasicPhysiologicalFactor("hunger", 0.0, m_config.getInt("EAT_STOPS_PER_DAY"), millisecondsPerTick);
		m_basicFactorMap["thirst"] = new BasicPhysiologicalFactor("thirst", 0.0, m_config.getInt("DRINK_STOPS_PER_DAY"), millisecondsPerTick);
		m_basicFactorMap["pee_urgency"] = new BasicPhysiologicalFactor("pee_urgency", 0.0, m_config.getInt("PEE_STOPS_PER_DAY"), millisecondsPerTick);
		m_basicFactorMap["poo_urgency"] = new BasicPhysiologicalFactor("poo_urgency", 0.0, m_config.getInt("POO_STOPS_PER_DAY"), millisecondsPerTick);

		m_basicFactorList = new List<string>();
		m_basicFactorList.AddRange(m_basicFactorMap.Keys);
	}

	public void TimeTick()
	{
		UpdateBasicFactors();
		UpdateFitness();
		UpdateEnergy();
		
		m_modeCounts[(int)m_currentMode]++;

		if(m_currentMode != AvatarMode.SLEEP)
		{
			m_currentMode = AvatarMode.IDLE;
		}

		foreach(string factor in m_basicFactorMap.Keys)
		{
			m_factorSummaryMap[factor] = m_basicFactorMap[factor].value;
		}
		m_factorSummaryMap["energy"] = m_energy;
		m_factorSummaryMap["fitness"] = m_fitness;
		
		if(m_connector != null)
		{
			// Send updated values to OAC
			m_connector.SendMessage("sendAvatarSignalsAndTick", m_factorSummaryMap);

			// Also update values holding by OCConnector, which would be displayed 
			// in psi panel in unity
			m_connector.SetDemandValue("Energy", (float)m_energy);
			m_connector.SetDemandValue("Integrity", (float)m_fitness);
		}	
	}

	/**
     * This method should be invoked to update the physiological model, 
     * once an action has been done.
     * Currently, it would be invoked by ActionManager, by using 
     *      SendMessage("processPhysiologicalEffect", phyEffect)
     */
	public void ProcessPhysiologicalEffect(PhysiologicalEffect effect)
	{
		effect.applyEffect(this);
		// Ensure energy is in acceptable bounds
		m_energy = OpenCog.Utility.NumberUtil.zeroOneCut(m_energy);

	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	private void UpdateBasicFactors()
	{
		foreach(string key in this.basicFactorList)
		{
			basicFactorMap[key].updateValue();
		}
	}

	private void UpdateEnergy()
	{
		if(m_currentMode == AvatarMode.IDLE)
		{
			m_energy += IDLE_ENERGY_DECREASE_RATE;
		}
		else
		if(m_currentMode == AvatarMode.SLEEP)
		{
			m_energy += SLEEP_ENERGY_INCREASE_RATE;
		}

		if(m_basicFactorMap["hunger"].value > 0.9)
		{
			m_energy += STARVING_ENERGY_DECREASE_RATE;
		}

		if(m_basicFactorMap["thirst"].value > 0.9)
		{
			m_energy += STARVING_ENERGY_DECREASE_RATE;
		}
		this.m_energy = OpenCog.Utility.NumberUtil.zeroOneCut(this.energy);
	}

	/**
     * Fitness is something related to basic physiological factors.
     */
	private void UpdateFitness()
	{
		// We should update this sometime to reflect the impact of not enough energy on integrity
		/**
        this._fitness = 1.0 - (0.4 * basicFactorMap["hunger"].value +
                            0.3 * basicFactorMap["thirst"].value +
                            0.2 * basicFactorMap["poo_urgency"].value +
                            0.1 * basicFactorMap["pee_urgency"].value);
         */
 		
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------

	public enum AvatarMode
	{
		SLEEP,
		IDLE,
		ACTIVE
	}

	public struct BasicPhysiologicalFactor
	{
		public string name;
		public double value;
		public float frequency;
		private float millisecondsPerTick;
		private static long MILLISECONDS_PER_DAY = 24 * 60 * 60 * 1000;

		public BasicPhysiologicalFactor(string name, double value, int frequency, float millisecondsPerTick)
		{
			this.name = name;
			this.value = value;
			this.frequency = frequency;
			this.millisecondsPerTick = millisecondsPerTick;
		}

		/**
     * Update the value of physiological factor.
     */
		public void UpdateValue()
		{
			// (MILLISECONDS_PER_DAY / frequency) means how long the urgency will increase to 1
			double delta = this.millisecondsPerTick / (MILLISECONDS_PER_DAY / frequency);
			this.value = NumberUtil.zeroOneCut(this.value + delta);
			//Debug.Log("Physiological: " + this.name + " " + this.value);
		}

		public void Increase(double delta)
		{
			this.value = NumberUtil.zeroOneCut(this.value + delta);
		}

		public void Decrease(double delta)
		{
			this.value = NumberUtil.zeroOneCut(this.value - delta);
		}

		public void Reset()
		{
			this.value = 0.0;
		}
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCPhysiologicalModel

}// namespace OpenCog




