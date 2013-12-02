using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsonFx;
using JsonFx.Json;
using UnityEngine;
using ZMQ;
using OpenCog.Extensions;
using OpenCog.Network;

/// <summary>
/// The OpenCog Unity3D Embodiment Subscriber.  Listens for messages of 
/// certain types coming from the OpenCog Embodiment server.  Currently
/// implemented with ZeroMQ (v 2.2) for connection magic and JsonFx (v 2.0)
/// for serialization.
/// </summary>
public class OCUnity3DEmbodimentSubscriber : OCMonoBehaviour
{

// 0MQ Context = Single-Threaded (but not main thread)
private Context _context;
  
// 0MQ Message Pattern = PUBLISH/SUBSCRIBE
// 0MQ Infrastructure = BIND/CONNECT
private Socket _subscriber;
  
// The types of messages we'll be publishing
public string[] messageTypes;
  
public Encoding encoding = Encoding.Unicode;
  
// 0MQ Transport = TCP (use local machine/port for now)
public string address = "tcp://localhost:5556";
       
// Use this for initialization
void Start()
{
  // create single-threaded context
  _context = new Context(1);
    
  // create our subscriber socket
  _subscriber = _context.Socket(SocketType.SUB);
    
  // subscribe to each message type we're interested in
  foreach(string messageType in messageTypes)
  {
    _subscriber.Subscribe(messageType, encoding);
  }
  
  // eventually we'll bind with the remote Embodiment Server,
  // however just connect for now
  _subscriber.Connect(address);
}
  
// Update is called once per frame
void Update()
{
  // note: the blocking version doesn't play nice with Unity3D.
  // also note: there's still some random crashes from bad thread cleanup
  string payload = _subscriber.Recv(encoding, SendRecvOpt.NOBLOCK);
  
  if(payload != default(string))
  {
    // make sure to pull out that prepended message type before we deserialize
    string jsonStr = payload.Substring(payload.IndexOf("{"));
    OCMessage message = JsonReader.Deserialize<OCMessage>(jsonStr);
    Debug.Log("In OCUnity3DEmbodimentSubscriber.Update, message = " + message.ToString());
  }
}

}//class OCUnity3DEmbodimentSubscriber

