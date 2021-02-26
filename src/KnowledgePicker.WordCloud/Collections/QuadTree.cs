using KnowledgePicker.WordCloud.Primitives;
using System.Collections.Generic;

namespace KnowledgePicker.WordCloud.Collections
{
    /// <summary>
    /// A Quadtree is a structure designed to partition space so that it's
    /// faster to find out what is inside or outside a given area. See
    /// http://en.wikipedia.org/wiki/Quadtree.
    /// </summary>
    /// <remarks>
    /// This Quadtree contains items that have an area (<see
    /// cref="RectangleD"/>). It will store a reference to the item in the quad
    /// that is just big enough to hold it. Each quad has a bucket that contains
    /// multiple items.
    /// </remarks>
    public class QuadTree<T> where T : LayoutItem
    {
        private readonly RectangleD rectangle;
        private readonly QuadTreeNode<T> root;

        public QuadTree(RectangleD rectangle)
        {
            this.rectangle = rectangle;
            root = new QuadTreeNode<T>(this.rectangle);
        }

        public int Count => root.Count;

        public void Insert(T item)
        {
            root.Insert(item);
        }

        public IEnumerable<T> Query(RectangleD area)
        {
            return root.Query(area);
        }

        public bool HasContent(RectangleD area)
        {
            return root.HasContent(area);
        }
    }
}
