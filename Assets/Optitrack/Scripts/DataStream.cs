using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Optitrack {

public class DataStream {

    // Protocol version
    private int major = 2;
    private int minor = 7;

    private const int maxObjectsCount = 5;  // "5 ought to be enough for anybody"
    private OptiTrackRigidBody[] rigidBodies = new OptiTrackRigidBody[maxObjectsCount];
    private Dictionary<string, int> rigidBodyNames = new Dictionary<string, int>();

    public OptiTrackRigidBody GetRigidbody(int index) {
        return rigidBodies[index];
    }

    public OptiTrackRigidBody GetRigidbody(string name) {
        int id = -1;
        if (rigidBodyNames.TryGetValue(name, out id)) {
            return rigidBodies[id];
        } else {
            return null;
        }
    }

    public void ParsePacket(Byte[] b) {
        int ptr = 0;
        var intBuffer = new int[16];

        // message ID
        Buffer.BlockCopy(b, ptr, intBuffer, 0, 2); ptr += 2;
        var messageID = (uint)intBuffer[0];

        // size
        // Buffer.BlockCopy(b, ptr, intBuffer, 0, 2);
        ptr += 2;
        // var nBytes = (uint)intBuffer[0];

        if (messageID == 7) { // Frame of Mocap Data
            // frame number
            // Buffer.BlockCopy(b, ptr, intBuffer, 0, 4);
            ptr += 4;
            // var frameNumber = (uint)intBuffer[0];

            // number of data sets (markersets, rigidbodies, etc)
            Buffer.BlockCopy(b, ptr, intBuffer, 0, 4); ptr += 4;
            var nMarkerSets = (uint)intBuffer[0];

            for (int i = 0; i < nMarkerSets; i++) {
                // Markerset name
                var nChars = 0;
                while (b[ptr + nChars] != '\0') { nChars++; }
                var strName = System.Text.Encoding.UTF8.GetString(b, ptr, nChars);
//                Debug.Log(string.Format("{0} {1}", i, strName));
                if (strName != "all") {
                    rigidBodyNames[strName] = i;
                }

                ptr += nChars + 1;

                // marker data
                Buffer.BlockCopy(b, ptr, intBuffer, 0, 4); ptr += 4;
                var nMarkers = (uint)intBuffer[0];
                // skipping actual marker data
                ptr += (int)nMarkers * 3 * 4; // vector3 * sizeof(int)
            }

            // unidentified markers
            Buffer.BlockCopy(b, ptr, intBuffer, 0, 4); ptr += 4;
            var nOtherMarkers = (uint)intBuffer[0];
            ptr += (int)nOtherMarkers * 3 * 4; // vector3 * sizeof(int)

            // rigid Bodies
            Buffer.BlockCopy(b, ptr, intBuffer, 0, 4); ptr += 4;
            var nRigidBodies = intBuffer[0];
            for (int i = 0; i < nRigidBodies; i++) {
                var rigidBody = new OptiTrackRigidBody(); // FIXME: Should be preallocated maybe?
                // rigid body pos/ori
                Buffer.BlockCopy(b, ptr, intBuffer, 0, 4); ptr += 4;
                rigidBody.ID = (int)(uint)intBuffer[0];
//                Debug.Log("RigidBodyID: " + rigidBody.ID.ToString());
                var pos = new float[3];
                Buffer.BlockCopy(b, ptr, pos, 0, 4 * 3); ptr += 4 * 3;
                rigidBody.position.x = pos[0];
                rigidBody.position.y = pos[1];
                rigidBody.position.z = pos[2];
                var ori = new float[4];
                Buffer.BlockCopy(b, ptr, ori, 0, 4 * 4); ptr += 4 * 4;
                rigidBody.orientation.x = ori[0];
                rigidBody.orientation.y = ori[1];
                rigidBody.orientation.z = ori[2];
                rigidBody.orientation.w = ori[3];

                // associated marker positions
                Buffer.BlockCopy(b, ptr, intBuffer, 0, 4); ptr += 4;
                var nRigidMarkers = (uint)intBuffer[0];
                ptr += (int)nRigidMarkers * 3 * 4; // vector3 * sizeof(float)

                if (major >= 2) {
                    // associated marker IDs
                    ptr += (int)nRigidMarkers * 4; // sizeof(int)

                    // associated marker sizes
                    ptr += (int)nRigidMarkers * 4; // sizeof(float)

                    // should parse marker metadata
                } else {
                    // should parse rigidMarker data
                }

                if (major >= 2) {
                    // Mean marker error
                    ptr += 4;
                }

                // 2.6 and later
                if( ((major == 2) && (minor >= 6)) || (major > 2) || (major == 0) ) {
                    // params
                    ptr += 2;
                }

                rigidBodies[rigidBody.ID - 1] = rigidBody;
            }

            // skeletons (version 2.1 and later)
            if( ((major == 2) && (minor>0)) || (major>2)) {
                Buffer.BlockCopy(b, ptr, intBuffer, 0, 4); ptr += 4;
                var nSkeletons = (uint)intBuffer[0];
                // Debug.Log("nSkeletons " + nSkeletons.ToString());
                for (int i = 0; i < nSkeletons; i++) {
                    // rigid body pos/ori
                    ptr += 4; // id
                    ptr += 3 * 4; // pos: vector3 * sizeof(float)
                    ptr += 4 * 4; // ori: vector3 * sizeof(float)
                    // associated marker positions
                    Buffer.BlockCopy(b, ptr, intBuffer, 0, 4); ptr += 4;
                    var nRigidMarkers = (uint)intBuffer[0];
                    ptr += (int)nRigidMarkers * 3 * 4; // vector3 * sizeof(float)
                    // associated marker IDs
                    ptr += (int)nRigidMarkers * 4; // sizeof(int)
                    // associated marker sizes
                    ptr += (int)nRigidMarkers * 4; // sizeof(float)

                    // Mean marker error (2.0 and later)
                    if (major >= 2) {
                        ptr += 4;
                    }

                    // Tracking flags (2.6 and later)
                    if( ((major == 2) && (minor >= 6)) || (major > 2) || (major == 0) ) {
                        // params
                        ptr += 2;
                    }
                }
            }

            // labeled markers (version 2.3 and later)
            if( ((major == 2)&&(minor>=3)) || (major>2)) {
                Buffer.BlockCopy(b, ptr, intBuffer, 0, 4); ptr += 4;
                var nLabeledMarkers = (uint)intBuffer[0];
                // Debug.Log("nLabeledMarkers " + nLabeledMarkers.ToString());
                for (int i = 0; i < nLabeledMarkers; i++) {
                    ptr += 4; // id
                    ptr += 3 * 4; // x,y,z: sizeof(float)
                    ptr += 4; // size: sizeof(float)

                    if( ((major == 2) && (minor >= 6)) || (major > 2) || (major == 0) ) {
                        // params
                        ptr += 2;
                    }
                }
            }

            // Force Plate data (version 2.9 and later)
            if ( ((major == 2) && (minor >= 9)) || (major > 2) ) {
                Buffer.BlockCopy(b, ptr, intBuffer, 0, 4); ptr += 4;
                var nForcePlates = (uint)intBuffer[0];
                // Debug.Log("nForcePlates " + nForcePlates.ToString());
                for (int i = 0; i < nForcePlates; i++) {
                    // ID
                    // Buffer.BlockCopy(b, ptr, intBuffer, 0, 4);
                    ptr += 4;
                    // var plateId = (uint)intBuffer[0];
                    // Channel Count
                    Buffer.BlockCopy(b, ptr, intBuffer, 0, 4); ptr += 4;
                    var nChannels = (uint)intBuffer[0];
                    // Channel Data
                    for (int j = 0; j < nChannels; j++) {
                        Buffer.BlockCopy(b, ptr, intBuffer, 0, 4); ptr += 4;
                        var nFrames = (uint)intBuffer[0];
                        for (int k = 0; k < nFrames; k++) {
                            // Buffer.BlockCopy(b, ptr, intBuffer, 0, 4);
                            ptr += 4;
                            // var val = (uint)intBuffer[0];
                        }
                    }
                }
            }

            // latency
            // Buffer.BlockCopy(b, ptr, intBuffer, 0, 4);
            ptr += 4;
            // var latency = (uint)intBuffer[0];

            // timecode
            ptr += 4; // timecode
            ptr += 4; // timecodeSub

            // timestamp
            if ( ((major == 2) && (minor>=7)) || (major>2) ) {
                ptr += 8;
            } else {
                ptr += 4;
            }

            // frame params
            ptr += 2;

            // end of data tag
            ptr += 4;

        } else if (messageID == 5) { // Data descriptions
            Debug.Log("DirectParseClient: Data descriptions");
        }
        // else if (messageID == 100) {}
    }
}

}