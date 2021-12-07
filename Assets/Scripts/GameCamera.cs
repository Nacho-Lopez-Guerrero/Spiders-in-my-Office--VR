using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {

	private Vector3 cameraTarget;
	private Transform target;
	private float _combatZoomTimer = 4;

	public bool combatZoom;

	// Use this for initialization
	void Start () 
	{
		target = GameObject.FindWithTag("Player").transform;
		combatZoom = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(target != null)
		{
			if(combatZoom)
			{
				cameraTarget = new Vector3(target.position.x, target.position.y + 9, target.position.z - 4);
				transform.position = Vector3.Lerp(transform.position, cameraTarget, Time.deltaTime);
			}
			else
			{
				cameraTarget = new Vector3(target.position.x, target.position.y + 11, target.position.z - 4);
				transform.position = Vector3.Lerp(transform.position, cameraTarget, Time.deltaTime * 10f);
			}
		}
		combatZoom = false;
		if(combatZoom && _combatZoomTimer < 0)
		{
			combatZoom = false;
			ResetZoomTimer();
		}
		else if(combatZoom)
		{
			_combatZoomTimer -= Time.deltaTime;
		}
	}

	public void ResetZoomTimer()
	{
		_combatZoomTimer = 3;
	}
}
