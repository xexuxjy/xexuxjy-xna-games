using UnityEngine;
using System.Collections;

public class OverlandCameraControl : MonoBehaviour {

    public bool UseSmoothUpdate = true;
	// Use this for initialization
	void Start () 
    {
	}
	
	// Update is called once per frame
	void LateUpdate () 
    {
        if (UseSmoothUpdate)
        {
            SmoothUpdate();
        }
        else
        {
            OldUpdate();
        }

	}

    void OldUpdate()
    {
        if (TargetObject != null)
        {
            Vector3 target = TargetObject.rigidbody.transform.position;
            transform.LookAt(target);
            target += LookatOffset;
            transform.position = target;
        }
    }

    public float SmoothUpdateScalar = 2f;
    void SmoothUpdate()
    {
        float deltaTime = Time.deltaTime;
        Vector3 target = TargetObject.rigidbody.transform.position;
        transform.LookAt(target);

        target += LookatOffset;
        Vector3 diff = target - transform.position;
        diff *= deltaTime * SmoothUpdateScalar;
        transform.position += diff;
    }

    void SmoothUpdate2()
    {
        Transform targetTransform = TargetObject.transform;
        // The distance in the x-z plane to the target
        float distance = 3.0f;
        // the height we want the camera to be above the target
        float height = 2.0f;
        // How much we 
        float heightDamping = 2.0f;
        float rotationDamping = 3.0f;


        // Calculate the current rotation angles
        float wantedRotationAngle = targetTransform.eulerAngles.y;
        float wantedHeight = targetTransform.position.y + height;

        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;

        // Damp the rotation around the y-axis
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        // Convert the angle into a rotation
        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        Vector3 target = TargetObject.rigidbody.transform.position;


        transform.LookAt(target);
        target += LookatOffset;
        transform.position = target;
 
        //transform.position = targetTransform.position;
        //transform.position -= currentRotation * Vector3.forward * distance;

        //// Set the height of the camera
        //Vector3 temp = transform.position;
        //temp.y = currentHeight;
        //transform.position = temp;

        // Always look at the target
        //transform.LookAt(targetTransform);

    }


    public GameObject TargetObject;
    public Vector3 LookatOffset = new Vector3(0, 5, -1);
}
