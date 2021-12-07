using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {

	public float health = 100;
	protected bool _dead = false;
	private float _deathTimer = 7;
	public float expOnDeath;

	public GameObject blood;
	public GameObject deathSplashBlood;

	private GameCamera _cameraScript;

	private Player _player;
	public AI1 _aiScript;

	public AudioSource[] aSources;

	public bool _isPlayer;

	private MobGenerator _mobGenerator;

	void Start()
	{
		_player  = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		_mobGenerator  = GameObject.FindGameObjectWithTag("MobGenerator").GetComponent<MobGenerator>();

		//_aiScript  = GetComponent<AI>();

		aSources = GetComponents<AudioSource>();
	}

	void Awake()
	{
		_cameraScript  = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameCamera>();
	}


	public virtual void TakeDamage(float dmg)				//virtual to allow override
	{
		health -= dmg;

		//Debug.Log(health);


		if(_cameraScript.combatZoom)
		{
			_cameraScript.ResetZoomTimer();
		}

		_cameraScript.combatZoom = true;

		if(health <= 0 && !_dead)
		{
			health = 0;

			if(!_isPlayer)
			{
				_aiScript.Target = null;
				_player.AddExperience(expOnDeath);
				_aiScript._weaponMount.GetComponent<SphereCollider>().enabled = false;
				GetComponent<CharacterController>().enabled = false;
				transform.parent.transform.parent.transform.parent = null;
				Instantiate(deathSplashBlood, new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z), transform.rotation);
				_mobGenerator.RefreshEnemiesLeft();
			}
			aSources[0].Play();
			//Debug.Log("Player Health: " + health);
			SendMessage("KillMe", AdvancedMovement.Forward.none);

			_dead = true;
		}
	}

	public virtual void Die()
	{

		if(!_isPlayer)
		{
			Destroy(transform.parent.transform.parent.gameObject);
			_mobGenerator.CheckEnemiesLeft();
		}
		else
			Application.LoadLevel(0);
	}

	void Update()
	{
		if(_dead)
			_deathTimer -= Time.deltaTime;
		if(_deathTimer < 0)
			Die();


				          
	}
}
