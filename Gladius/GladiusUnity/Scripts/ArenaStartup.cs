using UnityEngine;
using System.Collections;
using Gladius;
using System.Collections.Generic;
using System;

public class ArenaStartup : CommonStartup
{
    public GameObject arenaPrefab;
    public Arena Arena;
    public TurnManager TurnManager;
    public CombatEngine CombatEngine;
    public CameraManager CameraManager;

    public String ArenaDataName = "GladiusData/ArenaData/Arena1Data";

    public bool setupLos = false;

    //public String SchoolOneFile = "Orins-School";
    //public String SchoolTwoFile = "Legionaires-School";

    public String EncounterFile = "TestEncounter";


    // Use this for initialization
    public override void ChildStart()
    {
        //CommonState state = GladiusGlobals.GameStateManager.StateData;
        //state.CameraManager = GameObject.Find("MainCamera").GetComponent<CameraManager>();
        //state.TurnManager = GetComponent<TurnManager>();
        //state.Arena  = GetComponent<Arena>();
        //state.MovementGrid = GameObject.Find("MovementGrid").GetComponent<MovementGrid>();
        //state.CombatEngine = GetComponent<CombatEngine>();
        //state.CombatEngineUI = GetComponent<CombatEngineUI>();
        //state.Crowd = GetComponent<Crowd>();
        //state.BattleData  = new BattleData();
        //state.LOSTester  = gameObject.AddComponent<LOSTester>();
        
        
        ArenaLoader.SetupArena(ArenaDataName, Arena);

        List<BaseActor> actors = new List<BaseActor>();

        // not coming from choice screen
        if (GladiusGlobals.ArenaEncounter == null)
        {
            GladiusGlobals.ArenaEncounter = new ArenaEncounter();
            GladiusGlobals.ArenaEncounter.Encounter = Encounter.Load(EncounterFile);
        }

        foreach (EncounterSide side in GladiusGlobals.ArenaEncounter.Encounter.Sides)
        {
            foreach (CharacterData cd in side.CharacterDataList)
            {
                cd.TeamName = side.TeamName;
                cd.School = side.School;

                string modelName = GladiusGlobals.AdjustModelName(cd.ActorClassData.MeshName);

                string objectPath = GladiusGlobals.CharacterModelsRoot + modelName;
                //String objectPath = "ModelPrefabs/" + modelName+"Prefab";
                UnityEngine.Object resourceToInstantiate = Resources.Load(objectPath);
                if (resourceToInstantiate == null)
                {
                    Debug.LogErrorFormat("Couldn't create [{0}]", objectPath);
                }
                else
                {
                    GameObject baseActorGameObject = CreateCharacter(resourceToInstantiate);
                    if (baseActorGameObject != null)
                    {
                        baseActorGameObject.name = "BaseActor" + cd.Name;
                        BaseActor ba1 = baseActorGameObject.GetComponent<BaseActor>();
                        ba1.Arena = Arena;
                        ba1.CombatEngine = CombatEngine;
                        ba1.CameraManager = CameraManager;

                        ba1.SetupCharacterData(cd);
                        if (cd.TeamName == "Player")
                        {
                            ba1.PlayerControlled = true;
                        }

                        actors.Add(ba1);
                    }
                    //ba1.SetupSkills(GladiusGlobals.AttackSkillDictionary);
                }
                SetupPropData();
            }
        }

        AssignPointList(actors, 0, Arena.PlayerPointList, 0);
        AssignPointList(actors, 1, Arena.PlayerPointList, 1);
        AssignPointList(actors, 2, Arena.Team1PointList, 0);
        AssignPointList(actors, 3, Arena.Team1PointList, 1);

        foreach (BaseActor actor in actors)
        {
            Arena.MoveActor(actor, actor.ArenaPoint);
            TurnManager.AddActor(actor);
        }

        TurnManager.StartRound();
    }

    public GameObject CreateCharacter(UnityEngine.Object resourceToInstantiate)
    {
        GameObject baseActorGameObject = Instantiate(resourceToInstantiate) as GameObject;
        return baseActorGameObject;

    }


    public void AssignPointList(List<BaseActor> actors,int actorIndex,List<Point> pointList,int pointListIndex)
    {
        if (actorIndex < actors.Count && pointListIndex < pointList.Count)
        {
            actors[actorIndex].SetStartPosition(pointList[pointListIndex]);
        }
    }


    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!setupLos)
        {
            setupLos = true;
            //GladiusGlobals.LOSTester.SetStartAndEnd(new Point(10,0), new Point(10, 14));
        }
        //GladiusGlobals.TurnManager.Update();
    }


    public void SetupPropData()
    {
        foreach (Prop prop in GladiusGlobals.ArenaEncounter.Encounter.Props)
        {
            string objectPath = GladiusGlobals.PropModelsRoot + prop.ModelName;
            //String objectPath = "ModelPrefabs/" + modelName+"Prefab";
            UnityEngine.Object resourceToInstantiate = Resources.Load(objectPath);
            if (resourceToInstantiate != null)
            {
                GameObject newProp = Instantiate(resourceToInstantiate) as GameObject;
                newProp.isStatic = true;
                newProp.name = "Prop-" + prop.Location.X + "-" + prop.Location.Y;
                Bounds b = ModelWindowHolder.GetEncapsulatingLocalBounds(newProp);
                BoxCollider bc = newProp.AddComponent<BoxCollider>();
                newProp.transform.localScale = GladiusGlobals.DefaultPropModelScale;
                //b.
                //FIXME - seemsto be in different place graphically to arena square?

                Arena.PlaceProp(prop.Location, b,newProp);
                //newProp.SetActive(false);
                //Material m = newProp.GetComponent<Renderer>().material;
                //m.getsh
                //// alpha out a bit so i can test.
                //Color color = newProp.GetComponent<Renderer>().material.color;
                //Color colora = new Color(color.r, color.g,color.b, 0.5f);
                //newProp.GetComponent<Renderer>().material.color = colora;
            }
        }
    }

}
