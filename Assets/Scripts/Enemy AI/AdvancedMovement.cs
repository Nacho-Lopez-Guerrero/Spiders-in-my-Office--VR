using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterController))]				//Requires a ChracterController to the GameObject this script is attached to
public class AdvancedMovement : MonoBehaviour 
{
	public enum Turn
	{
		left = -1,
		none = 0,
		right = 1
	}


	public enum Forward
	{
		back = -1,
		none = 0,
		forward = 1
	}


	public enum State
	{
		Idle,
		Init,
		Setup,
		Run
	}


	public float rotateSpeed = 250;
	public float walkSpeed = .1f;
	public float strafeSpeed = .01f;
	public float runMultiplier = 4;
	public float gravity = 5;
	public float airtime = 0;									//How long we have been in the air
	public float fallTime = 0.5f;								//Time we have to be falling before the system knows it's a fall
	public float jumpHeight = 1.75f;
	public float jumpTime = 2;

	public CollisionFlags _collisionFlags;

	public AnimationClip meleeAttack;
	public AnimationClip combatIdle;

	private Transform _myTransform;
	private CharacterController _controller;
	private Vector3 _moveDirection;

	private Turn _turn;
	private Turn _strafe;
	private Forward _forward;
	private bool _run;
	private bool _jump;
	private bool _isSwimming;
	
	private State _state;
	private BaseCharacter _bc;

	public bool dead = false;

	public AI1 aiScript;


	void Awake()
	{
		_myTransform = transform;
		_bc = gameObject.GetComponent<BaseCharacter>();
		_controller = GetComponent<CharacterController>();
		_state = State.Init;
	}


	// Use this for initialization
	IEnumerator Start () 
	{
		while(true)
		{
			switch(_state)
			{
				case State.Init:
					Init();
					break;
				case State.Setup:
					Setup();
					break;
				case State.Run:
					ActionPicker();
					break;
			}
			yield return null;
		}
	}


	void Update()
	{
	}


	private void Init()
	{
		if(!GetComponent<CharacterController>()) return;
		if(!GetComponent<Animation>()) return;

		_state = AdvancedMovement.State.Setup;
	}


	private void Setup()
	{
		animation.Stop();
		
		animation.wrapMode = WrapMode.Loop;
		animation["Jump"].layer = 1;							//Layer = Crossfades priority
		animation["Jump"].wrapMode = WrapMode.Once;
		animation["death"].wrapMode = WrapMode.Once;

		//animation.Play("idle");

		_moveDirection = Vector3.zero;
		_turn = Turn.none;
		_forward = Forward.none;
		_run = false;
		_strafe = Turn.none;
		_jump = false;
		_isSwimming = false;
		_state = AdvancedMovement.State.Run;
	}


	private void ActionPicker()
	{
		//_myTransform.Rotate(0, (int)_turn * Time.deltaTime * rotateSpeed, 0);

		
		if((_controller.isGrounded || _isSwimming) && !dead)
		{
//			Debug.Log("**** On the ground ****");

		//	Debug.Log("Forward: " + _forward);
		//	Debug.Log("State: " + _state);
			airtime = 0;
			
			_moveDirection = new Vector3((int)_strafe, 0, (int)_forward);
			_moveDirection = _myTransform.TransformDirection(_moveDirection).normalized;
			_moveDirection *= walkSpeed;
			
			//Walk & Run
			if(_forward != Forward.none)
			{
				if(_isSwimming)
				{
					Swim();
				}
				else if(_run)
				{
					_moveDirection *= runMultiplier;
					Run();
				}
				else
				{
					Walk();
					//Debug.Log("Walking");

				}
			}
			else if(_strafe != Turn.none)
			{
				Strafe();
			}
			else
			{
//				Debug.Log("Idle");

				Idle();
			}
			//Jump
			if(_jump)
			{
				if(airtime < jumpTime)
				{
					_moveDirection.y += jumpHeight;
					Jump();
					_jump = false;
				}
			}
		}
		else
		{
//			Debug.Log("**** Not on the ground ****");
			
			if((_collisionFlags & CollisionFlags.CollidedBelow) == 0)
			{
				airtime += Time.deltaTime;
				
				if(airtime > fallTime)
				{
					Fall();
				}
			}
		}

		if(!_isSwimming)
			_moveDirection.y -= gravity * Time.deltaTime;

		if(dead)
		{
			_moveDirection.x = 0;
			_moveDirection.y = 0;
			_moveDirection.z = 0;

		}

		
		if(_controller.enabled)
			_collisionFlags = _controller.Move(_moveDirection);

	}


	public void MoveMeForward(Forward z)
	{
		_forward = z;
	}


	public void ToggleRun()
	{
		_run = !_run;
	}


	public void ActivateRun()
	{
		_run = true;
	}


	public void DeActivateRun()
	{
		_run = false;
	}


	public void RotateMe(Turn y)
	{
		_turn = y;
	}


	public void StrafeMe(Turn x)
	{
		_strafe = x;
	}


	public void JumpMe()
	{
		_jump = true;
	}


	public void SwimMe(bool swim)
	{
		_isSwimming = swim;
	}

	public void KillMe()
	{
		dead = true;
		animation.CrossFade("death");
	}



	/// <summary>
	///	Below there are functions that play Animations 
	/// </summary>
	public void Fall()
	{
//		animation.CrossFade("Fall");
	}


	public void Idle()
	{
		animation.Stop("Walk");

		if(!_bc.InCombat)
			animation.CrossFade("idle");
		else
		{
			if(!animation.isPlaying)
				animation.Play(combatIdle.name);

		}
			//animation.Play(combatIdle.name);


	}

	public void Walk()
	{
		animation["Walk"].speed = 3f;
		animation.CrossFade("Walk");
	}


	public void Strafe()
	{
		animation.CrossFade("Strafe");
	}


	public void Jump()
	{
		animation["Jump"].speed = 3f;
		animation.CrossFade("Jump");
	}


	public void Attack()
	{
		int randAttack = Random.Range(0, 3);
		animation.CrossFade("Attack" + randAttack);
		Debug.Log("Attack" + randAttack);
	}


	public void PlayMeleeAttack()
	{	
		int randAttack = Random.Range(1, 4);   //[1,3]
		animation["Attack" + randAttack].wrapMode = WrapMode.Once;
		animation.CrossFade("Attack" + randAttack);
		Debug.Log("ANIMACION ATAUQE");

		//if(meleeAttack == null)
		//{
		//	Debug.LogWarning("We need a meleeAttack Animation for this mob!");
		//	animation.Play(meleeAttack.name);
	//		return;
		//}
//		Debug.LogWarning("MELEE ATTACK");

		//animation.Play(meleeAttack.name);
		
		//animation[meleeAttack.name].speed = animation[meleeAttack.name].length / 20f;
		//Debug.Log("Length: " + meleeAttack.length + " | Speed: " + animation[meleeAttack.name].speed );

	}

	public void Run()
	{
		animation["Run"].speed = 1.2f;
		animation.CrossFade("Run");
	}


	public void Swim()
	{	Debug.Log("Swimming");
		animation.CrossFade("swim");
	}

	public void StartAttacking(Transform target)
	{
		aiScript.StartAttacking(target);
	}
}
