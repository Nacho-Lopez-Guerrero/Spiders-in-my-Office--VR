using UnityEngine;
using System.Collections;

public class GameTime : MonoBehaviour
{
	public enum TimeOfDay
	{
		Idle,
		SunRise,
		SunSet
	}

	public Transform[] suns;
	public float dayCycleInMinutes = 1;						//daycycle in REAL minutes

	public float sunRise;									//the time of day that we start the sunrise
	public float sunSet;									//the time of day that we start the sunset
	public float skyBoxBlendModifier;						//the speed at which the textures in the skybox blend

	public Color ambLightMax;
	public Color ambLightMin;

	public float morningLight;
	public float nightLight;

	private bool _isMorning = false;

	private Sun[] _sunScripts;

	private const float DEGREES_PER_SECOND = 360 / DAY;

	private float _degreeRotation;

	private float _timeOfDay;

	private float _dayCicleInSeconds;

	private const float SECOND = 1;
	private const float MINUTE = 60 * SECOND;
	private const float HOUR =  60 * MINUTE;
	private const float DAY = 24 * HOUR;					//Time ingame in seconds

	private TimeOfDay _tod;
	private float _noonTime;								//this is the time of day when it is noon (mediodia)		

	private float _morningLength;
	private float _eveningLength;

	// Use this for initialization
	void Start () 
	{	
		_tod = TimeOfDay.Idle;

		_dayCicleInSeconds = dayCycleInMinutes * MINUTE;

		RenderSettings.skybox.SetFloat("_Blend", 0);		//Puts blend slider value to 0 (Day)

		_sunScripts = new Sun[suns.Length];

		//make sure that all our suns have the script, if not add it
		for(int cnt = 0; cnt < suns.Length; cnt++)
		{
			Sun temp = suns[cnt].GetComponent<Sun>();

			if(temp == null)
			{
				Debug.Log("Sun Script not found... Adding it!!");
				suns[cnt].gameObject.AddComponent<Sun>();
				temp = suns[cnt].GetComponent<Sun>();
			}
			_sunScripts[cnt] = temp;
		}

		//Start a day at 0 seconds
		_timeOfDay = 0;

		//Set the degreeRotation to the amount of degrees that have to rotate for a day
		_degreeRotation = DEGREES_PER_SECOND * DAY / _dayCicleInSeconds;

		sunRise *= _dayCicleInSeconds;
		sunSet *= _dayCicleInSeconds;
		_noonTime = _dayCicleInSeconds / 2;
		_morningLength = _noonTime - sunRise;			//The length of the morning in seconds
		_eveningLength = sunSet -_noonTime;				//The length of the evening in seconds
		morningLight *= _dayCicleInSeconds;
		nightLight *= _dayCicleInSeconds;

		//Setup lights in the sunsScripts to minLight values to start
		SetupLighting();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//position the sun in the sky by adjusting angle the flare is shining from
		for(int cnt = 0; cnt < suns.Length; cnt++)
			suns[cnt].Rotate(new Vector3(_degreeRotation, 0, 0) * Time.deltaTime);

		//updates dayTime
		_timeOfDay += Time.deltaTime;

		//if day timer is over the limit of how long a day lasta, resets dayTimer
		if(_timeOfDay > _dayCicleInSeconds)
			_timeOfDay -= _dayCicleInSeconds;

		if(_timeOfDay > sunRise && _timeOfDay < _noonTime)
		{
			AdjustLighting(true);
		}
		else if(_timeOfDay > _noonTime && _timeOfDay < sunSet)
		{
			AdjustLighting(false);
		}

		//The sun is past the sunrise, before the sunset point, and the day skybox has not fully fadded in
		if(_timeOfDay > sunRise && _timeOfDay < sunSet && RenderSettings.skybox.GetFloat("_Blend") < 1)
		{
			_tod = GameTime.TimeOfDay.SunRise;
			BlendSkyBox();
		}
		else if(_timeOfDay > sunSet && RenderSettings.skybox.GetFloat("_Blend") > 0)
		{
			_tod = GameTime.TimeOfDay.SunSet;
			BlendSkyBox();
		}
		else
			_tod = GameTime.TimeOfDay.Idle;

//		Debug.Log(_timeOfDay);

		//Control the outside lighting effects according to the time of day
		if(!_isMorning && _timeOfDay > morningLight && _timeOfDay < nightLight)
		{
			Messenger<bool>.Broadcast("Morning Light Time", true);
			_isMorning = true;
			Debug.Log("Morning");
		}
		else if(_isMorning && _timeOfDay > nightLight)
		{
			Messenger<bool>.Broadcast("Morning Light Time", false);
			_isMorning = false;
			Debug.Log("Night");
		}

	}

	private void BlendSkyBox()
	{
		float temp = 0;

		switch(_tod)
		{
		case TimeOfDay.SunRise:
			temp = (_timeOfDay - sunRise) / _dayCicleInSeconds * skyBoxBlendModifier;
			break;
		case TimeOfDay.SunSet:
			temp = (_timeOfDay - sunSet) / _dayCicleInSeconds * skyBoxBlendModifier;
			temp = 1 - temp;
			break;
		}

//		Debug.Log(temp);
//		= _timeOfDay / _dayCicleInSeconds * 2;

		RenderSettings.skybox.SetFloat("_Blend", temp);				//Puts blend slider value to 0 (Day)

	}

	private void SetupLighting()
	{
		RenderSettings.ambientLight = ambLightMin;

		for(int cnt = 0; cnt < _sunScripts.Length; cnt++)
		{
			if(_sunScripts[cnt].giveLight)
				suns[cnt].GetComponent<Light>().intensity = _sunScripts[cnt]._minLightBrightness;
		}
	}

	private void AdjustLighting(bool brighten)
	{
		float pos = 0;

		if(brighten)
			pos = (_timeOfDay - sunRise) / _morningLength;	//Get the position of the sun in the morning sky (% of light ths un is gonna emit)
		else
			pos = (sunSet - _timeOfDay) / _eveningLength;		//Get the position of the sun in the evening sky (% of light ths un is gonna emit)

		RenderSettings.ambientLight = new Color(ambLightMin.r + ambLightMax.r * pos,
		                                        ambLightMin.g + ambLightMax.g * pos,
		                                        ambLightMin.b + ambLightMax.b * pos);

		for(int cnt = 0; cnt < _sunScripts.Length; cnt++)
		{
			if(_sunScripts[cnt].giveLight)
				_sunScripts[cnt].GetComponent<Light>().intensity = _sunScripts[cnt]._maxLightBrightness * pos;	
		}
	}
}
