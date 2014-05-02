﻿using UnityEngine;
using System.Collections;

public class MainMenuScript : MonoBehaviour
{

    bool adjustUp = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    //void Update()
    //{
    //    RaycastHit hit;
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    //var select = GameObject.FindWithTag("select").transform;
    //    if (Physics.Raycast(ray, out hit, 100.0f))
    //    {
    //        if (hit.collider.gameObject.name.StartsWith("MainMenu"))
    //        {
    //            //{
    //            //Debug.Log("Clicked on : " + hit.collider.gameObject.name);
    //            //Debug.Log("R : " + renderer.material.color);
    //            if (hit.collider.gameObject.renderer.material.color.a <= 0)
    //            {
    //                adjustUp = true;
    //            }
    //            if (hit.collider.gameObject.renderer.material.color.a >= 1)
    //            {
    //                adjustUp = false;
    //            }

    //            Color c = new Color(0, 0, 0, 1f);
    //            //Debug.Log("CA " + colourAdjust);
    //            //Debug.Log("R : " + renderer.material.color.r);
    //            if (adjustUp)
    //            {

    //                hit.collider.gameObject.renderer.material.color += c * Time.deltaTime;
    //            }
    //            else
    //            {
    //                hit.collider.gameObject.renderer.material.color -= c * Time.deltaTime;
    //            }
    //        }


    //    }

    //}


    void OnMouseOver()
    {

        if (renderer.material.color.a <= 0)
        {
            adjustUp = true;
        }
        if (renderer.material.color.a >= 1)
        {
            adjustUp = false;
        }

        Color c = new Color(0, 0, 0, 1f);
        //Debug.Log("CA " + colourAdjust);
        //Debug.Log("R : " + renderer.material.color.r);
        if (adjustUp)
        {

            renderer.material.color += c * Time.deltaTime;
        }
        else
        {
            renderer.material.color -= c * Time.deltaTime;
        }
    }

    void OnMouseExit()
    {
        Color c = renderer.material.color;
        c.a = 1;
        renderer.material.color = c;
        adjustUp = false;
    }


    void OnMouseDown()
    {
        Debug.Log("Clicked on : " + gameObject.name);
        if (gameObject.name == "MainMenuArenaOption")
        {
            Application.LoadLevel("ArenaScene");
        }
        else if (gameObject.name == "MainMenuOverlandOption")
        {
            Application.LoadLevel("walktest");
        }



        //RaycastHit hit;
        //Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        ////var select = GameObject.FindWithTag("select").transform;
        //if (Physics.Raycast(ray, out hit, 100.0f))
        //{
        //    //if(hit.collider.gameObject.name == "MainMenuArenaOption")
        //    //{
        //    //Debug.Log("Clicked on : " + hit.collider.gameObject.name);
        //    Debug.Log("R : " + renderer.material.color);


        //}
        //select.tag = "none";
        //hit.collider.transform.tag = "select";

    }

}
