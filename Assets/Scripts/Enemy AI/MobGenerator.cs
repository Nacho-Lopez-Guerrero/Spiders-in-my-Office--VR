using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MobGenerator : MonoBehaviour {

	public enum State 
	{
		Idle,
		Initialize,
		Setup,
		SpawnMobs
	}


	public GameObject[] mobPrefabs;				//An array to hold all of the prefabs of mobs we want to spawn
	public GameObject[] spawnPoints;			//This array will hold a reference to all the spawnpoints in the scene
	public GameObject enemySpawnEffect;			//This array will hold a reference to all the spawnpoints in the scene

	public State state;							//This is our local variable that holds our current state

	private GameGUI _gui;
	private int _waveNumber;
	private int _enemiesLeft;

	private bool _newWave;

	void Awake()
	{
		state = MobGenerator.State.Initialize;
		_gui = GameObject.FindGameObjectWithTag("GUI").GetComponent<GameGUI>();
		_waveNumber = 1;
		_newWave = false;
		_gui.SetWaveInfo(_waveNumber);
		_enemiesLeft = spawnPoints.Length;
		_gui.SetEnemiesLeftInfo(_enemiesLeft, _enemiesLeft);
	}


	// Use this for initialization
	IEnumerator Start () 
	{
		while(true)
		{
			switch(state)
			{
			case State.Initialize:
				Initialize();
				break;
			case State.Setup:
				Setup();
				break;
			case State.SpawnMobs:
				SpawnMob();
				break;


			}

			yield return 0;					//IEnumerator
		}
	}


	// Update is called once per frame
	void LateUpdate () {
		//CheckEnemiesLeft();
	}


	//Make sure everything is initialized before going to the next step
	private void Initialize()
	{
		Debug.Log("****We are in Initialize() function!!****");

		if(!checkForMobPrefabs() || !checkForSpawnPoints())
			return;

		state = MobGenerator.State.Setup;
	}


	private void Setup()
	{
		Debug.Log("****We are in Setup() function!!****");

		state = MobGenerator.State.SpawnMobs;

	}


	private void SpawnMob()
	{
		Debug.Log("****Spawn Mob function ****");

		GameObject[] freeSpawnPoints = AvailableSpawnPoints();


		for(int cnt = 0; cnt < freeSpawnPoints.Length; cnt ++)
		{
			GameObject go = Instantiate(mobPrefabs[Random.Range(0,mobPrefabs.Length)], 
			                            freeSpawnPoints[cnt].transform.position, 
			                            Quaternion.identity) as GameObject;

			GameObject spawnEffect = Instantiate(enemySpawnEffect, 
			                                     freeSpawnPoints[cnt].transform.position, 
			                            Quaternion.identity) as GameObject;

			go.transform.parent = freeSpawnPoints[cnt].transform;
			spawnEffect.transform.parent = go.transform.GetChild(0).transform.GetChild(0).transform;
			spawnEffect.transform.localPosition = Vector3.zero;

		}

		state = MobGenerator.State.Idle;
	}


	//Check if we have at least one mob prefab to spawn
	private bool checkForMobPrefabs()
	{
		if(mobPrefabs.Length > 0)
			return true;
		else 
			return false;
	}


	//check to see if we have at leaast one spawnpoint
	private bool checkForSpawnPoints()
	{
		if(spawnPoints.Length > 0)
		   	return true;
		else 
			return false;
	}


	//generate a list of available spawnpoints that do not have any mobs childed to it
	private GameObject[] AvailableSpawnPoints()
	{
		List<GameObject> gos = new List<GameObject>();
		
		if(_newWave)
		{
			_waveNumber += 1;
			GameObject[] temp = new GameObject[2 * spawnPoints.Length];


			for(int i = 0; i < temp.Length; i++)
			{
				if(i < spawnPoints.Length)
					temp[i] = spawnPoints[i];
				else
				{
					temp[i] = new GameObject();
					temp[i].transform.parent = transform;
					temp[i].name = "Spawn Point";
					temp[i].transform.position = spawnPoints[i - spawnPoints.Length].transform.position;

					Vector3 tempPosition = temp[i].transform.position;
					tempPosition.x += Random.Range(-3, 3);
					tempPosition.z += Random.Range(-3, 3);

					temp[i].transform.position = tempPosition;
				}

			}

			spawnPoints = temp;

			_enemiesLeft = spawnPoints.Length;

			_gui.SetEnemiesLeftInfo(_enemiesLeft, spawnPoints.Length);
			_gui.SetWaveInfo(_waveNumber);

			_newWave = false;
		}

		for(int cnt = 0; cnt < spawnPoints.Length; cnt ++)
		{
			if(spawnPoints[cnt].transform.childCount == 0)
			{
//				Debug.Log("**** SpawnPOint Available ****");
				gos.Add(spawnPoints[cnt]);
			}
		}

		return gos.ToArray();
	}

	public void CheckEnemiesLeft()
	{
		if(_enemiesLeft == 0)
		{
			_newWave = true;

			//Si hay tantos spawn libres como spawn point -> Habran muerto todos los malos
			SpawnMob();
		}


	}

	public void RefreshEnemiesLeft()
	{
		_enemiesLeft -= 1;
		_gui.SetEnemiesLeftInfo(_enemiesLeft, spawnPoints.Length);
	}

}
