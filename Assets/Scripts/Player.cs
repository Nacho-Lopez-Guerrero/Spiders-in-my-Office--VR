using UnityEngine;
using System.Collections;

public class Player : Entity {

	private int _level;
	private float _currentLevelExperience;
	private float _experienceToLevel;

	private GameGUI _gui;
	private Animator _animator;

	public GameObject deathSplash;
	public string initialGunName;

	//public AudioSource[] aSources;

	void Start()
	{
		_gui = GameObject.FindGameObjectWithTag("GUI").GetComponent<GameGUI>();
		_animator = GetComponent<Animator>();

		_gui.SetHealthInfo(100, (int)health);
		_gui.SetWeaponIcon(initialGunName);

		//aSources = GetComponents<AudioSource>();

		LevelUp();
	}

	public void AddExperience(float exp)
	{
		_currentLevelExperience += exp;

		if(_currentLevelExperience >= _experienceToLevel)
		{
			_currentLevelExperience -= _experienceToLevel;  
			LevelUp();
		}


		_gui.SetPlayerExperience(_currentLevelExperience/_experienceToLevel, _level);
	}

	public void LevelUp()
	{
		_level++;
		_experienceToLevel = _level * 50 + Mathf.Pow(_level * 2,2);

		AddExperience(0);
	}	

	public void OnTriggerEnter(Collider	other)
	{
		if(other.gameObject.CompareTag("EnemyWeapon") )
		{
			EnemyWeapon enemyWeapon = other.gameObject.GetComponent<EnemyWeapon>();

		//	enemyWeapon.UnderAttack = true;

			if(enemyWeapon.attackTimer <= 0)
			{
				TakeDamage(other.gameObject.GetComponent<EnemyWeapon>().weapondamage);
				Instantiate(blood, other.transform.position, Quaternion.Inverse(other.transform.localRotation));
				_gui.SetHealthInfo(100, (int)health);

				enemyWeapon.ResetAttackTimer();

				if(!_dead)
					aSources[1].Play();

			}
		}
	}

	public void OntriggerExit(Collider	other)
	{
		//if(other.gameObject.CompareTag("EnemyWeapon") )
			//other.gameObject.GetComponent<EnemyWeapon>().UnderAttack = false;
	}

	public void KillMe()
	{
		Instantiate(deathSplash, transform.position, Quaternion.LookRotation(Vector3.up));
		_gui.ShowDeadMessage();
		Destroy(gameObject);
	}

}
