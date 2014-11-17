
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
	public class OCLogger : OCSingletonScriptableObject< OCLogger >
	{

		//---------------------------------------------------------------------------

		#region Private Member Data

		//---------------------------------------------------------------------------
			
		/// <summary>
		/// The current log level.  If a message is logged with a level less than the
		/// current log level, it will be displayed in the Unity Console output 
		/// window.
		/// </summary>
		private LogLevel _currentLevel;
		
		/// <summary>
		/// Is the logger enabled?
		/// </summary>
		private bool _isLogEnabled;
		
		

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
		public static LogLevel CurrentLevel
		{
			get{ return Instance._currentLevel;}
			set{ Instance._currentLevel = value;}
		}
		
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
		public void OnEnable()
		{
			LogLevel logLevel = LogLevel.NONE;
			try
			{
				logLevel = (LogLevel)
					Enum.Parse(typeof(LogLevel), OCConfig.Instance.get("LOG_LEVEL"));
			}
			catch(ArgumentException ae)
			{
				UnityEngine.Debug.LogError
					("In OCLogger.OnEnable: Failed to construct [" + ae.Message + "]");
			}
			CurrentLevel = logLevel;
				
			IsLogEnabled = logLevel > LogLevel.NONE;
		}
			
		/// <summary>
		/// Logs an Error with the specified message, usually a string.
		/// </summary>
		/// <param name='message'>
		/// Log info object, usually a string.
		/// </param>
		public static void Error(System.Object message)
		{
			Instance.Log(LogLevel.ERROR, message, true);
		}
			
		/// <summary>
		/// Logs a Warning with the specified message, usually a string.
		/// </summary>
		/// <param name='message'>
		/// Log info object, usually a string.
		/// </param>
		public static void Warn(System.Object message)
		{
			Instance.Log(LogLevel.WARN, message);
		}
			
		/// <summary>
		/// Logs some Info with the specified message, usually a string.
		/// </summary>
		/// <param name='message'>
		/// Log info object, usually a string.
		/// </param>
		public static void Info(System.Object message)
		{
			Instance.Log(LogLevel.INFO, message);
		}
			
		/// <summary>
		/// Logs some Debugging info with the specified message, usually a string.
		/// Avoid using Debug as the function name as it collids with UnityEngine.Debug
		/// </summary>
		/// <param name='message'>
		/// Log info object, usually a string.
		/// </param>
		public static void Debugging(System.Object message)
		{
			if (Instance != null)
				Instance.Log(LogLevel.DEBUG, message, true);
		}
		
		/// <summary>
		/// Logs some Finely-detailed info with the specified message, 
		/// usually a string.
		/// </summary>
		/// <param name='message'>
		/// Log info object, usually a string.
		/// </param>
		public static void Fine(System.Object message)
		{
			Instance.Log(LogLevel.FINE, message);
		}
			

		//---------------------------------------------------------------------------

		#endregion

		//---------------------------------------------------------------------------

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
		private void Log(LogLevel level, System.Object message, bool showTrace=false)
		{
			if(!_isLogEnabled)
			{
				return;
			}

			string now = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss:fff");
			
			string logToPrint = "[" + now + "] [" + level.ToString() + "] ";

			if(showTrace)
			{
				StackTrace trace = new StackTrace();
				StackFrame frame = null;
				MethodBase method = null;

				frame = trace.GetFrame(2);
				method = frame.GetMethod();

				string callingMethod = method.ReflectedType.Name + "::" + method.Name;
				logToPrint += callingMethod + ": ";
			}

			logToPrint += message.ToString();

			if(level <= _currentLevel)
			{
				/** 
				 * Use unity api for writing information to 
				 * unity editor console. 
				 */
				switch(level)
				{
				case LogLevel.FINE:
				case LogLevel.DEBUG:
				case LogLevel.INFO:
					{
						UnityEngine.Debug.Log(logToPrint);
						break;
					}	
				case LogLevel.WARN:
					{
						UnityEngine.Debug.LogWarning(logToPrint);
						break;
					}
				case LogLevel.ERROR:
					{
						UnityEngine.Debug.LogError(logToPrint);
						break;
					}
				}
			}
		}
				
		//---------------------------------------------------------------------------

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
			ERROR,
			WARN,
			INFO,
			DEBUG,
			FINE
		};	

		//---------------------------------------------------------------------------

		#endregion

		//---------------------------------------------------------------------------

	}// class OCLogger

}// namespace OpenCog




