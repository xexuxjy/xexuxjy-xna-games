using UnityEngine;
using System.Collections;
using Gladius;
using Gladius.arena;
using System.Collections.Generic;
using System;

public class ArenaStartup : MonoBehaviour
{

    public GameObject baseActorPrefab;
    public String ArenaDataName = "GladiusArena/ArenaData/Arena1Data";
    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = 30;

        if (GladiusGlobals.AttackSkillDictionary == null)
        {
            GladiusGlobals.AttackSkillDictionary = new AttackSkillDictionary();
            GladiusGlobals.AttackSkillDictionary.LoadExtractedData(null);
        }

        LocalisationData localisationData = new LocalisationData();
        localisationData.LoadExtractedData(null);

        GladiusGlobals.ItemManager.LoadExtractedData(null);

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
        int numActors = 4;

        for (int i = 0; i < numActors; ++i)
        {
            ActorClass actorClass = ActorClass.Barbarian;
            if (i == 0)
            {
                actorClass = ActorClass.Urlan;
            }
            if (i == 1)
            {
                actorClass = ActorClass.Ursula;
            }


            GameObject baseActorGameObject = (GameObject)Instantiate(baseActorPrefab);
			baseActorGameObject.name = "BaseActor"+i;
            //BaseActor ba1 = ActorGenerator.GenerateActor(1, actorClass, this);
            BaseActor ba1 = baseActorGameObject.GetComponent<BaseActor>();

            if (i < 2)
            {
                ba1.Team = GladiusGlobals.PlayerTeam;
                ba1.Name = i == 0 ? "Urlan" : "Ursula";
                SetActor1(ba1);
                ba1.PlayerControlled = true;
            }
            else
            {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
                ba1.Name = "Monster" + i;
                ba1.Team = "EnemyTeam";
                ActorGenerator.SetActorStats(5, actorClass, ba1);
                SetActor2(ba1);
            }
            ba1.Arena = GladiusGlobals.Arena;
            //ba1.LoadContent();
			actors.Add (ba1);
            ba1.SetupSkills(GladiusGlobals.AttackSkillDictionary);
        }

        //actors[0].CurrentPosition = new Point(2, 2);

        actors[0].ArenaPoint = GladiusGlobals.Arena.PlayerPointList[0];
        actors[1].ArenaPoint = GladiusGlobals.Arena.PlayerPointList[1];

        actors[2].ArenaPoint = GladiusGlobals.Arena.Team1PointList[0];
        actors[3].ArenaPoint = GladiusGlobals.Arena.Team1PointList[1];

        //actors[0].CurrentPosition = new Point(10, 10);
        //actors[1].CurrentPosition = new Point(11, 10);
        //actors[2].CurrentPosition = new Point(13, 10);
        //actors[3].CurrentPosition = new Point(13, 13);

        for (int i = 0; i < numActors; ++i)
        {
            GladiusGlobals.Arena.MoveActor(actors[i], actors[i].ArenaPoint);
            GladiusGlobals.TurnManager.QueueActor(actors[i]);

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
        //GladiusGlobals.TurnManager.Update();
    }
}
