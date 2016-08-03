using UnityEditor;
using UnityEngine;

public class RendererBoundsGizmo : MonoBehaviour
{
    public bool ShowCenter;

    public Color Color = Color.white;

    public bool DrawCube = true;

    public bool DrawSphere = false;

    /// <summary>
    /// When the game object is selected this will draw the gizmos
    /// </summary>
    /// <remarks>Only called when in the Unity editor.</remarks>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = this.Color;

        // get renderer bonding box
        var bounds = new Bounds();
        var initBound = false;
        if (GetBoundWithChildren(this.transform, ref bounds, ref initBound))
        {
            if (this.DrawCube)
            {
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
            if (this.DrawSphere)
            {
                Gizmos.DrawWireSphere(bounds.center, Mathf.Max(Mathf.Max(bounds.extents.x, bounds.extents.y), bounds.extents.z));
            }
        }

        if (this.ShowCenter)
        {
            Gizmos.DrawLine(new Vector3(bounds.min.x, bounds.center.y, bounds.center.z), new Vector3(bounds.max.x, bounds.center.y, bounds.center.z));
            Gizmos.DrawLine(new Vector3(bounds.center.x, bounds.min.y, bounds.center.z), new Vector3(bounds.center.x, bounds.max.y, bounds.center.z));
            Gizmos.DrawLine(new Vector3(bounds.center.x, bounds.center.y, bounds.min.z), new Vector3(bounds.center.x, bounds.center.y, bounds.max.z));
        }

        Handles.BeginGUI();
        var view = SceneView.currentDrawingSceneView;
        var pos = view.camera.WorldToScreenPoint(bounds.center);
        var size = GUI.skin.label.CalcSize(new GUIContent(bounds.ToString()));
        GUI.Label(new Rect(pos.x - (size.x / 2), -pos.y + view.position.height + 4, size.x, size.y), bounds.ToString());
        Handles.EndGUI();
    }

    public static bool GetBoundWithChildren(Transform transform, ref Bounds pBound, ref bool encapsulate)
    {
        var bound = new Bounds();
        var didOne = false;

        // get 'this' bound
        if (transform.gameObject.GetComponent<Renderer>() != null)
        {
            bound = transform.gameObject.GetComponent<Renderer>().bounds;
            if (encapsulate)
            {
                pBound.Encapsulate(bound.min);
                pBound.Encapsulate(bound.max);
            }
            else
            {
                pBound.min = bound.min;
                pBound.max = bound.max;
                encapsulate = true;
            }

            didOne = true;
        }

        // union with bound(s) of any/all children
        foreach (Transform child in transform)
        {
            if (GetBoundWithChildren(child, ref pBound, ref encapsulate))
            {
                didOne = true;
            }
        }

        return didOne;
    }
}


