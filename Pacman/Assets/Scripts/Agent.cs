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
			List<Node> childNodes = node.Expand (problem);
			childNodes.ForEach (c => fringe.Add (c));
		}
		return null;
	}

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
				List<Node> childNodes = node.Expand (problem);
				foreach(Node childNode in childNodes)
				{
					//if(!closed.ContainsKey(childNode.State.idState))
						fringe.Add(childNode);
				}
				logFringe(fringe);
			}
		}
		return null;
	}

	public Node DFTS()
	{
		return TreeSearch (new LIFO ());
	}

	public Node BFTS()
	{
		return TreeSearch(new FIFO());
	}

	public Node DFGS()
	{
		return GraphSearch(new LIFO());
	}
	
	public Node BFGS()
	{
		return GraphSearch(new FIFO());
	}

	public Node UCTS()
	{
		return TreeSearch (new PriorityG ());
	}

	public Node UCGS()
	{
		return TreeSearch (new PriorityG ());
	}

	public Node DLTS(Problem problem,int limit)
	{
		return RDLTS (new Node (this.problem.initialState, null, null, 0), problem, limit);
	}

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

	public Node IDTS(Problem problem,int limit)
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

	public Node DLGS(Problem problem,int limit)
	{
		Dictionary<string,bool> closed = new Dictionary<string,bool> ();
		return RDLGS (new Node (this.problem.initialState, null, null, 0), problem, limit,ref closed);
	}
	
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
public class PriorityG: Fringe{
	public override Node Pop()
	{
		Node ret = this.OrderBy(n=>n.Cost).First();
		this.Remove (ret);
		return ret;
	}
}



