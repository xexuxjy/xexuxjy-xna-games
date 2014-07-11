﻿using UnityEngine;
using System.Collections;
using Gladius;
using Gladius.arena;
using System.Collections.Generic;
using System;

public class ArenaStartup : MonoBehaviour
{

    public GameObject baseActorPrefab;
    public String ArenaDataName = "GladiusData/ArenaData/Arena1Data";

    public bool setupLos = false;

    //public String SchoolOneFile = "Orins-School";
    //public String SchoolTwoFile = "Legionaires-School";

    public String EncounterFile = "TestEncounter";


    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = 30;

        if (GladiusGlobals.LocalisationData == null)
        {
            GladiusGlobals.LocalisationData = new LocalisationData();
            GladiusGlobals.LocalisationData.Load(null);
        }

        if (GladiusGlobals.AttackSkillDictionary == null)
        {
            GladiusGlobals.AttackSkillDictionary = new AttackSkillDictionary();
            GladiusGlobals.AttackSkillDictionary.Load(null);
        }


        GladiusGlobals.ItemManager.Load(null);

        ActorGenerator.InitCategories();

        //if (GladiusGlobals.TurnManager == null)
        //{
        //    GladiusGlobals.TurnManager = new TurnManager();
        //}

        if (GladiusGlobals.Arena == null)
        {
            //int dims =32;
            //GladiusGlobals.Arena = new Arena(dims, dims);
            GladiusGlobals.Arena = ArenaLoader.BuildArena(ArenaDataName);

        }

        List<BaseActor> actors = new List<BaseActor>();


        Encounter encounter = Encounter.Load(EncounterFile);
        foreach (EncounterSide side in encounter.Sides)
        {
            foreach (String name in side.ChosenGladiators)
            {
                CharacterData cd = side.School.Gladiators[name];
                cd.TeamName = side.TeamName;
                GameObject baseActorGameObject = (GameObject)Instantiate(baseActorPrefab);
                baseActorGameObject.name = "BaseActor" + name;
                BaseActor ba1 = baseActorGameObject.GetComponent<BaseActor>();
                ba1.SetupCharacterData(cd);
                ba1.Arena = GladiusGlobals.Arena;
                if (cd.TeamName == "Player")
                {
                    SetActor1(ba1);
                    ba1.PlayerControlled = true;
                }
                else
                {
                    SetActor2(ba1);
                }
                actors.Add(ba1);
                ba1.SetupSkills(GladiusGlobals.AttackSkillDictionary);
            }
        }

        actors[0].ArenaPoint = GladiusGlobals.Arena.PlayerPointList[0];
        actors[1].ArenaPoint = GladiusGlobals.Arena.PlayerPointList[1];

        actors[2].ArenaPoint = GladiusGlobals.Arena.Team1PointList[0];
        actors[3].ArenaPoint = GladiusGlobals.Arena.Team1PointList[1];

        foreach (BaseActor actor in actors)
        {
            GladiusGlobals.Arena.MoveActor(actor, actor.ArenaPoint);
            GladiusGlobals.TurnManager.QueueActor(actor);
        }

    }

    void Awake()
    {
    }

    public void SetActor1(BaseActor actor)
    {
        actor.HeadModelName = "w_head_01_01";
        actor.ShoulderModelName = "w_shoulder_01";
        actor.BodyModelName = "w_body_01";
        actor.HandModelName = "w_hand_01";
        actor.ShoesModelName = "w_shoes_01";
        actor.HelmetModelName = "w_helmet_01";
        actor.SetMeshData();
    }

    public void SetActor2(BaseActor actor)
    {
        actor.HeadModelName = "w_head_01_02";
        actor.ShoulderModelName = "w_shoulder_02";
        actor.BodyModelName = "w_body_02";
        actor.HandModelName = "w_hand_02";
        actor.ShoesModelName = "w_shoes_02";
        actor.HelmetModelName = "w_helmet_02";
        actor.SetMeshData();
    }



    // Update is called once per frame
    void Update()
    {
        if (!setupLos)
        {
            setupLos = true;
            GladiusGlobals.LOSTester.SetStartAndEnd(new Point(10,0), new Point(10, 14));
        }
        //GladiusGlobals.TurnManager.Update();
    }
}