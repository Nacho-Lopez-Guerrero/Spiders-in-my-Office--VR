using UnityEngine;
using System.Collections;

[AddComponentMenu("Environments/Sun")]

public class Sun : MonoBehaviour 
{
	public float _maxLightBrightness;
	public float _minLightBrightness;

	public float _maxFlareBrightness;
	public float _minFlareBrightness;

	public bool giveLight = false;

	public void Start()
	{
		if(GetComponent<Light>() != null)
			giveLight = true;
	}


}
