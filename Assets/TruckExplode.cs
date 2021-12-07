using UnityEngine;
using System.Collections;

public class TruckExplode : MonoBehaviour 
{
	public GameObject explosion;
	private bool _exploded;

	// Use this for initialization
	void Start ()
	{
		_exploded = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player") && !_exploded)
		{
			Instantiate(explosion, transform.position, Quaternion.identity);
			animation.Play("TruckExplode");
			GetComponent<AudioSource>().Play();
			_exploded = true;
		}
	}
}
