using UnityEngine;
using System.Collections;

public class Shell : MonoBehaviour {

	private float lifetime = 5f;

	private Material mat;
	private Color originalCol;
	private float fadePercent;
	private float deathTime;
	private bool fading;


	// Use this for initialization
	void Start () 
	{
		mat = renderer.material;
		originalCol = mat.color;
		deathTime = Time.time + lifetime;

		StartCoroutine("Fade");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator Fade()
	{
		while(true)
		{
			yield return new WaitForSeconds(.1f);
			if(fading)
			{
				fadePercent += Time.deltaTime;
				mat.color = Color.Lerp(originalCol, Color.clear, fadePercent);

				if(fadePercent >= 1)
				{
					Destroy(gameObject);
				}
			}
			if(Time.time > deathTime)
			{
				fading = true;
			}
		}
	}

	void OnTriggerEnter(Collider c)
	{
		if(c.tag == "Ground")
		{
			//Debug.Log("<color=red>Fatal error:</color> AssetBundle not found");
			rigidbody.Sleep();
			Vector3 temp = transform.position;
			temp.y += 0.04f;
			transform.position = temp;
		}

	}
}
