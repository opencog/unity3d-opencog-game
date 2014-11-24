using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using OpenCog.Utilities.Logging;

namespace OpenCog.Network
{
	public class OldMessageHandler
	{

		private OCNetworkElement ne;
			
		/// <summary>
		/// states 
		/// </summary>
		static readonly int DOING_NOTHING = 0;
		static readonly int READING_MESSAGES = 1;
		
		/// <summary>
		/// TCP socket where the connection is being handled. 
		/// </summary>
		private Socket socket;
		
		private Thread thread;
		
		/// <summary>
		/// Message handling fields 
		/// </summary>
		private OCMessage.MessageType currentMessageType;
		private string currentMessageTo;
		private string currentMessageFrom;
		private StringBuilder currentMessage;
		private List<OCMessage> messageBuffer;
		
		private bool useMessageBuffer = false;
		private int maxMessagesInBuffer = 100;
		
		private int lineCount;
		private int state;
		
		public OldMessageHandler (OCNetworkElement ne, Socket socket)
		{
			this.ne = ne;
			this.socket = socket;
			this.lineCount = 0;
			this.state = DOING_NOTHING;
			
			this.currentMessageTo = null;
			this.currentMessageFrom = null;
			this.currentMessage = new StringBuilder();
			this.messageBuffer = new List<OCMessage>();
			
			this.thread = new Thread(this.run);
		}
		
		public void start()
		{
			this.thread.Start();	
		}
		
		public void run()
		{
			UnityEngine.Debug.Log(OCLogSymbol.CONNECTION + "OldMessageHandler.run() Start handling socket connection.");
			StreamReader reader = null;
			StreamWriter writer = null;
			
//			try {
//				OpenCog.Utility.Console.Console console = OpenCog.Utility.Console.Console.Instance;
//				console.AddConsoleEntry("MessageHandler online, waiting for messages...", "Unity World", OpenCog.Utility.Console.Console.ConsoleEntry.Type.COMMAND);	
//			} catch (Exception ex) {
//			}
			
			try
			{
				Stream s = new NetworkStream(this.socket);
				reader = new StreamReader(s);
				writer = new StreamWriter(s);
			}
			catch( IOException ioe )
			{
				this.socket.Close();
				UnityEngine.Debug.LogError(OCLogSymbol.ERROR + "MessageHandler: An I/O error occured. [" + 
					               ioe.Message + "].");	
			}
			
			bool endInput = false;
			
			while( !endInput )
			{
				try
				{
					// TODO [LAKE] Make some tests to judge the read time.
					string line = reader.ReadLine();
					
					if(line != null)
					{
						//string answer = this.parse(line);
						this.parse(line);

						//UnityEngine.Debug.Log ("Just parsed '" + line + "'");
					}
					else
					{
						UnityEngine.Debug.Log (OCLogSymbol.CONNECTION +  "OldMessageHandler.run() read an empty line. The connection will shut down.");
						
						endInput = true;
					}	
				}
				catch( IOException ioe )
				{
					UnityEngine.Debug.LogError(OCLogSymbol.ERROR + "OldMessageHandler: An I/O error occured. [" + 
						               ioe.Message + "].");	
					endInput = true;
				}
				
				//UnityEngine.Debug.Log ("Still not ending input! I'm still here!");
			} // while
			
			UnityEngine.Debug.Log (OCLogSymbol.DESTROY + "OldMessageHandler.run() shutting down connection.");
			
			try
			{
				reader.Close();
				writer.Close();
				this.socket.Close();
			}
			catch( IOException ioe ) 
			{
				UnityEngine.Debug.LogError(OCLogSymbol.ERROR +"OldMessageHandler: An I/O error occured. [" + 
					               ioe.Message + "].");	
			}	
		}
		
		/// <summary>
		/// Parse a text line from message received. 
		/// </summary>
		/// <param name="inputLine">
		/// The raw data that received by server socket.
		/// </param>
		/// <returns>
		/// An 'OK' string if the line was successfully parsed,
		/// a 'FAILED' string if something went wrong,
		/// null if there is still more to parse.
		/// </returns>
		public string parse(string inputLine)
		{
//			UnityEngine.Debug.Log("OldMessageHandler.parse(" + inputLine + ")");

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
					UnityEngine.Debug.Log (OCLogSymbol.CONNECTION + "NOTIFY_NEW_MESSAGE!");
					if(token.MoveNext()) // Has more elements
					{	
						// Get new message number.
						int numberOfMessages = int.Parse(token.Current.ToString());
	
						this.ne.NotifyNewMessages(numberOfMessages);
						answer = OCNetworkElement.OK_MESSAGE;

                        UnityEngine.Debug.Log(OCLogSymbol.CONNECTION + "Notified about [" + 
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

						System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "Unavailable element message received for [" + 
						          id + "].");
						this.ne.MarkAsUnavailable(id);
						answer = OCNetworkElement.OK_MESSAGE;
					}
					else
					{
						answer = OCNetworkElement.FAILED_MESSAGE;	
					}
				}
				else if(command.Equals("AVAILABLE_ELEMENT"))
				{
					if(token.MoveNext()) // Has more elements
					{	
						string id = token.Current.ToString();

						UnityEngine.Debug.Log(OCLogSymbol.CONNECTION + "Available element message received for [" + 
						          id + "].");
						this.ne.MarkAsAvailable(id);
						answer = OCNetworkElement.OK_MESSAGE;
					}
					else
					{
						answer = OCNetworkElement.FAILED_MESSAGE;	
					}
				}
				else if(command.Equals("START_MESSAGE")) // Parse a common message
				{
					if(this.state == READING_MESSAGES)
					{
						// A previous message was already read.
						UnityEngine.Debug.Log(OCLogSymbol.CONNECTION + "START_MESSAGE: From [" + this.currentMessageFrom +
						          "] to [" + this.currentMessageTo +
						          "] Type [" + this.currentMessageType + "]");
					
						OCMessage message = OCMessage.CreateMessage(this.currentMessageFrom, this.currentMessageTo, this.currentMessageType, this.currentMessage.ToString());
						if( message == null )
						{
							UnityEngine.Debug.LogError(OCLogSymbol.ERROR + "Could not factory message from the following string: " +
							               this.currentMessage.ToString());	
						}
						if(this.useMessageBuffer)
						{
							this.messageBuffer.Add(message);
							if(messageBuffer.Count > this.maxMessagesInBuffer)
							{
								this.ne.PullMessage(this.messageBuffer);	
								this.messageBuffer.Clear();
							}
						}
						else
						{
							this.ne.PullMessage(message);	
						}
							
						this.lineCount = 0;
						this.currentMessageTo = "";
						this.currentMessageFrom = "";
						this.currentMessageType = OCMessage.MessageType.NONE;
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
							UnityEngine.Debug.LogError(OCLogSymbol.ERROR + "Unexepcted command [" +
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
								this.currentMessageType = (OCMessage.MessageType) int.Parse(token.Current.ToString());
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
					this.lineCount = 0;
				}
				else if(command.Equals("NO_MORE_MESSAGES"))
				{
					if(this.state == READING_MESSAGES)
					{
//						UnityEngine.Debug.Log("onLine (NO_MORE_LINES_IN_CURRENT_MESSAGE): From [" + this.currentMessageFrom +
//						          "] to [" + this.currentMessageTo +
//						          "] Type [" + this.currentMessageType + "]: " + this.currentMessage.ToString());	
						
						OCMessage message = OCMessage.CreateMessage(this.currentMessageFrom,
						                                  this.currentMessageTo,
						                                  this.currentMessageType,
						                                  this.currentMessage.ToString());
						
						if(message == null)
						{
							UnityEngine.Debug.LogError(OCLogSymbol.ERROR + "Could not factory message from the following string: [" +
							               this.currentMessage.ToString() + "]");
						}
						if(this.useMessageBuffer)
						{
							this.messageBuffer.Add(message);
							this.ne.PullMessage(messageBuffer);
							this.messageBuffer.Clear();
						}
						else
						{
							this.ne.PullMessage(message);	
						}
						
						// reset variables to default values
						this.lineCount = 0;
						this.currentMessageTo = "";
						this.currentMessageFrom = "";
						this.currentMessageType = OCMessage.MessageType.NONE;
						this.currentMessage.Remove(0, this.currentMessage.Length);
						this.state = DOING_NOTHING; // quit reading state
						answer = OCNetworkElement.OK_MESSAGE;
					}
					else
					{
						UnityEngine.Debug.LogError(OCLogSymbol.ERROR + "Unexpected command [" +
						               command + "]. Discarding line [" +
						               inputLine + "]");
						answer = OCNetworkElement.FAILED_MESSAGE;
					}
				}
				else
				{
					UnityEngine.Debug.LogError(OCLogSymbol.ERROR + "onLine: Unexpected command [" +
					               command + "]. Discarding line [" +
					               inputLine + "]");
					answer = OCNetworkElement.FAILED_MESSAGE;
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
					UnityEngine.Debug.LogError(OCLogSymbol.ERROR + "Unexpected dataline. Discarding line [" +
					               inputLine + "]");
					answer = OCNetworkElement.FAILED_MESSAGE;
				}
			} // end processing selector 'd'
			else
			{
				UnityEngine.Debug.LogError(OCLogSymbol.ERROR + "Invalid selector [" + selector
				               + "]. Discarding line [" + inputLine + "].");
				answer = OCNetworkElement.FAILED_MESSAGE;
			} // end processing selector
			
			return answer;
		}
	}
}
