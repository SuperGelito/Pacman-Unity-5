using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
public class Environment : MonoBehaviour {

	//List of objects
	GameObject pacman;
	List<GameObject> pacdots;
	Node solution;
	// Use this for initialization
	void Start () {
		//Initialize pacman and pacdots
		pacman = GameObject.FindGameObjectWithTag ("Pacman");
		pacdots = GameObject.FindGameObjectsWithTag ("Pacdot").ToList();
		solution = null;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Space))
		{
			SearchAgent agent = new SearchAgent ((new Problem (pacman, pacdots)));
			solution = agent.BFGS ();
			if(solution != null)
			{
				List<Node> path = solution.Path();
				List<Vector2> moves = new List<Vector2>();
				foreach(Node n in path.OrderBy(p=>p.Depth))
				{
					if(n.Action.HasValue)
						moves.Add(n.Action.Value);
				}
				pacman.GetComponent<PacmanMove>().SetRoute(moves);
			}
		}
	}

	/*void Search(object problem)
	{
		//Problem p = new Problem (pacman, pacdots);
		SearchAgent agent = new SearchAgent ((Problem)problem);
		solution = agent.DFGS ();
	}*/

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

