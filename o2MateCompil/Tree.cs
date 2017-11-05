using System;
using System.Collections.Generic;
using System.Text;

namespace o2Mate
{
    /// <summary>
    /// A leaf of a tree
    /// </summary>
    /// <typeparam name="T">data type content</typeparam>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class Leaf<T> : List<Node<T>>
    {
        #region Private Fields
        T _obj;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the object in a leaf node
        /// </summary>
        public T Object
        {
            get { return this._obj; }
            set { this._obj = value; }
        }
        #endregion
    }

    /// <summary>
    /// A node in a tree
    /// </summary>
    /// <typeparam name="T">data type content</typeparam>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class Node<T> : Leaf<T>
    {
        #region Private Fields
        Node<T> _subNode;
        #endregion

        #region Public Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Node()
        {
            // deprecated
            this._subNode = null;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the sub node
        /// [deprecated]
        /// </summary>
        public Node<T> SubNode
        {
            get
            {
                if (this._subNode == null)
                    this._subNode = new Node<T>();
                return this._subNode;
            }
        }
        #endregion
    }

    /// <summary>
    /// A tree class
    /// </summary>
    /// <typeparam name="T">data type content</typeparam>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class Tree<T>
    {
        #region Private Fields
        Stack<Node<T>> _parents;
        Node<T> _root;
        Node<T> _current;
        Node<T> _lastNode;
        #endregion

        #region Public Constructor
        /// <summary>
        /// Constructor with no argument
        /// </summary>
        public Tree()
        {
            this._parents = new Stack<Node<T>>();
            this._root = new Node<T>();
            this._current = this._root;
            this._lastNode = null;
        }

        /// <summary>
        /// Constructs a new tree and attach a node as root
        /// </summary>
        /// <param name="node">node tree</param>
        public Tree(Node<T> node)
        {
            this._parents = new Stack<Node<T>>();
            this._root = node;
            this._current = this._root;
            this._lastNode = null;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Root node tree
        /// </summary>
        public Node<T> Root
        {
            get { return this._root; }
        }
        /// <summary>
        /// Current node
        /// </summary>
        public Node<T> Current
        {
            get { return this._current; }
        }

        /// <summary>
        /// Number of root nodes
        /// </summary>
        public int RootCount
        {
            get { return this._root.Count; }
        }

        /// <summary>
        /// Inserted last node 
        /// </summary>
        public Node<T> Last
        {
            get { return this._lastNode; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Enumerator
        /// </summary>
        /// <returns>an enumerator</returns>
        public List<Node<T>>.Enumerator GetEnumerator()
        {
            return this._root.GetEnumerator();
        }

        /// <summary>
        /// Adds a new data content object as a child of the current node
        /// </summary>
        /// <param name="obj">object</param>
        public void Add(T obj)
        {
            Node<T> node = new Node<T>();
            node.Object = obj;
            this._lastNode = node;
            this._current.Add(node);
        }

        /// <summary>
        /// Move into the inserted last child of the current node
        /// </summary>
        public void Push()
        {
            this._parents.Push(this._current);
            this._current = this._lastNode;
            this._lastNode = null;
        }

        /// <summary>
        /// Up move
        /// </summary>
        public void Pop()
        {
            this._current = this._parents.Pop();
        }
        #endregion
    }
}
