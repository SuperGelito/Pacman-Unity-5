using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	public float levelStartDelay = 2f;
	public float turnDelay = .1f;
	public static GameManager instance = null;

	//Board manager object
	public BoardManager boardScript;
	private Text levelText;
	private GameObject levelImage;
	List<Enemy> enemies;
	bool enemiesMoving;
	private int level=1;
	public int foodPoints = 100;
	[HideInInspector] public bool playersTurn =true;
	private bool doingSetup;

	// Use this for initialization
	void Awake () {

		//Check if instance already exists
		if (instance == null)
			//if not, set instance to this
			instance = this;
		//If instance already exists and it's not this:
		else if (instance != this)
			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
			Destroy(gameObject);    

		//Sets this to not be destroyed when reloading scene
		DontDestroyOnLoad(gameObject);
		enemies = new List<Enemy> ();
		boardScript = GetComponent<BoardManager> ();
		InitGame ();
	}

	void OnLevelWasLoaded(int index)
	{
		level++;
		InitGame ();
	}

	public void GameOver()
	{
		levelText.text = "After " + level + " number of days you starved";
		levelImage.SetActive (true);
		enabled = false;
	}

	/// <summary>
	/// Initialize the level
	/// </summary>
	void InitGame()
	{
		doingSetup = true;
		levelImage = GameObject.Find ("LevelImage");
		levelText = GameObject.Find ("LevelText").GetComponent<Text> ();
		levelText.text = "Day " + level;
		levelImage.SetActive (true);
		Invoke ("HideLevelImage", levelStartDelay);
		enemies.Clear ();
		boardScript.SetupScene (level);
	}

	private void HideLevelImage()
	{
		doingSetup = false;
		levelImage.SetActive (false);
	}

	// Update is called once per frame
	void Update () {
		if (playersTurn || enemiesMoving || doingSetup)
			return;

		StartCoroutine (moveEnemies());
	}

	public void addEnemyToList(Enemy script)
	{
		enemies.Add (script);
	}

	private IEnumerator moveEnemies()
	{
		enemiesMoving = true;
		yield return new WaitForSeconds(this.turnDelay);

		if(enemies.Count == 0)
			yield return new WaitForSeconds(this.turnDelay);
		for (int i=0; i< this.enemies.Count; i++) {
			enemies[i].MoveEnemy();
			yield return new WaitForSeconds(enemies[i].moveTime);
		}
		enemiesMoving = false;

		playersTurn = true;
	}
}
