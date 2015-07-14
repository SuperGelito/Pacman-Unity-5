using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

	//Heuristic
	public int HeurTree(State dest)
	{
		return dest.GetDots ().Count;
	}

	//Heuristic
	public int HeurGraph(State dest)
	{
		//Get the number of dots and handle them with higher weight
		int numberDots = dest.GetNumberOfDots ();
		//numberDots *= 1000;
		int weightNumberDots = 1000;
		int weightNearestDot = 10;
		//loop number of dots to ensure nearest dot
		string nearestDot = string.Empty;
		int nearestPositionCount = 1000;
		Vector2 pacmanPos = dest.GetPacmanPos ();
		foreach(var dot in dest.GetDots())
		{
			int dotPositionCount = (int)(Mathf.Abs(pacmanPos.x - dot.Value.x) + Mathf.Abs(pacmanPos.y - dot.Value.y)) * weightNearestDot;
			//Get total amount of difference
			if(dotPositionCount < nearestPositionCount)
			{
				nearestPositionCount = dotPositionCount;
				nearestDot = dot.Key;
			}
		}


		return numberDots * weightNumberDots + nearestPositionCount;
	}

	//Utility function
	public double Utility(State dest)
	{
		int weightNumberDots = 1000;
		int weightDistance = 1;
		Vector2 pacmanPos = dest.GetPacmanPos ();
		Dictionary<string, Vector2> destDots = dest.GetDots ();
		double distancePoints = 0;
		foreach(var dot in destDots)
		{
			float dotDistance = Mathf.Abs(pacmanPos.x - dot.Value.x) + Mathf.Abs(pacmanPos.y - dot.Value.y);
			dotDistance+=CalculateDotWallCollisions(pacmanPos,dot.Value);
			distancePoints += (1/(dotDistance * dotDistance));
		}
		double ret = (200 - destDots.Count ()) * weightNumberDots + distancePoints * weightDistance;
		return ret;
	}

	/*
	 public KeyValuePair<string,double>? GetNearestDot(State state)
	{
		List<KeyValuePair<string,double>> dotsDistanceList = new List<KeyValuePair<string, double>> ();
		Vector2 pacmanPos = state.GetPacmanPos ();
		Dictionary<string, Vector2> destDots = state.GetDots ();
		foreach(var dot in destDots)
		{
			double dotDistance = Mathf.Abs(pacmanPos.x - dot.Value.x) + Mathf.Abs(pacmanPos.y - dot.Value.y);
			//double dotDistance = Vector2.Distance(pacmanPos,dot.Value);
			dotsDistanceList.Add(new KeyValuePair<string,double>(dot.Key,dotDistance));
		}
		KeyValuePair<string,double>? ret = null;
		if (dotsDistanceList.OrderBy (d => d.Value).Any ()) {

			string key = dotsDistanceList.OrderBy (d => d.Value).First ().Key;
			double dotDist = dotsDistanceList.OrderBy (d => d.Value).First ().Value;
			ret=new KeyValuePair<string, double>(key,dotDist);
		}
		return ret;
	}
	public Vector2 GetBaseAngleForCompare(RaycastHit2D wallImpact)
	{
		Vector2 baseAngle = Vector2.up;
		BoxCollider2D wall = (BoxCollider2D)wallImpact.collider;
		Vector2 wallCenter = wall.offset;
		Vector2 impactPoint = wallImpact.point;
		float yMax = wallCenter.y + wall.size.y / 2;
		float yMin = wallCenter.y - wall.size.y / 2;
		if (wallImpact.distance < 2) {
			//if x collision comes from left
			if (impactPoint.x < wallCenter.x) {
				if (impactPoint.y == yMax || impactPoint.y == yMin )
					baseAngle = Vector2.right;
				else {
					if (impactPoint.y >= wallCenter.y)
						baseAngle = Vector2.up * -1;
					else if (impactPoint.y < wallCenter.y)
						baseAngle = Vector2.up;
				}
			} else {
				//if not impact comes from right
				if (impactPoint.y == yMax || impactPoint.y == yMin)
					baseAngle = Vector2.right * -1;
				else {
					if (impactPoint.y >= wallCenter.y)
						baseAngle = Vector2.up;
					else if (impactPoint.y < wallCenter.y)
						baseAngle = Vector2.up * -1;
				}
			}
		}

		return baseAngle;
	}
	*/
	public float CalculateDotWallCollisions(Vector2 pacmanPos,Vector2 dotPos)
	{	

		//Calculate collision Horizontal and Vertical
		float distanceHV = 0;
		Vector2 midPointHV = new Vector2 (pacmanPos.x, dotPos.y);
		distanceHV += CalculateDotCollision (pacmanPos, midPointHV);
		if (distanceHV == 0)
			distanceHV += CalculateDotCollision (midPointHV, dotPos);
		//Calculate collision Vertical and Horizontal
		float distanceVH = 0;
		Vector2 midPointVH = new Vector2 (dotPos.x, pacmanPos.y);
		distanceVH += CalculateDotCollision (pacmanPos, midPointVH);
		if (distanceVH == 0)
			distanceVH += CalculateDotCollision (midPointVH, dotPos);
		return Mathf.Min(distanceHV,distanceVH);
	}

	public float CalculateDotCollision(Vector2 sourcePos,Vector2 destPos)
	{
		float distanceToAdd = 0;
		Vector2 direction = new Vector2 (destPos.x - sourcePos.x,destPos.y - sourcePos.y);
		float distance = Mathf.Abs (destPos.x - sourcePos.x) + Mathf.Abs (destPos.y - sourcePos.y);

		//Calculate collision
		RaycastHit2D wallimpact;
		if (!pacmanScript.validCircle (sourcePos, direction,distance, out wallimpact)) {
			BoxCollider2D wall = (BoxCollider2D)wallimpact.collider;
			float min;
			float max;
			float point;
			float destPoint;
			if(direction.x == 0)
			{
				min = wall.offset.x - wall.size.x/2 - pacmanRadius;
				max = wall.offset.x + wall.size.x/2 + pacmanRadius;
				point = wallimpact.point.x;
				destPoint = destPos.x;
			}
			else
			{
				min = wall.offset.y - wall.size.y/2 - pacmanRadius;
				max = wall.offset.y + wall.size.y/2 + pacmanRadius;
				point = wallimpact.point.y;
				destPoint = destPos.y;
			}
			//Use values to add minimum sum
			if(destPoint > max)
				destPoint = max;
			if(destPoint < min)
				destPoint = min;
			float distanceMax = Mathf.Abs(max-point) + Mathf.Abs(destPoint-max);
			float distanceMin = Mathf.Abs(min-point) + Mathf.Abs(destPoint-min);
			distanceToAdd += Mathf.Min(distanceMax,distanceMin);
		}
		return distanceToAdd;
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
			if(pacmanScript.validCircle(pacmanPos,mov))
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
			Dictionary<string,Vector2> dots = state.GetDots();
			Dictionary<string,Vector2> nextDots = new Dictionary<string, Vector2>();
			foreach(string dotKey in dots.Keys)
			{
				nextDots.Add(dotKey,dots[dotKey]);
			}
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
			idState+=dotKey + "-";
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
	public int Cost;
	public int CostAcumulated;
	public int HeurTree;
	public int CostHeurTree{
		get{ return this.Cost + this.HeurTree;}
	}
	public int CostAcumulatedHeurTree{
		get{ return this.CostAcumulated + this.HeurTree;}
	}
	public int HeurGraph;
	public int CostHeurGraph{
		get{ return this.Cost + this.HeurGraph;}
	}
	public int CostAcumulatedHeurGraph{
		get{ return this.CostAcumulated + this.HeurGraph;}
	}
	public double Utility;
	public int Depth;

	/// <summary>
	/// Initializes a new instance of the <see cref="Node"/> class.
	/// </summary>
	/// <param name="state">State to be attached to node</param>
	/// <param name="Parent">Parent node of this node</param>
	/// <param name="action">Action vector2 direction that generated the node</param>
	/// <param name="cost">Cost of execute action from parent node</param>
	public Node(State state,Node parent,Vector2? action,int cost,int heurTree=0,int heurGraph=0,double utility=0){
		this.State = state;
		this.Parent = parent;
		this.Action = action;
		this.Cost = cost;
		this.CostAcumulated = cost + (parent == null ? 0 : parent.CostAcumulated);
		this.HeurTree = heurTree;
		this.HeurGraph = heurGraph;
		this.Utility = utility;
		//this.CostHeur = this.Cost + this.Heur;
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
			int heurtree = prob.HeurTree(destination);
			int heurgraph = prob.HeurGraph(destination);
			double utility = prob.Utility(destination);
			Node parent = this;

			childNodes.Add(new Node(destination,this,action,cost,heurtree,heurgraph,utility));
		}

		return childNodes;
	}
	
}

