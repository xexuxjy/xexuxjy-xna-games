//using UnityEngine;
//using System.Collections;
//using System;

//public class RagdollUIController : CommonStartup
//{
//    //dfDropdown m_characterChoiceDD;
//    //dfDropdown m_weaponDD;
//    //dfDropdown m_shieldDD;
//    //dfDropdown m_armourDD;
//    //dfDropdown m_helmetDD;

//    //GameObject m_gameModel;
//    //GameObject m_weaponModel;
//    //GameObject m_shieldModel;
//    //GameObject m_armourModel;
//    //GameObject m_helmetModel;

//    //ItemManager m_itemManger;
//    //// Use this for initialization
//    //public override void ChildStart()
//    //{
//    //    dfSprite background  = GetComponent<dfSprite>();
//    //    m_characterChoiceDD = background.Find<dfDropdown>("CharacterChoice");
//    //    m_weaponDD = background.Find<dfDropdown>("WeaponChoice");
//    //    m_shieldDD = background.Find<dfDropdown>("ShieldChoice");
//    //    m_armourDD = background.Find<dfDropdown>("ArmourChoice");
//    //    m_helmetDD = background.Find<dfDropdown>("HelmetChoice");

//    //    SetupInitialData();   

//    //}

//    //void SetupInitialData()
//    //{
//    //    m_itemManger = GladiusGlobals.GameStateManager.ItemManager;

//    //    foreach (string model in m_models)
//    //    {
//    //        m_characterChoiceDD.AddItem(model);
//    //    }

//    //    foreach (String itemName in m_itemManger.Keys)
//    //    {
//    //        Item item = m_itemManger[itemName];
//    //        dfDropdown dd = null;
//    //        if (item.Location == ItemLocation.Weapon)
//    //        {
//    //            dd = m_weaponDD;
//    //        }
//    //        else if (item.Location == ItemLocation.Shield)
//    //        {
//    //            dd = m_shieldDD;
//    //        }
//    //        else if (item.Location == ItemLocation.Armor)
//    //        {
//    //            dd = m_armourDD;
//    //        }
//    //        else if (item.Location == ItemLocation.Helmet)
//    //        {
//    //            dd = m_helmetDD;
//    //        }

//    //        if (dd != null)
//    //        {
//    //            dd.AddItem(item.Name);
//    //        }

//    //    }


//    //    m_characterChoiceDD.SelectedIndexChanged += M_characterChoiceDD_SelectedIndexChanged;
//    //    m_weaponDD.SelectedIndexChanged += M_weaponDD_SelectedIndexChanged;
//    //    m_shieldDD.SelectedIndexChanged += M_shieldDD_SelectedIndexChanged;
//    //    m_armourDD.SelectedIndexChanged += M_armourDD_SelectedIndexChanged;
//    //    m_helmetDD.SelectedIndexChanged += M_helmetDD_SelectedIndexChanged;
//    //}

//    //private void M_helmetDD_SelectedIndexChanged(dfControl control, int value)
//    //{
//    //    String key = m_helmetDD.Items[value];
//    //    Item item = m_itemManger[key];
//    //    if (item != null) ;
//    //    {
//    //        LoadAndAttachModel(m_mountPoints[2], item.MeshName);
//    //    }
//    //}

//    //private void M_armourDD_SelectedIndexChanged(dfControl control, int value)
//    //{
//    //    String key = m_armourDD.Items[value];
//    //    Item item = m_itemManger[key];
//    //    if (item != null) ;
//    //    {
//    //        LoadAndAttachModel(m_mountPoints[3], item.MeshName);
//    //    }
//    //}

//    //private void M_shieldDD_SelectedIndexChanged(dfControl control, int value)
//    //{
//    //    String key = m_shieldDD.Items[value];
//    //    Item item = m_itemManger[key];
//    //    if (item != null);
//    //    {
//    //        LoadAndAttachModel(m_mountPoints[1], item.MeshName);
//    //    }
//    //}

//    //private void M_weaponDD_SelectedIndexChanged(dfControl control, int value)
//    //{
//    //    String key = m_weaponDD.Items[value];
//    //    Item item = m_itemManger[key];
//    //    if (item != null);
//    //    {
//    //        LoadAndAttachModel(m_mountPoints[0], item.MeshName);

//    //    }

//    //}

//    //private void M_characterChoiceDD_SelectedIndexChanged(dfControl control, int value)
//    //{
//    //    if (m_gameModel != null)
//    //    {
//    //        Destroy(m_gameModel);
//    //    }

//    //    string modelName = m_models[value];
//    //    String objectPath = GladiusGlobals.CharacterModelsRoot + modelName;
//    //    //String objectPath = "ModelPrefabs/" + modelName+"Prefab";
//    //    if (!objectPath.EndsWith(".mdl"))
//    //    {
//    //        objectPath += ".mdl";
//    //    }
//    //    Debug.Log("Loading model : " + objectPath);
//    //    m_gameModel = Instantiate(Resources.Load(objectPath)) as GameObject;
        
//    //    GameObject.Find("/ItemCamHolder").GetComponent<ModelWindowHolder>().AttachModelToWindow(m_gameModel);

//    //}


//    //// Update is called once per frame
//    //void Update()
//    //{

//    //}

//    //public void DetachItem(Transform t)
//    //{
//    //    for (int i = 0; i < t.childCount; ++i)
//    //    {
//    //        Transform ct = t.GetChild(i);
//    //        // ignore odd mount points - amazon and others
//    //        if (!ct.name.StartsWith("mp"))
//    //        {
//    //            ct.parent = null;
//    //            Destroy(ct.gameObject);
//    //            break;
//    //        }
//    //    }
//    //}


//    //public void LoadAndAttachModel(String boneName, String modelName)
//    //{
//    //    Transform boneTransform = null;
//    //    Quaternion localRotation = Quaternion.identity;
//    //    // deal with the fact that mp's can appear on each hand
//    //    if (boneName == m_mountPoints[0])
//    //    {
//    //        Transform t = GladiusGlobals.FuzzyFindChild("armWrist_R", m_gameModel.transform);
//    //        boneTransform = GladiusGlobals.FuzzyFindChild(boneName, t);
//    //    }
//    //    else if (boneName == m_mountPoints[1])
//    //    {
//    //        boneTransform = GladiusGlobals.FuzzyFindChild(boneName, m_gameModel.transform);
//    //    }
//    //    else
//    //    {
//    //        boneTransform = GladiusGlobals.FuzzyFindChild(boneName, m_gameModel.transform);
//    //    }

//    //    if (boneTransform != null)
//    //    {
//    //        // align rotations
//    //        if (boneTransform.localRotation.eulerAngles.y > 100)
//    //        {
//    //            localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
//    //        }
//    //        DetachItem(boneTransform);

//    //        String fullModelName = GladiusGlobals.ModelsRoot + modelName;
//    //        GameObject goPrefab = Resources.Load(fullModelName) as GameObject;
//    //        if (goPrefab != null)
//    //        {
//    //            GameObject load = Instantiate(goPrefab) as GameObject;
//    //            if (load != null)
//    //            {
//    //                load.transform.parent = boneTransform;
//    //                load.transform.localPosition = Vector3.zero;
//    //                load.transform.localRotation = localRotation;

//    //                // Thanks to shiftys post at : http://forum.unity3d.com/threads/how-to-use-local-scale.271694/
//    //                float itemScale = 1 / load.transform.parent.localScale.x / load.transform.root.localScale.x;
//    //                load.transform.localScale = new Vector3(itemScale, itemScale, itemScale);

//    //            }
//    //        }
//    //        else
//    //        {
//    //            Debug.LogWarning("Unable to load : " + fullModelName);
//    //        }
//    //    }
//    //    else
//    //    {
//    //        Debug.LogWarning("Can't find boneName : " + boneName);
//    //    }
//    //}



//    //private string[] m_models = new string[] { "amazon","archer", "archerF","banditA", "banditAF", "banditBF","barbarian","barbarianF" };
//    //private string[] m_mountPoints = new string[] { "mpWeapon", "mpShield", "mpHelmet","zero" };

//}
