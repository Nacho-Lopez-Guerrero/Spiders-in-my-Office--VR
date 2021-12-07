using UnityEngine;
using System.Collections;

public class AutoDestory : MonoBehaviour 
{
	public float autoDestroyTime;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		autoDestroyTime -= Time.deltaTime;

		if(autoDestroyTime <= 0)
			Destroy(gameObject);
	}
}
