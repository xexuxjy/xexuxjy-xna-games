using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Gladius;

public class CameraManager : MonoBehaviour
{
    public float xMargin = 2f;		// Distance in the x axis the player can move before the camera follows.
    public float zMargin = 2f;		// Distance in the y axis the player can move before the camera follows.
    public float xSmooth = 8f;		// How smoothly the camera catches up with it's target movement in the x axis.
    public float zSmooth = 8f;		// How smoothly the camera catches up with it's target movement in the y axis.
    public Vector2 maxXAndZ;		// The maximum x and y coordinates the camera can have.
    public Vector2 minXAndZ;		// The minimum x and y coordinates the camera can have.

    public float sensitivityX = 5F;
    public float sensitivityY = 5F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    float rotationY = 0F;


    public CameraManager()
    {
    }

    public void Start()
    {
        GladiusGlobals.CameraManager = this;
        CurrentCameraMode = CameraMode.Normal;
        //Camera c = FindGameObjectWithTag("Main Camera") as Camera;
        Camera c = (Camera)GameObject.FindObjectOfType(typeof(Camera));
        GladiusGlobals.Camera = c;

        minXAndZ = new Vector2(-100, -100);
        maxXAndZ = -minXAndZ;
    }

    //public void UpdateInput(InputState inputState)
    //{
    //    // manual change camera.
    //    if (inputState.IsNewKeyPress(Keys.F1))
    //    {
    //        DefaultCameraOverride = true;
    //        InternalActiveCamera = m_chaseCamera;
    //    }
    //    if (inputState.IsNewKeyPress(Keys.F2))
    //    {
    //        DefaultCameraOverride = true;
    //        InternalActiveCamera = m_freeCamera;
    //    }
    //    if (inputState.IsNewKeyPress(Keys.F3))
    //    {
    //        DefaultCameraOverride = true;
    //        InternalActiveCamera = m_staticCamera;
    //    }
    //    if (inputState.IsNewKeyPress(Keys.F4))
    //    {
    //        DefaultCameraOverride = false;
    //    }

    //    ActiveCamera.UpdateInput(inputState);
    //}


    bool CheckXMargin()
    {
        // Returns true if the distance between the camera and the player in the x axis is greater than the x margin.
        return Mathf.Abs(transform.position.x - TargetObject.transform.position.x) > xMargin;
    }


    bool CheckZMargin()
    {
        // Returns true if the distance between the camera and the player in the y axis is greater than the y margin.
        return Mathf.Abs(transform.position.z - TargetObject.transform.position.z) > zMargin;
    }

    public void FixedUpdate()
    {
        if (Input.GetButtonDown("CameraNormal") || Input.GetButtonDown("PadLeftStickPress"))
        {
            CurrentCameraMode = CameraMode.Normal;
        }
        if (Input.GetButtonDown("CameraManual") || Input.GetButtonDown("PadRightStickPress"))
        {
            CurrentCameraMode = CameraMode.Manual;
        }

        //return;
        switch (CurrentCameraMode)
        {
            case CameraMode.Normal:
                UpdateCameraNormal();
                break;
            case CameraMode.Manual:
                UpdateCameraManual();
                break;
            default:
                break;
        }
    }


    public void UpdateCameraManual()
    {
        float movementSpeed = 0.1f;

        if (Input.GetButton("DebugLeft"))
        {
            transform.position -= transform.right * movementSpeed;
        }
        if (Input.GetButton("DebugRight"))
        {
            transform.position += transform.right * movementSpeed;
        }
        if (Input.GetButton("DebugForward"))
        {
            transform.position += transform.forward * movementSpeed;
        }
        if (Input.GetButton("DebugBackward"))
        {
            transform.position -= transform.forward* movementSpeed;
        }


        float downMovement = Input.GetAxis("PadTriggerLeft");
        float upMovement = Input.GetAxis("PadTriggerRight");

        
        if (Input.GetButton("DebugUp") || upMovement > 0f)
        {
            transform.position += transform.up* movementSpeed;
        }
        if (Input.GetButton("DebugDown") || downMovement > 0f)
        {
            transform.position -= transform.up* movementSpeed;
        }


        
        float lsh = Input.GetAxis("PadLeftStickH");
        if (lsh != 0.0f)
        {
            transform.position += transform.right * movementSpeed * lsh;
        }

        float lsv = Input.GetAxis("PadLeftStickV");
        if (lsv != 0.0f)
        {
            transform.position += transform.forward* movementSpeed * lsv;
        }


        float rsh = Input.GetAxis("PadRightStickH");

        float rsv = Input.GetAxis("PadRightStickV");


        

        float rotationX = transform.localEulerAngles.y + rsh * sensitivityX;
        rotationY += rsv * sensitivityY;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

        transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);



        //float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
        //            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        //rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
		transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);


    }

    public void Update1()
    {
        if (TargetObject != null)
        {
            Transform targetTransform = TargetObject.transform;
            Transform cameraTransform = transform;
            //ActiveCamera.Update();
            // By default the target x and y coordinates of the camera are it's current x and y coordinates.
            Vector3 targetPos = targetTransform.position;
            Vector3 cameraPos = cameraTransform.position;

            // If the player has moved beyond the x margin...
            if (CheckXMargin())
            {
                // ... the target x coordinate should be a Lerp between the camera's current x position and the player's current x position.
                targetPos.x = Mathf.Lerp(cameraPos.x, targetPos.x, xSmooth * Time.deltaTime);
            }
            // If the player has moved beyond the y margin...
            if (CheckZMargin())
            {
                // ... the target y coordinate should be a Lerp between the camera's current y position and the player's current y position.
                targetPos.z = Mathf.Lerp(cameraPos.z, targetPos.z, zSmooth * Time.deltaTime);
            }


            // The target x and y coordinates should not be larger than the maximum or smaller than the minimum.
            targetPos.x = Mathf.Clamp(targetPos.x, minXAndZ.x, maxXAndZ.x);
            targetPos.y = Mathf.Clamp(targetPos.y, minXAndZ.y, maxXAndZ.y);

            // Set the camera's position to the target position with the same z component.
            //cameraTransform.position = new Vector3 (targetX, targetY, cameraTransform.position.z);

            //targetPos.y = 5;
            //cameraTransform.position = targetTransform.position;
            //cameraTransform.position = offsetPos;


            Vector3 offset = TargetObject.transform.forward * -3;
            offset += new Vector3(0, 1, 0);


            //Vector3 offset = TargetObject.transform.forward * 10;
            //offset += new Vector3(0, 5, 0);

            Vector3 newCameraPos = targetPos + offset;
            cameraTransform.position = newCameraPos;
            cameraTransform.LookAt(targetPos);
            //cameraTransform.position += new Vector3(0,3,0);
            //Debug.Log ("Camera pos :"+cameraTransform.position+"   tgt :"+lookatPos);
        }
    }

    public void UpdateCameraNormal()
    {
        if (m_combatModeActive)
        {
            // The distance in the x-z plane to the target
            float distance = 3.0f;
            // the height we want the camera to be above the target
            float height = 1f;
            // How much we 
            float heightDamping = 2.0f;
            float rotationDamping = 3.0f;


            Vector3 midPoint = (m_actor1.CameraFocusPoint + m_actor2.CameraFocusPoint) / 2f;
            // side on view
            Vector3 fwd = m_actor1.gameObject.transform.right;


            // Calculate the current rotation angles
            //float calcAngle = Vector3.Dot(fwd, Vector3.forward);
            //calcAngle *= Mathf.Rad2Deg;
            float calcAngle = Vector3.Angle(fwd, Vector3.forward);


            float wantedRotationAngle = calcAngle;
            float wantedHeight = m_actor1.CameraFocusPoint.y+height;

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
            transform.position = midPoint;
            //transform.position -= fwd * distance;
            transform.position -= currentRotation * Vector3.forward * distance;

            // Set the height of the camera
            Vector3 temp = transform.position;
            temp.y = currentHeight;
            transform.position = temp;

            // Always look at the target
            transform.LookAt(midPoint);
        }
        else if (GladiusGlobals.TurnManager.CurrentActor != null && GladiusGlobals.TurnManager.CurrentActor.ArenaPoint != GladiusGlobals.MovementGrid.CurrentCursorPoint)
        {
            // focus on the grid cursor, and ideally a line between that and a nearby target.
            Vector3 cursorV3 = GladiusGlobals.MovementGrid.CurrentV3;
            Vector3 desiredForward = GladiusGlobals.TurnManager.CurrentActor.transform.forward;
            BaseActor target = GladiusGlobals.Arena.FindNearestEnemy(GladiusGlobals.TurnManager.CurrentActor);
            if (target != null)
            {
                Point nearestPoint = GladiusGlobals.Arena.PointNearestLocation(GladiusGlobals.MovementGrid.CurrentCursorPoint, target.ArenaPoint, true);
                Vector3 targetV3 = GladiusGlobals.Arena.ArenaToWorld(nearestPoint);
                Vector3 temp = targetV3 - cursorV3;
                if(temp.sqrMagnitude > float.Epsilon)
                {
                    //desiredForward = temp.normalized;
                }
            }

            float wantedRotationAngle = Vector3.Angle(desiredForward, Vector3.forward);
            float wantedHeight = cursorV3.y+2f;
            float wantedDistance = 2;
            

            StandardLookAt(cursorV3+(desiredForward*2), wantedRotationAngle, wantedHeight,wantedDistance);


        }
        else if (TargetObject != null)
        {
            Transform targetTransform = TargetObject.transform;
            // Calculate the current rotation angles
            float wantedRotationAngle = targetTransform.eulerAngles.y;
            float wantedHeight = targetTransform.position.y+2f;
            float distance = 3;
            StandardLookAt(targetTransform.position, wantedRotationAngle, wantedHeight,distance);
        }
    }


    private void StandardLookAt(Vector3 position,float wantedRotationAngle,float height,float distance)
    {
        float heightDamping = 2.0f;
        float rotationDamping = 3.0f;

        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;


        // Damp the rotation around the y-axis
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, height, heightDamping * Time.deltaTime);

        // Convert the angle into a rotation
        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        transform.position = position;
        transform.position -= currentRotation * Vector3.forward * distance;
        //transform.rotation = currentRotation;

        // Set the height of the camera
        Vector3 temp = transform.position;
        temp.y = currentHeight;
        transform.position = temp;

        // Always look at the target
        transform.LookAt(position);

    }

    public void SetCombatModeActive(bool active ,BaseActor actor1, BaseActor actor2)
    {
        m_combatModeActive = active;
        m_actor1 = actor1;
        m_actor2 = actor2;
    }


    private bool m_combatModeActive;
    private BaseActor m_actor1;
    private BaseActor m_actor2;



    //public Transform TargetTransform
    //{
    //    get;
    //    set;
    //}

    public GameObject TargetObject
    {
        get;
        set;
    }

    private Vector3 _targetPos;
    public Vector3 TargetPosition
    {
        get
        {
            if (TargetObject != null)
            {
                return TargetObject.transform.position;
            }
            return _targetPos;
        }

        set
        {
            _targetPos = value;
        }
    }

    private CameraMode m_cameraMode;
    public CameraMode CurrentCameraMode
    {
        get
        {
            return m_cameraMode;
        }
        set
        {
            m_cameraMode = value;
        }
    }


}
