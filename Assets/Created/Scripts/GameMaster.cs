using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour 
{
	public Bomb interactingBomb;
	private bool bGameRunning = false;
	public Transform slab;
	public Transform leftBounds;
	public Transform rightBounds;
	public Transform topBounds;
	public Transform bottomBounds;
	private bool Active = false;
	Vector2 pos1 = new Vector2(0,0);
	Vector2 pos2 = new Vector2(0,0);
	Vector2 dir = new Vector2(0,0);

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!bGameRunning) //game isn't running, preparation phase
		{
			//click on the bomb, select it
			if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0))
			{
				Ray ray = camera.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit))
				{
					//Debug.Log (hit.transform.name);
					if(hit.transform.CompareTag("Bomb"))
					{
						//Debug.Log("hitting");
						interactingBomb = hit.transform.GetComponent<Bomb>();
						interactingBomb.bIsActive = true;
					}
				}
			}
			else if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved && interactingBomb != null || Input.GetMouseButton(0) && interactingBomb != null) //dragging to set fuse
			{
				Ray ray = camera.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit))
				{
					Vector3 tapPos = new Vector3(hit.point.x, 0, hit.point.z);
					interactingBomb.SendMessage("CalcFuseLength", tapPos);
				}
			}
			else if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && interactingBomb != null ||Input.GetMouseButtonUp(0) && interactingBomb != null)	//release, deselect bomb
			{
				interactingBomb.bIsActive = false;
				interactingBomb = null;
			}

			//camera controls. 
			//if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer)
			//{
#if UNITY_STANDALONE
			if(Input.mousePosition.x < Screen.width/10*9 && transform.position.x > leftBounds.position.x)
			{
				transform.position += new Vector3(-0.2f,0,0);
			}
			if(Input.mousePosition.x > Screen.width/10 && transform.position.x < rightBounds.position.x)
			{
				transform.position += new Vector3(0.2f,0,0);
			}
			if(Input.mousePosition.y > Screen.height/6*5 && transform.position.z < topBounds.position.z)
			{
				transform.position += new Vector3(0,0,0.2f);
			}
			if(Input.mousePosition.y < Screen.height/6 && transform.position.z > bottomBounds.position.z)
			{
				transform.position += new Vector3(0,0,-0.2f);
			}
#endif
			//}
#if UNITY_ANDROID 
			//or ios
			/*		
			//Acceleration style panning.
			if(Input.acceleration.x < -0.2f && transform.position.x > leftBounds.position.x)
			{
				transform.position += new Vector3(-0.1f*Input.acceleration.normalized.magnitude,0,0);
			}
			if(Input.acceleration.x > 0.2f && transform.position.x < rightBounds.position.x)
			{
				transform.position += new Vector3(0.1f*Input.acceleration.normalized.magnitude,0,0);
			}
			if(Input.acceleration.y > 0.2f && transform.position.z < topBounds.position.z)
			{
				transform.position += new Vector3(0,0,0.1f*Input.acceleration.normalized.magnitude);
			}
			if(Input.acceleration.y < -0.2f && transform.position.z > bottomBounds.position.z)
			{
				transform.position += new Vector3(0,0,-0.1f*Input.acceleration.normalized.magnitude);
			}
			*/
			//Two finger drag style panning.
			if(interactingBomb == null && Input.touchCount > 1)
			{
				if(Input.GetTouch(1).phase == TouchPhase.Began)
				{
					pos1 = Input.GetTouch(0).position;
					pos2 = Input.GetTouch(1).position;
					dir = (pos2-pos1).normalized;
				}
				if(Input.GetTouch(1).phase == TouchPhase.Stationary && Input.GetTouch(0).phase == TouchPhase.Stationary)
				{
					pos1 = Input.GetTouch(0).position;
					pos2 = Input.GetTouch(1).position;
					dir = (pos2-pos1).normalized;

					if(transform.position.x > rightBounds.position.x)
					{
						//transform.position -= new Vector3(0.1f,0,0);
					}
					else if(transform.position.x < leftBounds.position.x)
					{
						//transform.position += new Vector3(0.1f,0,0);
					}
					else if(transform.position.y > topBounds.position.y)
					{
						//transform.position -= new Vector3(0,0,0.1f);
					}
					else if(transform.position.y < bottomBounds.position.y)
					{
						//transform.position += new Vector3(0,0,0.1f);
					}
					//else
					//{
						transform.position += new Vector3(dir.x*0.1f,0,dir.y*0.1f);
					//}
				}
			}
#endif
			//Debug.Log(slab.rigidbody.velocity.magnitude);
		}
		else
		{
			transform.position = new Vector3(slab.position.x,transform.position.y,slab.position.z);
			//temporary restart condition
			if(slab.rigidbody.velocity.magnitude > 1)
			{
				Active = true;
			}
			if(Active)
			{
				Debug.Log(GameObject.FindGameObjectsWithTag("Bomb").Length);
				//Debug.Log(slab.rigidbody.velocity.magnitude);
				if(slab.position.y < -5)
				{
					Application.LoadLevel(Application.loadedLevel);
				}
			}
		}
	}

	void OnGUI()
	{
		//button to start the game.
		if(!bGameRunning)
		{
			if(GUI.Button (new Rect(Screen.width/2-Screen.width/10,0,Screen.width/5,Screen.height/15),"Ignite"))
			{
				foreach (GameObject charge in GameObject.FindGameObjectsWithTag("Bomb"))
				{
					charge.GetComponent<Bomb>().bIsLit = true;
					bGameRunning = true;
				}
			}
		}
		else if(slab.rigidbody.velocity.magnitude < 0.1f && Active)
		{
			if(GUI.Button (new Rect(Screen.width/2-Screen.width/10,0,Screen.width/5,Screen.height/15),"Restart"))
			{
				Application.LoadLevel(Application.loadedLevel);
			}
		}
	}
}
