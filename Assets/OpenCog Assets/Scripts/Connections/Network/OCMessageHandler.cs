
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
using OpenCog.Utilities.Logging;


#region Usings, Namespaces, and Pragmas
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using OpenCog.Attributes;
using OpenCog.Extensions;
using OpenCog.Network;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;
using OpenCog.Utility;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Network
{

/// <summary>
/// The OpenCog MessageHandler.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCMessageHandler : OCSingletonMonoBehaviour<OCMessageHandler>
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	
	private OCNetworkElement _networkElement;
	/// <summary>
	/// The TCP socket where the connection is being handled.
	/// </summary>
			
	/// <summary>
	/// The global state.
	/// </summary>
	private readonly int DOING_NOTHING = 0;
	private readonly int READING_MESSAGES = 1;
		
	/// <summary>
	///Message handling fields.
	/// </summary>
	private OCMessage.MessageType _messageType;
	private string _messageTo;
	private string _messageFrom;
	private StringBuilder _message;
	private List<OCMessage> _messageBuffer;
		
	private bool _useMessageBuffer = false;
	private int _maxMessagesInBuffer = 100;
		
	private int _lineCount;
	private int _state;
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------
	
	public static OCMessageHandler Instance
	{
		get {
			return GetInstance<OCMessageHandler>();
		}
	}

			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------
	

//	public IEnumerator StartProcessing(System.Net.Sockets.Socket workSocket)
//	{
//		UnityEngine.Debug.Log ("OCMessageHandler::StartProcessing");
//		yield return StartCoroutine(UpdateMessages(workSocket));
////		UnityEngine.Debug.Log ("BALLS!");
////		yield return null;
//	}
		
	/*public IEnumerator UpdateMessages(System.Net.Sockets.Socket workSocket)
	{
		UnityEngine.Debug.Log ("OCMessageHandler::UpdateMessages");
		
		StreamReader reader = null;
		StreamWriter writer = null;
			
		try
		{
			Stream s = new NetworkStream(workSocket);
			reader = new StreamReader(s);
			writer = new StreamWriter(s);
		}
		catch( IOException ioe )
		{
			workSocket.Close();
			Debug.LogError("An I/O error occured.  [" + ioe.Message + "].");
		}
			
		bool endInput = false;
			
		while (true)
		{
			while( !endInput )
			{
				try
				{
					//@TODO Make some tests to judge the read time.
					string line = reader.ReadLine();
					
					if(line != null)
					{
						//string answer = Parse(line);
						Parse (line);
							
						//UnityEngine.Debug.Log ("Just parsed '" + line + "'");
					}
					else
					{
						UnityEngine.Debug.Log ("No more input, line == null");
							
						endInput = true;
							
		//					UnityEngine.Debug.Log ("Setting OCNetworkElement.IsHandling to false...");
		//						
		//					OCNetworkElement.Instance.IsHandlingMessages = false;
					}
				}
				catch( IOException ioe )
				{
					UnityEngine.Debug.Log ("An I/O error occured.  [" + ioe.Message + "].");
					endInput = true;
				}
				catch (System.Exception ex)
				{
					UnityEngine.Debug.Log ("A general error occured.  [" + ex.Message + "].");
					endInput = true;
				}
				
				yield return new UnityEngine.WaitForSeconds(0.1f);
			}
				
			if (endInput)
			{
				yield return new UnityEngine.WaitForSeconds(0.5f);
				
				endInput = false;	
			}
		}
			
		try
		{
			reader.Close();
			writer.Close();
			workSocket.Close();
		}
		catch( IOException ioe )
		{
			UnityEngine.Debug.Log ("Something went wrong: " + ioe.Message);
			//Debug.LogError("An I/O error occured.  [" + ioe.Message + "].");
			endInput = true;
		}
	
		yield return null;
	}
	*/
		
	public void UpdateMessagesSync(System.Net.Sockets.Socket workSocket)
	{
		UnityEngine.Debug.Log ("OCMessageHandler::UpdateMessagesSync");
		
		StreamReader reader = null;
		StreamWriter writer = null;
			
		try
		{
			Stream s = new NetworkStream(workSocket);
			reader = new StreamReader(s);
			writer = new StreamWriter(s);
		}
		catch( IOException ioe )
		{
			workSocket.Close();
			Debug.LogError("An I/O error occured.  [" + ioe.Message + "].");
		}
			
		bool endInput = false;
			
		while( !endInput )
		{
			try
			{
				//@TODO Make some tests to judge the read time.
				string line = reader.ReadLine();
					
				if(line != null)
				{
					UnityEngine.Debug.Log ("Parsing line: " + line);
						
					//string answer = Parse(line);
					Parse(line);
				}
				else
				{
					endInput = true;
				}
			}
			catch( IOException ioe )
			{
				UnityEngine.Debug.Log ("An I/O error occured.  [" + ioe.Message + "].");
				endInput = true;
			}
		}
			
		try
		{
			reader.Close();
			writer.Close();
			workSocket.Close();
		}
		catch( IOException ioe )
		{
			UnityEngine.Debug.Log ("Something went wrong: " + ioe.Message);
			//Debug.LogError("An I/O error occured.  [" + ioe.Message + "].");
			endInput = true;
		}
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	override protected void Initialize()
	{
			UnityEngine.Debug.Log (OCLogSymbol.INIT + "OCMessageHandler::Initialize");
			
		try {
			_lineCount = 0;
			_state = DOING_NOTHING;
				
			_messageTo = null;
			_messageFrom = null;
			_message = new StringBuilder();
			_messageBuffer = new List<OCMessage>();	
		} catch (System.Exception ex) {
			UnityEngine.Debug.LogError (OCLogSymbol.ERROR + "OCMessageHandler::Initialize, something went wrong: " + ex.ToString());
		}
			
		
	}
		
	private string ParseNotifyNewMessage(IEnumerator token)
	{
		UnityEngine.Debug.Log ("OCMessageHandler::ParseNotifyNewMessage");
			
		string answer = null;
		
		if(token.MoveNext()) // Has more elements
		{	
			// Get new message number.
			int numberOfMessages = int.Parse(token.Current.ToString());

			_networkElement.NotifyNewMessages(numberOfMessages);
			answer = OCNetworkElement.OK_MESSAGE;

              System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +"onLine: Notified about [" +
			          numberOfMessages + "] messages in Router.");
		}
		else
		{
			answer = OCNetworkElement.FAILED_MESSAGE;	
		}
			
		return answer;
	}
		
	private string ParseUnavailableElement(IEnumerator token)
	{
		System.Console.WriteLine(OCLogSymbol.RUNNING + "OCMessageHandler::ParseUnavailableElement");
			
		string answer = null;
			
		if(token.MoveNext()) // Has more elements
		{	
			// Get unavalable element id.
			string id = token.Current.ToString();

            System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +"Unavailable element message received for [" + id + "].");
			_networkElement.MarkAsUnavailable(id);
			answer = OCNetworkElement.OK_MESSAGE;
		}
		else
		{
			answer = OCNetworkElement.FAILED_MESSAGE;
		}	
				
		return answer;
	}
		
	private string ParseAvailableElement(IEnumerator token)
	{
		UnityEngine.Debug.Log ("OCMessageHandler::ParseAvailableElement");
			
		string answer = null;
			
		if(token.MoveNext()) // Has more elements
		{	
			string id = token.Current.ToString();

              System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +"onLine: Available element message received for [" + 
			          id + "].");
			_networkElement.MarkAsAvailable(id);
			answer = OCNetworkElement.OK_MESSAGE;
		}
		else
		{
			answer = OCNetworkElement.FAILED_MESSAGE;
		}
				
		return answer;
	}
		
	private string ParseStartMessage(string inputLine, string command, IEnumerator token)
	{
		UnityEngine.Debug.Log (OCLogSymbol.RUNNING + "OCMessageHandler::ParseStartMessage");
			
		string answer = null;
			
		if(_state == READING_MESSAGES)
		{
			// A previous message was already read.
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +"onLine: From [" + _messageFrom +
			          "] to [" + _messageTo +
			          "] Type [" + _messageType + "]");
		
			OCMessage message = OCMessage.CreateMessage(_messageFrom,
			                                  _messageTo,
			                                  _messageType,
			                                  _message.ToString());
			if( message == null )
			{
				Debug.LogError("Could not factory message from the following string: " +
				               _message.ToString());
			}
			if(_useMessageBuffer)
			{
				_messageBuffer.Add(message);
				if(_messageBuffer.Count > _maxMessagesInBuffer)
				{
					_networkElement.PullMessage(_messageBuffer);
					_messageBuffer.Clear();
				}
			}
			else
			{
				_networkElement.PullMessage(message);
			}
				
			_lineCount = 0;
			_messageTo = "";
			_messageFrom = "";
			_messageType = OCMessage.MessageType.NONE;
			_message.Remove(0, _message.Length);
		}
		else
		{
			if(_state == DOING_NOTHING)
			{
				// Enter reading state from idle state.
				_state = READING_MESSAGES;
			}
			else
			{
				Debug.LogError("onLine: Unexepcted command [" +
				               command + "]. Discarding line [" +
				               inputLine + "]");	
			}
		}
		
		if( token.MoveNext() )
		{
			_messageFrom = token.Current.ToString();
			
			if( token.MoveNext() )
			{
				_messageTo = token.Current.ToString();
				if( token.MoveNext() )
				{
					_messageType = (OCMessage.MessageType) int.Parse(token.Current.ToString());
				}
				else
				{
					answer = OCNetworkElement.FAILED_MESSAGE;
				}
			}
			else
			{
				answer = OCNetworkElement.FAILED_MESSAGE;
			}	
		}
		else
		{
			answer = OCNetworkElement.FAILED_MESSAGE;
		}
		_lineCount = 0;
			
		return answer;
	}
		
	private string ParseNoMoreMessages(string inputLine, string command, IEnumerator token)
	{
		UnityEngine.Debug.Log ("OCMessageHandler::ParseNoMoreMessages");
			
		string answer = null;
			
		if(_state == READING_MESSAGES)
		{
			UnityEngine.Debug.Log("onLine: From [" + _messageFrom +
			          "] to [" + _messageTo +
			          "] Type [" + _messageType + "]: " + _message.ToString());
			
			OCMessage message = OCMessage.CreateMessage(_messageFrom,
			                                  _messageTo,
			                                  _messageType,
			                                  _message.ToString());
			
			if(message == null)
			{
				UnityEngine.Debug.Log("Could not factory message from the following string: [" +
				               _message.ToString() + "]");
			}
			if(_useMessageBuffer)
			{
				UnityEngine.Debug.Log ("Using message buffer...");
				_messageBuffer.Add(message);
				_networkElement.PullMessage(_messageBuffer);
				_messageBuffer.Clear();
			}
			else
			{	
				UnityEngine.Debug.Log ("Not using message buffer...pulling instead...");
				_networkElement.PullMessage(message);
			}
			
			// reset variables to default values
			_lineCount = 0;
			_messageTo = "";
			_messageFrom = "";
			_messageType = OCMessage.MessageType.NONE;
			_message.Remove(0, _message.Length);
			_state = DOING_NOTHING; // quit reading state
			answer = OCNetworkElement.OK_MESSAGE;
		}
		else
		{
			UnityEngine.Debug.Log("onLine: Unexpected command [" +
			               command + "]. Discarding line [" +
			               inputLine + "]");
			answer = OCNetworkElement.FAILED_MESSAGE;
		}
	
		return answer;
	}
	
	private string ParseCMessage(string inputLine)
	{
		UnityEngine.Debug.Log ("OCMessageHandler::ParseCMessage");
			
		string contents = inputLine.Substring(1);
		string answer = null;
			
		string[] tokenArr = contents.Split(' ');
		IEnumerator token = tokenArr.GetEnumerator();
		token.MoveNext();
		string command = token.Current.ToString();
		
		if(command.Equals("NOTIFY_NEW_MESSAGE"))
		{
			answer = ParseNotifyNewMessage (token);
		}
		else if(command.Equals("UNAVAILABLE_ELEMENT"))
		{
			answer = ParseUnavailableElement(token);
		}
		else if(command.Equals("AVAILABLE_ELEMENT"))
		{
			answer = ParseAvailableElement(token);
		}
		else if(command.Equals("START_MESSAGE")) // Parse a common message
		{
			answer = ParseStartMessage (inputLine, command, token);
		}
		else if(command.Equals("NO_MORE_MESSAGES"))
		{
			answer = ParseNoMoreMessages(inputLine, command, token);	
		}
		else
		{
			Debug.LogError("onLine: Unexpected command [" + command + "]. Discarding line [" + inputLine + "]");
			answer = OCNetworkElement.FAILED_MESSAGE;
		} // end processing command.
			
		return answer;
	}
		
	private string ParseDMessage(string inputLine)
	{
		UnityEngine.Debug.Log ("OCMessageHandler::ParseDMessage");
			
		string answer = null;
			
		string contents = inputLine.Substring(1);
			
		if(_state == READING_MESSAGES)
		{
			if(_lineCount > 0)
			{
				_message.Append("\n");
			}
			_message.Append(contents);
			_lineCount++;
		}
		else
		{
			Debug.LogError("onLine: Unexpected dataline. Discarding line [" +
			               inputLine + "]");
			answer = OCNetworkElement.FAILED_MESSAGE;
		}
			
		return answer;
	}
		
			
		
	/// <summary>
	/// Parse a text line from message received. 
	/// </summary>
	/// <param name='inputLine'>
	/// The raw data that received by server socket.
	/// </param>
	/// <returns>
	/// An 'OK' string if the line was successfully parsed,
	/// a 'FAILED' string if something went wrong,
	/// null if there is still more to parse.
	/// </returns> 
	private string Parse(string inputLine)
	{
		UnityEngine.Debug.Log ("OCMessageHandler::Parse (" + inputLine + ")");
		
		string answer = null;
					
		if (_networkElement == null)
			_networkElement = OCNetworkElement.Instance;
			
		UnityEngine.Debug.Log ("_networkElement == null? I wonder..." + ((_networkElement == null) ? "yes...it is..." : "no...it isn't" ));		
			
//		if (_networkElement != null) {
//			//UnityEngine.Debug.Log ("OCMessageHandler is using a NetworkElement with ID " + _networkElement.VerificationGuid + "...");
//		}
//		else {
//			//UnityEngine.Debug.Log("_networkElement == null");
//		}
			
		char selector = inputLine[0];
		
		if(selector == 'c')
		{
			answer = ParseCMessage(inputLine);
		} 
		else if(selector == 'd')
		{
			answer = ParseDMessage(inputLine);
			
		} 
		else
		{
			Debug.LogError("onLine: Invalid selector [" + selector
			               + "]. Discarding line [" + inputLine + "].");
			answer = OCNetworkElement.FAILED_MESSAGE;
		} 
		
		return answer;
	}
		
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------		

	public OCMessageHandler()
	{
		
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class MessageHandler

}// namespace OpenCog.Network




