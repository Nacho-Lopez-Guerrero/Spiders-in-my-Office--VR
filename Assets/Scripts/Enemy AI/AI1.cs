using UnityEngine;
using System.Collections;

//[RequireComponent (typeof(AdvancedMovement))]
[RequireComponent (typeof(SphereCollider))]
public class AI1 : MonoBehaviour 
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

	public Transform _target;
	public float runDistance = 20;
	
	private Transform _myTransform;
	private Transform _home;

	private const float ROTATION_DAMP = 0.03f;
	private const float FORWARD_DAMP = 0.9f;

	private State _state;

	private SphereCollider _sphereCollider;

	public AdvancedMovement _advancedMovement;
	public Mob _mobScript;

	public float baseMeleeRange = 3;

	public GameObject _weaponMount;

	void Awake()
	{
		//_advancedMovement = gameObject.GetComponent<AdvancedMovement>();
		//_mobScript = gameObject.GetComponent<Mob>();

	}


	void Start()
	{
		_state = AI1.State.Init;
		StartCoroutine("FSM");
	}


	IEnumerator FSM()										//FSM = Finite State Machine
	{
//		Debug.LogWarning("Combat: " + _me.inCombat);

		while(_state != AI1.State.Idle)
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
//		Debug.Log("Init");

		_home = transform.parent.transform.parent.transform.parent.transform.parent.transform;
		_myTransform = transform;

		_sphereCollider = GetComponent<SphereCollider>();
		if(_sphereCollider == null)
		{
			Debug.LogError("SphereCollider not present");
			return;
		}

		_state = AI1.State.Setup;
	}


	private void Setup()
	{
	//	Debug.Log("****Setup****");

		_sphereCollider.center = _advancedMovement.gameObject.GetComponent<CharacterController>().center;
		_sphereCollider.isTrigger = true;
		_state = AI1.State.Idle;
	}


	private void Search()
	{
		//Debug.Log("****Search****");

		if(_target == null)
		{
			_state = AI1.State.Idle;

			if(_mobScript.InCombat)
				_mobScript.InCombat = false;
		}
		else
		{
			if(!_mobScript.InCombat)
				_mobScript.InCombat = true;

			_state = AI1.State.Decide;	
		}
	}


	private void Decide()
	{
		//Debug.Log("****Decide****");

		Move();

		//Create a routine to decide what to do with the targeted player
		int opt = -1;
		if(_target != null && _target.CompareTag("Player"))
		{				
			//Debug.Log("Melee Reset Timer: " + _mobScript.meleeResetTimer);
//			Debug.Log("Player distancee: " + Vector3.Distance(_advancedMovement.gameObject.transform.position, _target.position));

			if(Vector3.Distance(_advancedMovement.gameObject.transform.position, _target.position) < baseMeleeRange)
			{
				_mobScript.meleeResetTimer -= Time.deltaTime;

				if(_mobScript.meleeResetTimer <= 0)
				{		
					Debug.Log("In melee range");
					//opt = Random.Range(0, 3);
					opt = 0;
				}
			}
			else
			{
				//if(_mobScript.meleeResetTimer > 0)
				//	_mobScript.meleeResetTimer -= Time.deltaTime;


				_mobScript.meleeResetTimer = _mobScript.meleeAttackTimer;


				//_weaponMount.GetComponent<SphereCollider>().enabled = false;

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
				//Debug.Log("Option: " + opt + " not defined");
				break;
			}
			
		}

		_state = AI1.State.Search;
	}


	private void MeleeAttack()
	{





		//decide if we hit or not
		if(true)
		{
			_advancedMovement.gameObject.SendMessage("PlayMeleeAttack");
		}
		_mobScript.meleeResetTimer = _mobScript.meleeAttackTimer;

		
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
		_state = AI1.State.Retreat;
	}

	private void Flee()
	{
//		Debug.Log("****Flee****");

		Move();
//		_alive = false;

		_state = AI1.State.Search;
	}


	private void Retreat()
	{
	//	Debug.Log("****Retreat****");
		_myTransform.LookAt(_target);
		Move();
		_state = AI1.State.Search;
	}


	private void Move()
	{
//		Debug.LogWarning("Combat: " + _me.inCombat);

		if(_target)
		{				
			float dist = Vector3.Distance(_target.position, _advancedMovement.gameObject.transform.position);

			if(_target.name == "SpawnPoint")
			{
				if(dist < baseMeleeRange + 2)
				{	
					_state = AI1.State.Idle;
					_target = null;

					_advancedMovement.gameObject.SendMessage("MoveMeForward", AdvancedMovement.Forward.none);
					_advancedMovement.gameObject.SendMessage("RotateMe", AdvancedMovement.Turn.none);
					return;
				}
			}


			Quaternion rot = Quaternion.LookRotation(_target.transform.position - _myTransform.position);
			_advancedMovement.gameObject.transform.rotation = Quaternion.Slerp(_advancedMovement.gameObject.transform.rotation, rot, Time.deltaTime * 7);


			Vector3 dir = (_target.position - _advancedMovement.gameObject.transform.position).normalized;
			float direction = Vector3.Dot(dir, _advancedMovement.gameObject.transform.forward);										//Deveuelve un valor entre 1 y -1 dependeindo de la posicion realtiva de los dos vectores

			//Debug.Log("Distancia de jugador: " + dist + "|" + "BaseMeleeRange: " + baseMeleeRange);

			if(direction > FORWARD_DAMP && dist > baseMeleeRange)
			{
				if(dist > runDistance)
					_advancedMovement.gameObject.SendMessage("ActivateRun", AdvancedMovement.Forward.none);											
				else
					_advancedMovement.gameObject.SendMessage("DeActivateRun", AdvancedMovement.Forward.none);											
				
				_advancedMovement.gameObject.SendMessage("MoveMeForward", AdvancedMovement.Forward.forward);								//Sends a message to all monobehaviour scripts that are ATTACHED to this gameObject

				_weaponMount.GetComponent<SphereCollider>().enabled = false;
			}
			else    //Si esta cerca del player en MeleeRange
			{
				//_advancedMovement.GetComponent<Animation>().Stop("Walk");
				_weaponMount.GetComponent<SphereCollider>().enabled = true;

				_advancedMovement.gameObject.SendMessage("MoveMeForward", AdvancedMovement.Forward.none);
				//_advancedMovement.gameObject.SendMessage("StrafeMe", AdvancedMovement.Forward.none);
			}
		}
	}

	public Transform Target
	{
		get{ return _target; }
		set{ _target = value; }
	}

	public void StartAttacking(Transform target)
	{	

		if(!_advancedMovement.dead)
			_target = target;

		_state = AI1.State.Search;

		StartCoroutine("FSM");
	}

	public void OnTriggerEnter(Collider other)
	{
		//Debug.Log("PlayerDetected!!");
		if(other.CompareTag("Player"))
		{	
			if(!_advancedMovement.dead)
				_target = other.transform;
			//PC.Instance.InCombat = true;
			_state = AI1.State.Search;
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
