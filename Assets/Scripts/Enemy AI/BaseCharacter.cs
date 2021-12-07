using UnityEngine;
using System.Collections;
using System;			//added to acces the 'enum' class

public class BaseCharacter : MonoBehaviour 
{
	public GameObject weaponMount;
	public GameObject offHandMount;

	public GameObject characterMaterialMesh;
	public GameObject hairMount;
	public GameObject helmetMount;

	private string _name;
	private int _level;
	private uint _freeExp;



	public bool _inCombat;

	public float meleeAttackTimer = 20;
	public float meleeResetTimer = 0f;

	//public float meleeAttackSpeed = 1;


	public virtual void Awake()
	{			
		_name = string.Empty;
		_level = 0;
		_freeExp = 0;
		_inCombat = false;
	}


	//Setters and Getters
	public string Name
	{
		get{ return _name; }
		set{ _name = value; }
	}

	public bool InCombat
	{
		get{ return _inCombat; }
		set{ _inCombat = value; }
	}

	public int Level
	{
		get{ return _level; }
		set{ _level = value; }
	}


	public uint FreeExp
	{
		get{ return _freeExp; }
		set{ _freeExp = value; }
	}
	//end Getters Setters



	public void AddExp(uint exp)
	{
		_freeExp += exp;
		CalculateLevel();
	}


	public void CalculateLevel()
	{

	}




	//public Item EquipedWeapon
	//{
	//	get { return _equipment[(int)EquipmentSlot.MainHand]; }
	//	set {
	//		_equipment[(int)EquipmentSlot.MainHand] = value;
	//		//_equipedWeapon = value;
	//		
	//		if(weaponMount.transform.childCount > 0)
	//			Destroy(weaponMount.transform.GetChild(0).gameObject);
	//		//if(_equipedWeapon != null)
	//		
	//		if(_equipment[(int)EquipmentSlot.MainHand] != null) 
	//		{
	//			GameObject mesh = Instantiate(Resources.Load(GameSettings2.MELEE_WEAPON_MESH_PATH + _equipment[(int)EquipmentSlot.MainHand].Name), weaponMount.transform.position,weaponMount.transform.rotation) as GameObject;
	//			mesh.transform.localScale = new Vector3(mesh.transform.localScale.x * 3, mesh.transform.localScale.y * 3,mesh.transform.localScale.z * 3);
	//			mesh.transform.parent = weaponMount.transform;
	//		}
	//	}
	//}


	public void CalculateMeleeAttackSpeed()
	{

	}



}
