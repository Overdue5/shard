//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by a tool.
//     Runtime Version: 1.1.4322.573
//
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

using System;
using System.Collections;

namespace Server.Engines.Reports
{
    /// <summary>
    /// Strongly typed collection of Server.Engines.Reports.ChartItem.
    /// </summary>
    public class ChartItemCollection : CollectionBase
    {
        /// <summary>
        /// Gets or sets the value of the Server.Engines.Reports.ChartItem at a specific position in the ChartItemCollection.
        /// </summary>
        public ChartItem this[int index]
        {
            get => ((ChartItem)(List[index]));
            set => List[index] = value;
        }

		public int Add( string name, int value )
		{
			return Add( new ChartItem( name, value ) );
		}

        /// <summary>
        /// Append a Server.Engines.Reports.ChartItem entry to this collection.
        /// </summary>
        /// <param name="value">Server.Engines.Reports.ChartItem instance.</param>
        /// <returns>The position into which the new element was inserted.</returns>
        public int Add(ChartItem value)
        {
            return List.Add(value);
        }
        
        /// <summary>
        /// Determines whether a specified Server.Engines.Reports.ChartItem instance is in this collection.
        /// </summary>
        /// <param name="value">Server.Engines.Reports.ChartItem instance to search for.</param>
        /// <returns>True if the Server.Engines.Reports.ChartItem instance is in the collection; otherwise false.</returns>
        public bool Contains(ChartItem value)
        {
            return List.Contains(value);
        }
        
        /// <summary>
        /// Retrieve the index a specified Server.Engines.Reports.ChartItem instance is in this collection.
        /// </summary>
        /// <param name="value">Server.Engines.Reports.ChartItem instance to find.</param>
        /// <returns>The zero-based index of the specified Server.Engines.Reports.ChartItem instance. If the object is not found, the return value is -1.</returns>
        public int IndexOf(ChartItem value)
        {
            return List.IndexOf(value);
        }
        
        /// <summary>
        /// Removes a specified Server.Engines.Reports.ChartItem instance from this collection.
        /// </summary>
        /// <param name="value">The Server.Engines.Reports.ChartItem instance to remove.</param>
        public void Remove(ChartItem value)
        {
            List.Remove(value);
        }
        
        /// <summary>
        /// Returns an enumerator that can iterate through the Server.Engines.Reports.ChartItem instance.
        /// </summary>
        /// <returns>An Server.Engines.Reports.ChartItem's enumerator.</returns>
        public new ChartItemCollectionEnumerator GetEnumerator()
        {
            return new ChartItemCollectionEnumerator(this);
        }
        
        /// <summary>
        /// Insert a Server.Engines.Reports.ChartItem instance into this collection at a specified index.
        /// </summary>
        /// <param name="index">Zero-based index.</param>
        /// <param name="value">The Server.Engines.Reports.ChartItem instance to insert.</param>
        public void Insert(int index, ChartItem value)
        {
            List.Insert(index, value);
        }
        
        /// <summary>
        /// Strongly typed enumerator of Server.Engines.Reports.ChartItem.
        /// </summary>
        public class ChartItemCollectionEnumerator : IEnumerator
        {
            
            /// <summary>
            /// Current index
            /// </summary>
            private int _index;
            
            /// <summary>
            /// Current element pointed to.
            /// </summary>
            private ChartItem _currentElement;
            
            /// <summary>
            /// Collection to enumerate.
            /// </summary>
            private readonly ChartItemCollection _collection;
            
            /// <summary>
            /// Default constructor for enumerator.
            /// </summary>
            /// <param name="collection">Instance of the collection to enumerate.</param>
            internal ChartItemCollectionEnumerator(ChartItemCollection collection)
            {
                _index = -1;
                _collection = collection;
            }
            
            /// <summary>
            /// Gets the Server.Engines.Reports.ChartItem object in the enumerated ChartItemCollection currently indexed by this instance.
            /// </summary>
            public ChartItem Current
            {
                get
                {
                    if (((_index == -1) 
                                || (_index >= _collection.Count)))
                    {
                        throw new IndexOutOfRangeException("Enumerator not started.");
                    }
                    else
                    {
                        return _currentElement;
                    }
                }
            }
            
            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    if (((_index == -1) 
                                || (_index >= _collection.Count)))
                    {
                        throw new IndexOutOfRangeException("Enumerator not started.");
                    }
                    else
                    {
                        return _currentElement;
                    }
                }
            }
            
            /// <summary>
            /// Reset the cursor, so it points to the beginning of the enumerator.
            /// </summary>
            public void Reset()
            {
                _index = -1;
                _currentElement = null;
            }
            
            /// <summary>
            /// Advances the enumerator to the next queue of the enumeration, if one is currently available.
            /// </summary>
            /// <returns>true, if the enumerator was succesfully advanced to the next queue; false, if the enumerator has reached the end of the enumeration.</returns>
            public bool MoveNext()
            {
                if ((_index 
                            < (_collection.Count - 1)))
                {
                    _index = (_index + 1);
                    _currentElement = _collection[_index];
                    return true;
                }
                _index = _collection.Count;
                return false;
            }
        }
    }
}
