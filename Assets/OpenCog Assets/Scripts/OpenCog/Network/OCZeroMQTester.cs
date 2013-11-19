using UnityEngine;
using System.Collections;
using System.Text;
using ZMQ;

public class OCZeroMQTester : MonoBehaviour
{

public static OCZeroMQTester _instance;

// Use this for initialization
void Start()
{
        _instance = this;
        Debug.Log("START NETWORK COMMUNICATION");
}

// Update is called once per frame
void Update()
{
        using(Context context = new Context(1))
        {
                using(Socket client = context.Socket(SocketType.REQ))
                {
                        string sendMSG = "Fuuuuuk U!";
                        client.Connect("tcp://localhost:31415");
                        client.Bind("tcp://127.0.0.1:31415");
                        client.Send(sendMSG, Encoding.Unicode);
                        string message = client.Recv(Encoding.Unicode);
                        Debug.Log("Received request: " + message);
                }
        }
}

}//class OCZeroMQTester

