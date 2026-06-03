using UnityEngine;

public class TestLateAnimUpdate : MonoBehaviour
{
    
    // Update the local transform data after anim?
    public void LateUpdate()
    {
        Quaternion q = transform.localRotation;
        Vector3 v = q.eulerAngles;
        v.y += 180;
        transform.localRotation = Quaternion.Euler(v);
    }
}
