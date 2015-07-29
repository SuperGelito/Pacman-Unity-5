using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Player : MovingObject
{
	public int wallDamage = 1;
	public int pointsPerFood = 10;
	public int pointsPerSoda = 20;

	public float restartLevelDelay = 1;
	private Animator animator;
	private int food;
	public Text foodText;


	public AudioClip moveSound1;				//1 of 2 Audio clips to play when player moves.
	public AudioClip moveSound2;				//2 of 2 Audio clips to play when player moves.
	public AudioClip eatSound1;					//1 of 2 Audio clips to play when player collects a food object.
	public AudioClip eatSound2;					//2 of 2 Audio clips to play when player collects a food object.
	public AudioClip drinkSound1;				//1 of 2 Audio clips to play when player collects a soda object.
	public AudioClip drinkSound2;				//2 of 2 Audio clips to play when player collects a soda object.
	public AudioClip gameOverSound;				//Audio clip to play when player dies.

	private Vector2 touchOrigin = -Vector2.one;

	// Use this for initialization
	protected override void Start ()
	{
		animator = GetComponent<Animator> ();
		food = GameManager.instance.foodPoints;

		foodText.text = "Food: " + food;
		base.Start ();
	}

	private void OnDisable()
	{
		GameManager.instance.foodPoints = food;
	}

	// Update is called once per frame
	void Update ()
	{
		if (!GameManager.instance.playersTurn)
			return;

		int horizontal = 0;
		int vertical = 0;
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
		horizontal = (int)Input.GetAxisRaw ("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");

		if (horizontal != 0)
			vertical = 0;
#else

		if(Input.touchCount > 0)
		{
			Touch myTouch = Input.touches[0];

			if(myTouch.phase == TouchPhase.Began)
			{
				touchOrigin = myTouch.position;
			}

			else if( myTouch.phase == TouchPhase.Ended && touchOrigin.x >=0)
			{
				Vector2 touchEnd = myTouch.position;
				float x = touchEnd.x - touchOrigin.x;
				float y = touchEnd.y - touchOrigin.y;

				if(Mathf.Abs(x) > Mathf.Abs (y))
					horizontal = x > 0 ? 1 : -1;
				else
					vertical = y > 0 ? 1 : -1;
			}
		}
#endif
		if (horizontal != 0 || vertical != 0) {
			AttemptMove<Wall>(horizontal,vertical);
		}
	}

	protected override void AttemptMove<T> (int dirX, int dirY)
	{

		food--;
		foodText.text = "Food: " + food;
		base.AttemptMove <T>(dirX, dirY);

		RaycastHit2D hit;
		if (Move (dirX, dirY, out hit)) {
			SoundManager.instance.RandomizeSfx(this.moveSound1,this.moveSound2);
		}
		CheckIfGameOver ();

		GameManager.instance.playersTurn = false;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Exit") {
			Invoke ("Restart", restartLevelDelay);
			enabled = false;
		} else if (other.tag == "Food") {
			food += pointsPerFood;
			foodText.text = "Food: " + food;
			other.gameObject.SetActive (false);
			SoundManager.instance.RandomizeSfx(this.eatSound1,this.eatSound2);
		} else if (other.tag == "Soda") {
			food += pointsPerSoda;
			foodText.text = "Food: " + food;
			other.gameObject.SetActive (false);
			SoundManager.instance.RandomizeSfx(this.drinkSound1,this.drinkSound2);
		}
	}

	protected override void OnCantMove<T> (T component)
	{
		if (component.GetType () == typeof(Wall)) {
			Wall wall = component as Wall;
			wall.DamageWall (wallDamage);
			animator.SetTrigger ("PlayerChop");
		}
	}

	private void Restart()
	{
		Application.LoadLevel (Application.loadedLevel);
	}

	public void LoseFood(int loss)
	{
		animator.SetTrigger ("PlayerHit");
		food -= loss;
		foodText.text = "Food: " + food;
		CheckIfGameOver ();
	}

	private void CheckIfGameOver()
	{
		if (food <= 0) {
			SoundManager.instance.PlaySingle(gameOverSound);
			SoundManager.instance.musicSource.Stop ();
			GameManager.instance.GameOver ();
		}
	}
}

