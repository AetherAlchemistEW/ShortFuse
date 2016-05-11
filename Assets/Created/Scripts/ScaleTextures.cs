using UnityEngine;
using System.Collections;

public class ScaleTextures : MonoBehaviour 
{
	public float magnitude;
	// Use this for initialization
	void Start () 
	{
		renderer.material.mainTextureScale = new Vector2(transform.localScale.x*magnitude, transform.localScale.y*magnitude);
		renderer.material.SetTextureScale("_BumpMap", new Vector2(transform.localScale.x*magnitude, transform.localScale.y*magnitude));
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
