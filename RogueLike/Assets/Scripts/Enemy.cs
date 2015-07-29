using UnityEngine;
using System.Collections;

public class Enemy : MovingObject
{
	public int playerDamage = 1;

	private Animator animator;
	private Transform target;
	private bool skipMove;
	
	public AudioClip enemySound1;				//1 of 2 Audio clips to play when player moves.
	public AudioClip enemySound2;
	// Use this for initialization
	protected override void Start ()
	{
		GameManager.instance.addEnemyToList (this);
		animator = GetComponent<Animator> ();
		target = GameObject.FindGameObjectWithTag ("Player").transform;
		base.Start ();
	}


	protected override void AttemptMove<T> (int dirX, int dirY)
	{
		if (skipMove) {
			skipMove=false;
			return;
		}

		base.AttemptMove<T> (dirX, dirY);
		skipMove = true;
	}

	//MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
	public void MoveEnemy ()
	{
		//Declare variables for X and Y axis move directions, these range from -1 to 1.
		//These values allow us to choose between the cardinal directions: up, down, left and right.
		int xDir = 0;
		int yDir = 0;
		
		//If the difference in positions is approximately zero (Epsilon) do the following:
		if(Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
			
			//If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
			yDir = target.position.y > transform.position.y ? 1 : -1;
		
		//If the difference in positions is not approximately zero (Epsilon) do the following:
		else
			//Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left).
			xDir = target.position.x > transform.position.x ? 1 : -1;
		
		//Call the AttemptMove function and pass in the generic parameter Player, because Enemy is moving and expecting to potentially encounter a Player
		AttemptMove <Player> (xDir, yDir);
	}

	protected override void OnCantMove<T> (T component)
	{
		Player hitPlayer = component as Player;
		animator.SetTrigger ("EnemyAttack");
		SoundManager.instance.RandomizeSfx(this.enemySound1,this.enemySound2);
		hitPlayer.LoseFood (playerDamage);
	}
}

