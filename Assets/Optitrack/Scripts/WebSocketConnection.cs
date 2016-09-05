using System;
using WebSocketSharp;
using System.Timers;

public class WebSocketConnection {
    
    public bool IsDead       = true;
    public bool IsConnecting = false;

    private WebSocket ws;
    private Timer keepAliveTimer;

    public WebSocketConnection(string url, EventHandler<MessageEventArgs> onMessage) {
        this.ws = new WebSocket(url);
        this.ws.OnMessage += onMessage;
        this.ws.OnOpen += (sender, e) => {
            UnityEngine.Debug.Log(string.Format("Connected to {0}", url));
            this.IsConnecting = false;
            this.IsDead = false;
        };
        this.ws.OnError += (sender, e) => {
            if (!this.IsDead) {
                UnityEngine.Debug.Log(string.Format("Disconnected from {0}", url));
            }
            this.IsConnecting = false;
            this.IsDead = true;
        };

        this.keepAliveTimer = new Timer(200);
        this.keepAliveTimer.Elapsed += keepAliveLoop;
        this.keepAliveTimer.AutoReset = true;
    }

    public void Send(byte[] payload) {
        this.ws.Send(payload);
    }

    public void Send(string payload) {
        this.ws.Send(payload);
    }

    public void Connect() {
        this.IsDead = true;
        this.keepAliveTimer.Enabled = true;
    }

    public void Disconnect() {
        keepAliveTimer.Stop();
        keepAliveTimer.Dispose();
        this.ws.CloseAsync();
    }

    private void keepAliveLoop(object source, ElapsedEventArgs e) {
        if (this.IsDead && !this.IsConnecting) {
            this.IsConnecting = true;
            this.ws.ConnectAsync();
        }
    }
}
