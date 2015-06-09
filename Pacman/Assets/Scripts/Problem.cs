using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Problem
{
	float pacmanRadius;
	float dotRadius;
	public State initialState;
	//Pacman script is accessed to use simulator for action validations
	PacmanMove pacmanScript;
	//Initialize problem
	public Problem(GameObject pacman,List<GameObject> pacDots){
		pacmanScript = pacman.GetComponent<PacmanMove> ();
		//Assign radius
		pacmanRadius = pacmanScript.radius;
		dotRadius = 0.25f;
		//Assign initialState
		Vector2 pacmanPos = pacman.transform.position;

		//Structure to store information from dots
		Dictionary<string,Vector2> dotPos = new Dictionary<string, Vector2> ();

		//Loop dots
		foreach (GameObject dot in pacDots) {
			dotPos.Add(dot.name,dot.transform.position);
		}
		//Set initialState
		initialState = new State (pacmanPos, dotPos);
	}
	//Goal test
	public bool GoalTest(State state)
	{
		return state.GetNumberOfDots() == 0;
	}

	//Path cost
	public int PathCost(State origin,Vector2 action,State dest)
	{
		return 1;
	}

	//Successor
	public List<KeyValuePair<Vector2,State>> Successor(State state)
	{
		//Next states
		List<KeyValuePair<Vector2,State>> nextStates = new List<KeyValuePair<Vector2, State>> ();

		//Get all theoretically valid movements
		List<Vector2> totalMovs = pacmanScript.GetDefaultMovements ();
		//Create a list with valid movements
		List<Vector2> validMovs = new List<Vector2> ();
		//Get state pacman position
		Vector2 pacmanPos = state.GetPacmanPos ();
		//Get state dots
		//Dictionary<string,Vector2> dots = state.GetDots();
		//Loop movements to validate using unity collisions
		foreach (Vector2 mov in totalMovs) {
			//If the movement is valid is moved to valid movements
			if(pacmanScript.valid(pacmanPos,mov))
				validMovs.Add(mov);
		}

		//Loop valid movements to check result
		foreach (Vector2 validMov in validMovs) {
			//Set valid mov new values
			Vector2 movUpdated = new Vector2();
			movUpdated.x = validMov.x / 2;
			movUpdated.y = validMov.y /2;

			//Set next pacman state
			Vector2 nextPacmanPos = pacmanPos + movUpdated;

			//Calculate successor state
			//Create intervals to check if some dot is collides with pacman
			float xLeft = nextPacmanPos.x - pacmanRadius;
			float xRight = nextPacmanPos.x + pacmanRadius;
			float yDown = nextPacmanPos.y - pacmanRadius;
			float yUp = nextPacmanPos.y + pacmanRadius;

			//Copy dots to next state
			Dictionary<string,Vector2> nextDots = state.GetDots();
			//Loop dots to check collisions
			List<string> dotsToRemove = new List<string>();
			foreach(string dotKey in nextDots.Keys)
			{
				Vector2 dotPos = nextDots[dotKey];
				//bool collision = false;
				bool collisionx = false;
				bool collisiony = false;
				if((xLeft >= dotPos.x - dotRadius && xLeft <= dotPos.x + dotRadius) ||
				   (xRight <= dotPos.x + dotRadius && xRight >= dotPos.x - dotRadius) ||
				   xLeft <= dotPos.x - dotRadius && xRight >=dotPos.x + dotRadius)
					collisionx=true;

				if((yDown >= dotPos.y - dotRadius && yDown <= dotPos.y + dotRadius) ||
				   (yUp <= dotPos.y + dotRadius && yUp >= dotPos.y - dotRadius) ||
				   yDown <= dotPos.y - dotRadius && yUp >=dotPos.y + dotRadius)
					collisiony=true;

				if(collisionx && collisiony)
					dotsToRemove.Add(dotKey);
			}
			foreach(string dot in dotsToRemove)
			{
				nextDots.Remove(dot);
			}
			nextStates.Add(new KeyValuePair<Vector2,State>(validMov,new State(nextPacmanPos,nextDots)));
		}

		return nextStates;
	}

}
/// <summary>
/// This class represents a state with position for pacman, ghosts and dots
/// </summary>
public class State
{
	public string idState;
	Vector2 pacman;
	Dictionary<string,Vector2> dots;
	public State(Vector2 pacmanPosition,Dictionary<string,Vector2> dotsPositions)
	{
		pacman = pacmanPosition;
		dots = dotsPositions;
		idState += pacman.ToString ();
		foreach (string dotKey in dots.Keys) {
			idState+=dotKey;
		}
	}
	
	public Vector2 GetPacmanPos()
	{
		return pacman;
	}

	public Dictionary<string,Vector2> GetDots()
	{
		return dots;
	}

	public int GetNumberOfDots()
	{
		return dots.Count;
	}
}

/// <summary>
///This class represent a node with a state and relation to parent and action coming from
/// </summary>
public class Node
{
	public State State;
	Node Parent;
	public Vector2? Action {get;set;}
	int Cost;
	public int Depth;

	/// <summary>
	/// Initializes a new instance of the <see cref="Node"/> class.
	/// </summary>
	/// <param name="state">State to be attached to node</param>
	/// <param name="Parent">Parent node of this node</param>
	/// <param name="action">Action vector2 direction that generated the node</param>
	/// <param name="cost">Cost of execute action from parent node</param>
	public Node(State state,Node parent,Vector2? action,int cost){
		this.State = state;
		this.Parent = parent;
		this.Action = action;
		this.Cost = cost;
		if (parent != null)
			this.Depth = parent.Depth + 1;
		else
			this.Depth = 0;
	}

	/// <summary>
	/// Path this to root.
	/// </summary>
	public List<Node> Path()
	{
		List<Node> path = new List<Node> ();
		Node node = this;
		path.Add (node);
		while (node.Parent != null) {
			path.Add(node.Parent);
			node=node.Parent;
		}
		return path;
	}

	/// <summary>
	/// Return nodes after expanding
	/// </summary>
	/// <param name="prob">Problem that generates successors</param>
	public List<Node> Expand(Problem prob)
	{
		List<Node> childNodes = new List<Node> ();
		//Get successors for this state
		State origin = this.State;

		List<KeyValuePair<Vector2,State>> successors = prob.Successor (origin);
		//Create a node with each posibility
		foreach (var successor in successors) {
			Vector2 action = successor.Key;
			State destination = successor.Value;
			int cost = prob.PathCost(this.State,successor.Key,successor.Value);
			Node parent = this;

			childNodes.Add(new Node(destination,this,action,cost));
		}

		return childNodes;
	}
	
}

