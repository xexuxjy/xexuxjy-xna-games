using UnityEngine;
using System.Collections;
using Gladius;

public class ModelWindowHolder : MonoBehaviour
{
    private GameObject m_gameObjectInstance;
    private GameObject m_gameObjectPrefab;
    private GameObject m_modelCamera;
    private dfTextureSprite m_parentSprite;
    GUITexture gtexture = new GUITexture();
    public float RotationSpeed = 30f;

    private float m_angle;

    public void Awake()
    {
        m_modelCamera = GameObject.Find("ModelCamera");
        m_modelCamera.GetComponent<SnapshotCamera>().ScreenReadyEvent += new SnapshotCamera.ScreenReadyEventDelegate(ModelWindowHolder_ScreenReadyEvent);
        m_parentSprite = GetComponent<dfTextureSprite>();
    }

    void ModelWindowHolder_ScreenReadyEvent()
    {
        m_parentSprite.Texture = m_modelCamera.GetComponent<SnapshotCamera>().screenshot;
        //gtexture.texture = m_modelCamera.GetComponent<SnapshotCamera>().screenshot;
        //m_parentPanel. = gtexture;
    }

    public void AttachedModelPrefabToWindow(GameObject prefab)
    {
        if (prefab != null)
        {
            // if we have a different prefab
            if (m_gameObjectPrefab != prefab)
            {
                m_gameObjectPrefab = prefab;
                if (m_gameObjectInstance != null)
                {
                    Destroy(m_gameObjectInstance);
                    m_gameObjectInstance = null;
                }
                m_angle = 0f;
                //dfPanel parentPanel = GetComponent<dfPanel>();
                //Bounds bb = parentPanel.GetBounds();
                //Vector3 extents = bb.extents;
                //extents.y *=-1f;
                //extents.z = 0.2f;

                m_gameObjectInstance = Instantiate(m_gameObjectPrefab) as GameObject;
                
                int uiLayerId = LayerMask.NameToLayer("UI");
                GladiusGlobals.MoveToLayer(m_gameObjectInstance.transform, uiLayerId);


                m_gameObjectInstance.transform.parent = m_modelCamera.transform;
                m_gameObjectInstance.transform.localPosition = new Vector3(0,0,1);
                m_gameObjectInstance.transform.localRotation = Quaternion.Euler(new Vector3(90, 90, 0));
                // base this off the bb of the object, then scale it to the dims of the window
                m_gameObjectInstance.transform.localScale = new Vector3(50f, 50f, 50f);
                m_modelCamera.GetComponent<SnapshotCamera>().RequestSnapshot();
            }
        }
    }




    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        m_angle += RotationSpeed * Time.deltaTime;

        if (m_gameObjectInstance != null)
        {
            m_gameObjectInstance.transform.localRotation = Quaternion.Euler(new Vector3(m_angle, m_angle, 0));
        }


    }
}
