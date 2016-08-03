using UnityEngine;
using System.Collections;
using Gladius;
using UnityEngine.UI;

public class ModelWindowHolder : MonoBehaviour
{
    private GameObject m_gameObjectInstance;
    public GameObject m_guiCameraTarget;
    public GameObject m_modelCameraObject;
    private Camera m_objectCamera;
    
    public float RotationSpeed = 30f;
    public Vector3 m_localCenter;
    public Vector3 m_localBounds;
    public float m_desiredHeight;
    public float m_desiredDistance;

    private float m_angle;

    public bool Rotate = true;

    public RenderTexture RenderTexture;

    public void Start()
    {
        GameObject dummyCameraObject = new GameObject();
        dummyCameraObject.transform.parent = gameObject.transform;
        dummyCameraObject.transform.localPosition = GladiusGlobals.DefaultOffScreenItemCamPosition;
        m_objectCamera = dummyCameraObject.AddComponent<Camera>();
        m_objectCamera.backgroundColor = Color.black;
        
        RenderTexture = new RenderTexture(1024,1024,24,RenderTextureFormat.ARGB32);
        if (m_objectCamera != null)
        {
            m_objectCamera.targetTexture = RenderTexture;
            m_objectCamera.clearFlags = CameraClearFlags.SolidColor;
            m_objectCamera.depth = -1f;
            GetComponent<RawImage>().texture = RenderTexture;
        }
    }

    public void AttachModelToWindow(GameObject instance, bool force = false)
    {
        if (m_gameObjectInstance != instance)
        {
            if (m_gameObjectInstance != null)
            {
                Destroy(m_gameObjectInstance);
            }
            m_gameObjectInstance = instance;
            m_angle = 0f;
            int uiLayerId = LayerMask.NameToLayer("UI");
            GladiusGlobals.MoveToLayer(m_gameObjectInstance.transform, uiLayerId);

            BaseActor ba = m_gameObjectInstance.GetComponent<BaseActor>();
            if (ba != null)
            {
                ba.DisableNormalUpdates = true;
            }

            m_gameObjectInstance.transform.parent = m_objectCamera.transform;
            m_gameObjectInstance.transform.localPosition = new Vector3(0, 0, 0);

            Bounds b = GetEncapsulatingLocalBounds(m_gameObjectInstance);

            Matrix4x4 im = Matrix4x4.TRS(Vector3.zero, m_gameObjectInstance.transform.localRotation, Vector3.one);

            Vector3 adjustedBoundsCenter = im.MultiplyPoint(b.center);
            Vector3 adjustedBoundsExtents = im.MultiplyPoint(b.extents);


            GladiusAnim gladiusAnim = m_gameObjectInstance.GetComponent<GladiusAnim>();
            if (gladiusAnim != null && gladiusAnim.HasAnimation(AnimationEnum.Idle))
            {
                gladiusAnim.PlayAnimation(AnimationEnum.Idle);
            }

            //http://forum.unity3d.com/threads/dynamic-loaded-object-fit-to-screen-size.349794/
            Vector3 objectFrontCenter = adjustedBoundsCenter - m_gameObjectInstance.transform.forward * adjustedBoundsExtents.z;

            //Get the far side of the triangle by going up from the center, at a 90 degree angle of the camera's forward vector.
            Vector3 triangleFarSideUpAxis = Quaternion.AngleAxis(90, m_gameObjectInstance.transform.right) * transform.forward;

            //Calculate the up point of the triangle.
            const float MARGIN_MULTIPLIER = 1.5f;
            Vector3 triangleUpPoint = objectFrontCenter + triangleFarSideUpAxis * adjustedBoundsExtents.y * MARGIN_MULTIPLIER;

            //The angle between the camera and the top point of the triangle is half the field of view.
            //The tangent of this angle equals the length of the opposing triangle side over the desired distance between the camera and the object's front.
            float desiredDistance = Vector3.Distance(triangleUpPoint, objectFrontCenter) / Mathf.Tan(Mathf.Deg2Rad * m_objectCamera.fieldOfView / 2);

            m_gameObjectInstance.transform.localPosition = transform.forward * desiredDistance - objectFrontCenter;

            m_gameObjectInstance.transform.localRotation = Quaternion.AngleAxis(180, Vector3.up) * m_gameObjectInstance.transform.localRotation;

            int ibreak = 0;
        }
    }

    //public void PositionCamera()
    //{
    //    Bounds objectBounds = ObjectToView.GetComponent<Renderer>().bounds;
    //    Vector3 objectFrontCenter = objectBounds.center - ObjectToView.transform.forward * objectBounds.extents.z;

    //    //Get the far side of the triangle by going up from the center, at a 90 degree angle of the camera's forward vector.
    //    Vector3 triangleFarSideUpAxis = Quaternion.AngleAxis(90, ObjectToView.transform.right) * transform.forward;
    //    //Calculate the up point of the triangle.
    //    const float MARGIN_MULTIPLIER = 1.5f;
    //    Vector3 triangleUpPoint = objectFrontCenter + triangleFarSideUpAxis * objectBounds.extents.y * MARGIN_MULTIPLIER;

    //    //The angle between the camera and the top point of the triangle is half the field of view.
    //    //The tangent of this angle equals the length of the opposing triangle side over the desired distance between the camera and the object's front.
    //    float desiredDistance = Vector3.Distance(triangleUpPoint, objectFrontCenter) / Mathf.Tan(Mathf.Deg2Rad * GetComponent<Camera>().fieldOfView / 2);

    //    transform.position = -transform.forward * desiredDistance + objectFrontCenter;
    //}


    public static Bounds GetEncapsulatingLocalBounds(GameObject go)
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        MeshFilter mf = go.GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
            bounds = mf.sharedMesh.bounds;
        else if (go.GetComponent<Renderer>() is SkinnedMeshRenderer)
            bounds = ((SkinnedMeshRenderer)go.GetComponent<Renderer>()).localBounds;

        foreach (MeshFilter m in go.GetComponentsInChildren<MeshFilter>())
            bounds.Encapsulate(m.sharedMesh.bounds);
        foreach (SkinnedMeshRenderer m in go.GetComponentsInChildren<SkinnedMeshRenderer>())
            bounds.Encapsulate(m.localBounds);

        return bounds;
    }

    public static Bounds GetEncapsulatingWorldBounds(GameObject go)
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        MeshFilter mf = go.GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
            bounds = mf.sharedMesh.bounds;
        else if (go.GetComponent<Renderer>() is SkinnedMeshRenderer)
            bounds = ((SkinnedMeshRenderer)go.GetComponent<Renderer>()).bounds;

        foreach (MeshFilter m in go.GetComponentsInChildren<MeshFilter>())
            bounds.Encapsulate(m.sharedMesh.bounds);
        foreach (SkinnedMeshRenderer m in go.GetComponentsInChildren<SkinnedMeshRenderer>())
            bounds.Encapsulate(m.bounds);

        return bounds;
    }



    public void AttachModelPrefabToWindow(GameObject prefab)
    {
        //if (prefab != null)
        //{
        //    // if we have a different prefab
        //    if (m_gameObjectPrefab != prefab)
        //    {
        //        m_gameObjectPrefab = prefab;
        //        if (m_gameObjectInstance != null)
        //        {
        //            Destroy(m_gameObjectInstance);
        //            m_gameObjectInstance = null;
        //        }
        //        m_angle = 0f;

        //        m_gameObjectInstance = Instantiate(m_gameObjectPrefab) as GameObject;

        //        AttachModelToWindow(m_gameObjectInstance);
        //    }
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (Rotate)
        {
            m_angle += RotationSpeed * Time.deltaTime;

            if (m_gameObjectInstance != null)
            {
                Quaternion q = Quaternion.Euler(new Vector3(m_angle, m_angle, 0));
                m_gameObjectInstance.transform.localRotation = q;
            }
        }
    }

    public void ReparentTarget(GameObject newParent)
    {
        m_guiCameraTarget.transform.parent = newParent.transform;
        m_guiCameraTarget.transform.localRotation = Quaternion.identity;
        m_guiCameraTarget.transform.localPosition = Vector3.zero;
        m_guiCameraTarget.transform.localScale = Vector3.one;

    }

}
