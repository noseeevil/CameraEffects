using UnityEngine;
using System.Collections;

namespace Optitrack {

public class OptiTrackObject : MonoBehaviour {

    public string RigidbodyName;

    public Vector3 rotationOffset;

    [Header("Dont allow rotation on head")]
    public bool AllowRotation;
    public bool StillMode;
    public bool IgnoreY;
    [Header("Lerp Speed")]
    [Range(0,1f)]
    public float LerpAmount = 1f;  // No Lerp by default

    private Vector3 lastPosition = Vector3.zero;
    private Quaternion lastRotation = Quaternion.identity;

    void Update () {
        if (StillMode) {
            transform.localPosition = new Vector3(0, 1.8f, 0);
            return;
        }

        Vector3 pos = OptiTrackManager.Instance.getPosition(RigidbodyName);
        Quaternion rot = OptiTrackManager.Instance.getOrientation(RigidbodyName);
        rot = rot * Quaternion.Euler(rotationOffset);

        pos = Vector3.Lerp(lastPosition, pos, LerpAmount);
        lastPosition = pos;
        rot = Quaternion.Lerp(lastRotation, rot, LerpAmount);
        lastRotation = rot;

        if (IgnoreY) {
            pos.y = 0;
            rot = Quaternion.Euler(new Vector3(0, rot.eulerAngles.y, 0));
        }
        transform.localPosition = pos;

        if (AllowRotation) {
            transform.localRotation = rot;
        }
    }
}

}
