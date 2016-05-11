using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour 
{
	public Vector3 blastDirection;
	public float fuseLength;
	//private float fuseTimer; (redundant?)
	public ParticleSystem sparks;
	public ParticleSystem explosion;
	private LineRenderer fuseRenderer;
	public Rigidbody slab;
	public bool bIsActive = false;
	public bool bIsLit = false;
	private Vector3 screenPos;
	private Vector3 newPos;
	public Vector3[] FusePositions;

	//audio

	// Use this for initialization
	void Start () 
	{
		//assign private variables
		sparks = GetComponentInChildren<ParticleSystem>();
		//slab = GetComponentInParent<Rigidbody>();
		fuseRenderer = GetComponent<LineRenderer>();
		fuseRenderer.SetPosition(0, transform.position);
	}
	
	// Update is called once per frame
	void Update () 
	{
		//if it's lit, child to the slab and run the fuse, when the fuse runs out, apply the force and unlight.
		if(bIsLit)
		{
			if(transform.parent != slab.transform)
			{
				transform.parent = slab.transform;
			}
			//fuseRenderer.SetPosition(0, transform.position);
			//fuse timer will initially be set to the fuse length
			if(fuseLength > 0)
			{
				fuseLength-=Time.smoothDeltaTime;
				sparks.enableEmission = true;
				//calculate fuse end position then set sparks there too
				//sparks.transform.position;
				//newPos = (newPos-transform.position).normalized * (fuseLength/2) + transform.position;
				FuseForm();
				//fuseRenderer.SetPosition(1,newPos);
				sparks.transform.position = FusePositions[FusePositions.Length-1];
				if(fuseLength <= 0)
				{
					slab.AddForce(blastDirection * 200);
					fuseRenderer.enabled = false;
					renderer.enabled = false;
					sparks.enableEmission = false;
					bIsLit = false;
				}
			}
			else
			{
				fuseRenderer.enabled = false;
				bIsLit = false;
				Destroy(this.gameObject);
			}
		}
	}

	void OnGUI()
	{
		if(fuseLength > 0)
		{
			screenPos = Camera.main.WorldToScreenPoint(transform.position);
			//display timer if dragging fuse (bIsActive)
			//if(bIsActive)
			//{
				GUI.Label(new Rect(screenPos.x,(Screen.height-screenPos.y)-10,20,20), Mathf.Round(fuseLength).ToString());
			//}
		}
	}

	void CalcFuseLength(Vector3 TapPos)
	{
		fuseLength = Vector3.Distance(transform.position, TapPos) * 2;
		fuseLength = Mathf.Round(fuseLength);
		fuseRenderer.SetPosition(1, TapPos);
		newPos = TapPos;
		FusePositions[0] = transform.position;
		FusePositions[3] = newPos;
		FusePositions[1] = (newPos-transform.position).normalized * ((fuseLength/2)/3) + transform.position;
		FusePositions[2] = (newPos-transform.position).normalized * (((fuseLength/2)/3)*2) + transform.position;
		for(int i = 0; i < FusePositions.Length; i++)
		{
			fuseRenderer.SetPosition(i, FusePositions[i]);
		}
	}

	void FuseForm()
	{
		FusePositions[0] = transform.position;
		FusePositions[1] = (FusePositions[1]-FusePositions[0]).normalized * ((fuseLength/2)/(FusePositions.Length-1)) + transform.position;
		FusePositions[2] = (FusePositions[2]-FusePositions[1]).normalized * ((fuseLength/2)/(FusePositions.Length-1)) + FusePositions[1];
		FusePositions[3] = (FusePositions[3]-FusePositions[2]).normalized * ((fuseLength/2)/(FusePositions.Length-1)) + FusePositions[2];
		for(int i = 0; i < FusePositions.Length; i++)
		{
			fuseRenderer.SetPosition(i, FusePositions[i]);
		}
	}
}
