using UnityEngine;
using System.Collections;

public class Interactable : MonoBehaviour 
{
	public enum Type
	{
		WinZone, ConveyerBelt, Magnet
	}
	public Type type;
	//the list of constants I'm using as type specifiers
	private Transform slab;
	public Vector3 conveyerDirection;
	public int Polarity;
	// Use this for initialization
	void Start () 
	{
		slab = GameObject.Find("Slab").transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(type == Type.ConveyerBelt)	//scroll the texture in our designated direction
		{
			renderer.material.mainTextureOffset += new Vector2(conveyerDirection.x * Time.smoothDeltaTime, conveyerDirection.z * Time.smoothDeltaTime) ;
		}

		if(type == Type.Magnet) //draw the player in
		{
			if(Vector3.Distance(slab.position, transform.position) < 5)
			{
				slab.rigidbody.AddRelativeForce((slab.position - transform.position)*Polarity);
			}
		}
	}

	void OnTriggerEnter(Collider hit)
	{
		if(hit.transform == slab)
		{
			if(type == Type.WinZone)	//temporary win condition
			{
				slab.position = transform.position;
				Debug.Log("WIN");
				Application.LoadLevel(Application.loadedLevel+1);
			}
		}
	}

	void OnTriggerStay(Collider hit)
	{
		if(hit.transform == slab)
		{
			if(type == Type.ConveyerBelt)	//slide the slab in our designated direction
			{
				slab.position += conveyerDirection * Time.smoothDeltaTime;
			}
		}
	}
}
