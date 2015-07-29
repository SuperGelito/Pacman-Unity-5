using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class SearchAgent
{
	Problem problem;
	Node solution;
	public SearchAgent(Problem prob)
	{
		problem = prob;
		solution = null;
	}
	/// <summary>
	/// Tree limited/unlimited search, does not open previously opened states
	/// </summary>
	/// <returns>Node with a goal</returns>
	/// <param name="fringe">States to begin with search</param>
	/// <param name="limit">optional parameter to set a limit in search and avoid infinite state spaces</param>
	private Node TreeSearch(Fringe fringe,int? limit=null)
	{
		//Set the initial node
		Node initialNode = new Node (this.problem.initialState, null, null, 0);
		//Add to the fringe
		fringe.Add (initialNode);
		//Loop fringe to get a 
		while (fringe.Any()) {
			Node node = fringe.Pop ();
			if (problem.GoalTest (node.State))
				return node;
			if(!limit.HasValue || (limit.HasValue && node.Depth < limit))
			{
				List<Node> childNodes = node.Expand (problem);
				childNodes.ForEach (c => fringe.Add (c));
			}
		}
		return null;
	}

	/// <summary>
	/// Graph limited/unlimited search, open previously opened states
	/// </summary>
	/// <returns>Node with a goal</returns>
	/// <param name="fringe">States to begin with search</param>
	/// <param name="limit">optional parameter to set a limit in search and avoid infinite state spaces</param>
	private Node GraphSearch(Fringe fringe,int? limit=null)
	{
		//Set the closed dictionary
		Dictionary<string,bool> closed = new Dictionary<string,bool> ();
		//Set the initial node
		Node initialNode = new Node (this.problem.initialState, null, null, 0);
		//Add to the fringe
		fringe.Add (initialNode);
		//Loop fringe to get a 
		while (fringe.Any()) {
			Node node = fringe.Pop ();
			//Log information about state
			logState(node.State);
			if (problem.GoalTest (node.State))
				return node;
			if(!closed.ContainsKey(node.State.idState))
			{
				closed[node.State.idState] = true;
				if(!limit.HasValue || (limit.HasValue && node.Depth < limit))
				{
						List<Node> childNodes = node.Expand (problem);
						foreach(Node childNode in childNodes)
						{
							fringe.Add(childNode);
						}
						logFringe(fringe);
				}
			}
		}
		return null;
	}
	/// <summary>
	/// Depth first Tree Search
	/// </summary>
	public Node DFTS()
	{
		return TreeSearch (new LIFO ());
	}
	/// <summary>
	/// Breadth first Tree search
	/// </summary>
	public Node BFTS()
	{
		return TreeSearch(new FIFO());
	}
	/// <summary>
	/// Depth first Graph Search
	/// </summary>
	public Node DFGS()
	{
		return GraphSearch(new LIFO());
	}
	/// <summary>
	/// Breadth first Graph search
	/// </summary>
	public Node BFGS()
	{
		return GraphSearch(new FIFO());
	}
	/// <summary>
	/// Uniform Cost Tree Search
	/// </summary>
	public Node UCTS()
	{
		return TreeSearch (new PriorityCost ());
	}
	/// <summary>
	/// Uniform Cost Graph Search
	/// </summary>
	public Node UCGS()
	{
		return GraphSearch (new PriorityCost ());
	}

	/// <summary>
	/// Depth limited tree search, search with an incremental depth limit
	/// </summary>
	/// <param name="problem">Problem to search in</param>
	/// <param name="limit">limit to apply to recursive algorithm</param>
	public Node DLTS(Problem problem,int limit)
	{
		return RDLTS (new Node (this.problem.initialState, null, null, 0), problem, limit);
	}
	/// <summary>
	/// Recursive Depth Limited tree search until depth limit is reached, every node is used as first node in search
	/// </summary>
	/// <param name="node">Node used to begin with search</param> 
	/// <param name="problem">Problem to search in</param>
	/// <param name="limit">limit to stop expanding nodes</param>
	public Node RDLTS(Node node,Problem problem,int limit)
	{
		//bool cutoffOcurred = false;
		if (problem.GoalTest (node.State))
			return node;
		else if (node.Depth == limit)
			return null;
		else {
			foreach(Node successor in node.Expand(problem))
			{
				Node result = RDLTS(successor,problem,limit);

				if(result != null)
					return result;
			}
			return null;
		}

	}
	/// <summary>
	/// Iterative Depth limited tree search, search with an incremental depth limit until limit is reached or solution is found
	/// </summary>
	/// <param name="problem">Problem to search in</param>
	/// <param name="maxlimit">maximum limit to apply to recursive algorithm</param>
	public Node IDTS(Problem problem,int maxlimit)
	{
		Node result=null;
		for(int i=1;i<=maxlimit;i++)
		{
			result = DLTS(problem,i);
			if(result != null)
				return result;
		}
		return result;
	}

	/// <summary>
	/// Depth limited graph search, search with an incremental depth limit
	/// </summary>
	/// <param name="problem">Problem to search in</param>
	/// <param name="limit">limit to apply to recursive algorithm</param>
	public Node DLGS(Problem problem,int limit)
	{
		Dictionary<string,bool> closed = new Dictionary<string,bool> ();
		return RDLGS (new Node (this.problem.initialState, null, null, 0), problem, limit,ref closed);
	}

	/// <summary>
	/// Recursive Depth Limited graph search until depth limit is reached, every node is used as first node in search
	/// </summary>
	/// <param name="node">Node used to begin with search</param> 
	/// <param name="problem">Problem to search in</param>
	/// <param name="limit">limit to stop expanding nodes</param>
	public Node RDLGS(Node node,Problem problem,int limit,ref Dictionary<string,bool> closed)
	{
		//bool cutoffOcurred = false;
		if (problem.GoalTest (node.State))
			return node;
		else if (node.Depth == limit)
			return null;
		else {
			if(!closed.ContainsKey(node.State.idState))
			{
			closed[node.State.idState] = true;
			foreach(Node successor in node.Expand(problem))
			{
				Node result = RDLGS(successor,problem,limit,ref closed);
				
				if(result != null)
					return result;
				}
			}
			return null;
		}
		
	}
	/// <summary>
	/// Iterative Depth limited graph search, search with an incremental depth limit until limit is reached or solution is found
	/// </summary>
	/// <param name="problem">Problem to search in</param>
	/// <param name="maxlimit">maximum limit to apply to recursive algorithm</param>
	public Node IDGS(Problem problem,int limit)
	{
		Node result=null;
		for(int i=1;i<=limit;i++)
		{
			result = DLTS(problem,i);
			if(result != null)
				return result;
		}
		return result;
	}

	/// <summary>
	/// A* Tree Search
	/// </summary>
	public Node AstarTS()
	{
		return TreeSearch (new PriorityCostAcumulatedHeurTree ());
	}
	/// <summary>
	/// A* Graph Search
	/// </summary>
	public Node AstarGS()
	{
		return GraphSearch (new PriorityCostAcumulatedHeurGraph ());
	}

	public void logState(State state)
	{
		Vector2 pacmanpos = state.GetPacmanPos ();
		int pacmanDotsLeft = state.GetNumberOfDots ();
		Debug.Log(string.Format("Pacman position: X {0} Y {1} , dots left: {2}",pacmanpos.x.ToString(),pacmanpos.y.ToString(),pacmanDotsLeft.ToString()));
	}

	public void logFringe(Fringe fringe)
	{
		Debug.Log(string.Format("Number of nodes open: {0}",fringe.Count().ToString()));
	}

	public bool ProblemSolved()
	{
		return solution != null;
	}

	public Node GetSolution()
	{
		return solution;
	}
}

public class ReflextAgent
{
	Problem problem;
	Node CurrentNode;
	List<ActorNames> actors;
	public ReflextAgent(Problem prob)
	{
		problem = prob;
		CurrentNode = new Node (this.problem.initialState, null, null, 0);
		actors = new List<ActorNames> ();
		foreach (ActorNames actorName in CurrentNode.State.GetActors ().Keys) {
			actors.Add(actorName);
		}
	}
	
	public Node GetNextNode()
	{
		if (!problem.GoalTest (CurrentNode.State)) {
			List<Node> successors = CurrentNode.Expand (problem);
			PriorityUtility listOfSuccessors = new PriorityUtility (MiniMax.Max,successors);
			return listOfSuccessors.Pop ();
		}
		else
		return null;
	}

	public Node MinimaxSearch(int deep,string actor)
	{
		ActorNames actorName = (ActorNames)System.Enum.Parse( typeof( ActorNames ), actor );
		int currentActorIndex = actors.IndexOf (actorName);
		PriorityUtility listNodes;
		List<Node> successors = CurrentNode.Expand (problem,actorName);
		MiniMax minmax = MiniMaxMode (currentActorIndex);
		listNodes = new PriorityUtility (minmax,successors);
		//Setup Alpha and Beta values
		double alpha = double.NegativeInfinity;
		double beta = double.PositiveInfinity;
		double currentValue;
		//Set the current value of node
		if (minmax == MiniMax.Max)
			currentValue = double.NegativeInfinity;
		else
			currentValue = double.PositiveInfinity;
		Node selectedNode = null;
		foreach (Node succ in listNodes) {
			succ.Utility = MiniMaxUtility (succ, deep, GetNextActorIndex(currentActorIndex),alpha,beta);
			if(minmax == MiniMax.Max)
			{
				if(succ.Utility > currentValue)
				{
					currentValue = succ.Utility;
					selectedNode = succ;
					alpha = currentValue;
				}
			}
			else if(minmax == MiniMax.Min)
			{
				if(succ.Utility < currentValue)
				{
					currentValue = succ.Utility;
					selectedNode = succ;
					beta = currentValue;
				}
			}
			if(minmax == MiniMax.Max && currentValue > beta)
				return selectedNode;
			if(minmax == MiniMax.Min && currentValue < alpha)
				return selectedNode;
		}

		return listNodes.Pop ();
	}

	public double MiniMaxUtility(Node node,int deep,int currentActorIndex,double alpha,double beta)
	{
		if (problem.GoalTest (node.State) || node.Depth == deep) {
			return node.Utility;
		} else {
			MiniMax minmax = MiniMaxMode (currentActorIndex);
			List<Node> successors = node.Expand (problem,actors[currentActorIndex]);
			PriorityUtility listNodes = new PriorityUtility (minmax,successors);
			double currentValue;
			//Set the current value of node
			if (minmax == MiniMax.Max)
				currentValue = double.NegativeInfinity;
			else
				currentValue = double.PositiveInfinity;
			foreach (Node succ in listNodes) {
				succ.Utility = MiniMaxUtility (succ, deep, GetNextActorIndex(currentActorIndex),alpha,beta);
				if(minmax == MiniMax.Max)
				{
					if( succ.Utility > currentValue)
					{
						currentValue = succ.Utility;
						alpha = currentValue;
					}
				}
				else if(minmax == MiniMax.Min && succ.Utility < currentValue)
				{
					if(succ.Utility < currentValue)
					{
						currentValue = succ.Utility;
						beta = currentValue;
					}
				}
				if(minmax == MiniMax.Max && currentValue > beta)
					return currentValue;
				if(minmax == MiniMax.Min && currentValue < alpha)
					return currentValue;
			}
			return listNodes.Pop().Utility;
		}

	}

	public MiniMax MiniMaxMode(int currentActorIndex)
	{
		if (actors [currentActorIndex] == ActorNames.Pacman)
			return MiniMax.Max;
		else
			return MiniMax.Min;
	}
	
	public int GetNextActorIndex(int currentActorIndex)
	{
		int ret;
		if (currentActorIndex + 1 >= actors.Count)
			ret = 0;
		else
			ret = currentActorIndex + 1;
		return ret;
	}
}


public abstract class Fringe : List<Node>
{
	public abstract Node Pop ();
}
/// <summary>
/// Return Last IN First OUT
/// </summary>
public class LIFO: Fringe{
	public override Node Pop()
	{
		Node ret = this.Last();
		this.Remove (ret);
		return ret;
	}
}
/// <summary>
/// Return First IN First Out
/// </summary>
public class FIFO: Fringe{
	public override Node Pop()
	{
		Node ret = this.First();
		this.Remove (ret);
		return ret;
	}
}

/// <summary>
/// Return node with less cost
/// </summary>
public class PriorityCost: Fringe{
	public override Node Pop()
	{
		Node ret = this.OrderBy(n=>n.Cost).First();
		this.Remove (ret);
		return ret;
	}
}

public class PriorityHeurTree: Fringe{
	public override Node Pop()
	{
		Node ret = this.OrderBy(n=>n.HeurTree).First();
		this.Remove (ret);
		return ret;
	}
}

public class PriorityHeurGraph: Fringe{
	public override Node Pop()
	{
		Node ret = this.OrderBy(n=>n.HeurGraph).First();
		this.Remove (ret);
		return ret;
	}
}

public class PriorityCostAcumulatedHeurTree: Fringe{
	public override Node Pop()
	{
		Node ret = this.OrderBy(n=>n.CostAcumulatedHeurTree).First();
		this.Remove (ret);
		return ret;
	}
}

public class PriorityCostAcumulatedHeurGraph: Fringe{
	public override Node Pop()
	{
		Node ret = this.OrderBy(n=>n.CostAcumulatedHeurGraph).First();
		this.Remove (ret);
		return ret;
	}
}

public class PriorityUtility: Fringe{
	MiniMax Order;
	public PriorityUtility(MiniMax order)
	{
		Order = order;
	}
	public PriorityUtility(MiniMax order,List<Node> source)
	{
		foreach (Node node in source) {
			this.Add(node);
		}
		Order = order;
	}
	public override Node Pop()
	{
		Node ret;
		if (Order == MiniMax.Min) {
			ret = this.OrderBy (n => n.Utility).First ();
		} else {
			ret = this.OrderByDescending (n => n.Utility).First ();
		}
		this.Remove (ret);
		return ret;
	}
}

public enum MiniMax{
	Min,
	Max
}
