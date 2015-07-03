using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
public class Environment : MonoBehaviour {

	//List of objects
	GameObject pacman;
	//List<GameObject> pacdots;
	Node solution;
	bool reflexMode;
	//ReflextAgent reflexAgent;
	// Use this for initialization
	void Start () {
		//Initialize pacman
		pacman = GameObject.FindGameObjectWithTag ("Pacman");
		//pacdots = GameObject.FindGameObjectsWithTag ("Pacdot").ToList();
		solution = null;
		//reflexAgent = null;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.S))
		{
			Dictionary<string,object> percepts = Percept(pacman);
			List<GameObject> pacdots = (List<GameObject>)percepts["Pacdot"];
			SearchAgent agent = new SearchAgent ((new Problem (pacman, pacdots)));
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
		if (Input.GetKey (KeyCode.R)) {
			reflexMode = !reflexMode;
		}
		if (reflexMode) {
			if(!pacman.GetComponent<PacmanMove>().isMoving)
			{
				Dictionary<string,object> percepts = Percept(pacman);
				List<GameObject> pacdots = (List<GameObject>)percepts["Pacdot"];
				Problem prob = new Problem(pacman,pacdots);
				ReflextAgent reflexAgent = new ReflextAgent(prob);
				Node nextNode = reflexAgent.GetNextNode();
				if(nextNode!=null)
				{
					ExecuteActionPacman(pacman,nextNode.Action.Value);
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
		List<GameObject> pacdots = GameObject.FindGameObjectsWithTag ("Pacdot").ToList();
		Dictionary<string,object> percepts = new Dictionary<string, object> ();
		//percepts.Add ("Pacman", pacman);
		percepts.Add ("Pacdot", pacdots);
		return percepts;
	}

	//Execute action
	void ExecuteActionPacman(GameObject agent,Vector2 action)
	{
		agent.GetComponent<PacmanMove> ().Move (action);
	}

	//Get the sequence of actions based on percept
	//TODO: Define step function Step and Run for be used in a reflex agent (i.e. Pacman in a environment with ghosts)

}

