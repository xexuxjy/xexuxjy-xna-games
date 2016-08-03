using System.Collections;
using UnityEngine;

public class CameraFacingBillboard : MonoBehaviour
{
    public Camera m_Camera;

    void Update()
    {
        if (m_Camera == null)
        {
            m_Camera = GameObject.Find("MainCamera").GetComponent<Camera>();
        }
        if (m_Camera != null)
        {
            transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
                m_Camera.transform.rotation * Vector3.up);
        }
    }
}