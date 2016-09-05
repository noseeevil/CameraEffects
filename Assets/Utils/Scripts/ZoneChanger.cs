using UnityEngine;
using System.Collections;

public class ZoneChanger : MonoBehaviour {

	public Transform[] Zones;
	public Transform CameraSetup;
	public Transform SecondPlayerSetup;

	private int currentZone;

	void Start () {
		currentZone = 0;
		updateParenting();
	}
	

	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)) {
			currentZone = (currentZone + 1) % Zones.Length;
			updateParenting();
		}
	}

	void updateParenting() {
		CameraSetup.SetParent(Zones[currentZone]);
		if (SecondPlayerSetup != null) {
			SecondPlayerSetup.SetParent(Zones[currentZone]);
		}
	}
}
