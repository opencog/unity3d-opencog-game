using System;
using System.Collections;
using System.Text;
using JsonFx;
using JsonFx.Json;
using UnityEngine;
using ZMQ;
using OpenCog.Extensions;
using OpenCog.Network;

/// <summary>
/// The OpenCog Unity3D Embodiment Publisher.  Broadcasts messages of 
/// certain types to the OpenCog Embodiment Server.  Currently
/// implemented with ZeroMQ (v 2.2) for connection magic and JsonFx (v 2.0)
/// for serialization.
/// </summary>
public class OCUnity3DEmbodimentPublisher : OCMonoBehaviour
{

// 0MQ Context = Single-Threaded (but not main thread)
private Context _context;

// 0MQ Message Pattern = PUBLISH/SUBSCRIBE
// 0MQ Infrastructure = BIND/CONNECT
private Socket _publisher;
        
// The types of messages we'll be publishing
public string[] messageTypes;
        
public Encoding encoding = Encoding.Unicode;
        
// 0MQ Transport = TCP (use local machine/port for now)
public string address = "tcp://127.0.0.1:5556";

// Use this for initialization
void Start()
{
  // create single-threaded context
  _context = new Context(1);
  
  // create our publisher socket
  _publisher = _context.Socket(SocketType.PUB);
  
  // eventually we'll bind with the remote Embodiment Server
  _publisher.Bind(address);
}
  
// Update is called once per frame
void Update()
{
  // just using the spacebar for testing
  if(Input.GetKeyDown(KeyCode.Space))
  {
    Debug.Log("In OCUnity3DEmodimentPublisher.Update, sending...");
    foreach(string messageType in messageTypes)
    {
      // use OCMessage as an example... may not be like this in the end
      OCMessage message = new OCMessage();
      
      // just use some stub content for now
      message.MessageContent = "I am a " + messageType + ".";
      
      // serialize using JsonFx and prepend our messageType so 0MQ knows
      // what type of message to look for
      string messageStr = messageType + " " + JsonWriter.Serialize(message);
      
      // finally send it
      _publisher.Send(messageStr, encoding);
    }
  }
}

}//class OCUnity3DEmbodimentPublisher

