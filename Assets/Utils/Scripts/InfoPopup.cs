using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfoPopup : MonoBehaviour {

    public Text DisplayText;
    public Animation Anim;

    public void Display(string text)
    {
        this.DisplayText.text = text;
        this.Anim.Play();
    }
}
