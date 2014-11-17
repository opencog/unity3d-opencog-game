
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

#region Namespaces

using System.Collections;
using System.Diagnostics;
using System.Reflection;
using OpenCog.Attributes;
using OpenCog.Extensions;
using Debug = UnityEngine.Debug;
using Enum = System.Enum;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;
using ArgumentException = System.ArgumentException;
using OpenCog;

#endregion

namespace OpenCog.Utilities.Logging
{

	/// <summary>
	/// The OpenCog Logger.  Allows for logging at various levels of detail,
	/// based on the configuration level.
	/// </summary>
	#region Class Attributes


	#endregion
	public class OCLogger :OCSingletonScriptableObject< OCLogger >
	{

		//---------------------------------------------------------------------------

		#region Private Member Data

		//---------------------------------------------------------------------------
			
		/// <summary>
		/// The current log level.  If a message is logged with a level less than the
		/// current log level, it will be displayed in the Unity Console output 
		/// window.
		/// </summary>
		//private LogLevel _currentLevel;
		
		/// <summary>
		/// Is the logger enabled?
		/// </summary>
		private bool _isLogEnabled;

		public bool dateEntries = false;
		public bool printFine = false;
		public bool printDebug = true;
		
		

		//---------------------------------------------------------------------------

		#endregion

		//---------------------------------------------------------------------------

		#region Accessors and Mutators

		//---------------------------------------------------------------------------
			
		/// <summary>
		/// Gets or sets the current log level.  If a message is logged with a level 
		/// less than the current log level, it will be displayed in the Unity 
		/// Console output window.
		/// </summary>
		/// <value>
		/// The current log level.
		/// </value>
		/*public static LogLevel CurrentLevel
		{
			get{ return Instance._currentLevel;}
			set{ Instance._currentLevel = value;}
		}*/
		
		/// <summary>
		/// Gets or sets a value indicating whether the logger is enabled.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is log enabled; otherwise, <c>false</c>.
		/// </value>
		public static bool IsLogEnabled
		{
			get{ return Instance._isLogEnabled;}
			set{ Instance._isLogEnabled = value;}
		}	
		
		
		public static OCLogger Instance
		{
			get 
			{
				return GetInstance<OCLogger>();	
			}
		}
		

				
		//---------------------------------------------------------------------------

		#endregion

		//---------------------------------------------------------------------------

		#region Public Member Functions

		//---------------------------------------------------------------------------
			
		/// <summary>
		/// Raises the enable event when OCLogger is loaded.  Initialized here.
		/// </summary>
		/*public void OnEnable()
		{
			LogLevel logLevel = LogLevel.NONE;
			try
			{
				logLevel = (LogLevel)
					Enum.Parse(typeof(LogLevel), OCConfig.Instance.get("LOG_LEVEL"));
			}
			catch(ArgumentException ae)
			{
				Debug.LogError
					("In OCLogger.OnEnable: Failed to construct [" + ae.Message + "]");
			}
			CurrentLevel = logLevel;
				
			IsLogEnabled = logLevel > LogLevel.NONE;
		}*/
			
		/// <summary>
		/// Logs an Error with the specified message, usually a string.
		/// </summary>
		/// <param name='message'>
		/// Log info object, usually a string.
		/// </param>
		public static void Error(System.Object message, UnityEngine.Object context = null, string symbol = OCLogSymbol.ERROR)
		{
			Instance.Log (LogLevel.ERROR, message, context, symbol);
		}
			
		/// <summary>
		/// Logs a Warning with the specified message, usually a string.
		/// </summary>
		/// <param name='message'>
		/// Log info object, usually a string.
		/// </param>
		public static void Warn(System.Object message, UnityEngine.Object context = null, string symbol = OCLogSymbol.WARN)
		{
			Instance.Log(LogLevel.WARN, message, context, symbol);
		}
			
		/// <summary>
		/// Logs some "Running as Normal" information with the specified message, usually a string.
		/// </summary>
		/// <param name='message'>
		/// Log info object, usually a string.
		/// </param>
		public static void Normal(System.Object message, UnityEngine.Object context = null, string symbol = OCLogSymbol.NORMAL)
		{
			Instance.Log(LogLevel.NORMAL, message, context, symbol);
		}
			
		/// <summary>
		/// Logs some Debugging info with the specified message, usually a string.
		/// Avoid using Debug as the function name as it collids with UnityEngine.Debug
		/// </summary>
		/// <param name='message'>
		/// Log info object, usually a string.
		/// </param>
		public static void Debugging(System.Object message, UnityEngine.Object context = null, string symbol = OCLogSymbol.DEBUG)
		{
			if (Instance != null)
				Instance.Log(LogLevel.DEBUG, message, context, symbol);
		}
		
		/// <summary>
		/// Logs some Finely-detailed info with the specified message, 
		/// usually a string.
		/// </summary>
		/// <param name='message'>
		/// Log info object, usually a string.
		/// </param>
		public static void Fine(System.Object message, UnityEngine.Object context = null, string symbol = OCLogSymbol.FINE)
		{
			Instance.Log(LogLevel.FINE, message, context, symbol);
		}

		/// <summary>
		/// Log that something is being destroyed
		/// </summary>
		/// <param name='message'>
		/// Log info object, usually a string.
		/// </param>
		public static void Destroy(System.Object message, UnityEngine.Object context = null, string symbol = OCLogSymbol.DESTROY)
		{
			Instance.Log(LogLevel.DESTROY, message, context, symbol);
		}


		/// <summary>
		/// Log that something is being initialize
		/// </summary>
		/// <param name='message'>
		/// Log info object, usually a string.
		/// </param>
		public static void Init(System.Object message, UnityEngine.Object context = null, string symbol = OCLogSymbol.INIT)
		{
			Instance.Log(LogLevel.INIT, message, context, symbol);
		}

		/// <summary>
		/// Log that something is being created
		/// </summary>
		/// <param name='message'>
		/// Log info object, usually a string.
		/// </param>
		public static void Create(System.Object message, UnityEngine.Object context = null, string symbol = OCLogSymbol.CREATE)
		{
			Instance.Log(LogLevel.CREATE, message, context, symbol);
		}

			

		//---------------------------------------------------------------------------

		#endregion

		//------------------------------------------------------ ---------------------

		#region Private Member Functions

		//---------------------------------------------------------------------------
				
		/// <summary>
		/// Log at the specified level, message and showTrace.
		/// </summary>
		/// <param name='level'>
		/// Log Level.
		/// </param>
		/// <param name='message'>
		/// Log info object, usually a string.
		/// </param>
		/// <param name='showTrace'>
		/// Show trace.
		/// </param>
		private void Log(LogLevel level, System.Object message, UnityEngine.Object context = null, string symbol = "", bool showTrace = false)
		{
			if(!_isLogEnabled)
			{
				return;
			}      
	        
			string logToPrint = message.ToString();

			if(showTrace)
			{
				StackTrace trace = new StackTrace();
				StackFrame frame = null;
				MethodBase method = null;

				frame = trace.GetFrame(2);
				method = frame.GetMethod();

				string callingMethod = method.ReflectedType.Name + "." + method.Name;
	            logToPrint += " At " + callingMethod + "";
			}

			if(dateEntries)
			{
				string now = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss:fff");
				logToPrint += " At [" + now + "].";
			}

        
			switch(level)
			{

				case LogLevel.DEBUG:
				{
					if(printDebug)
						Debug.Log(symbol + logToPrint, context);
					else
						System.Console.WriteLine(symbol + logToPrint); //should only print to log
					break;
				}	
				case LogLevel.NORMAL:
				{
					Debug.Log(symbol + logToPrint);
				break;
				}	
				case LogLevel.WARN:
				{
					Debug.LogWarning(symbol + logToPrint, context);
					break;
				}
				case LogLevel.ERROR:
				{
					Debug.LogError(symbol + logToPrint, context);
					break;
				}
				case LogLevel.DESTROY:
				{
					if(printFine)
						Debug.Log(symbol + logToPrint);
					else
						System.Console.WriteLine(symbol + logToPrint); //should only print to log
					break;
				}	
				case LogLevel.INIT:
				{
					if(printFine)
						Debug.Log(symbol + logToPrint);
					else
						System.Console.WriteLine(symbol + logToPrint); //should only print to log
					break;
				}
				case LogLevel.CREATE:
				{
					if(printFine)
						Debug.Log(symbol + logToPrint);
					else
						System.Console.WriteLine(symbol + logToPrint); //should only print to log
					break;
				}
				case LogLevel.FINE:
				{
					if(printFine)
						Debug.Log(symbol + logToPrint);
					else
						System.Console.WriteLine(symbol + logToPrint); //should only print to log
					break;
				}
				default:
				{
					if(printFine)
					{
						Debug.Log(symbol + logToPrint);
					}
					else
					{
						//This apparently WILL print to the Editor/Webplayer Log File WITHOUT printing to the Unity IN-Editor Console
						//Making it possible to log important information somewhere without cluttering unity
						System.Console.WriteLine(symbol + logToPrint);
					}
					break;
				}
			};

		}
			
		//---------------+-----------------------------------------------------------

		#endregion

		//---------------------------------------------------------------------------

		#region Other Members

		//---------------------------------------------------------------------------		

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenCog.OCLogger"/> 
		/// class.  Intitialization occurs in the OnEnable function, not here.
		/// </summary>
		public OCLogger()
		{
		}
			
		/// <summary>
		/// Enumerator of the log level.
		/// </summary>
		public enum LogLevel
		{
			NONE,
			NORMAL,
			ERROR,
			WARN,
			DEBUG,
			FINE,
			DESTROY,
			CREATE,
			INIT
		};	

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	}// class OCLogger

}// namespace OpenCog




