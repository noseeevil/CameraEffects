using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace Optitrack {

class DirectStateObject {
    public Socket workSocket = null;
    public const int BufferSize = 65507;
    public byte[] buffer = new byte[BufferSize];
    public StringBuilder sb = new StringBuilder();
}

public class MulticastSocketClient : OptitrackDataSource {

    protected const int    dataPort           = 1511;
    protected const string multicastIPAddress = "239.255.42.99";

    protected Socket client;
    protected bool isActiveThread   = false;
    protected DataStream dataStream = null;

    protected static OptitrackDataSource instance;
    public static OptitrackDataSource Instance { get {
        if (instance == null) {
            instance = new MulticastSocketClient();
        }
        return instance;
    }}

    protected Socket setupClient() {
        var client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

        var ipep = new IPEndPoint(IPAddress.Any, dataPort);
        client.Bind(ipep);

        var ip = IPAddress.Parse(multicastIPAddress);
        client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));
        return client;
    }

    public void Start() {
        try {
            dataStream = new DataStream();
            client = setupClient();
            isActiveThread = receive(client);
        } catch (Exception e) {
            Debug.LogError("[UDP] " + e.ToString());
        }
    }

    protected bool receive(Socket client) {
        try {
            var state = new DirectStateObject();
            state.workSocket = client;
            client.BeginReceive(state.buffer, 0, DirectStateObject.BufferSize, 0, new AsyncCallback(receiveCallback), state);
        } catch (Exception e) {
            Debug.Log("[UDP] " + e.ToString());
            return false;
        }
        return true;
    }

    protected void receiveCallback(IAsyncResult result) {
        try {
            var state = (DirectStateObject) result.AsyncState;
            var client = state.workSocket;
            int bytesRead = client.EndReceive(result);
            if (bytesRead > 0 && isActiveThread) {
                dataStream.ParsePacket(state.buffer);
                client.BeginReceive(state.buffer, 0, DirectStateObject.BufferSize, 0, new AsyncCallback(receiveCallback), state);
            } else {
                Debug.LogWarning("[UDP] - End ReceiveCallback");
                if(isActiveThread == false) {
                    Debug.LogWarning("[UDP] - Closing port");
//                  _isInitRecieveStatus = false;
                    client.Close();
                }
                // Signal that all bytes have been received.
            }
        } catch (Exception e) {
            Debug.LogError("[UDP] " + e.ToString());
        }
    }

    public void Close() {
        isActiveThread = false;
    }
    public DataStream GetDataStream() {
        return dataStream;
    }
}

}
