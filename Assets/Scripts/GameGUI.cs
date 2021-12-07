using UnityEngine;
using System.Collections;

public class GameGUI : MonoBehaviour {

	public Transform experienceBar;

	public TextMesh levelText;
	public TextMesh ammoText;
	public TextMesh healthText;
	public TextMesh experienceLevelText;
	public TextMesh enemyWaveNumber;
	public TextMesh enemiesLeftNumber;
	public TextMesh deadMessage;

	public GameObject weaponIcon;

	private bool _playerDead = false;
	private float _deathTimer = 10;

	public void SetPlayerExperience(float percentToLevel, int playerLevel)
	{
		experienceBar.localScale = new Vector3(percentToLevel, 1, 1);
		levelText.text = "Level: " + playerLevel;

		experienceLevelText.text = (int)(playerLevel * 150 * percentToLevel) + "/" + playerLevel * 150 + " EXP";
	}

	public void SetAmmoInfo(int totalAmmo, int ammoInMag)
	{
		//if(totalAmmo == 0 && ammoInMag == 0)
		//	ammoText.color = Color.red;
		//else 

		if(ammoInMag == 0)
			ammoText.color = Color.red;
		else if(ammoInMag <= 5)
			ammoText.color = Color.yellow;
		else
			ammoText.color = Color.green;

		ammoText.text = ammoInMag + "/" + totalAmmo;
	}

	public void SetHealthInfo(int totalHealth, int currentHealth)
	{
		if(currentHealth >  49)
			healthText.color = Color.green;
		else if(currentHealth >  11)
			healthText.color = Color.yellow;
		else
			healthText.color = Color.red;

		healthText.text = currentHealth + "/" + totalHealth;
	}

	public void SetWeaponIcon(string weaponName)
	{
		weaponIcon.renderer.material.mainTexture = Resources.Load("Materials/HUD Textures/" + weaponName + "Icon", typeof(Texture2D)) as Texture2D;
	}

	public void SetWaveInfo(int waveNumber)
	{
		enemyWaveNumber.text = waveNumber + "/10";
	}

	public void SetEnemiesLeftInfo(int enemiesLeft, int totalEnemies)
	{
		enemiesLeftNumber.text = enemiesLeft + "/" + totalEnemies;
	}

	public void ShowDeadMessage()
	{
		deadMessage.active = true;
		_playerDead = true;
	}

	void Update()
	{
		if(_playerDead)
		{
			_deathTimer -= Time.deltaTime;
		
			if(_deathTimer < 0)
				Application.LoadLevel(0);	
		}
	}
}
