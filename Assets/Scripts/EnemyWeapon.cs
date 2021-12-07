using UnityEngine;
using System.Collections;

public class EnemyWeapon : MonoBehaviour {

	public float weapondamage;
	public float attackTimer;			//A bit less of the attack animation duration + meleeResetTimer

	private float _originalAttackTimer;

	// Use this for initialization
	void Start () 
	{
		_originalAttackTimer = attackTimer;
	}
	
	// Update is called once per frame
	void Update () 
	{
		attackTimer -= Time.deltaTime;


	}


	public void ResetAttackTimer()
	{
		attackTimer = _originalAttackTimer;
	}
}
