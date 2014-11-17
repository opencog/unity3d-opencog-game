
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

	#region Class Attributes

	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	[OCExposePropertyFields]
	[Serializable]
		
	#endregion

	/// <summary>
	/// The OpenCog OCPhysiologicalModel.
	/// </summary>
	public class OCPhysiologicalModel : OCMonoBehaviour
	{
		//---------------------------------------------------------------------------
		#region Constants For Model
		//---------------------------------------------------------------------------

		private long MILLISECONDS_PER_DAY = 24 * 60 * 60 * 1000;

		// Actually, the following parameters should be decided by the amount of eating and drinking.
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

		//---------------------------------------------------------------------------
		#endregion
		#region Private Member Data
		//---------------------------------------------------------------------------
		private double _energy;

		private int[] _modeCounts;

		// Currently equivalent to the "integrity" level.
		private double _fitness; 
			
		//Create a system parameters instance.
		private OCConfig _config;


		//Update the physiological model every 0.5 second.
		private readonly float _updateInterval = 0.5f;

	     //The timer accumulates during the update, after the value of timer exceeding the interval, some actions would be triggered.
		private float _updateTimer = 0.0f;

		private float _millisecondsPerTick;
		//	private bool at_home_flag; 
	    
		//---------------------------------------------------------------------------
		#endregion
	    #region Private Variables
		//---------------------------------------------------------------------------

	   	//Map of basic physiological factors: hunger, thirst, pee urgency, poo urgency etc.
		private Dictionary<string, BasicPhysiologicalFactor> _basicFactorMap = new Dictionary<string,BasicPhysiologicalFactor>();

		// A quick way to traverse the dictionary "basicFactorMap" by recording all its keys, C# does not allow 
		// modifying the value when traversing a dictionary.
		private List<string> _basicFactorList = null;

	    //Summary for the value of all physiological factors, including hunger, thirst, pee urgency, poo urgency, 
	   	//energy, fitness. The summary is used to send to OAC in a tick message.
		private Dictionary<string, double> _factorSummaryMap = new Dictionary<string, double>();
				
		private OCConnectorSingleton _connector;

		private AvatarMode _currentMode;


		//---------------------------------------------------------------------------
		#endregion
		#region Accessors and Mutators
		//---------------------------------------------------------------------------

		public AvatarMode CurrentMode
		{
			get { return _currentMode;}
			set { _currentMode = value;}
		}

		public double Fitness
		{
			get { return _fitness; }
			set { _fitness = value; }
		}

		public Dictionary<string, BasicPhysiologicalFactor> BasicFactorMap
		{
			get { return _basicFactorMap;}
			set { _basicFactorMap = value;}
		}

		public double Energy
		{
			get { return _energy; }
			set { _energy = value; }
		}
				
		//---------------------------------------------------------------------------
		#endregion
		#region Public Member Functions
		//---------------------------------------------------------------------------

		void Awake()
		{
			_config = OCConfig.Instance;

			// Initialize parameters below.
			_modeCounts = new int[3];
			_modeCounts[(int)AvatarMode.IDLE] = 0;
			_modeCounts[(int)AvatarMode.SLEEP] = 0;
			_modeCounts[(int)AvatarMode.ACTIVE] = 0;

			_millisecondsPerTick = _config.getLong("MILLISECONDS_PER_TICK");
			
			this.IDLE_ENERGY_DECREASE_RATE = - _millisecondsPerTick / (MILLISECONDS_PER_DAY / _config.getInt("EAT_STOPS_PER_DAY"));
			this.SLEEP_ENERGY_INCREASE_RATE = - IDLE_ENERGY_DECREASE_RATE * 5;
			this.STARVING_ENERGY_DECREASE_RATE = IDLE_ENERGY_DECREASE_RATE * 2;
			this.FITNESS_DECREASE_OUTSIDE_HOME = _config.getFloat("FITNESS_DECREASE_OUTSIDE_HOME");
			this.EAT_ENERGY_INCREASE = _config.getFloat("EAT_ENERGY_INCREASE");
			this.EAT_POO_INCREASE = _config.getFloat("EAT_POO_INCREASE");
			this.DRINK_THIRST_DECREASE = _config.getFloat("EAT_THIRST_DECREASE");
			this.DRINK_PEE_INCREASE = _config.getFloat("DRINK_PEE_INCREASE");

			_energy = _config.getFloat("INIT_ENERGY");
			_fitness = _config.getFloat("INIT_FITNESS");
			_currentMode = AvatarMode.IDLE;

			//		this.AT_HOME_DISTANCE = config.getFloat("AT_HOME_DISTANCE");
			//		this.FITNESS_INCREASE_AT_HOME = config.getFloat("FITNESS_INCREASE_AT_HOME");
			//		this.at_home_flag = false;

			SetupBasicFactors();

			_connector = OCConnectorSingleton.Instance;
		}

		void Update()
		{
			//UnityEngine.Debug.Log ("OCPhysiologicalModel::Update!");
		}

		void FixedUpdate()
		{
			//UnityEngine.Debug.Log ("OCPhysiologicalModel::FixedUpdate! UpdateTimer = " + _updateTimer.ToString () + ", UpdateInterval = " + _updateInterval.ToString());
			_updateTimer += UnityEngine.Time.fixedDeltaTime;
			if(_updateTimer >= _updateInterval)
			{
				//	CheckAtHomeStatus();
				
				// Some actions here
				TimeTick();

				// Reset the timer.
				_updateTimer = 0.0f;
			}
		}

		public void SetupBasicFactors()
		{
			_basicFactorMap["hunger"] = new BasicPhysiologicalFactor("hunger", 0.0, _config.getInt("EAT_STOPS_PER_DAY"), _millisecondsPerTick);
			_basicFactorMap["thirst"] = new BasicPhysiologicalFactor("thirst", 0.0, _config.getInt("DRINK_STOPS_PER_DAY"), _millisecondsPerTick);
			_basicFactorMap["pee_urgency"] = new BasicPhysiologicalFactor("pee_urgency", 0.0, _config.getInt("PEE_STOPS_PER_DAY"), _millisecondsPerTick);
			_basicFactorMap["poo_urgency"] = new BasicPhysiologicalFactor("poo_urgency", 0.0, _config.getInt("POO_STOPS_PER_DAY"), _millisecondsPerTick);
			
			//_basicFactorMap["energy"] = new BasicPhysiologicalFactor("energy", 0.0, _config.getInt("ENERGY_STOPS_PER_DAY"), _millisecondsPerTick);
			//_basicFactorMap["fitness"] = new BasicPhysiologicalFactor("fitness", 0.0, _config.getInt("FITNESS_STOPS_PER_DAY"), _millisecondsPerTick);

			_basicFactorList = new List<string>();
			_basicFactorList.AddRange(_basicFactorMap.Keys);
		}

		public void TimeTick()
		{
			//UnityEngine.Debug.Log ("OCPhysiologicalModel::TimeTick!");
			UpdateBasicFactors();
			UpdateFitness();
			UpdateEnergy();
			
			_modeCounts[(int)_currentMode]++;

			if(_currentMode != AvatarMode.SLEEP)
			{
				_currentMode = AvatarMode.IDLE;
			}

			foreach(string factor in _basicFactorMap.Keys)
			{
				_factorSummaryMap[factor] = _basicFactorMap[factor].value;
			}
			_factorSummaryMap["energy"] = _energy;
			_factorSummaryMap["fitness"] = _fitness;
			
			if(_connector != null)
			{
				//UnityEngine.Debug.Log ("OCPhysiologicalModel::TimeTick: _connector != null, yaay!!");
				// Send updated values to OAC
				//			_connector.SendMessage("sendAvatarSignalsAndTick", _factorSummaryMap);
				_connector.SendAvatarSignalsAndTick(_factorSummaryMap);
				

				// Also update values holding by OCConnector, which would be displayed 
				// in psi panel in unity
				_connector.SetDemandValue("Energy", (float)_energy);
				_connector.SetDemandValue("Integrity", (float)_fitness);
			}	
		}

		/**
	     * This method should be invoked to update the physiological model, 
	     * once an action has been done.
	     * Currently, it would be invoked by ActionManager, by using 
	     *      SendMessage("processPhysiologicalEffect", phyEffect)
	     */
		public void ProcessPhysiologicalEffect(OCPhysiologicalEffect effect)
		{
			effect.ApplyEffect(this);
			// Ensure energy is in acceptable bounds
			_energy = OpenCog.Utility.NumberUtil.zeroOneCut(_energy);

		}

		//---------------------------------------------------------------------------
		#endregion
		#region Private Member Functions
		//---------------------------------------------------------------------------
		
		private void UpdateBasicFactors()
		{
			foreach(string key in _basicFactorList)
			{
				_basicFactorMap[key].UpdateValue();
			}
		}

		private void UpdateEnergy()
		{
			if(_currentMode == AvatarMode.IDLE)
			{
				_energy += IDLE_ENERGY_DECREASE_RATE;
			}
			else
			if(_currentMode == AvatarMode.SLEEP)
			{
				_energy += SLEEP_ENERGY_INCREASE_RATE;
			}

			if(_basicFactorMap["hunger"].value > 0.9)
			{
				_energy += STARVING_ENERGY_DECREASE_RATE;
			}

			if(_basicFactorMap["thirst"].value > 0.9)
			{
				_energy += STARVING_ENERGY_DECREASE_RATE;
			}
			_energy = OpenCog.Utility.NumberUtil.zeroOneCut(_energy);
		}

	
	    //Fitness is something related to basic physiological factors.
		private void UpdateFitness()
		{
			// We should update this sometime to reflect the impact of not enough energy on integrity
			/**
	        this._fitness = 1.0 - (0.4 * basicFactorMap["hunger"].value +
	                            0.3 * basicFactorMap["thirst"].value +
	                            0.2 * basicFactorMap["poo_urgency"].value +
	                            0.1 * basicFactorMap["pee_urgency"].value);
	         */
	 		_fitness = _fitness - FITNESS_DECREASE_OUTSIDE_HOME;
		}
				
		//---------------------------------------------------------------------------
		#endregion
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

	     	// Update the value of physiological factor.
			public void UpdateValue()
			{
				// (MILLISECONDS_PER_DAY / frequency) means how long the urgency will increase to 1
				double delta = this.millisecondsPerTick / (MILLISECONDS_PER_DAY / frequency);
				this.value = NumberUtil.zeroOneCut(this.value + delta);
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




