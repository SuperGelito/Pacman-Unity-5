using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
public class Environment : MonoBehaviour {

	//List of objects
	GameObject pacman;
	List<GameObject> ghost;
	Node solution;
	bool reflexMode;
	//ReflextAgent reflexAgent;
	// Use this for initialization
	void Start () {
		//Initialize pacman
		pacman = GameObject.FindGameObjectWithTag ("Pacman");
		ghost = GameObject.FindGameObjectsWithTag ("Ghost").ToList();

		//pacdots = GameObject.FindGameObjectsWithTag ("Pacdot").ToList();
		solution = null;
		//reflexAgent = null;
	}
	
	// Update is called once per frame
	void Update () {
		Dictionary<string,object> percepts = Percept(pacman);
		List<GameObject> pacdots = (List<GameObject>)percepts["Pacdot"];
		Dictionary<string,GameObject> actors = (Dictionary<string,GameObject>)percepts["Actors"];
		if (Input.GetKey(KeyCode.P))
		{
			SearchAgent agent = new SearchAgent ((new Problem (actors, pacdots)));
			solution = agent.AstarGS ();
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
		if (Input.GetKey (KeyCode.O)) {
			reflexMode = !reflexMode;
		}
		if (reflexMode) {
				foreach(string actorName in actors.Keys)
				{
				percepts = Percept(pacman);
				pacdots = (List<GameObject>)percepts["Pacdot"];
				actors = (Dictionary<string,GameObject>)percepts["Actors"];
				Problem prob = new Problem(actors,pacdots);
				ReflextAgent reflexAgent = new ReflextAgent(prob);

				bool isMoving;
				bool inJail = false;
				if(actorName == "Pacman")
					isMoving = pacman.GetComponent<PacmanMove>().isMoving;
				else
					isMoving = actors[actorName].GetComponent<GhostMove>().isMoving;

				if(!isMoving && !inJail)
				{
					Node nextNode = reflexAgent.MinimaxSearch(4,actorName);
					if(nextNode!=null)
					{
						if(actorName == "Pacman")
							ExecuteActionPacman(pacman,nextNode.Action.Value);
						else
							ExecuteActionGhost(actors[actorName],nextNode.Action.Value);
					}
				}

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
	Dictionary<string,object> Percept(GameObject agent){
		pacman = GameObject.FindGameObjectWithTag ("Pacman");
		ghost = GameObject.FindGameObjectsWithTag ("Ghost").ToList();
		List<GameObject> pacdots = GameObject.FindGameObjectsWithTag ("Pacdot").ToList();
		Dictionary<string,GameObject> actors = new Dictionary<string, GameObject>();
		actors.Add(pacman.name,pacman);
		foreach(GameObject g in ghost)
		{
			actors.Add(g.name,g);
		}
		Dictionary<string,object> percepts = new Dictionary<string, object> ();
		//percepts.Add ("Pacman", pacman);
		percepts.Add ("Pacdot", pacdots);
		percepts.Add ("Actors", actors);
		return percepts;
	}

	//Execute action
	void ExecuteActionPacman(GameObject agent,Vector2 action)
	{
		agent.GetComponent<PacmanMove> ().Move (action);
	}

	//Execute action
	void ExecuteActionGhost(GameObject agent,Vector2 action)
	{
		agent.GetComponent<GhostMove> ().Move (action);
	}

	//Get the sequence of actions based on percept
	//TODO: Define step function Step and Run for be used in a reflex agent (i.e. Pacman in a environment with ghosts)

}

