using UnityEngine;
using System.Collections;

public class FpsCounter : MonoBehaviour, OSDDisplayable
{
    public string FormatString   = "{0:00.0}fps";
	public float  UpdateInterval = 0.5f;

	private string data;
	public string Data {
		get { return data; }
	}
	 
	private float accum   = 0; // FPS accumulated over the interval
	private int   frames  = 0; // Frames drawn over the interval
	private float timeleft;    // Left time for current interval
		
	void Awake() {
	   Application.targetFrameRate = 60;
	   this.useGUILayout = false;
	}
	 
	void Start() {
	    timeleft = UpdateInterval; 
	}
	 
	void Update() {
	    timeleft -= Time.deltaTime;
	    accum += Time.timeScale / Time.deltaTime;
	    ++frames;
	    if (timeleft <= 0.0) {
	    	float fps = accum/frames;
			data = System.String.Format(FormatString, fps);

	        timeleft = UpdateInterval;
	        accum = 0.0F;
	        frames = 0;
	    }
	}
}