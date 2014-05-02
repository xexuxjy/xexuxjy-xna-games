using UnityEngine;
using System.Collections;
using System;

public class OverlandCharacterControl : MonoBehaviour {

    public float MovementSpeed = 10f;
	// Use this for initialization
	void Start () 
    {
    	
	}
	
    void PlayIdle()
    {
        String animName = "idle-2";
        if (!animation.IsPlaying(animName))
        {
            animation.Play(animName);
        }
        
    }

    void PlayRun()
    {
        String animName = "walk";
        if (!animation.IsPlaying(animName))
        {
            animation.Play(animName);
        }
    }

	// Update is called once per frame
	void Update () 
    {
        Vector3 adjust = new Vector3();
	    if(CursorLeftHeld())
        {
            adjust += Vector3.left;
        }
        if (CursorRightHeld())
        {
            adjust += Vector3.right;
        }
        if (CursorUpHeld())
        {
            adjust += Vector3.forward;
        }
        if (CursorDownHeld())
        {
            adjust += Vector3.back;
        }

        if (adjust.sqrMagnitude > 0)
        {
            adjust *= MovementSpeed * Time.deltaTime;
            Quaternion q = Quaternion.LookRotation(adjust);
                            
            transform.position += adjust;
            transform.rotation = q;
            PlayRun();

        }
        else
        {
            PlayIdle();
        }

	}

    public bool CursorLeftPressed()
    {
        return Input.GetButtonUp("CursorLeft");
    }

    public bool CursorRightPressed()
    {
        return Input.GetButtonUp("CursorRight");

    }

    public bool CursorUpPressed()
    {
        return Input.GetButtonUp("CursorUp");

    }

    public bool CursorDownPressed()
    {
        return Input.GetButtonUp("CursorDown");
    }

    public bool CursorLeftHeld()
    {
        return Input.GetButton("CursorLeft");
    }
    public bool CursorRightHeld()
    {
        return Input.GetButton("CursorRight");
    }
    public bool CursorUpHeld()
    {
        return Input.GetButton("CursorUp");
    }
    public bool CursorDownHeld()
    {
        return Input.GetButton("CursorDown");
    }


    public bool Action1Pressed()
    {
        return Input.GetButtonDown("Action1");
    }

    public bool Action2Pressed()
    {
        return Input.GetButtonDown("Action2");

    }

    public bool Action3Pressed()
    {
        return Input.GetButtonDown("Action3");

    }

    public bool Action4Pressed()
    {
        return Input.GetButtonDown("Action4");
    }


    public void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Town")
        {
            m_currentTownData = other.GetComponent<TownData>();
            if (m_currentTownData != null)
            {
                Debug.Log("Found Town " + m_currentTownData.TownName);
            }
        }
    
    }


    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Town")
        {
            m_currentTownData = null;
            Debug.Log("Left Town " + m_currentTownData.TownName);
        }
    }


    private TownData m_currentTownData;
}
