using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AdvancedMovement))]
[RequireComponent (typeof(SphereCollider))]
public class AI : MonoBehaviour 
{
	private enum State
	{
		Idle,				//do nothing
		Init,				//make sure that everything we need is here
		Setup,				//assign the values to the things we need
		Search,				//find the player
		Attack,				//attack the player
		Retreat,			//retreat to spawn point
		Decide,				//decide what to do with the targeted player
		Flee				//return to the nearest spawnpoint with another mob
	}

	private Transform _target;
	public float runDistance = 20;

	private Transform _myTransform;
	private Transform _home;

	private const float ROTATION_DAMP = 0.03f;
	private const float FORWARD_DAMP = 0.9f;

	private State _state;

	private SphereCollider _sphereCollider;

	private AdvancedMovement _advancedMovement;
	private Mob _mobScript;

	public float baseMeleeRange = 3;

	
	void Awake()
	{
		_advancedMovement = gameObject.GetComponent<AdvancedMovement>();
		_mobScript = gameObject.GetComponent<Mob>();

	}


	void Start()
	{
		_state = AI.State.Init;
		StartCoroutine("FSM");
	}


	IEnumerator FSM()										//FSM = Finite State Machine
	{
//		Debug.LogWarning("Combat: " + _me.inCombat);

		while(_state != AI.State.Idle)
		{
			switch(_state)
			{
			case State.Init:
				Init();
				break;
			case State.Setup:
				Setup();
				break;
			case State.Search:
				Search();
				break;
			case State.Decide:
				Decide();
				break;
			}

			yield return null;
		}
	}

	/*
	void LateUpdate()
	{

		if(_colliderFlag) //& !_alive)
		{
//			Debug.Log("REACTIVAMOS FSM");

//			_state = AI.State.Search;
//			_alive = true;
			StopAllCoroutines();
			StartCoroutine("FSM");					//aniadido por mi!!!!!!!!!
		}
	}
*/

	private void Init()
	{
		Debug.Log("Init");

		_home = transform.parent.transform.parent.transform;
		_myTransform = transform;

		_sphereCollider = GetComponent<SphereCollider>();
		if(_sphereCollider == null)
		{
			Debug.LogError("SphereCollider not present");
			return;
		}

		_state = AI.State.Setup;
	}


	private void Setup()
	{
	//	Debug.Log("****Setup****");

		_sphereCollider.center = GetComponent<CharacterController>().center;
		_sphereCollider.isTrigger = true;
		_state = AI.State.Idle;
	}


	private void Search()
	{
	//	Debug.Log("****Search****");

		if(_target == null)
		{
			_state = AI.State.Idle;

			if(_mobScript.InCombat)
				_mobScript.InCombat = false;
		}
		else
		{
			if(!_mobScript.InCombat)
				_mobScript.InCombat = true;

			_state = AI.State.Decide;	
		}
	}


	private void Decide()
	{
	//	Debug.Log("****Decide****");

		Move();

		//Create a routine to decide what to do with the targeted player
		int opt = 0;
		if(_target != null && _target.CompareTag("Player"))
		{
			if(Vector3.Distance(transform.position, _target.position) < baseMeleeRange && _mobScript.meleeResetTimer <= 0)
			{
//				Debug.Log("In melee range");
				opt = Random.Range(0, 3);
			}
			else
			{
				if(_mobScript.meleeResetTimer > 0)
					_mobScript.meleeResetTimer -= Time.deltaTime;

				//Debug.Log("NOT In melee range : " + "Timer: " +_me.meleeResetTimer);
				opt = Random.Range(1, 3);
			}

			switch(opt)
			{
			case 0:
				MeleeAttack();
				break;
			case 1:
				RangedAttack();
				break;
			case 2:
				MagicAttack();
				break;
			default:
				Debug.Log("Option: " + opt + " not defined");
				break;
			}
			
		}
		_state = AI.State.Search;
	}


	private void MeleeAttack()
	{


		_mobScript.meleeResetTimer = _mobScript.meleeAttackTimer;



		//decide if we hit or not
		if(true)
		{
			SendMessage("PlayMeleeAttack");
		}


	}


	private void RangedAttack()
	{
//		Debug.Log("Ranged");

	}


	private void MagicAttack()
	{
//		Debug.Log("Magic");

	}


	private void Attack()
	{
	//	Debug.Log("****Attack****");

//		Move();
		_state = AI.State.Retreat;
	}

	private void Flee()
	{
//		Debug.Log("****Flee****");

		Move();
//		_alive = false;

		_state = AI.State.Search;
	}


	private void Retreat()
	{
	//	Debug.Log("****Retreat****");
		_myTransform.LookAt(_target);
		Move();
		_state = AI.State.Search;
	}


	private void Move()
	{
//		Debug.LogWarning("Combat: " + _me.inCombat);

		if(_target)
		{				
			float dist = Vector3.Distance(_target.position, _myTransform.position);

			if(_target.name == "Spawn Point" )
			{
				if(dist < baseMeleeRange)
				{	
					_state = AI.State.Idle;
					_target = null;

					SendMessage("MoveMeForward", AdvancedMovement.Forward.none);
					SendMessage("RotateMe", AdvancedMovement.Turn.none);
					return;
				}
			}


				Quaternion rot = Quaternion.LookRotation(_target.transform.position - _myTransform.position);
				_myTransform.rotation = Quaternion.Slerp(_myTransform.rotation, rot, Time.deltaTime * 7);


			Vector3 dir = (_target.position - _myTransform.position).normalized;
			float direction = Vector3.Dot(dir, _myTransform.forward);										//Deveuelve un valor entre 1 y -1 dependeindo de la posicion realtiva de los dos vectores

			if(direction > FORWARD_DAMP && dist > baseMeleeRange)
			{
				if(dist > runDistance)
					SendMessage("ActivateRun", AdvancedMovement.Forward.none);											
				else
					SendMessage("DeActivateRun", AdvancedMovement.Forward.none);											
				
				SendMessage("MoveMeForward", AdvancedMovement.Forward.forward);								//Sends a message to all monobehaviour scripts that are ATTACHED to this gameObject
			}
			else
				SendMessage("MoveMeForward", AdvancedMovement.Forward.none);
		}
	}

	public Transform Target
	{
		get{ return _target; }
		set{ _target = value; }
	}
	

	public void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{	
			if(!_advancedMovement.dead)
				_target = other.transform;
			//PC.Instance.InCombat = true;
			_state = AI.State.Search;
			//_sphereCollider.enabled = false;			//Walk around SphereCollider
			StartCoroutine("FSM");
		}
	}


	public void OnTriggerExit(Collider other)
	{
//		Debug.Log("Trigger Exit");
		if(other.CompareTag("Player"))
		{
			if(!_advancedMovement.dead)
				_target = _home;

			if(_mobScript.InCombat)
				_mobScript.InCombat = false;
		}

	//		Debug.LogError("-> Combat: " + _me.inCombat);
	}
}
