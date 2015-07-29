using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GhostMove : MonoBehaviour {
	//Set the velocity
	public float speed = 0.3f;
	//Is alive
	public bool isAlive;
	//Is Moving
	public bool isMoving;
	//Destination position
	Vector2 dest = Vector2.zero;
	//Radious of ghost
	public float radius = 0.9f;
	//Distance to move per action
	float distance = 0.5f;
	//Flat to set if ghost is in jail
	public bool inJail;
	//Respawn
	Vector2 respawn;
	//Init point
	public Vector2 initPoint;
	void Start()
	{
		dest = transform.position;
		isAlive = true;
		respawn = GameObject.FindGameObjectWithTag ("Respawn").transform.position;
		//initPoint = GameObject.FindGameObjectWithTag ("InitGhost").transform.position;
		inJail = true;
	}

	// Update is called once per frame
	void FixedUpdate () {

		if ((Vector2)transform.position == respawn) {
			dest = initPoint;
		}
		if ((Vector2)transform.position == initPoint) {
			inJail=false;
		}
		// Move closer to Destination
		Vector2 p = Vector2.MoveTowards(transform.position, dest, speed);
		GetComponent<Rigidbody2D>().MovePosition(p);
		
		// Check for Input if not moving
		if ((Vector2)transform.position == dest) {
			//Is not moving
			isMoving=false;
			
			if (Input.GetKey (KeyCode.W))
				Move (Vector2.up);
			if (Input.GetKey (KeyCode.D))
				Move (Vector2.right);
			if (Input.GetKey (KeyCode.S))
				Move (-Vector2.up);
			if (Input.GetKey (KeyCode.A))
				Move (-Vector2.right);
			
		} else {
			isMoving=true;
		}
		//Check for animation to be used
		// Animation Parameters
		Vector2 dir = dest - (Vector2)transform.position;
		GetComponent<Animator>().SetFloat("DirX", dir.x);
		GetComponent<Animator>().SetFloat("DirY", dir.y);
	}

	public List<Vector2> GetDefaultMovements()
	{
		List<Vector2> movs = new List<Vector2> ();
		movs.Add (Vector2.up);
		movs.Add (Vector2.right);
		movs.Add (-Vector2.up);
		movs.Add (-Vector2.right);
		return movs;
	}
	bool validCircle(Vector2 dir) {
		// Cast Line from 'next to Pac-Man' to 'Pac-Man'
		Vector2 pos = transform.position;
		return validCircle (pos, dir);
	}
	public bool validCircle(Vector2 pos,Vector2 dir)
	{
		//Mask of bytes representing 8, if more collision layer needed use 1 << 8 || 1 << 6
		int mask = 9 << 8;
		//Vector2 posDir = new Vector2 (pos.x + dir.x, pos.y + dir.y);
		//RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos,mask);
		RaycastHit2D hit = Physics2D.CircleCast(pos,radius,dir,distance,mask);
		return (hit.collider == null);
	}

	public void Move(Vector2 dir)
	{
		if (validCircle (dir)) {
			dir.x = dir.x/2;
			dir.y = dir.y/2;
			dest = (Vector2)transform.position + dir;
		}
	}

	void OnTriggerEnter2D(Collider2D co) {
		if (co.name == "Pacman") {
			PacmanMove sc = co.gameObject.GetComponent<PacmanMove>();
			sc.Kill();

		}
	}

	bool GhostInJail()
	{
		return (Vector2)transform.position == respawn;
	}
}
