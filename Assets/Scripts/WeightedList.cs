using System;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine;

[Serializable]
class WeightedList<T>
{
    [Serializable]
    public struct Entry
    {
        public Entry(float w, T i)
        {
            weight = w;
            item = i;
        }

        public float weight;
        public T item;
    }

    public List<Entry> entries = new List<Entry>();
    private float accumulatedWeight;  

    public void InitializeWeights()
    {
        foreach (Entry entry in entries)
        {
            accumulatedWeight += entry.weight;
        }
    }

    public T GetRandom()
    {
        float r = UnityEngine.Random.value * accumulatedWeight;

        float weightCounter = 0;

        foreach (Entry entry in entries)
        {
            weightCounter += entry.weight;
            if (weightCounter >= r)
            {
                return entry.item;
            }
        }
        return default(T); //should only happen when there are no entries
    }
}
