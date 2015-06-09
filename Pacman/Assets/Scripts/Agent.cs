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

	private Node TreeSearch(Fringe fringe)
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

	private Node GraphSearch(Fringe fringe)
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
				childNodes.ForEach (c => fringe.Add (c));
				logFringe(fringe);
			}
		}
		return null;
	}

	public Node DFS()
	{
		return TreeSearch (new LIFO ());
	}

	public Node BFS()
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

public class LIFO: Fringe{
	public override Node Pop()
	{
		Node ret = this.Last();
		this.Remove (ret);
		return ret;
	}
}

public class FIFO: Fringe{
	public override Node Pop()
	{
		Node ret = this.First();
		this.Remove (ret);
		return ret;
	}
}



