using UnityEngine;
using System;
using System.Collections;

namespace Optitrack {

public class OptiTrackManager : MonoBehaviour {

    public float scale    = 1.0f;
    public Vector3 origin = Vector3.zero;

    public enum ConnectionTypes { Multicast, Websocket }
    public ConnectionTypes ConnectionType;
    public string WebsocketAddress = "127.0.0.1:8080";

    private static OptiTrackManager instance;
    public static OptiTrackManager Instance { get { return instance; } }

    private OptitrackDataSource socketClient;

    void Awake() {
        instance = this;
    }

//    ~OptiTrackManager() {
//        stop();
//    }

    void Start() {
        if (ConnectionType == ConnectionTypes.Multicast) {
            Debug.Log("Listening multicast");
            socketClient = MulticastSocketClient.Instance;
        } else {
            socketClient = new WebSocketClient();
            (socketClient as WebSocketClient).Address = WebsocketAddress;
        }
        socketClient.Start();
        Application.runInBackground = true;
    }

    public void OnApplicationQuit() {
        socketClient.Close();
    }

    public Vector3 getPosition(string rigidbodyName) {
        DataStream dataStream = socketClient.GetDataStream();
        if (dataStream != null) {
            var rigidBody = dataStream.GetRigidbody(rigidbodyName);
            if (rigidBody != null) {
                return adjustPosition(rigidBody.position);
            }
        }
        return Vector3.zero;
    }

    public Vector3 getPosition(int rigidbodyIndex) {
        DataStream dataStream = socketClient.GetDataStream();
        if (dataStream != null) {
            var rigidBody = dataStream.GetRigidbody(rigidbodyIndex);
            if (rigidBody != null) {
                return adjustPosition(rigidBody.position);
            }
        }
        return Vector3.zero;
    }

    private Vector3 adjustPosition(Vector3 src) {
        Vector3 pos = origin + src * scale;
        pos.x = -pos.x; // not really sure if this is the best way to do it
        //pos.y = pos.y; // these may change depending on your configuration and calibration
        //pos.z = -pos.z;
        return pos;
    }

    public Quaternion getOrientation(string rigidbodyName) {
        DataStream dataStream = socketClient.GetDataStream();
        if (dataStream != null) {
             var rigidBody = dataStream.GetRigidbody(rigidbodyName);
            if (rigidBody != null) {
                return adjustOrientation(rigidBody.orientation);
            }
        }
        return Quaternion.identity;           
    }

    public Quaternion getOrientation(int rigidbodyIndex) {
        DataStream dataStream = socketClient.GetDataStream();
        if (dataStream != null) {
            var rigidBody = dataStream.GetRigidbody(rigidbodyIndex);
            if (rigidBody != null) {
                return adjustOrientation(rigidBody.orientation);
            }
        }
        return Quaternion.identity;
    }

    private Quaternion adjustOrientation(Quaternion src) {
        Vector3 euler = src.eulerAngles;
        euler = new Vector3(-euler.x, 180.0f - euler.y, euler.z); // these may change depending on your calibration
        return Quaternion.Euler(euler);
    }
}

public class Marker {
    public int ID = -1;
    public Vector3 pos;
}

public class OptiTrackRigidBody {
    public string name = "";
    public int ID = -1;
//    public int parentID = -1;
    public Vector3 position;
    public Quaternion orientation;
}

public interface OptitrackDataSource {
    void Start();
    void Close();
    DataStream GetDataStream();
}

}
