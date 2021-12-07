using UnityEngine;
using System.Collections;

public class ShipRotate : MonoBehaviour {

	public float PlanetRotateSpeed = -0.005f;
	public float OrbitSpeed = 0.0005f;
	protected GameObject capsule;
	
	void Start () {
		capsule = GameObject.Find("capsule"); 
	}
	
	void Update () {
		
		transform.RotateAround (capsule.transform.position, -Vector3.up, OrbitSpeed* Time.deltaTime);
	}	
}
