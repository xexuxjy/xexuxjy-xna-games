//using UnityEngine;
//using System.Collections;

//public class Planet : MonoBehaviour
//{

//    public float Mass;
//    public float Radius;
//    public float OrbitRadius;
//    public float OrbitalVelocity = 100.0f;
//    public float Rotation = 10.0f;

//    public GameObject OrbitPoint;

//    private float rotationDeg;

//    // Use this for initialization
//    void Start()
//    {
//        if (transform.parent != null && transform.parent.GetComponent<Planet>() != null)
//        {
//            OrbitPoint = transform.parent.gameObject;

//            Vector3 startPosition = OrbitPoint.transform.position;
//            startPosition += (Vector3.right * OrbitRadius);
//            transform.localPosition = startPosition;

//        }

//        transform.localScale = new Vector3(Radius, Radius, Radius);
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (OrbitPoint != null)
//        {
//            float speedScalar = GameObject.Find("SpeedSlider").GetComponent<dfSlider>().Value;

//            speedScalar *= 0.01f;

//            //Vector3 diff = OrbitPoint.transform.position - gameObject.transform.position;
//            //float dist = diff.magnitude;
//            //diff.Normalize();
//            //GetComponent<Rigidbody>().AddForce(diff  * (10/dist));
//            //rotationDeg += (Time.deltaTime * OrbitalVelocity * speedScalar);
//            rotationDeg = (Time.deltaTime * OrbitalVelocity * speedScalar);

//            transform.position =
//             RotatePointAroundPivot(transform.position,
//                                transform.parent.position,
//                                Quaternion.Euler(0, rotationDeg, 0)); 


//        }
//        GetComponent<Rigidbody>().AddTorque(Vector3.up * Rotation);
        
//    }


//    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle)
//    {
//        return angle * (point - pivot) + pivot;
//    }
//}
