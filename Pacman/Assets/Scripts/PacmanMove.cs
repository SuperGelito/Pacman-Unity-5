using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PacmanMove : MonoBehaviour {
	//Set the velocity
	public float speed = 0.4f;
	//Is alive
	public bool isAlive;
	//Destination position
	Vector2 dest = Vector2.zero;
	//Radious of pacman
	public float radius = 0.9f;
	//Distance to move per action
	float distance = 0.45f;
	//Actions to move
	List<Vector2> route;
	#region Unity events
	// Use this for initialization
	void Start () {
		dest = transform.position;
		isAlive = true;
		route = new List<Vector2> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// Move closer to Destination
		Vector2 p = Vector2.MoveTowards(transform.position, dest, speed);
		GetComponent<Rigidbody2D>().MovePosition(p);

		// Check for Input if not moving
		if ((Vector2)transform.position == dest) {
			//Check for action to move
			if(route.Any())
			{
				Vector2 nextMov = route.First();
				route.Remove(route.First());
				Move (nextMov);
			}

			if (Input.GetKey(KeyCode.UpArrow))
				Move(Vector2.up);
			if (Input.GetKey(KeyCode.RightArrow))
				Move(Vector2.right);
			if (Input.GetKey(KeyCode.DownArrow))
				Move(-Vector2.up);
			if (Input.GetKey(KeyCode.LeftArrow))
				Move(-Vector2.right);
			 
		}

		//Check for animation to be used
		// Animation Parameters
		Vector2 dir = dest - (Vector2)transform.position;
		GetComponent<Animator>().SetFloat("DirX", dir.x);
		GetComponent<Animator>().SetFloat("DirY", dir.y);
	}
	#endregion

	#region Pacman movement functions
	public List<Vector2> GetDefaultMovements()
	{
		List<Vector2> movs = new List<Vector2> ();
		movs.Add (Vector2.up);
		movs.Add (Vector2.right);
		movs.Add (-Vector2.up);
		movs.Add (-Vector2.right);
		return movs;
	}

	bool valid(Vector2 dir) {
		// Cast Line from 'next to Pac-Man' to 'Pac-Man'
		Vector2 pos = transform.position;
		return valid (pos, dir);
	}

	public bool valid(Vector2 pos,Vector2 dir)
	{
		//Mask of bytes representing 8, if more collision layer needed use 1 << 8 || 1 << 6
		int mask = 1 << 8;
		//Vector2 posDir = new Vector2 (pos.x + dir.x, pos.y + dir.y);
		//RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos,mask);
		RaycastHit2D hit = Physics2D.CircleCast(pos,radius,dir,distance,mask);
		return (hit.collider == null);
	}

	public void SetRoute(List<Vector2> route)
	{
		this.route = route;
	}

	public void Move(Vector2 dir)
	{
		if (valid (dir)) {
			dir.x = dir.x/2;
			dir.y = dir.y/2;
			dest = (Vector2)transform.position + dir;
		}
	}

	public void Kill()
	{
		isAlive = false;
		Destroy(gameObject);
	}


	#endregion
}
