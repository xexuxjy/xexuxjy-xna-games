using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Parabox.Stl;
using TMPro;
using UnityEditor;
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

    private bool Paused = false;
    
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
        ActorGenerator.ActorGeneratorInit();
        String name = characterMeshHolder.gameObject.name.Replace(".mdl", "");
        ActorClassDef actorClassDef = ActorGenerator.ActorClassDefs.Find(x => x.name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        characterMeshHolder.SetupCharacterData(ActorGenerator.CreateCharacterForLevel(actorClassDef.name, 1), null);

        gladiusAnim = characterMeshHolder.GladiusAnim;
        
        foreach (TextAsset animAsset in characterMeshHolder.CharacterAnims)
        {
            string animationName = AnimationLoader.StandardiseAnimName(animAsset.name);
            gladiusAnim.AddAnim(animationName, animAsset);
        }
        
        CurrentIndex = gladiusAnim.m_allAnimations.FindIndex(x => x.name == "idle");

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
        
        GUILayoutOption[] ButtonLayoutOptions = new GUILayoutOption[] { GUILayout.Height(100.0f),GUILayout.Width(100.0f) };
        
        if (GUILayout.Button("Next", ButtonLayoutOptions))
        {
            NextAnim();
        }
        if (GUILayout.Button("Prev", ButtonLayoutOptions))
        {
            PreviousAnim();
        }

        
        if (GUILayout.Button(Paused?"Resume":"Pause", ButtonLayoutOptions))
        {
            Paused = !Paused;
            characterMeshHolder.AnimationPaused = Paused;
        }

        if (Paused)
        {
            if (GUILayout.Button("Export STL", ButtonLayoutOptions))
            {
                // Bake mesh and run exporter
                GameObject bakedPrefab = MeshBaker.CreateBakedPrefab(characterMeshHolder.gameObject);

                string saveName = bakedPrefab.name+"-"+DateTime.Now.ToString("yyyyMMddHHmmss");
                
                string path = EditorUtility.SaveFilePanel("Save Mesh to STL", "", saveName, "stl");

                Exporter.Export(path, new GameObject[]{bakedPrefab}, FileType.Binary);

                Destroy(bakedPrefab);
            }
        }




        guiStyle.fontSize = 32; //change the font size
        guiStyle.normal.textColor = Color.black;

        int xpos = 1000;
        int ypos = 100;
        int height = 80;
        
        if (gladiusAnim.CurrentAnimation != null)
        {
            GUI.Label(new Rect(xpos, ypos, 400, height), String.Format("{0}  {1:0.000}  {2:0.000}", gladiusAnim.CurrentAnimation.name, gladiusAnim.CurrentAnimation.mLength, gladiusAnim.CurrentAnimation.AnimTime), guiStyle);
            ypos += height;
            GUI.Label(new Rect(xpos, ypos, 400, height), String.Format("POS {0}  ROT {1}  XPOS {2} XROT {3}", gladiusAnim.CurrentAnimation.mNumPos, gladiusAnim.CurrentAnimation.mNumRot, gladiusAnim.CurrentAnimation.mNumXPos, gladiusAnim.CurrentAnimation.mNumXRot), guiStyle);
            ypos += height;
        }
        GUI.Label(new Rect(xpos, ypos, 400, height), String.Format("{0}/{1}", CurrentIndex, gladiusAnim.m_allAnimations.Count), guiStyle);

        ypos += height;
        GUI.Label(new Rect(xpos, ypos, 400, height), "Events : " + m_animationEvents, guiStyle);

    }

    private GUIStyle guiStyle = new GUIStyle(); //create a new variable
}
