using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OSD : MonoBehaviour {

	public Text DisplayText;

	void Update () {
		var t = "";
		foreach(OSDDisplayable i in GetComponents<OSDDisplayable>()) {
			t += i.Data + " ";
		}
		DisplayText.text = t;
	}
}

interface OSDDisplayable {
	string Data { get; }
}
