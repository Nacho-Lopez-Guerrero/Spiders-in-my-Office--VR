#pragma strict

var open : boolean = false;

var openAnimationString : String;
var closeAnimationString : String;

var buttonTransform : Transform;
var distToOpen : float = 4;

@HideInInspector
var playerTransform : Transform;
@HideInInspector
var cameraTransform : Transform;

var openSound : AudioClip;
var closeSound : AudioClip;
var openDoorHUDText : TextMesh;

function Awake ()
{
	playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
	cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
	if(open)
		animation.Play(openAnimationString);
}

function Start () {

}

function Update () 
{
	if(playerTransform != null)
	{
		var alreadyChecked : boolean = false;
		var angle : float = Vector3.Angle(buttonTransform.position - cameraTransform.position, buttonTransform.position + (cameraTransform.right * buttonTransform.localScale.magnitude) - cameraTransform.position);
		if(Vector3.Distance(playerTransform.position, buttonTransform.position) <= distToOpen)
		{
			if(Vector3.Angle(buttonTransform.position - cameraTransform.position, cameraTransform.forward) < angle)
			{
				openDoorHUDText.active = true;
				if(Input.GetButton("Use") && !animation.isPlaying)
					{
						if(open)
						{
							openDoorHUDText.text = "Press  'A' on the pad  to   open   door";

							animation.Play(closeAnimationString);
							open = false;
							alreadyChecked = true;
							if(closeSound)
								audio.PlayOneShot(closeSound);
						}
						if(!open && !alreadyChecked)
						{
							openDoorHUDText.text = "Press  'A' on the pad  to   close   door";
							animation.Play(openAnimationString);
							open = true;
							if(openSound)
								audio.PlayOneShot(openSound);
						}
				}
			}
		}
		else if(openDoorHUDText)
				openDoorHUDText.active = false;

	}
}