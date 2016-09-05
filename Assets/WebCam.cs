using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class WebCam : MonoBehaviour
{
    public Material TargetMaterial;

	private WebCamTexture webcamTexture;
//	private Texture2D     targetTexture;

	void Start()
    {
		string name = WebCamTexture.devices[0].name;
		this.webcamTexture = new WebCamTexture();//   (name, 1920, 1080, 30);
        //this.webcamTexture = new WebCamTexture(name, 1920, 1080, 30);
//		this.targetTexture = new Texture2D(Config.Instance.CroppedWidth, Config.Instance.CroppedHeight, TextureFormat.RGB24, false);

        if (this.TargetMaterial != null)
        {
            this.TargetMaterial.mainTexture = this.webcamTexture;
        }

		this.webcamTexture.Play();

        WebCamDevice[] devices = WebCamTexture.devices;
        for( int i = 0 ; i < devices.Length ; i++ )
            Debug.Log(devices[i].name); 

	}

//	public byte[] Shoot(string filename)
//    {
//		var pixels = webcamTexture.GetPixels(
//			(webcamTexture.width - Config.Instance.CroppedWidth) / 2,
//			(webcamTexture.height - Config.Instance.CroppedHeight) / 2,
//			Config.Instance.CroppedWidth,
//			Config.Instance.CroppedHeight
//		);
//		targetTexture.SetPixels(pixels);
//		var bytes = targetTexture.EncodeToJPG(80);
//		File.WriteAllBytes(filename, bytes);
//		return bytes;
//	}

}
