using UnityEngine;
using System.Collections;
using WebSocketSharp;

namespace Optitrack {

public class WebSocketClient : OptitrackDataSource {

    public string Address = "127.0.0.1:8080";

    protected DataStream dataStream = null;
    protected WebSocketConnection client;

	public void Start() {
        dataStream = new DataStream();
        var url = string.Format("ws://{0}/optitrack", Address);
        client = new WebSocketConnection(url, onMessage);
        client.Connect();
	}

    public void Close() {
        client.Disconnect();
    }

    public DataStream GetDataStream() {
        return dataStream;
    }

    private void onMessage(object sender, MessageEventArgs e) {
        var data = e.RawData;
        dataStream.ParsePacket(data);
    }
}

}
