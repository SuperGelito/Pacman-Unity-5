using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Problem
{
	float actorRadius;
	float dotRadius;
	public State initialState;
	//Pacman script is accessed to use simulator for action validations
	PacmanMove pacmanScript;
	//GhostScript
	//GhostMove ghostScript;
	//Initialize problem
	public Problem(Dictionary<string,GameObject> actors,List<GameObject> pacDots){
		pacmanScript = actors[ActorNames.Pacman.ToString()].GetComponent<PacmanMove> ();
		//Assign radius
		actorRadius = pacmanScript.radius;
		dotRadius = 0.25f;
		//Assign initialState

		//Get actorPositions
		Dictionary<ActorNames,Vector2> actorPos = new Dictionary<ActorNames, Vector2> ();
		foreach (string actorName in actors.Keys) {
			ActorNames actorNameEnum = (ActorNames)System.Enum.Parse( typeof( ActorNames ), actorName );
			actorPos.Add(actorNameEnum,(Vector2)actors[actorName].transform.position);
		}

		//Structure to store information from dots
		Dictionary<string,Vector2> dotPos = new Dictionary<string, Vector2> ();

		//Loop dots
		foreach (GameObject dot in pacDots) {
			dotPos.Add(dot.name,dot.transform.position);
		}
		//Set initialState
		initialState = new State (actorPos, dotPos);
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

		Vector2 pacmanPos = dest.GetPacmanPos ();

		//Feature1:Calculate dots left
		Dictionary<string, Vector2> destDots = dest.GetDots ();
		//Feature2:Calculate distance from all dots
		double distancePoints = 0;
		foreach(var dot in destDots)
		{
			float dotDistance = Mathf.Abs(pacmanPos.x - dot.Value.x) + Mathf.Abs(pacmanPos.y - dot.Value.y);
			//float distanceNearestWall;
			//float distanceOverWall = CalculateWallCollisions(pacmanPos,dot.Value);
			//dotDistance += distanceOverWall;
			//if(distanceNearestWall != 0)
			//dotDistance = dotDistance * (1+(distanceNearestWall/100f)); 
			distancePoints += (1/(dotDistance * dotDistance));
		}
		//Feature3:Calculate distance from pacman to ghost
		List<Vector2> ghostPositions = new List<Vector2> ();
		ghostPositions.Add(dest.GetGhostPos (ActorNames.Blinky));
		ghostPositions.Add(dest.GetGhostPos (ActorNames.Pinky));
		ghostPositions.Add(dest.GetGhostPos (ActorNames.Inky));
		ghostPositions.Add(dest.GetGhostPos (ActorNames.Clyde));
		float ghostDistance = 0;
		float deathImpact = 0;
		List<Vector2> quandrantMatching = new List<Vector2> ();
		int ghostinsameposition = 0;
		foreach (Vector2 gpos in ghostPositions) {
			ghostDistance += Mathf.Abs(pacmanPos.x - gpos.x) + Mathf.Abs(pacmanPos.y - gpos.y);
			//ghostDistance += CalculateWallCollisions (pacmanPos, gpos);

			//Feature4: Calculate death in next movement
			RaycastHit2D hit;
			int mask = 1 << 9 | 1 << 8;
			if(!pacmanScript.validLine(pacmanPos,gpos,out hit,mask))
			{
				if(hit.collider.tag == "Ghost")
					deathImpact+=1;

			}
				



			//Feature5: Add ghost to a cuadrant of pacman to see if he is surrounded
			if(gpos.x > pacmanPos.x)
				if(!quandrantMatching.Contains(Vector2.right))
					quandrantMatching.Add(Vector2.right);
			if(gpos.x < pacmanPos.x)
				if(!quandrantMatching.Contains(Vector2.right * -1))
					quandrantMatching.Add(Vector2.right * -1);
			if(gpos.y > pacmanPos.y)
				if(!quandrantMatching.Contains(Vector2.up))
					quandrantMatching.Add(Vector2.up);
			if(gpos.y < pacmanPos.y)
				if(!quandrantMatching.Contains(Vector2.up * -1))
					quandrantMatching.Add(Vector2.up * -1);



		}
		//feature6:Get number of ghosts in same position
		ghostinsameposition= ghostPositions.Count() + ghostPositions.GroupBy(g => g).Count () ;


		//Feature weight
		//f1
		int weightNumberDots = 100;
		//f2
		int weightDistance = 1;
		//f3
		int weightGhostDistance = 10;
		//f4
		int weightDeath = -1000;
		//f5
		int weightquandrantMatching = -20;
		//f6
		int weightGhostInSamePosition = 50;

		double ret = (200 - destDots.Count ()) *weightNumberDots//f1
			+ distancePoints * weightDistance//f2
				+ ghostDistance * weightGhostDistance//f3
				+ weightDeath * deathImpact//f4
				+ weightquandrantMatching * quandrantMatching.Count()//f5
				+ weightGhostInSamePosition * ghostinsameposition;//f6
		return ret;
	}
	
	public float CalculateWallCollisions(Vector2 pacmanPos,Vector2 dotPos)
	{	
		float distance = 0;
		float error = 1;
		//Calculate collision Horizontal and Vertical
		float distanceVH = 0;
		float distanceWallDestVH = 0;
		Vector2 midPointVH = new Vector2 (pacmanPos.x, dotPos.y);
		distanceVH += CalculateDotCollision (pacmanPos, midPointVH,ref distanceWallDestVH);
		if (distanceVH == 0) {
			distanceVH += CalculateDotCollision (midPointVH, dotPos,ref distanceWallDestVH);
		}

		//Calculate collision Vertical and Horizontal
		float distanceHV = 0;
		float distanceWallDestHV = 0;
		Vector2 midPointHV = new Vector2 (dotPos.x, pacmanPos.y);
		distanceHV += CalculateDotCollision (pacmanPos, midPointHV,ref distanceWallDestHV);
		if (distanceHV == 0) {
			distanceHV += CalculateDotCollision (midPointHV, dotPos,ref distanceWallDestHV);
		}

		if ((distanceWallDestHV - distanceWallDestVH) < error) {
			if (distanceHV < distanceVH) {
				distance = distanceHV;
			} else {
				distance = distanceVH;
			}
		} else if (distanceWallDestHV == 0 || distanceWallDestHV > distanceWallDestVH)
			distance = distanceHV;
		else if (distanceWallDestVH == 0 || distanceWallDestVH > distanceWallDestHV) 
			distance = distanceVH;
		return distance;
	}

	public float CalculateDotCollision(Vector2 sourcePos,Vector2 destPos,ref float distanceWallDest)
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
			//float distanceWallDest;
			if(direction.x == 0)
			{
				min = wall.offset.x - wall.size.x/2 - actorRadius;
				max = wall.offset.x + wall.size.x/2 + actorRadius;
				point = wallimpact.point.x;
				destPoint = destPos.x;
				distanceWallDest = Mathf.Abs (wallimpact.point.y - sourcePos.y);
			}
			else
			{
				min = wall.offset.y - wall.size.y/2 - actorRadius;
				max = wall.offset.y + wall.size.y/2 + actorRadius;
				point = wallimpact.point.y;
				destPoint = destPos.y;
				distanceWallDest = Mathf.Abs (wallimpact.point.x - sourcePos.x);
			}
			//Use values to add minimum sum
			if(destPoint > max)
				destPoint = max;
			if(destPoint < min)
				destPoint = min;
			float distanceMax = Mathf.Abs(max-point) + Mathf.Abs(destPoint-max);
			float distanceMin = Mathf.Abs(min-point) + Mathf.Abs(destPoint-min);
			distanceToAdd += Mathf.Min(distanceMax,distanceMin);
			//Calculate distance between wall and destination
			//distanceToAdd = distanceToAdd * (1+(distanceWallDest/100f));
		}
		return distanceToAdd;
	}

	//Successor
	public List<KeyValuePair<Vector2,State>> Successor(State state,ActorNames actor)
	{
		//Next states
		List<KeyValuePair<Vector2,State>> nextStates = new List<KeyValuePair<Vector2, State>> ();

		//Get all theoretically valid movements
		List<Vector2> totalMovs = pacmanScript.GetDefaultMovements ();
		//Create a list with valid movements
		List<Vector2> validMovs = new List<Vector2> ();

		//Get state pacman position
		Vector2 actorPos = state.GetActorPos (actor);
		//Loop movements to validate using unity collisions
		foreach (Vector2 mov in totalMovs) {
			//If the movement is valid is moved to valid movements
			if(pacmanScript.validCircle(actorPos,mov))
				validMovs.Add(mov);
		}

		//Loop valid movements to check result
		foreach (Vector2 validMov in validMovs) {
			//Set valid mov new values
			Vector2 movUpdated = new Vector2();
			movUpdated.x = validMov.x / 2;
			movUpdated.y = validMov.y /2;

			//Set next pacman state
			Vector2 nextActorPos = actorPos + movUpdated;

			//Calculate successor state
			//Create intervals to check if some dot is collides with pacman
			float xLeft = nextActorPos.x - actorRadius;
			float xRight = nextActorPos.x + actorRadius;
			float yDown = nextActorPos.y - actorRadius;
			float yUp = nextActorPos.y + actorRadius;

			//Copy dots to next state
			Dictionary<string,Vector2> dots = state.GetDots();
			Dictionary<string,Vector2> nextDots = new Dictionary<string, Vector2>();
			foreach(string dotKey in dots.Keys)
			{
				nextDots.Add(dotKey,dots[dotKey]);
			}

			if(actor == ActorNames.Pacman)
			{

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
			}
			//Configure nextState
			Dictionary<ActorNames,Vector2> oldActors = state.GetActors();
			Dictionary<ActorNames,Vector2> nextActors = new Dictionary<ActorNames, Vector2>();
			foreach(ActorNames actorKey in oldActors.Keys)
			{
				if(actorKey != actor)
					nextActors.Add(actorKey,oldActors[actorKey]);
				else
					nextActors.Add(actorKey,nextActorPos);
			}
			nextStates.Add(new KeyValuePair<Vector2,State>(validMov,new State(nextActors,nextDots)));
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
	//Vector2 pacman;
	Dictionary<string,Vector2> dots;
	Dictionary<ActorNames,Vector2> actors;
	/*public State(Vector2 pacmanPosition,Dictionary<string,Vector2> dotsPositions)
	{
		new State (pacmanPosition, dotsPositions, Dictionary<string,Vector2> ());
	}*/

	public State(Dictionary<ActorNames,Vector2> actorsPositions,Dictionary<string,Vector2> dotsPositions)
	{
		actors = actorsPositions;
		dots = dotsPositions;
		foreach (var actor in actors.Keys) {
			idState+=actor + "-" + actors[actor].ToString();
		}
		foreach (string dotKey in dots.Keys) {
			idState+=dotKey + "-";
		}
	}
	
	public Vector2 GetPacmanPos()
	{
		return GetActorPos(ActorNames.Pacman);
	}

	public Vector2 GetGhostPos(ActorNames ghostName)
	{
		return GetActorPos(ghostName);
	}

	public Vector2 GetActorPos(ActorNames actor)
	{
		return actors[actor];
	}

	public Dictionary<ActorNames,Vector2> GetActors()
	{
		return actors;
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

public enum ActorNames
{
	Pacman = 1,
	Blinky = 2,
	Pinky = 3,
	Inky = 4,
	Clyde = 5
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
	public List<Node> Expand(Problem prob,ActorNames actorName = ActorNames.Pacman)
	{
		List<Node> childNodes = new List<Node> ();
		//Get successors for this state
		State origin = this.State;
		List<KeyValuePair<Vector2,State>> successors = prob.Successor (origin,actorName);
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

