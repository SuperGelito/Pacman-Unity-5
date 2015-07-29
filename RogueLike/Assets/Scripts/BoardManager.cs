using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

	[Serializable]
	public class Count
	{
		public int minimum;
		public int maximum;
		public Count(int min,int max)
		{
			minimum = min;
			maximum = max;
		}
	}

	//Set number of rows and columns
	public int rows = 8;
	public int columns = 8;

	//Set maximum number of walls
	public Count wallCount = new Count(5,9);
	public Count foodCount = new Count(1,5);

	//Variables where well store prefabs that we will use for generating world
	public GameObject exit;
	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] outerWallTiles;
	public GameObject[] foodTiles;
	public GameObject[] enemyTiles;

	//Set board transform
	private Transform boardHolder;

	//Set list of all possible positions in grid
	private List<Vector3> gridPositions = new List<Vector3>();

	/// <summary>
	/// add to grid positions all posible positions to store items
	/// </summary>
	void InitializeGridPositions()
	{
		gridPositions.Clear ();
		for (int x=1; x<columns-1; x++)
			for (int y=1; y<rows-1; y++) {
			gridPositions.Add(new Vector3(x,y));
		}
	}

	/// <summary>
	/// Instantiate outer walls as limits and floor in the whole game grid
	/// </summary>
	void BoardSetup()
	{
		boardHolder = new GameObject ("Board").transform;
		for (int x=-1; x<=columns; x++)
		for (int y=-1; y<=rows; y++) {
			//Instantiate floor in whole game space
			GameObject toInstantiate;
			if(x == -1 || x == columns || y == -1 || y == rows)
				toInstantiate = outerWallTiles[Random.Range(0,outerWallTiles.Length)];
			else
				toInstantiate = floorTiles[Random.Range(0,floorTiles.Length)];
			//Instance prefab in a coordinates
			GameObject instance = (GameObject)Instantiate(toInstantiate,new Vector3(x,y),Quaternion.identity);
			//link instantiate object with board
			instance.transform.SetParent(boardHolder);
		}
	}

	/// <summary>
	/// Pop a random position from grid positions
	/// </summary>
	/// <returns>a random position of the collection</returns>
	Vector3 RandomPosition()
	{
		int randomIndex = Random.Range (0, gridPositions.Count);
		Vector3 randomPosition = gridPositions[randomIndex];
		gridPositions.RemoveAt (randomIndex);
		return randomPosition;
	}

	/// <summary>
	/// Set a number of instances based on a list of possible prefabs
	/// </summary>
	/// <param name="tileArray">List of prefabs to instantiate</param>
	/// <param name="minimum">Minimum number of instances</param>
	/// <param name="maximum">Maximum number of instances</param>
	void LayoutObjectAtRandom(GameObject[] tileArray,int minimum,int maximum)
	{
		int objectCount = Random.Range (minimum, maximum);

		for (int i=0; i<objectCount; i++) {
			Vector3 randomPosition = RandomPosition();
			GameObject tileToInstantiate = tileArray[Random.Range(0,tileArray.Length)];
			Instantiate(tileToInstantiate,randomPosition,Quaternion.identity);
		}
	}

	/// <summary>
	/// Creates the level
	/// </summary>
	/// <param name="level">Level.</param>
	public void SetupScene(int level)
	{
		//Create the board, floor and outer walls
		BoardSetup ();
		//Initialize item positions
		InitializeGridPositions ();
		//Create walls
		LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);
		//Create food
		LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);
		//Generate enemy number based on level number
		int enemyCount = (int)Mathf.Log (level, 2f);
		//Create enemies
		LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);
		//Create exit
		Instantiate(exit,new Vector3(columns-1,rows-1),Quaternion.identity);

	}
}
