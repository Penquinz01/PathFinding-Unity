using JetBrains.Annotations;
using System;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int currentItemCount;

    public Heap(int maxSize)
    {
        items = new T[maxSize];
    }

    public void AddItems(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;
        while (true)
        {
            T parentItem = items[parentIndex];
            if(item.CompareTo(parentItem) > 0)
            {
                Swap(parentItem, item);
            }
            else
            {
                break;
            }
            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }
    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
    }

    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = 2 * item.HeapIndex + 1;
            int childIndexRight = 2 * item.HeapIndex + 2;
            int swapIndex = 0;
            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;
                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else return;
            
        }
    }
    void Swap(T A,T B)
    {
        items[A.HeapIndex] = B;
        items[B.HeapIndex] = A;
        int temp  = A.HeapIndex;
        A.HeapIndex = B.HeapIndex;
        B.HeapIndex = temp;
    }
}

public interface IHeapItem<T>:IComparable<T>
{
    int HeapIndex { get; set; }

}
