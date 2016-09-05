using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
[ExecuteInEditMode]
#endif
public class SeparateAppsSetup : MonoBehaviour {
	/** Auto updates player settings depending on current scene. Useful for one-scene-one-build setup. */

	public string ProjectName = "demo";

	#if UNITY_EDITOR
	void Update() {
		setSettings();
	}

	void setSettings() {
		PlayerSettings.companyName = "Interactive Lab";
		PlayerSettings.productName = ProjectName;
		PlayerSettings.bundleIdentifier = "ru.interactivelab." + ProjectName.Replace(" ", "");
	}
	#endif
}
