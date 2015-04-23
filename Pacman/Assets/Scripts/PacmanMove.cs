using UnityEngine;
using System.Collections;

public class PacmanMove : MonoBehaviour {
	//Set the velocity
	public float speed = 0.4f;

	//Matrix with all positions
	public Transform[] positionsRow1;
	public Transform[] positionsRow2;
	public Transform[] positionsRow3;
	public Transform[] positionsRow4;
	public Transform[] positionsRow5;
	public Transform[] positionsRow6;
	public Transform[] positionsRow7;
	public Transform[] positionsRow8;
	public Transform[] positionsRow9;
	public Transform[] positionsRow10;
	public Transform[] positionsRow11;



	//Destination position
	Vector2 dest = Vector2.zero;

	// Use this for initialization
	void Start () {
		dest = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// Move closer to Destination
		Vector2 p = Vector2.MoveTowards(transform.position, dest, speed);
		GetComponent<Rigidbody2D>().MovePosition(p);

		// Check for Input if not moving
		if ((Vector2)transform.position == dest) {
			//Evaluate next movement
			if (Input.GetKey(KeyCode.UpArrow) && valid(Vector2.up))
				dest = (Vector2)transform.position + Vector2.up;
			if (Input.GetKey(KeyCode.RightArrow) && valid(Vector2.right))
				dest = (Vector2)transform.position + Vector2.right;
			if (Input.GetKey(KeyCode.DownArrow) && valid(-Vector2.up))
				dest = (Vector2)transform.position - Vector2.up;
			if (Input.GetKey(KeyCode.LeftArrow) && valid(-Vector2.right))
				dest = (Vector2)transform.position - Vector2.right;
		}

		//Check for animation to be used
		// Animation Parameters
		Vector2 dir = dest - (Vector2)transform.position;
		GetComponent<Animator>().SetFloat("DirX", dir.x);
		GetComponent<Animator>().SetFloat("DirY", dir.y);
	}

	Vector2 GetNextMovement(){
		return new Vector2 ();
	}


	bool valid(Vector2 dir) {
		// Cast Line from 'next to Pac-Man' to 'Pac-Man'
		Vector2 pos = transform.position;
		RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos);
		return (hit.collider == GetComponent<Collider2D>());
	}
}
