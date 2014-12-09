
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
using System.IO;
using System.Net;
using OpenCog.Attributes;
using OpenCog.Extensions;
using OpenCog.Utility;
using ProtoBuf;
using UnityEngine;
using OpenCog;

//namespace OpenCog
//{

/// <summary>
/// The OpenCog OCConfig.
/// </summary>
using OpenCog.Utilities.Logging;


#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
#endregion
public sealed class OCConfig : OCSingletonScriptableObject< OCConfig >
{
	#region Private Member Data

	private static Hashtable _settings = new Hashtable();
	
	#endregion
	#region Accessors and Mutators
	//---------------------------------------------------------------------------

	public static OCConfig Instance{get {return GetInstance<OCConfig>();}}
			
	//---------------------------------------------------------------------------
	#endregion
	#region Public Member Functions
	//---------------------------------------------------------------------------
		
	public OCConfig()
	{
		Defaults();
	}

	/// <summary>
	/// Raises the enable event for the OCConfig.
	/// </summary>
	public void Defaults()
	{
		_settings["GENERATE_TICK_MESSAGE"] = "true";

		// Proxy config
		_settings["MY_ID"] = "PROXY";
		_settings["MY_IP"] = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
		_settings["MY_PORT"] = "16315";
		
		// Router config
		_settings["ROUTER_ID"] = "ROUTER";
		_settings["ROUTER_IP"] = "192.168.1.48";
		_settings["ROUTER_PORT"] = "16312";
		
		// Spawner config
		_settings["SPAWNER_ID"] = "SPAWNER";
		
		// Unread messages management
		_settings["UNREAD_MESSAGES_CHECK_INTERVAL"] = "10";
		_settings["UNREAD_MESSAGES_RETRIEVAL_LIMIT"] = "1";
		_settings["NO_ACK_MESSAGES"] = "true";
		
		// Time for sleeping in server loop
		_settings["SERVER_LOOP_SLEEP_TIME"] = "100";
		
		// Time interval for sending perception (map-info, physiological) messages in milliseconds
		_settings["MESSAGE_SENDING_INTERVAL"] = "100";
		// Interval between time ticks (in milliseconds)
		_settings["TICK_INTERVAL"] = "500";
		
		// Number of simulated milliseconds per tick 
		// (Default value == TICK_INTERVAL) => (simulated time == real time)
		// For accelerating the simulated time (useful for automated tests), 
		// this value must/may be increased. 
		_settings["MILLISECONDS_PER_TICK"] = "500";
		
		// Parameters for controlling Physiological feelings:
		// 480 = 60/3 * 24 means the eat action is expected to happen once per 3 minute 
		// when the virtual pet does nothing else
		_settings["EAT_STOPS_PER_DAY"] = "480";
		_settings["DRINK_STOPS_PER_DAY"] = "480";
		_settings["PEE_STOPS_PER_DAY"] = "480";
		_settings["POO_STOPS_PER_DAY"] = "480";
		_settings["MAX_ACTION_NUM"] = "50";  // Maximum number of actions the avatar can do without eating
		_settings["EAT_ENERGY_INCREASE"] = "0.55";
		//		_settings["AT_HOME_DISTANCE"] = "3.8";  // Avatar is at home, if the distance between avatar and home is smalled than this value
		//		_settings["FITNESS_INCREASE_AT_HOME"] = "0.008333"; // Equals 1/(60/0.5), need 60 seconds at most to increase to 1
		_settings["FITNESS_DECREASE_OUTSIDE_HOME"] = "0.005556";  // Equals 1/(60*1.5/0.5), need 1.5 minutes at most to decrease to 0
		_settings["POO_INCREASE"] = "0.05";
		_settings["DRINK_THIRST_DECREASE"] = "0.15";
		_settings["DRINK_PEE_INCREASE"] = "0.05";	
		_settings["INIT_ENERGY"] = "1.0"; 
		_settings["INIT_FITNESS"] = "0.80"; 
		
		// Interval between messages sending (in milliseconds)
		_settings["MESSAGE_SENDING_INTERVAL"] = "100";
		
		// Map min/max position
		_settings["GLOBAL_POSITION_X"] = "500";	//"-165000";
		_settings["GLOBAL_POSITION_Y"] = "500";	//"-270000";
		_settings["AVATAR_VISION_RADIUS"] = "200";	//"64000";
		
		// Golden standard generation
		_settings["GENERATE_GOLD_STANDARD"] = "true";
		// filename where golden standard message will be recorded 
		_settings["GOLD_STANDARD_FILENAME"] = "GoldStandards.txt";
		
		_settings["AVATAR_STORAGE_URL"] = ""; // Default is Jack's appearance
		
		// There are six levels: NONE, ERROR, WARN, INFO, DEBUG, FINE.
		_settings["LOG_LEVEL"] = "DEBUG";
		
		// OpenCog properties persistence data file
		_settings["OCPROPERTY_DATA_FILE"] = ".\\oc_properties.dat";			
		
		// Testing and Buildbot Integration
		//_settings["TEST_AND_EXIT"] = "false";  //Use #define TEST_AND_EXIT instead...
		_settings["test"] = "";// will be "internal_XGA" in case of buildbot
		_settings["quit"] = "false";// should be "true" in case of buildbot 

		//run all tests
		_settings["UNITTEST_ALL"] = "false";

		//run each test
		_settings["UNITTEST_EMBODIMENT"] = "false";
		_settings["UNITTEST_BATTERY"] = "false";
		_settings["UNITTEST_PLAN"] = "false";
		_settings["UNITTEST_SECONDPLAN"] = "false";

		//parse as a string. is it 'neutral?' if not, parse as a bool XD
		_settings["UNITTEST_EXIT"] = "neutral";
	}
		
	/// <summary>
	/// Load passed file and redefines values for parameters.
	/// Parameters which are not mentioned in the file will keep their default value.
	/// Parameters which do not have default values are discarded.
	/// </summary>
	/// <param name='fileName'>
	/// The config file's name.
	/// </param>
	public void LoadFromFile(string fileName)
	{
		StreamReader reader = new StreamReader(fileName);
    char[] separator = {'=',' '};
      int linenumber = 0;
    
		while (!reader.EndOfStream)
		{
      string line = reader.ReadLine();
        linenumber++;
      
      // not a commentary or an empty line
      if(line.Length > 0 && line[0] != '#'){
          string[] tokens = line.Split(separator,StringSplitOptions.RemoveEmptyEntries);
          if (tokens.Length < 2)
          {
              if (Debug.isDebugBuild)
                    Debug.LogError("Invalid format at line " + linenumber +": '" + line + "'");
          }
          if (_settings.ContainsKey(tokens[0])) 
          {
              if (Debug.isDebugBuild) Debug.Log(tokens[0] + "=" + tokens[1]);
              _settings[tokens[0]] = tokens[1];
          }
          else
          {
                Debug.LogWarning("Ignoring unknown parameter name '" + tokens[0] + "' at line "
                        + linenumber + ".");
      	}           
      }
		}
			
		reader.Close();  
	}

	public void LoadFromTextAsset (TextAsset configFile)
	{
		string[] lines = configFile.text.Split(new string[]{ "\r\n", "\n"}, StringSplitOptions.None);
		int linenumber = 0;
		char[] separator = {'=',' '};
      
		foreach(string line in lines)
		{
			linenumber++;
			
	    // not a commentary or an empty line
	    if(line.Length > 0 && line[0] != '#'){
	        string[] tokens = line.Split(separator,StringSplitOptions.RemoveEmptyEntries);
	        if (tokens.Length < 2)
	        {
	            if (Debug.isDebugBuild)
	                  Debug.LogError("Invalid format at line " + linenumber +": '" + line + "'");
	        }
	        if (_settings.ContainsKey(tokens[0])) 
	        {
	            //if (Debug.isDebugBuild) Debug.Log(tokens[0] + "=" + tokens[1]);
	            _settings[tokens[0]] = tokens[1];
	        }
	        else
	        {
	              Debug.LogWarning("Ignoring unknown parameter name '" + tokens[0] + "' at line "
	                      + linenumber + ".");
	    	}           
	    }
		}
	}

	/// <summary>
	/// Handles configuration by reading in the arguments we recieved from the command line. 
	/// </summary>
	public void LoadFromCommandLine()
	{
		string[] args = System.Environment.GetCommandLineArgs();
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +"GetCommandLineArgs: {0}" +  String.Join(", ", args));
		
		foreach(string arg in args)
		{
			if(arg == args[0])
				continue;
				
			string[] keyValuePair = arg.Split(':');
			if(keyValuePair != null && _settings.ContainsKey(keyValuePair[0]))
			{
				if(keyValuePair[0] != "quit")
				{
              		//if (Debug.isDebugBuild) Debug.Log(tokens[0] + "=" + tokens[1]);
              		_settings[keyValuePair[0]] = keyValuePair[1];
				}
				else
					_settings["quit"] = "true";
					
			}
			else
			{
				Debug.LogWarning("Ignoring unknown command-line argument '" + arg + "'.");
			}
		}
	}
		
	/// <summary>
	/// Return current value of a given parameter.
	/// </summary>
	/// <param name='paramName'>
	/// Parameter name.
	/// </param>
	/// <param name='DEFAULT'>
	/// DEFAUL.
	/// </param>
	public string get(string paramName, string DEFAULT="")
	{
	    if (_settings.ContainsKey(paramName)) {
	        return (string) _settings[paramName];
	    } else {
	        return DEFAULT;
	    }
	}
	
	public long getLong(string paramName, long DEFAULT=0)
	{
	    if (_settings.ContainsKey(paramName)) {
	        return long.Parse((string)_settings[paramName]);
	    } else {
	        return DEFAULT;
	    }
	}
	
	public int getInt(string paramName, int DEFAULT=0)
	{
	    if (_settings.ContainsKey(paramName)) {
	        return int.Parse((string)_settings[paramName]);
	    } else {
	        return DEFAULT;
	    }
	}
	
	public float getFloat(string paramName, long DEFAULT=0)
	{
	    if (_settings.ContainsKey(paramName)) {
	        return float.Parse((string)_settings[paramName]);
	    } else {
	        return DEFAULT;
	    }
	}	

	public bool getBool(string paramName, bool DEFAULT=false)
	{
		if (_settings.ContainsKey(paramName)) {
			return bool.Parse((string)_settings[paramName]);
		} else {
			return DEFAULT;
		}
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
		
		
		
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCConfig

//}// namespace OpenCog




