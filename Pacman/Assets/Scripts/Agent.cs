using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SearchAgent
{
	Problem problem;
	public SearchAgent(Problem prob)
	{
		problem = prob;
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
		Dictionary<State,bool> closed = new Dictionary<State,bool> ();
		//Set the initial node
		Node initialNode = new Node (this.problem.initialState, null, null, 0);
		//Add to the fringe
		fringe.Add (initialNode);
		//Loop fringe to get a 
		while (fringe.Any()) {
			Node node = fringe.Pop ();
			if (problem.GoalTest (node.State))
				return node;
			if(!closed.ContainsKey(node.State))
			{
				closed[node.State] = true;
				List<Node> childNodes = node.Expand (problem);
				childNodes.ForEach (c => fringe.Add (c));
			}
		}
		return null;
	}

	public Node DFS()
	{
		return TreeSearch(new LIFO());
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



