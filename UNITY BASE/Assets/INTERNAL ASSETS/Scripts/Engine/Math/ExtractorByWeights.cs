using UnityEngine;
using System.Collections.Generic;

namespace EnglishKids.SortingTransport
{
    public class ExtractorByWeights<T>
    {
        private class DataBlock
        {
            public T item;
            public float weight;

            public DataBlock(T itemValue, float weightValue)
            {
                item = itemValue;
                weight = weightValue;
            }
        }

        //==================================================
        // Fields
        //==================================================

        private List<DataBlock> _list;
        private float _weightSum;

        //==================================================
        // Properties
        //==================================================

        public int Length
        {
            get { return _list.Count; }
        }

        //==================================================
        // Constructors
        //==================================================
                
        public ExtractorByWeights()
        {
            _list = new List<DataBlock>();
            _weightSum = 0f;
        }
        
        //==================================================
        // Methods
        //==================================================

        public void Add(T item, float weight)
        {
            if (weight > 0f)
            {
                _list.Add(new DataBlock(item, weight));
                _weightSum += weight;
            }
            else
            {
                Debug.LogError("Weights must be greater than zero");
            }
        }

        public T Get()
        {
            if (_list.Count == 0)
            {
                Debug.Log("Weight list is empty");
                return default(T);
            }

            float value = Random.Range(0f, _weightSum);
            float itemWeightPosition = 0f;
            int index = 0;

            for (int i = 0; i < _list.Count; i++)
            {
                if (value <= _list[i].weight + itemWeightPosition)
                {
                    index = i;
                    break;
                }

                itemWeightPosition += _list[i].weight;
            }

            return _list[index].item;
        }

        public void Clear()
        {
            _list.Clear();
            _weightSum = 0f;
        }
    }
}