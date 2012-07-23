// This is taken from the sample at http://www.xnawiki.com/index.php/QuadTree  . many thanks.

using System.Collections.Generic;
using Microsoft.Xna.Framework;

/// <summary>
/// A simple interface defining a position and a bounding sphere.
/// Any object that implements this interface can be used with the QuadTree class.
/// This interface would typically be moved into another source file.
/// Note: Currently the QuadTree does not use the BoundSphere member, so it does not need to be in the interface.
/// </summary>
public interface ISpatialNode
{
    Vector3 Position { get; }
    BoundingSphere BoundSphere { get; }
}

/// <summary>
/// A generic dynamic recursive QuadTree
/// </summary>
public class QuadTree<T> where T : ISpatialNode
{
    #region Fields

    public readonly BoundingBox BoundingBox;
    public readonly List<T> Objects;
    public int MaxObjects;
    public QuadTree<T> Parent;
    public QuadTree<T> TopLeft;
    public QuadTree<T> TopRight;
    public QuadTree<T> BottomLeft;
    public QuadTree<T> BottomRight;
    static List<QuadTree<T>> leavesInsideBound = new List<QuadTree<T>>();

    #endregion

    #region Initialization

    /// <summary>
    /// Constructor
    /// </summary>
    /// <typeparam name="maxObjects">The maximum number of objects per leaf. If an object is inserted into a leaf that
    /// already constains maxObject, then the leaf will be split, and the contained objects moved down to the appropriate
    /// child leaves</typeparam>
    /// <typeparam name="box">The bounding box that defines the size of the QuadTree</typeparam>
    public QuadTree(int maxObjects, BoundingBox box)
    {
        MaxObjects = maxObjects;
        box.Min.Y = float.MinValue;
        box.Max.Y = float.MaxValue;
        BoundingBox = box;
        Objects = new List<T>(maxObjects);
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <typeparam name="maxObjects">The maximum number of objects per leaf. If an object is inserted into a leaf that
    /// already constains maxObject, then the leaf will be split, and the contained objects moved down to the appropriate
    /// child leaves</typeparam>
    /// <typeparam name="position">The center of the QuadTree in world space</typeparam>
    /// <typeparam name="scale">The size of the QuadTree</typeparam>
    public QuadTree(int maxObjects, Vector3 position, Vector3 scale)
        : this(maxObjects, new BoundingBox(position - scale * 0.5f, position + scale * 0.5f))
    {
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Return True if this QuadTree is a Leaf (has no child nodes), or false if it is not a leaf.
    /// </summary>
    public bool IsLeaf
    {
        get { return TopLeft == null; }
    }

    /// <summary>
    /// Return the number of leaves contained by a QuadTree Node.
    /// </summary>
    public int LeafCount
    {
        get
        {
            return IsLeaf ? 4 : (TopLeft.LeafCount + TopRight.LeafCount + BottomLeft.LeafCount + BottomRight.LeafCount);
        }
    }

    #endregion

    #region Add Object

    /// <summary>
    /// Add a new object to the QuadTree. 
    /// This method will find the correct leaf, add the object to the leaf's list and then return the leaf.
    /// </summary>
    public QuadTree<T> AddObject(T spatial)
    {
        QuadTree<T> result = null;

        if (BoundingBox.Contains(spatial.Position) == ContainmentType.Contains)
        {
            if (TopLeft == null)
            {
                if (Objects.Count < MaxObjects)
                {
                    Objects.Add(spatial);
                    return this;
                }

                Split();

                if (TopLeft == null)
                {
                    MaxObjects *= 2;
                    return AddObject(spatial);
                }

                // continue down
            }

            result = TopLeft.AddObject(spatial);
            if (result == null)
            {
                result = TopRight.AddObject(spatial);
                if (result == null)
                {
                    result = BottomLeft.AddObject(spatial);
                    if (result == null)
                    {
                        result = BottomRight.AddObject(spatial);
                    }
                }
            }
        }
        return result;
    }

    #endregion

    #region Remove Object

    /// <summary>
    /// Remove an object from the QuadTree.
    /// This method will find the containing leaf, and remove the object from the leaf's list.
    /// </summary>
    public void RemoveObject(T spatial)
    {
        QuadTree<T> leaf = FindLeaf(spatial.Position);
        if (leaf != null)
        {
            leaf.Objects.Remove(spatial);
        }
    }

    #endregion

    #region Object Moved

    /// <summary>
    /// If an object is moved by your program, call this method, passing the object and the objects current leaf.
    /// The object will be reassigned to the correct leaf if it has moved outside the bounds of its current leaf.
    /// </summary>
    /// <typeparam name="prevNode">The leaf containing the object before it was moved</typeparam>
    public QuadTree<T> ObjectMoved(T spatial, QuadTree<T> prevNode)
    {
        QuadTree<T> result = null;

        if (BoundingBox.Contains(spatial.Position) == ContainmentType.Contains)
        {
            if (TopLeft == null)
            {
                if (this != prevNode)
                {
                    if (prevNode != null)
                        prevNode.Objects.Remove(spatial);

                    return AddObject(spatial);
                }
                return this;
            }

            if ((result = TopLeft.ObjectMoved(spatial, prevNode)) == null)
                if ((result = TopRight.ObjectMoved(spatial, prevNode)) == null)
                    if ((result = BottomLeft.ObjectMoved(spatial, prevNode)) == null)
                        result = BottomRight.ObjectMoved(spatial, prevNode);
        }
        return result;
    }

    #endregion

    #region Find Leaves

    /// <summary>
    /// Return the QuadTree leaf that contains the specified position
    /// </summary>
    public QuadTree<T> FindLeaf(Vector3 position)
    {
        if (BoundingBox.Contains(position) == ContainmentType.Contains)
        {
            if (TopLeft == null)
            {
                return this;
            }
            else
            {
                QuadTree<T> result;
                if ((result = TopLeft.FindLeaf(position)) == null)
                    if ((result = TopRight.FindLeaf(position)) == null)
                        if ((result = BottomLeft.FindLeaf(position)) == null)
                            result = BottomRight.FindLeaf(position);

                return result;
            }
        }
        return null;
    }

    /// <summary>
    /// Return a list of all QuadTree leaves that intersect the specified frustum
    /// </summary>
    public List<QuadTree<T>> GetLeavesInsideFrustrum(BoundingFrustum frustum)
    {
        leavesInsideBound.Clear();
        AddLeavesInsideFrustrum(frustum);
        return leavesInsideBound;
    }

    void AddLeavesInsideFrustrum(BoundingFrustum frustum)
    {
        if (frustum.Contains(BoundingBox) != ContainmentType.Disjoint)
        {
            if (TopLeft == null && Objects.Count > 0)
            {
                leavesInsideBound.Add(this);
            }
            else
            {
                TopLeft.AddLeavesInsideFrustrum(frustum);
                TopRight.AddLeavesInsideFrustrum(frustum);
                BottomLeft.AddLeavesInsideFrustrum(frustum);
                BottomRight.AddLeavesInsideFrustrum(frustum);
            }
        }
    }

    /// <summary>
    /// Return a list of all QuadTree leaves that intersect the specified bounding sphere
    /// </summary>
    public List<QuadTree<T>> GetLeavesInsideSphere(BoundingSphere sphere)
    {
        leavesInsideBound.Clear();
        AddLeavesInsideSphere(sphere);
        return leavesInsideBound;
    }

    void AddLeavesInsideSphere(BoundingSphere sphere)
    {
        if (sphere.Contains(BoundingBox) != ContainmentType.Disjoint)
        {
            if (TopLeft == null)
            {
                leavesInsideBound.Add(this);
            }
            else
            {
                TopLeft.AddLeavesInsideSphere(sphere);
                TopRight.AddLeavesInsideSphere(sphere);
                BottomLeft.AddLeavesInsideSphere(sphere);
                BottomRight.AddLeavesInsideSphere(sphere);
            }
        }
    }

    /// <summary>
    /// Return a list of all QuadTree leaves that intersect the specified band
    /// </summary>
    /// <typeparam name="innerSphere">Inner band edge</typeparam>
    /// <typeparam name="outerSphere">Outer band edge</typeparam>
    public List<QuadTree<T>> GetLeavesInsideSphereBand(BoundingSphere innerSphere, BoundingSphere outerSphere)
    {
        leavesInsideBound.Clear();
        AddLeavesInsideSphereBand(innerSphere, outerSphere);
        return leavesInsideBound;
    }

    void AddLeavesInsideSphereBand(BoundingSphere innerSphere, BoundingSphere outerSphere)
    {
        if (outerSphere.Contains(BoundingBox) != ContainmentType.Disjoint
            && innerSphere.Contains(BoundingBox) != ContainmentType.Contains)
        {
            if (TopLeft == null)
            {
                leavesInsideBound.Add(this);
            }
            else
            {
                TopLeft.AddLeavesInsideSphereBand(innerSphere, outerSphere);
                TopRight.AddLeavesInsideSphereBand(innerSphere, outerSphere);
                BottomLeft.AddLeavesInsideSphereBand(innerSphere, outerSphere);
                BottomRight.AddLeavesInsideSphereBand(innerSphere, outerSphere);
            }
        }
    }

    /// <summary>
    /// Return a list of all QuadTree leaves that intersect the specified bounding box
    /// </summary>
    public List<QuadTree<T>> GetLeavesInsideBox(BoundingBox box)
    {
        leavesInsideBound.Clear();
        AddLeavesInsideBox(box);
        return leavesInsideBound;
    }

    void AddLeavesInsideBox(BoundingBox box)
    {
        if (box.Contains(BoundingBox) != ContainmentType.Disjoint)
        {
            if (TopLeft == null)
            {
                leavesInsideBound.Add(this);
            }
            else
            {
                TopLeft.AddLeavesInsideBox(box);
                TopRight.AddLeavesInsideBox(box);
                BottomLeft.AddLeavesInsideBox(box);
                BottomRight.AddLeavesInsideBox(box);
            }
        }
    }

    #endregion

    #region Split

    /// <summary>
    /// Split a leaf into 4 x child leaves, and reassign contained objects to child leaves.
    /// </summary>
    void Split()
    {
        float halfScaleX = (BoundingBox.Max.X - BoundingBox.Min.X) * 0.5f;
        float halfScaleZ = (BoundingBox.Max.Z - BoundingBox.Min.Z) * 0.5f;
        Vector3 halfScale = new Vector3(halfScaleX, 0f, halfScaleZ);
        float qtrScaleX = halfScaleX * 0.5f;
        float qtrScaleZ = halfScaleZ * 0.5f;

        if (qtrScaleX != 0f && qtrScaleZ != 0f)
        {
            Vector3 topLeftPosition = BoundingBox.Min + new Vector3(qtrScaleX, 0f, qtrScaleZ);
            Vector3 topRightPosition = BoundingBox.Min + new Vector3(qtrScaleX + halfScaleX, 0f, qtrScaleZ);
            Vector3 bottomLeftPosition = BoundingBox.Min + new Vector3(qtrScaleX, 0f, qtrScaleZ + halfScaleZ);
            Vector3 bottomRightPosition = BoundingBox.Min + new Vector3(qtrScaleX + halfScaleX, 0f, qtrScaleZ + halfScaleZ);

            TopLeft = new QuadTree<T>(MaxObjects, topLeftPosition, halfScale);
            TopRight = new QuadTree<T>(MaxObjects, topRightPosition, halfScale);
            BottomLeft = new QuadTree<T>(MaxObjects, bottomLeftPosition, halfScale);
            BottomRight = new QuadTree<T>(MaxObjects, bottomRightPosition, halfScale);

            TopLeft.Parent = this;
            TopRight.Parent = this;
            BottomLeft.Parent = this;
            BottomRight.Parent = this;

            ReassignObjects();
            Objects.Clear();
        }
    }

    /// <summary>
    /// Reassign objects to child leaves, based on position.
    /// </summary>
    void ReassignObjects()
    {
        foreach (T spatial in Objects)
        {
            if (TopLeft.AddObject(spatial) == null)
                if (TopRight.AddObject(spatial) == null)
                    if (BottomLeft.AddObject(spatial) == null)
                        BottomRight.AddObject(spatial);
        }
    }

    #endregion
}