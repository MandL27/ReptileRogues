using Godot;
using System;
using System.Collections.Generic;

public class PriorityQueue<T>
{
	private LinkedList<PQItem<T>> Queue = new LinkedList<PQItem<T>>();
	public int Count
	{
		get => Queue.Count;
	}

	public void Push(PQItem<T> item)
	{
		for (LinkedListNode<PQItem<T>> i = Queue.First; i != null; i = i.Next)
		{
			if (i.Value.Priority > item.Priority)
			{
				Queue.AddBefore(i, item);
				return;
			}
		}
		Queue.AddLast(item);
	}

	public void Push(T item, int priority)
	{
		Push(new PQItem<T>
		{
			Data = item,
			Priority = priority
		});
	}

	public T Pop()
	{
		try
		{
			PQItem<T> head = Queue.First.Value;
			Queue.RemoveFirst();
			return head.Data;
		}
		catch
		{
			return default(T);
		}
	}

	public T Peek()
	{
		try
		{
			return Queue.First.Value.Data;
		}
		catch
		{
			return default(T);
		}
	}

	public bool IsEmpty()
	{
		return Queue.Count == 0;
	}
}

public struct PQItem<T>
{
	public T Data { get; set; }
	public int Priority { get; set; }
}