//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using System.Collections;

namespace Maynek.Notesvel
{
    public abstract class MyDictionaryBase<T> : IEnumerable<T> where T : class
    {
        //================================
        // Properties
        //================================
        private List<T> List { get; } = [];
        private Dictionary<string, T> Dictionary { get; } = [];

        public T this[string key]
        {
            get { return this.Dictionary[key]; }
            set { this.Dictionary[key] = value; }
        }

        public T this[int index]
        {
            get { return this.List[index]; }
            set { this.List[index] = value; }
        }

        public int Count
        {
            get { return this.List.Count; }
        }


        //================================
        // Methods
        //================================
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.List.GetEnumerator();
        }

        public void Add(string key, T value)
        {
            this.List.Add(value);
            this.Dictionary.Add(key, value);
        }

        public bool Contains(string key)
        {
            return this.Dictionary.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            if (this.Dictionary.TryGetValue(key, out T? item))
            {
                if (this.List.Remove(item))
                {
                    this.Dictionary.Remove(key);
                    return true;
                }
            }

            return false;
        }

        public void Sort(IComparer<T> comparer)
        {
            this.List.Sort(comparer);
        }

        public void Sort(Comparison<T> comparison)
        {
            this.List.Sort(comparison);
        }

    }
}
