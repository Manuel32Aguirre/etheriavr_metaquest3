using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

public class UDPReceiver : MonoBehaviour
{
    Thread receiveThread;
    UdpClient client;
    public int port = 12345;
    
    // Aquí guardaremos todos los mensajes que lleguen
    public ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();

    void Start() {
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveData() {
        client = new UdpClient(port);
        while (true) {
            try {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);
                messageQueue.Enqueue(text); // Encolar mensaje
            } catch (Exception) { }
        }
    }

    void OnApplicationQuit() {
        if (receiveThread != null) receiveThread.Abort();
        if (client != null) client.Close();
    }
}