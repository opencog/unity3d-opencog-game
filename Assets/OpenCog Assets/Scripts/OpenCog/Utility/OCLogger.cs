
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
using System;
using System.Collections;
using OpenCog.Attributes;
using OpenCog.Extensions;
using ProtoBuf;
using UnityEngine;
#endregion

namespace OpenCog
{

/// <summary>
/// The OpenCog OCLogger.
/// </summary>
#region Class Attributes


#endregion
public class OCLogger : OCSingleton< OCLogger, OCScriptableObject >
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------

	private LogLevel m_CurrentLevel;
	
	private bool m_IsLogEnabled;

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
		
	/// <summary>
	/// Raises the enable event when OCLogger is loaded.
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
			Debug.LogError
				("In OCLogger.OnEnable: Failed to construct [" + ae.Message + "]");
		}
		SetLevel(logLevel);
			
		if(logLevel > LogLevel.NONE)
		{
			isEnabled = true;
		}
		else
		{
			isEnabled = false;
		}
	}
		
	public void Error(System.Object logInfoObj)
	{
		Log(LogLevel.ERROR, logInfoObj, true);
	}
		
	public void Warn(System.Object logInfoObj)
	{
		Log(LogLevel.WARN, logInfoObj);
	}
		
	public void Info(System.Object logInfoObj)
	{
		Log(LogLevel.INFO, logInfoObj);
			
	}
		
	/** Avoid using Debug as the function name. */
	public void Debugging(System.Object logInfoObj)
    {
		Log(Level.DEBUG, logInfoObj, true);	
	}
	
	public void Fine(System.Object logInfoObj)
    {
		Log(Level.FINE, logInfoObj);
	}		
		
public void Log(Level level, System.Object logInfoObj, bool showTrace=false)
    {
		if (!m_IsLogEnabled) 
			return;

        string logToPrint;

        if (showTrace)
        {
            StackTrace trace = new StackTrace();
            StackFrame frame = null;
            MethodBase method = null;

            frame = trace.GetFrame(2);
            method = frame.GetMethod();

            string callingMethod = method.ReflectedType.Name + "::" + method.Name;
            logToPrint = "[" + level.ToString() + "] " +
                                callingMethod + ": " + logInfoObj.ToString();
        }
        else
        {
            logToPrint = "[" + level.ToString() + "] " + logInfoObj.ToString();
        }
		if (level <= m_CurrentLevel) {
			/** 
			 * Use unity api for writing information to 
			 * unity editor console. 
			 */
			switch (level) {
			case Level.FINE:
			case Level.DEBUG:
			case Level.INFO:
				{
					UnityEngine.Debug.Log(logToPrint);
					break;
				}	
			case Level.WARN:
				{
					UnityEngine.Debug.LogWarning(logToPrint);
					break;
				}
			case Level.ERROR:
				{
					UnityEngine.Debug.LogError(logToPrint);
					break;
				}
			}
		}
	}		
		
	public void SetLevel(Level level) {
		m_CurrentLevel = level;
	}
	
	public bool isEnabled() {
		return m_IsLogEnabled;
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

	/// <summary>
	/// Initializes a new instance of the <see cref="OpenCog.OCLogger"/> 
	/// class.  Generally, intitialization should occur in the Start or Awake 
	/// function, not here.
	/// </summary>
	public OCLogger()
	{
	}
		
	/// <summary>
	/// Enumerator of the log level.
	/// </summary>
	public enum LogLevel {NONE, ERROR, WARN, INFO, DEBUG, FINE};	

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCLogger

}// namespace OpenCog




