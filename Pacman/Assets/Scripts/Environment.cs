using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Environment : MonoBehaviour {

	//List of objects
	GameObject pacman;
	List<GameObject> pacdots;

	// Use this for initialization
	void Start () {
		//Initialize pacman and pacdots
		pacman = GameObject.FindGameObjectWithTag ("Pacman");
		pacdots = GameObject.FindGameObjectsWithTag ("Pacdot").ToList();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Space))
		{
			Problem p = new Problem (pacman, pacdots);
			SearchAgent agent = new SearchAgent (p);
			Node n = agent.DFGS ();
			//Get nodePath
			List<Node> path = n.Path();
			path.OrderBy(no => no.Depth).ToList();
		}
	}

	bool is_done()
	{
		//Loop pacmans
		return !pacman.GetComponent<PacmanMove> ().isAlive;
	}

	//Perceptions that an agent get from environment
	/*Dictionary<string,Object> Percept(GameObject agent){
		Dictionary<string,Object> percepts = new Dictionary<string, Object> ();
		percepts.Add ("Pacman", pacman);
		percepts.Add ("Pacdot", pacdots);
		return percepts;
	}*/

	//Execute action
	void ExecuteActionPacman(GameObject agent,Vector2 action)
	{
		agent.GetComponent<PacmanMove> ().Move (action);
	}

	//Get the sequence of actions based on percept
	//TODO: Define step function Step and Run for be used in a reflex agent (i.e. Pacman in a environment with ghosts)

}

