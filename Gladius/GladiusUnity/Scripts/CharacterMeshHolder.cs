using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterMeshHolder : MonoBehaviour
{
    public GameObject m_gameModel;
    public GameObject m_weaponModel;
    public GameObject m_shieldModel;
    public GameObject m_armourModel;
    public GameObject m_helmetModel;

    public List<string> ArmorVariants = new List<string>();
    public List<string> SkinVariants = new List<string>();

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

    public void SetForLocation(ItemLocation itemLocation, GameObject gameObject)
    {
        if (itemLocation == ItemLocation.Weapon)
        {
            m_weaponModel = gameObject;
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

    public void SetupCharacterData(CharacterData characterData, GameObject parent, bool createMainModel = true)
    {
        string modelName = characterData.ActorClassData.MeshName;
        if (createMainModel)
        {
            SetupMeshByName(modelName, parent);
        }
        else
        {
            m_gameModel = gameObject;
        }

        LoadAndAttachModelForCharacter(characterData, ItemLocation.Weapon);
        LoadAndAttachModelForCharacter(characterData, ItemLocation.Shield);
        LoadAndAttachModelForCharacter(characterData, ItemLocation.Helmet);
    }

    public void SetupMeshByName(string modelName, GameObject parent)
    {
        if (m_gameModel != null)
        {
            Destroy(m_gameModel);
        }

        modelName = GladiusGlobals.AdjustModelName(modelName);
        string objectPath = GladiusGlobals.CharacterModelsRoot + modelName;
        //String objectPath = "ModelPrefabs/" + modelName+"Prefab";
        m_gameModel = Instantiate(Resources.Load(objectPath)) as GameObject;

    }


    public void LoadAndAttachModelForCharacter(CharacterData characterData, ItemLocation itemLocation)
    {
        if (characterData.GetItemAtLocation(itemLocation) != null)
        {
            Item item = characterData.GetItemAtLocation(itemLocation);
            LoadAndAttachModel(itemLocation, item.MeshName, characterData.ActorClassData.IsTwoHanded);
        }

    }
    public void LoadAndAttachModel(ItemLocation itemLocation, string modelName, bool isTwoHanded = false)
    {
        string boneName = "";
        modelName = GladiusGlobals.AdjustModelName(modelName);

        if (itemLocation == ItemLocation.Weapon)
        {
            boneName = m_mountPoints[0];
        }
        else if (itemLocation == ItemLocation.Shield)
        {
            boneName = m_mountPoints[1];
        }
        else if (itemLocation == ItemLocation.Helmet)
        {
            boneName = m_mountPoints[2];
        }

        Transform boneTransform = null;
        Quaternion localRotation = Quaternion.identity;

        if (modelName.Contains("bow_"))
        {
            boneName = m_mountPoints[3];
            boneTransform = GladiusGlobals.FuzzyFindChild(boneName, m_gameModel.transform);
        }
        else
        {
            // deal with the fact that mp's can appear on each hand
            if (boneName == m_mountPoints[0])
            {
                Transform t = GladiusGlobals.FuzzyFindChild("armWrist_R", m_gameModel.transform);
                if (isTwoHanded)
                {
                    t = GladiusGlobals.FuzzyFindChild("armWrist_L", m_gameModel.transform);
                }
                boneTransform = GladiusGlobals.FuzzyFindChild(boneName, t);
            }
            else if (boneName == m_mountPoints[1])
            {
                boneTransform = GladiusGlobals.FuzzyFindChild(boneName, m_gameModel.transform);
            }
            else
            {
                boneTransform = GladiusGlobals.FuzzyFindChild(boneName, m_gameModel.transform);
            }
        }

        if (boneTransform != null)
        {
            // align rotations
            if (boneTransform.localRotation.eulerAngles.y > 100)
            {
                localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
            DetachItem(boneTransform);

            string fullModelName = GladiusGlobals.ModelsRoot + modelName;
            GameObject goPrefab = Resources.Load(fullModelName) as GameObject;
            if (goPrefab != null)
            {
                GameObject load = Instantiate(goPrefab) as GameObject;
                if (load != null)
                {
                    load.transform.parent = boneTransform;
                    load.transform.localPosition = Vector3.zero;
                    load.transform.localRotation = localRotation;

                    // Thanks to shiftys post at : http://forum.unity3d.com/threads/how-to-use-local-scale.271694/
                    float itemScale = 1 / load.transform.parent.localScale.x / load.transform.root.localScale.x;
                    load.transform.localScale = new Vector3(itemScale, itemScale, itemScale);

                    SetForLocation(itemLocation, load);
                }
                else
                {
                    Debug.LogWarning("Unable to load : " + fullModelName);
                }
            }
            else
            {
                Debug.LogWarning("Can't find boneName : " + boneName);
            }
        }
    }

    public void DetachItem(Transform t)
    {
        for (int i = 0; i < t.childCount; ++i)
        {
            Transform ct = t.GetChild(i);
            // ignore odd mount points - amazon and others
            if (!ct.name.StartsWith("mp"))
            {
                ct.parent = null;
                Destroy(ct.gameObject);
                break;
            }
        }
    }

    public void Update()
    {
        // bit ugly, but copy character animation to bow.
        if (false && m_weaponModel != null)
        {
            if (m_weaponModel.GetComponent<SkinnedMeshRenderer>())
            {
                Transform characterShaftTop = m_gameModel.transform.FindChild("mpBowShaftTop");
                Transform characterShaftBottom = m_gameModel.transform.FindChild("mpBowShaftTop");
                Transform characterBowString = m_gameModel.transform.FindChild("mpBowString");

                Transform weaponShaftTop = m_weaponModel.transform.FindChild("mpBowShaftTop");
                Transform weaponShaftBottom = m_weaponModel.transform.FindChild("mpBowShaftTop");
                Transform weaponrBowString = m_weaponModel.transform.FindChild("mpBowString");

                if (characterShaftTop && characterShaftBottom && characterBowString && weaponShaftTop && weaponShaftBottom && weaponrBowString)
                {
                    copyTransform(characterShaftTop, weaponShaftTop);
                    copyTransform(characterBowString, weaponrBowString);
                    copyTransform(characterShaftBottom, weaponShaftBottom);
                }
            }
        }
    }

    public void copyTransform(Transform from, Transform to)
    {
        to.localPosition = from.localPosition;
        to.localRotation = from.localRotation;
        to.localScale = from.localScale;
    }

    private string[] m_mountPoints = new string[] { "mpWeapon", "mpShield", "mpHelmet", "zero", "mpWeapon1" };

}
