using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour
{
	public LayerMask blockingLayer;
	public float moveTime = .1f;
	private BoxCollider2D collider;
	private Rigidbody2D rbody;
	private float inverseMoveTime;

	// Use this for initialization
	protected virtual void Start ()
	{
		this.collider = GetComponent<BoxCollider2D> ();
		this.rbody = GetComponent<Rigidbody2D> ();
		this.inverseMoveTime = 1 / moveTime;
	}

	protected IEnumerator SmoothMovement(Vector3 end)
	{
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

		while (sqrRemainingDistance > float.Epsilon)
		{
			Vector3 newPos = Vector3.MoveTowards(rbody.position,end,inverseMoveTime * Time.deltaTime);
			rbody.MovePosition(newPos);
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			yield return null;
		}

	}

	protected virtual void AttemptMove<T>(int dirX,int dirY)
		where T : Component
	{
		RaycastHit2D hit;
		bool canMove = Move (dirX, dirY,out hit);
		if (hit.transform == null) 
			return;

		T hitComponent = hit.transform.GetComponent<T> ();

		if (!canMove && hitComponent != null) {
			OnCantMove(hitComponent);
		}

	}

	protected bool Move(int xDir,int yDir,out RaycastHit2D hit)
	{
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2 (xDir, yDir);
		this.collider.enabled = false;
		hit = Physics2D.Linecast (start, end, this.blockingLayer);
		this.collider.enabled = true;
		if (hit.transform == null) {
			StartCoroutine(SmoothMovement(end));
			return true;
		}
		return false;
	}

	protected abstract void OnCantMove <T> (T component)
		where T : Component;

}

