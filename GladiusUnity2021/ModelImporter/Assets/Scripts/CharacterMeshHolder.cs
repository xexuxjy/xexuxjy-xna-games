using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;

public class CharacterMeshHolder : MonoBehaviour
{
    public GameObject m_weaponModel;
    public GameObject m_shieldModel;
    public GameObject m_armourModel;
    public GameObject m_helmetModel;
    public GameObject m_projectileModel;

    public Transform m_characterBone;
    public Transform m_translationBone;
    public Transform m_skullBone;
    public Transform m_cameraBone;
    public Transform m_zeroBone;
    public Transform m_mpShield;
    public Transform m_mpWeapon1;
    public Transform m_mpWeapon2;
    public Transform m_mpHelmet;
    public Transform m_mpHead;

    public Transform m_leftEyeLid;
    public Transform m_rightEyeLid;


    public List<string> ArmorVariants = new List<string>();
    public List<string> SkinVariants = new List<string>();
    public Dictionary<string, Transform> m_boneMap = new Dictionary<string, Transform>();

    private List<TransformPair> m_weaponTransformPairs = new List<TransformPair>();

    public Color ArmourTint = Color.white;

    public CharacterData CharacterData;

    public bool IsBlinkEnabled;
    private float m_blinkTime = (float)(GladiusGlobals.Random.NextDouble() + 2.5);


    public void UpdateBlink()
    {
        //if (IsBlinkEnabled && m_leftEyeLid != null && m_rightEyeLid != null)
        //{
        //    m_blinkTime -= Time.deltaTime;
        //    if (m_blinkTime < 0)
        //    {
        //        if (m_blinkTime < -0.15f)
        //        {
        //            m_blinkTime = (float)(GladiusGlobals.Random.NextDouble() + 2.5);
        //        }
        //        else
        //        {
        //            float lid = (float)Math.Sin((-m_blinkTime) / 0.15f * Math.PI) * 0.013f;
        //            Vector3 leftEye = m_leftEyeLid.transform.localPosition;
        //            Vector3 rightEye = m_rightEyeLid.transform.localPosition;
        //            leftEye.z -= lid;
        //            rightEye.z -= lid;
        //            m_leftEyeLid.transform.localPosition = leftEye;
        //            m_rightEyeLid.transform.localPosition = rightEye;

        //        }
        //    }
        //}


    }


    public GladiusCharacterAnim GladiusAnim
    {
        get;
        set;
    }

    public GameObject CameraTarget;

    private OptionalMaterials m_optionalMaterials = new OptionalMaterials();

    public void SetupCharacterData(CharacterData characterData, Transform parent)
    {
        SetupBones();

        LoadAndAttachModelForCharacter(characterData, ItemLocation.Weapon);
        LoadAndAttachModelForCharacter(characterData, ItemLocation.Shield);
        LoadAndAttachModelForCharacter(characterData, ItemLocation.Helmet);

        GladiusAnim = new GladiusCharacterAnim();
        GladiusAnim.Init(transform, characterData.CurrentClassDef);

        CameraTarget = new GameObject("CameraTarget");
        Transform ctParent = m_cameraBone != null ? m_cameraBone : transform;
        CameraTarget.transform.SetParent(ctParent, false);
        CameraTarget.transform.localPosition = GladiusGlobals.GMUp * 1.5f;
        CameraTarget.transform.localRotation = Quaternion.Inverse(GladiusGlobals.CharacterLocalRotation);

        BuildExtraMaterials();

        SelectArmourIndex(0);
        SelectSkinIndex(0);

        IsBlinkEnabled = true;

        transform.SetParent(parent);

    }

    private void BuildExtraMaterials()
    {
        foreach (string armorName in ArmorVariants)
        {
            Material m = GladiusGlobals.LoadMaterial(GladiusGlobals.MaterialsRoot + armorName);
            m_optionalMaterials.ArmourMaterials.Add(m);
        }
        foreach (string skinName in SkinVariants)
        {
            Material m = GladiusGlobals.LoadMaterial(GladiusGlobals.MaterialsRoot + skinName);
            m_optionalMaterials.SkinMaterials.Add(m);
        }

    }

    public void SetupBones()
    {
        FindAndAddTransform("mpShield", out m_mpShield);
        FindAndAddTransform("mpWeapon1", out m_mpWeapon1);
        FindAndAddTransform("mpWeapon2", out m_mpWeapon2);
        FindAndAddTransform("mpHelmet", out m_mpHelmet);

        FindAndAddTransform("character", out m_characterBone);
        FindAndAddTransform("faceSkull", out m_skullBone);
        FindAndAddTransform("Camera", out m_cameraBone);
        FindAndAddTransform("translation", out m_translationBone);
        FindAndAddTransform("Camera", out m_cameraBone);
        FindAndAddTransform("zero", out m_zeroBone);
        FindAndAddTransform("mpHead", out m_mpHead);

        //m_leftEyeLid = transform.FuzzyFindChild("faceUpEyeLid_L");
        //m_rightEyeLid = transform.FuzzyFindChild("faceUpEyeLid_R");

        if (m_characterBone != null)
        {
            Transform[] bones = m_characterBone.GetComponentsInChildren<Transform>();
            foreach (Transform bone in bones)
            {
                int index = bone.name.IndexOf("-");
                if (index > -1)
                {
                    string name = bone.name.Substring(0, index);
                    m_boneMap[name] = bone;
                }
            }

        }

    }

    private void FindAndAddTransform(string name, out Transform t)
    {
        t = transform.FuzzyFindChild(name);
        if (t != null)
        {
            m_boneTransformMap[name] = t;
        }
    }

    public Transform TransformForName(string name)
    {
        Transform t;
        m_boneTransformMap.TryGetValue(name, out t);
        return t;
    }

    private Dictionary<string, Transform> m_boneTransformMap = new Dictionary<string, Transform>();


    public void SelectArmourIndex(int index)
    {
        if (m_optionalMaterials != null)
        {
            List<Material> materials = m_optionalMaterials.ArmourMaterials;
            if (index == -1)
            {
                index = (int)Math.Floor((GladiusGlobals.Random.NextDouble() * materials.Count));
            }

            if (index <= materials.Count - 1)
            {
                Material newMaterial = materials[index];
                SkinnedMeshRenderer[] childSMR = GetComponentsInChildren<SkinnedMeshRenderer>();
                for (int i = 0; i < childSMR.Length; ++i)
                {
                    if (childSMR[i].material.name.ToLower().Contains("armor"))
                    {
                        childSMR[i].material = newMaterial;
                    }
                }
            }
        }
        ApplyArmourTint();
    }

    public void SelectSkinIndex(int index)
    {
        if (m_optionalMaterials != null)
        {
            List<Material> materials = m_optionalMaterials.SkinMaterials;
            if (index == -1)
            {
                index = (int)Math.Floor((GladiusGlobals.Random.NextDouble() * materials.Count));
            }

            if (index <= materials.Count - 1)
            {
                Material newMaterial = materials[index];
                SkinnedMeshRenderer[] childSMR = GetComponentsInChildren<SkinnedMeshRenderer>();
                for (int i = 0; i < childSMR.Length; ++i)
                {
                    if (childSMR[i].material.name.ToLower().Contains("skin"))
                    {
                        childSMR[i].material = newMaterial;
                    }
                }
            }
        }

    }


    public void ApplyArmourTint()
    {
        SkinnedMeshRenderer[] childSMR = GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < childSMR.Length; ++i)
        {
            if (childSMR[i].material.name.ToLower().Contains("armor"))
            {
                childSMR[i].material.color = ArmourTint;
            }
        }

    }

    public void ReenableMountPoints()
    {
        GladiusGlobals.SetSingleChildEnabled(m_mpShield, true);
        GladiusGlobals.SetSingleChildEnabled(m_mpWeapon1, true);
        GladiusGlobals.SetSingleChildEnabled(m_mpWeapon2, true);
        GladiusGlobals.SetSingleChildEnabled(m_mpHelmet, true);
    }


    public void CleanupModels()
    {
        DestroyModel(ref m_weaponModel);
        DestroyModel(ref m_shieldModel);
        DestroyModel(ref m_armourModel);
        DestroyModel(ref m_helmetModel);
    }

    private void DestroyModel(ref GameObject go)
    {
        if (go != null)
        {
            Destroy(go);
            go = null;
        }
    }

    public GameObject GetForLocation(ItemLocation itemLocation)
    {
        if (itemLocation == ItemLocation.Weapon)
        {
            return m_weaponModel;
        }
        if (itemLocation == ItemLocation.Shield)
        {
            return m_shieldModel;
        }
        if (itemLocation == ItemLocation.Helmet)
        {
            return m_helmetModel;
        }
        return null;
    }

    public void SetGameObjectForLocation(ItemLocation itemLocation, GameObject gameObject)
    {
        if (itemLocation == ItemLocation.Weapon)
        {
            m_weaponModel = gameObject;
            if (gameObject.name.Contains("bow_"))
            {
                m_weaponTransformPairs.Clear();
                m_weaponTransformPairs.Add(new TransformPair(transform.FuzzyFindChild("mpBowShaftTop"), transform.FuzzyFindChild("mpBowShaftTop")));
                m_weaponTransformPairs.Add(new TransformPair(transform.FuzzyFindChild("mpBowShaftTop"), transform.FuzzyFindChild("mpBowShaftTop")));
                m_weaponTransformPairs.Add(new TransformPair(transform.FuzzyFindChild("mpBowString"), transform.FuzzyFindChild("mpBowString")));
            }

        }
        else if (itemLocation == ItemLocation.Shield)
        {
            m_shieldModel = gameObject;
        }
        else if (itemLocation == ItemLocation.Helmet)
        {
            m_helmetModel = gameObject;
        }
    }


    public void AttachItemToCharacter(CharacterData characterData, Item item, int layerId = -1)
    {
        if (item != null && !characterData.CurrentClassDef.IsBeast)
        {

            Vector3 itemScale = characterData.CurrentClassDef.ScaleForItemSlot(item.ItemLocation);

            if (item.ItemLocation == ItemLocation.Weapon)
            {
                string meshName = item.MeshName;
                LoadAndAttachModel(m_mpWeapon1, item.ItemLocation, meshName, itemScale, layerId);
                if (item.MeshName2 != null)
                {
                    meshName = item.MeshName2;
                    LoadAndAttachModel(m_mpWeapon2, item.ItemLocation, meshName, itemScale, layerId);
                }
            }
            else
            {
                Transform t = GetTransformForLocation(item.ItemLocation, item.ItemSubType == "TwoHanded");
                LoadAndAttachModel(t, item.ItemLocation, item.MeshName, itemScale, layerId);
            }
        }

    }


    public void LoadAndAttachModelForCharacter(CharacterData characterData, ItemLocation itemLocation)
    {
        Item item = characterData.GetItemAtLocation(itemLocation);
        AttachItemToCharacter(characterData, item);
    }

    public Transform GetTransformForLocation(ItemLocation itemLocation, bool isTwoHanded = false)
    {
        //SetupBones();
        Transform boneTransform = null;

        if (itemLocation == ItemLocation.Weapon)
        {
            boneTransform = m_mpWeapon1;
        }
        else if (itemLocation == ItemLocation.Shield)
        {
            boneTransform = m_mpShield;
        }
        else if (itemLocation == ItemLocation.Helmet)
        {
            boneTransform = m_mpHelmet;
        }
        else if (itemLocation == ItemLocation.Projectile)
        {
            boneTransform = m_mpWeapon2;
        }
        return boneTransform;
    }
    public void LoadAndAttachModel(Transform boneTransform, ItemLocation itemLocation, string modelName, Vector3 scale, int layerId = -1)
    {
        Quaternion localRotation = Quaternion.identity;

        if (boneTransform != null)
        {
            DetachItem(boneTransform);

            string fullModelName = modelName;
            GameObject load = GladiusGlobals.InstantiateModel(fullModelName);

            if (load != null)
            {
                load.transform.parent = boneTransform;
                load.transform.localPosition = Vector3.zero;
                load.transform.localRotation = localRotation;

                // Thanks to shiftys post at : http://forum.unity3d.com/threads/how-to-use-local-scale.271694/
                float itemScale = 1 / load.transform.parent.localScale.x / load.transform.root.localScale.x;
                Vector3 tempScale = new Vector3(itemScale, itemScale, itemScale);
                tempScale = tempScale.Mult(scale);
                load.transform.localScale = tempScale;

                if (layerId != -1)
                {
                    GladiusGlobals.MoveToLayer(load.transform, layerId);
                }

                SetGameObjectForLocation(itemLocation, load);
            }
        }
    }

    public void DetachAllItems()
    {
        DetachItem(m_mpShield);
        DetachItem(m_mpWeapon1);
        DetachItem(m_mpWeapon2);
        DetachItem(m_mpHelmet);
        DetachItem(m_mpHead);
    }
    public void DetachItem(Transform t)
    {
        if (t != null)
        {
            for (int i = 0; i < t.childCount; ++i)
            {
                Transform ct = t.GetChild(i);
                // ignore odd mount points - amazon and others
                //if (!ct.name.StartsWith("mp"))
                {
                    ct.parent = null;
                    Destroy(ct.gameObject);
                    break;
                }
            }
        }
    }

    public void Update()
    {
        if (GladiusAnim != null)
        {
            GladiusAnim.Update();
        }
        foreach (TransformPair tp in m_weaponTransformPairs)
        {
            copyTransform(tp.From, tp.To);
        }
        UpdateBlink();

    }

    public void copyTransform(Transform from, Transform to)
    {
        if (from != null && to != null)
        {
            to.localRotation = from.localRotation;
            //to.localRotation = from.localRotation * Quaternion.Euler(0,180,0);
            to.localPosition = from.localPosition;
            to.localScale = from.localScale;
        }
    }

}

public struct TransformPair
{
    public TransformPair(Transform from, Transform to)
    {
        From = from;
        To = to;
    }

    public Transform From;
    public Transform To;
}


//[CustomEditor(typeof(CharacterMeshHolder))]
//public class CharacterMeshHolderEditor : Editor
//{


//    int _choiceIndex = 0;
//    string[] _choices = null;


//    public override void OnInspectorGUI()
//    {
//        CharacterMeshHolder meshHolder = target as CharacterMeshHolder;
//        // Draw the default inspector
//        DrawDefaultInspector();

//        if (meshHolder.DebugAnim)
//        {

//            if(meshHolder.GladiusAnim == null)
//            {
//                ActorGenerator.ActorGeneratorInit();
//                meshHolder.GladiusAnim = new GladiusCharacterAnim();
//                String name = meshHolder.gameObject.name.Replace(".mdl", "");
//                ActorClassDef actorClassDef = ActorGenerator.ActorClassDefs.Find(x => x.name.Equals(name,StringComparison.InvariantCultureIgnoreCase));

//                meshHolder.GladiusAnim.Init(null, actorClassDef);
//            }

//            if (_choices == null && meshHolder != null && meshHolder.GladiusAnim.m_animationMap.Values.Count > 0)
//            {
//                _choices = new string[meshHolder.GladiusAnim.m_animationMap.Keys.Count];
//                meshHolder.GladiusAnim.m_animationMap.Keys.CopyTo(_choices, 0);
//                Array.Sort<String>(_choices);
//            }
//            _choiceIndex = EditorGUILayout.Popup(_choiceIndex, _choices);
//            meshHolder.GladiusAnim.PlayAnimation(_choices[_choiceIndex]);
//            EditorUtility.SetDirty(target);
//        }
//    }
//}
