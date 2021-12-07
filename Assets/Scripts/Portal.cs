using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour 
{
	private const string DEFAULT_DROP_ZONE = "dz_ Default";
	public GameObject destination;

	// Use this for initialization
	void Start () 
	{
		if(destination == null)
			destination = GameObject.Find(DEFAULT_DROP_ZONE);	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			other.transform.position = destination.transform.position;
		}
	}
}
