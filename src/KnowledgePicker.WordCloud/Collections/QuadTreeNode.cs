using KnowledgePicker.WordCloud.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KnowledgePicker.WordCloud.Collections
{
    public class QuadTreeNode<T> where T : LayoutItem
    {
        private readonly Stack<T> contents = new Stack<T>();
        private QuadTreeNode<T>[] nodes = Array.Empty<QuadTreeNode<T>>();

        public QuadTreeNode(RectangleD bounds)
        {
            Bounds = bounds;
        }

        public bool IsEmpty
        {
            get { return Bounds.IsEmpty || nodes.Length == 0; }
        }

        public RectangleD Bounds { get; }

        public int Count
        {
            get
            {
                int count = 0;

                foreach (QuadTreeNode<T> node in nodes)
                    count += node.Count;

                count += contents.Count;

                return count;
            }
        }

        public IEnumerable<T> SubTreeContents
        {
            get
            {
                IEnumerable<T> results = Enumerable.Empty<T>();

                foreach (QuadTreeNode<T> node in nodes)
                    results = results.Concat(node.SubTreeContents);

                results = results.Concat(contents);
                return results;
            }
        }

        public bool HasContent(RectangleD queryArea)
        {
            return Query(queryArea).Any();
        }

        /// <summary>
        /// Query the QuadTree for items that are in the given area
        /// </summary>
        public IEnumerable<T> Query(RectangleD queryArea)
        {
            // This quad contains items that are not entirely contained by
            // it's four sub-quads. Iterate through the items in this quad 
            // to see if they intersect.
            foreach (T item in contents)
            {
                if (queryArea.IntersectsWith(item.Rectangle))
                    yield return item;
            }

            foreach (QuadTreeNode<T> node in nodes)
            {
                if (node.IsEmpty)
                    continue;

                // Case 1: Search area completely contained by sub-quad if a
                // node completely contains the query area, go down that branch
                // and skip the remaining nodes (break this loop).
                if (node.Bounds.Contains(queryArea))
                {
                    IEnumerable<T> subResults = node.Query(queryArea);
                    foreach (var subResult in subResults)
                    {
                        yield return subResult;
                    }
                    break;
                }

                // Case 2: Sub-quad completely contained by search area if the
                // query area completely contains a sub-quad, just add all the
                // contents of that quad and it's children to the result set.
                // You need to continue the loop to test the other quads.
                if (queryArea.Contains(node.Bounds))
                {
                    IEnumerable<T> subResults = node.SubTreeContents;
                    foreach (var subResult in subResults)
                    {
                        yield return subResult;
                    }
                    continue;
                }

                // Case 3: Search area intersects with sub-quad traverse into
                // this quad, continue the loop to search other quads.
                if (node.Bounds.IntersectsWith(queryArea))
                {
                    IEnumerable<T> subResults = node.Query(queryArea);
                    foreach (var subResult in subResults)
                    {
                        yield return subResult;
                    }
                }
            }
        }

        public void Insert(T item)
        {
            // If the item is not contained in this quad, there's a problem.
            if (!Bounds.Contains(item.Rectangle))
            {
                throw new ArgumentOutOfRangeException(nameof(item),
                    "Feature is out of the bounds of this quadtree node.");
            }

            // If the subnodes are null create them. May not be sucessfull: see
            // below. We may be at the smallest allowed size in which case the
            // subnodes will not be created.
            if (nodes.Length == 0)
                CreateSubNodes();

            // For each subnode: If the node contains the item, add the item to
            // that node and return. This recurses into the node that is just
            // large enough to fit this item.
            foreach (QuadTreeNode<T> node in nodes)
            {
                if (node.Bounds.Contains(item.Rectangle))
                {
                    node.Insert(item);
                    return;
                }
            }

            // If we make it to here, either
            // 1) none of the subnodes completely contained the item, or
            // 2) we're at the smallest subnode size allowed add the item to
            //    this node's contents.
            contents.Push(item);
        }

        private void CreateSubNodes()
        {
            // The smallest subnode has an area.
            if (Bounds.Height * Bounds.Width <= 10)
                return;

            double halfWidth = Bounds.Width / 2f;
            double halfHeight = Bounds.Height / 2f;

            nodes = new[]
            {
                new QuadTreeNode<T>(new RectangleD(Bounds.Location, new SizeD(halfWidth, halfHeight))),
                new QuadTreeNode<T>(new RectangleD(new PointD(Bounds.Left, Bounds.Top + halfHeight), new SizeD(halfWidth, halfHeight))),
                new QuadTreeNode<T>(new RectangleD(new PointD(Bounds.Left + halfWidth, Bounds.Top), new SizeD(halfWidth, halfHeight))),
                new QuadTreeNode<T>(new RectangleD(new PointD(Bounds.Left + halfWidth, Bounds.Top + halfHeight), new SizeD(halfWidth, halfHeight)))
            };
        }
    }
}
