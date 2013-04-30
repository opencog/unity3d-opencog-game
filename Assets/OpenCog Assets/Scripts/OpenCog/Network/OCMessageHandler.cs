
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
public class OCMessageHandler : OCScriptableObject
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	
	private OCNetworkElement m_NetworkElement;
		
	/// <summary>
	/// The TCP socket where the connection is being handled.
	/// </summary>
	private Socket m_Socket;
			
	/// <summary>
	/// The global state.
	/// </summary>
	private readonly int m_DOING_NOTHING = 0;
	private readonly int m_READING_MESSAGES = 1;
		
	/// <summary>
	///Message handling fields.
	/// </summary>
	private OCMessage.MessageType m_MessageType;
	private string m_MessageTo;
	private string m_MessageFrom;
	private StringBuilder m_Message;
	private List<OCMessage> m_MessageBuffer;
		
	private bool m_UseMessageBuffer = false;
	private int m_MaxMessagesInBuffer = 100;
		
	private int m_LineCount;
	private int m_State;
	
			
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

	public IEnumerator Start()
	{
		yield return StartCoroutine(Update);
	}
		
	public IEnumerator Update()
	{
		OCLogger.Info("Start handling socket connection.");
		
		StreamReader reader = null;
		StreamWriter writer = null;
			
		try
		{
			Stream s = new NetworkStream(m_Socket);
			reader = new StreamReader(s);
			writer = new StreamWriter(s);
		}
		catch( IOException ioe )
		{
			m_Socket.Close();
			OCLogger.Error("An I/O error occured.  [" + ioe.Message + "].");
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
					string answer = Parse(line);
				}
				else
				{
					endInput = true;
				}
			}
			catch( IOException ioe )
			{
				OCLogger.Error("An I/O error occured.  [" + ioe.Message + "].");
				endInput = true;
			}
			yield return null;
		}
			
		try
		{
			reader.Close();
			writer.Close();
			m_Socket.Close();
		}
		catch( IOException ioe )
		{
			OCLogger.Error("An I/O error occured.  [" + ioe.Message + "].");
			endInput = true;
		}
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	private void Initialize(OCNetworkElement networkElement, Socket socket)
	{
		m_NetworkElement = networkElement;
		m_Socket = socket;	
		m_LineCount = 0;
		m_State = m_DOING_NOTHING;
			
		m_MessageTo = null;
		m_MessageFrom = null;
		m_Message = new StringBuilder();
		m_MessageBuffer = new List<OCMessage>();
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
		string answer = null;
			
		char selector = inputLine[0];
		string contents = inputLine.Substring(1);
		
		if(selector == 'c')
		{
			string[] tokenArr = contents.Split(' ');
			IEnumerator token = tokenArr.GetEnumerator();
			token.MoveNext();
			string command = token.Current.ToString();
			
			if(command.Equals("NOTIFY_NEW_MESSAGE"))
			{
				if(token.MoveNext()) // Has more elements
				{	
					// Get new message number.
					int numberOfMessages = int.Parse(token.Current.ToString());

					m_NetworkElement.notifyNewMessages(numberOfMessages);
					answer = OCNetworkElement.OK_MESSAGE;

                      OCLogger.Debugging("onLine: Notified about [" + 
					          numberOfMessages + "] messages in Router.");
				}
				else
				{
					answer = OCNetworkElement.FAILED_MESSAGE;	
				}
			}
			else if(command.Equals("UNAVAILABLE_ELEMENT"))
			{
				if(token.MoveNext()) // Has more elements
				{	
					// Get unavalable element id.
					string id = token.Current.ToString();

                      OCLogger.Debugging("onLine: Unavailable element message received for [" + 
					          id + "].");
					this.ne.markAsUnavailable(id);
					answer = NetworkElement.OK_MESSAGE;
				}
				else
				{
					answer = NetworkElement.FAILED_MESSAGE;	
				}
			}
			else if(command.Equals("AVAILABLE_ELEMENT"))
			{
				if(token.MoveNext()) // Has more elements
				{	
					string id = token.Current.ToString();

                      OCLogger.Debugging("onLine: Available element message received for [" + 
					          id + "].");
					this.ne.markAsAvailable(id);
					answer = NetworkElement.OK_MESSAGE;
				}
				else
				{
					answer = NetworkElement.FAILED_MESSAGE;	
				}
			}
			else if(command.Equals("START_MESSAGE")) // Parse a common message
			{
				if(this.state == READING_MESSAGES)
				{
					// A previous message was already read.
					OCLogger.Debugging("onLine: From [" + this.currentMessageFrom +
					          "] to [" + this.currentMessageTo +
					          "] Type [" + this.currentMessageType + "]");
				
					OCMessage message = OCMessage.CreateMessage(this.currentMessageFrom,
					                                  this.currentMessageTo,
					                                  this.currentMessageType,
					                                  this.currentMessage.ToString());
					if( message == null )
					{
						OCLogger.Error("Could not factory message from the following string: " +
						               this.currentMessage.ToString());	
					}
					if(this.useMessageBuffer)
					{
						this.messageBuffer.Add(message);
						if(messageBuffer.Count > this.maxMessagesInBuffer)
						{
							this.ne.pullMessage(this.messageBuffer);	
							this.messageBuffer.Clear();
						}
					}
					else
					{
						this.ne.pullMessage(message);	
					}
						
					this.lineCount = 0;
					this.currentMessageTo = "";
					this.currentMessageFrom = "";
					this.currentMessageType = Message.MessageType.NONE;
					this.currentMessage.Remove(0, this.currentMessage.Length);
				}
				else
				{
					if(this.state == DOING_NOTHING)
					{
						// Enter reading state from idle state.
						this.state = READING_MESSAGES;	
					}
					else
					{
						OCLogger.Error("onLine: Unexepcted command [" +
						               command + "]. Discarding line [" +
						               inputLine + "]");	
					}
				}
				
				if( token.MoveNext() )
				{
					this.currentMessageFrom = token.Current.ToString();
					
					if( token.MoveNext() )
					{
						this.currentMessageTo = token.Current.ToString();
						if( token.MoveNext() )
						{
							this.currentMessageType = (Message.MessageType) int.Parse(token.Current.ToString());
						}
						else
						{
							answer = NetworkElement.FAILED_MESSAGE;
						}
					}
					else
					{
						answer = NetworkElement.FAILED_MESSAGE;
					}	
				}
				else
				{
					answer = NetworkElement.FAILED_MESSAGE;
				}
				this.lineCount = 0;
			}
			else if(command.Equals("NO_MORE_MESSAGES"))
			{
				if(this.state == READING_MESSAGES)
				{
					OCLogger.Info("onLine: From [" + this.currentMessageFrom +
					          "] to [" + this.currentMessageTo +
					          "] Type [" + this.currentMessageType + "].");	
					
					OCMessage message = OCMessage.CreateMessage(this.currentMessageFrom,
					                                  this.currentMessageTo,
					                                  this.currentMessageType,
					                                  this.currentMessage.ToString());
					
					if(message == null)
					{
						OCLogger.Error("Could not factory message from the following string: [" +
						               this.currentMessage.ToString() + "]");
					}
					if(this.useMessageBuffer)
					{
						this.messageBuffer.Add(message);
						this.ne.pullMessage(messageBuffer);
						this.messageBuffer.Clear();
					}
					else
					{
						this.ne.pullMessage(message);	
					}
					
					// reset variables to default values
					this.lineCount = 0;
					this.currentMessageTo = "";
					this.currentMessageFrom = "";
					this.currentMessageType = Message.MessageType.NONE;
					this.currentMessage.Remove(0, this.currentMessage.Length);
					this.state = DOING_NOTHING; // quit reading state
					answer = NetworkElement.OK_MESSAGE;
				}
				else
				{
					OCLogger.Error("onLine: Unexpected command [" +
					               command + "]. Discarding line [" +
					               inputLine + "]");
					answer = NetworkElement.FAILED_MESSAGE;
				}
			}
			else
			{
				OCLogger.Error("onLine: Unexpected command [" +
				               command + "]. Discarding line [" +
				               inputLine + "]");
				answer = NetworkElement.FAILED_MESSAGE;
			} // end processing command.
		} // end processing selector 'c'
		else if(selector == 'd')
		{
			if(this.state == READING_MESSAGES)
			{
				if(this.lineCount > 0)
				{
					this.currentMessage.Append("\n");	
				}
				this.currentMessage.Append(contents);
				this.lineCount++;
				
			}
			else
			{
				OCLogger.Error("onLine: Unexpected dataline. Discarding line [" +
				               inputLine + "]");
				answer = NetworkElement.FAILED_MESSAGE;
			}
		} // end processing selector 'd'
		else
		{
			OCLogger.Error("onLine: Invalid selector [" + selector
			               + "]. Discarding line [" + inputLine + "].");
			answer = NetworkElement.FAILED_MESSAGE;
		} // end processing selector
		
		return answer;
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------		

	public OCMessageHandler(OCNetworkElement networkElement, Socket socket)
	{
		Initialize(networkElement, socket);
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class MessageHandler

}// namespace OpenCog.Network




