using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//priority queue implemented using generic list with min binary heap
public class PriorityQueue<T> where T : IComparable<T>
{
    //list of items in our queue
    List<T> data;
    public int Count { get { return data.Count; }}

    //constructor
    public PriorityQueue()
    {
        this.data = new List<T>();
    }

    //add an item to the queue and sort using a min binary heap
    public void Enqueue(T item)
    {
        data.Add(item);

        // start at the last position in the heap
        int childindex = data.Count - 1;

        //using  binary heap to sort out the order
        while (childindex > 0)
        {
            //parent position
            int parentindex = (childindex - 1) / 2;

            //order well sorted, stop
            if (data[childindex].CompareTo(data[parentindex]) >= 0)
            {
                break;
            }
            //swap parent and child 
            T tmp = data[childindex];
            data[childindex] = data[parentindex];
            data[parentindex] = tmp;

            childindex = parentindex;

        }
    }
    // remove an item from queue and keep it sorted using a min binary heap
    public T Dequeue() 
    {
        // remove an item from queue and keep it sorted using a min binary heap
        int lastindex = data.Count - 1;

        // store the first item in the List in a variable
        T frontItem = data[0];

        //replace the first item with the last item
        data[0] = data[lastindex];

        //shorten the queue and remove the last position
        data.RemoveAt(lastindex);

        lastindex--;

        int parentindex = 0;

        while (true)
        {
            //chose the left child
            int childindex = parentindex * 2 + 1;

            if (childindex > lastindex)
            {
                break;
            }

            // compare left and righht childindex
            int rightchild = childindex + 1;

            if (rightchild <= lastindex && data[rightchild].CompareTo(data[childindex]) < 0)
            {
                childindex = rightchild;
            }
            // if the parent and child are already sorted, then stop sorting
            if (data[parentindex].CompareTo(data[childindex]) <= 0)
            {
                break;
            }

            T tmp = data[parentindex];
            data[parentindex] = data[childindex];
            data[childindex] = tmp;

            // move down the head on top until sorted
            parentindex = childindex;

        }
        //return the original first item
        return frontItem;
    }

    // look at the first item without dequeuing 
    public T Peek()
    {
        T frontItem = data[0];
        return frontItem;
    }

    public bool Contains(T item)
    {
        return data.Contains(item);
    }

    //return the data as a generic list
    public List<T> ToList()
    {
        return data;
    }

}
