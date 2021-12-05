//
// ObjectPool.cs
//
// Projector For LWRP
//
// Copyright (c) 2020 NYAHOON GAMES PTE. LTD.
//

using System.Collections.Generic;

namespace ProjectorForLWRP
{
	public static class ObjectPool<T> where T : new()
	{
		public static System.Action<T> clearFunction;
		static Stack<T> m_pool = new Stack<T>();
		public static T Get()
		{
			if (0 < m_pool.Count)
			{
				return m_pool.Pop();
			}
			return new T();
		}
		public static void Release(T obj)
		{
			if (obj != null)
			{
				clearFunction?.Invoke(obj);
				m_pool.Push(obj);
			}
		}
		public static void Clear()
		{
			m_pool.Clear();
		}

		//
		// Helper collection classes
		//
		public class CollectionBase<CollectionType>
		where CollectionType : ICollection<T>, new()
		{
			public int Count { get { return baseCollection.Count; } }
			public T AddNew()
			{
				T item = ObjectPool<T>.Get();
				baseCollection.Add(item);
				return item;
			}
			public bool Contains(T item)
			{
				return baseCollection.Contains(item);
			}
			public bool Remove(T item)
			{
				if (baseCollection.Remove(item))
				{
					ObjectPool<T>.Release(item);
					return true;
				}
				return false;
			}
			// Do not implement Clear here, because enumeration of ICollection<T> will generate garbage.
			// Implement Clear in derived class.
			//public void Clear()
			//{
			//    foreach (T item in baseCollection)
			//    {
			//        ObjectPool<T>.Release(item);
			//    }
			//}

			protected CollectionType baseCollection;

			public CollectionBase()
			{
				baseCollection = new CollectionType();
			}
		}

		public class List : CollectionBase<List<T>>
		{
			public T this[int index]
			{
				get
				{
					return baseCollection[index];
				}
				// hide set
			}
			public void RemoveAt(int index)
			{
				T item = this[index];
				baseCollection.RemoveAt(index);
				ObjectPool<T>.Release(item);
			}
			public void Clear()
			{
				foreach (T item in baseCollection)
				{
					ObjectPool<T>.Release(item);
				}
				baseCollection.Clear();
			}

			public List<T>.Enumerator GetEnumerator()
			{
				return baseCollection.GetEnumerator();
			}
		}
		public class AutoClearList : List
		{
			static AutoClearList()
			{
				ObjectPool<AutoClearList>.clearFunction = x => x.Clear();
			}
		}

		public class Map<KeyType>
		{
			Dictionary<KeyType, T> baseMap = new Dictionary<KeyType, T>();
			public T this[KeyType key]
			{
				get
				{
					T item;
					if (!baseMap.TryGetValue(key, out item))
					{
						item = ObjectPool<T>.Get();
						baseMap.Add(key, item);
					}
					return item;
				}
				// hide set
			}
			public void Clear()
			{
				foreach (var pair in baseMap)
				{
					ObjectPool<T>.Release(pair.Value);
				}
				baseMap.Clear();
			}
			public bool ContainsKey(KeyType key)
			{
				return baseMap.ContainsKey(key);
			}

			public void Remove(KeyType key)
			{
				T item;
				if (baseMap.TryGetValue(key, out item))
				{
					ObjectPool<T>.Release(item);
					baseMap.Remove(key);
				}
			}

			public Dictionary<KeyType, T>.KeyCollection Keys
			{
				get { return baseMap.Keys; }
			}

			public Dictionary<KeyType, T>.ValueCollection Values
			{
				get { return baseMap.Values; }
			}

			public Dictionary<KeyType, T>.Enumerator GetEnumerator()
			{
				return baseMap.GetEnumerator();
			}
		}
		public class AutoClearMap<KeyType> : Map<KeyType>
		{
			static AutoClearMap()
			{
				ObjectPool<AutoClearMap<KeyType>>.clearFunction = x => x.Clear();
			}
		}
	}
	namespace Collections
	{
		public class AutoClearList<T> : List<T>
		{
			public AutoClearList()
			{
				ObjectPool<AutoClearList<T>>.clearFunction = x => x.Clear();
			}
		}
		public class AutoClearSet<T> : HashSet<T>
		{
			public AutoClearSet()
			{
				ObjectPool<AutoClearList<T>>.clearFunction = x => x.Clear();
			}
		}
		public class AutoClearMap<TKey, TValue> : Dictionary<TKey, TValue>
		{
			public AutoClearMap()
			{
				ObjectPool<AutoClearMap<TKey, TValue>>.clearFunction = x => x.Clear();
			}
		}
	}
}
