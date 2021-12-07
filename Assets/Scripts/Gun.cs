using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AudioSource))]
public class Gun : MonoBehaviour {

	public enum GunType {Semi, Burst ,Auto};

	public float gunID;
	public GunType gunType;
	public float fireRate;		//rpm, frecuencia (por minuto)
	public float damage = 1;
	public LayerMask collisionMask;

	public int totalAmmo = 40;
	public int ammoPerMag = 10;

	// System
	private float secondsBetweenShots;
	private float nextPossibleShootTime;
	private int _currentAmmoInMag;
	private bool _reloading;

	// Components
	public Transform bulletSpawn;
	public Transform bulletSpawn2;

	public Transform shellEjectionPoint;
	public Rigidbody shell;
	public LineRenderer tracer;

	public AudioSource[] aSources;
	public GameObject muzzleFlash;

	[HideInInspector]
	public GameGUI gui;

	void Start()
	{
		secondsBetweenShots = 60/fireRate;
		if(GetComponent<LineRenderer>())
		{
			tracer = GetComponent<LineRenderer>();
		}

		_currentAmmoInMag = ammoPerMag;

		if(gui)
			gui.SetAmmoInfo(totalAmmo, _currentAmmoInMag);

		aSources = GetComponents<AudioSource>();

	}

	public void Shoot()
	{
		Ray rayShotgun = new Ray();

		if (CanShoot()) 
		{
			if(bulletSpawn2 != null)
			{
				rayShotgun = new Ray(bulletSpawn2.position, bulletSpawn2.forward);
			
				GameObject muzzle2 = Instantiate(muzzleFlash, bulletSpawn2.position, transform.rotation) as GameObject;
				muzzle2.transform.parent = bulletSpawn.transform;
				Vector3 temp2 = muzzle2.transform.localPosition;
				temp2.z += 0.5f;
				muzzle2.transform.localPosition = temp2;
			}

			Ray ray = new Ray(bulletSpawn.position, bulletSpawn.forward);

			GameObject muzzle = Instantiate(muzzleFlash, bulletSpawn.position, transform.rotation) as GameObject;
			muzzle.transform.parent = bulletSpawn.transform;
			Vector3 temp = muzzle.transform.localPosition;
			temp.z += 0.5f;
			muzzle.transform.localPosition = temp;



			RaycastHit hit;
			RaycastHit hit2;

			float shotDistance = 160;

			if(Physics.Raycast(ray, out hit, shotDistance, collisionMask))		// Guarda en 'hit' el valor de la colision del raycast   (collisionMask = Character)
			{
				shotDistance = hit.distance;

				if(hit.collider.gameObject.GetComponent<Entity>())
				{
					Instantiate(hit.collider.gameObject.GetComponent<Entity>().blood, hit.transform.position, Quaternion.Inverse(hit.collider.gameObject.transform.rotation));

					if(!hit.collider.gameObject.GetComponent<AdvancedMovement>().dead)
					{
						hit.collider.gameObject.GetComponent<Entity>().aSources[1].Play();
						hit.collider.GetComponent<Entity>().TakeDamage(damage);

						//Warn enemy if he is shot on idle state
						if(hit.collider.gameObject.GetComponent<AdvancedMovement>().aiScript.Target == null || hit.collider.gameObject.GetComponent<AdvancedMovement>().aiScript.Target.name == "Spawn Point")
							hit.collider.gameObject.GetComponent<AdvancedMovement>().StartAttacking(GameObject.FindGameObjectWithTag("Player").transform);
					}
				}
			}

			if(bulletSpawn2 != null)
			{
				Debug.Log("Shotgun Hit");

				if(Physics.Raycast(rayShotgun, out hit2, 20, collisionMask))		// Guarda en 'hit' el valor de la colision del raycast
				{
					if(hit2.collider.gameObject.GetComponent<Entity>())
					{

						Instantiate(hit2.collider.gameObject.GetComponent<Entity>().blood, hit2.collider.transform.position, hit2.collider.gameObject.transform.rotation);

						if(!hit2.collider.gameObject.GetComponent<AdvancedMovement>().dead)
							hit2.collider.GetComponent<Entity>().TakeDamage(damage);

					}
				}
			}

			_currentAmmoInMag--;
			nextPossibleShootTime = Time.time + secondsBetweenShots;

			aSources[0].Play();

			if(tracer)
			{
				StartCoroutine("RenderTracer", ray.direction * shotDistance);
				//StartCoroutine("RenderTracer2", rayShotgun.direction * shotDistance);
			}

			Rigidbody newShell = Instantiate(shell, shellEjectionPoint.position, Quaternion.identity) as Rigidbody;			// Quaternion.identity = sin rotacion
			newShell.AddForce(shellEjectionPoint.forward * Random.Range(150f, 200f) + bulletSpawn.forward * Random.Range(-10f, 10f));
			//Debug.DrawRay(ray.origin, ray.direction * shotDistance, Color.red, 1);

			if(gui)
				gui.SetAmmoInfo(totalAmmo, _currentAmmoInMag);

		}
		if(_currentAmmoInMag == 0)
			aSources[1].Play();

	}


	public void ShootContinuos()
	{
		if(gunType == GunType.Auto)
		{
			Shoot ();
		}
	}

	public bool CanShoot()
	{
		bool canShoot = true;

		if(Time.time < nextPossibleShootTime)
			canShoot = false;


		if(_currentAmmoInMag == 0)
			canShoot = false;

		if(_reloading)
			canShoot = false;

		return canShoot;
	}

	public bool Reload()
	{
		if(totalAmmo != 0 && _currentAmmoInMag != ammoPerMag)
		{
			_reloading = true;
			return true;
		}
		else
			return false;
	}

	public void FinishReload()
	{
		_reloading = false;
		_currentAmmoInMag = ammoPerMag;
		totalAmmo -= ammoPerMag;

		if(totalAmmo < 0)
		{
			_currentAmmoInMag += totalAmmo;
			totalAmmo = 0;
		}

		if(gui)
			gui.SetAmmoInfo(totalAmmo, _currentAmmoInMag);
	}

	IEnumerator RenderTracer(Vector3 hitPoint)
	{
		tracer.enabled = true;
		tracer.SetPosition (0, bulletSpawn.position);
		tracer.SetPosition(1, bulletSpawn.position + hitPoint);

		yield return null;
		tracer.enabled = false;
	}

	IEnumerator RenderTracer2(Vector3 hitPoint)
	{
		Debug.Log("Render Tracer 2");
		tracer.enabled = true;
		tracer.SetPosition (0, bulletSpawn2.position);
		tracer.SetPosition(1, bulletSpawn2.position + hitPoint);
		yield return null;
		tracer.enabled = false;
	}


	// Update is called once per frame
	void Update () {
	
	}
}
