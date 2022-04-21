using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class AnimationTester : MonoBehaviour
{
    public CharacterMeshHolder characterMeshHolder;
    public GladiusCharacterAnim gladiusAnim;


    public int CurrentIndex = 0;

    public bool LoopAnim = false;
    public bool FixedRate = true;
    public float AnimationSpeed = 1.0f;

    private string m_animationEvents = "";

    public bool RandomiseAnimationStart;


    public void Update()
    {
        if (gladiusAnim != null && gladiusAnim.CurrentAnimation != null)
        {
            if (LoopAnim)
            {
                gladiusAnim.CurrentAnimation.mFlags |= AnimationData.ANIM_LOOP;
            }
            else
            {
                gladiusAnim.CurrentAnimation.mFlags &= ~AnimationData.ANIM_LOOP;
            }

            gladiusAnim.CurrentPlayInfo.PlayRate = AnimationSpeed;

        }

    }

    public void Awake()
    {
        //characterMeshHolder.SetupCharacterData()
        ////characterMeshHolder = GetComponent<CharacterMeshHolder>();
        //characterMeshHolder.GladiusAnim = new GladiusCharacterAnim();

        ActorGenerator.ActorGeneratorInit();
        String name = characterMeshHolder.gameObject.name.Replace(".mdl", "");
        ActorClassDef actorClassDef = ActorGenerator.ActorClassDefs.Find(x => x.name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        characterMeshHolder.SetupCharacterData(ActorGenerator.CreateCharacterForLevel(actorClassDef.name, 1), null);

        gladiusAnim = characterMeshHolder.GladiusAnim;
        CurrentIndex = gladiusAnim.m_allAnimations.FindIndex(x => x.name == "idle");
        //CurrentIndex = gladiusAnim.m_allAnimations.FindIndex(x => x.name == "reactvictory");
        UpdateAnimData();
    }


        public void NextAnim()
    {
        CurrentIndex++;
        CurrentIndex = CurrentIndex % gladiusAnim.m_allAnimations.Count;
        UpdateAnimData();
    }

    public void UpdateAnimData()
    {
        gladiusAnim.PlayAnimation(gladiusAnim.m_allAnimations[CurrentIndex].name);
        m_animationEvents = "";
        if (RandomiseAnimationStart)
        {
            gladiusAnim.CurrentAnimation.RandomAnimTIme();
        }

    }


    public void PreviousAnim()
    {
        CurrentIndex--;
        if (CurrentIndex < 0)
        {
            CurrentIndex += gladiusAnim.m_allAnimations.Count;
        }
        UpdateAnimData();
    }


    public void OnGUI()
    {
        if (GUILayout.Button("Next"))
        {
            NextAnim();
        }
        if (GUILayout.Button("Prev"))
        {
            PreviousAnim();
        }


        guiStyle.fontSize = 32; //change the font size
        guiStyle.normal.textColor = Color.black;

        int ypos = 100;
        int height = 80;
        if (gladiusAnim.CurrentAnimation != null)
        {
            GUI.Label(new Rect(25, ypos, 400, height), String.Format("{0}  {1:0.000}  {2:0.000}", gladiusAnim.CurrentAnimation.name, gladiusAnim.CurrentAnimation.mLength, gladiusAnim.CurrentAnimation.AnimTime), guiStyle);
            ypos += height;
            GUI.Label(new Rect(25, ypos, 400, height), String.Format("POS {0}  ROT {1}  XPOS {2} XROT {3}", gladiusAnim.CurrentAnimation.mNumPos, gladiusAnim.CurrentAnimation.mNumRot, gladiusAnim.CurrentAnimation.mNumXPos, gladiusAnim.CurrentAnimation.mNumXRot), guiStyle);
            ypos += height;
        }
        GUI.Label(new Rect(25, ypos, 400, height), String.Format("{0}/{1}", CurrentIndex, gladiusAnim.m_allAnimations.Count), guiStyle);

        ypos += height;
        GUI.Label(new Rect(25, ypos, 400, height), "Events : " + m_animationEvents, guiStyle);

    }

    private GUIStyle guiStyle = new GUIStyle(); //create a new variable
}
