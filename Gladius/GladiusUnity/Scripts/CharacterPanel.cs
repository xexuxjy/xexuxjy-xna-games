using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class CharacterPanel : MonoBehaviour
{
    static string[] names = new string[] { "LVL", "XP", "Next", "HP", "DAM", "PWR", "ACC", "DEF", "INI", "CON", "MOV", "Arm", "Wpn" };
    GameObject[] statComponents;

    public GameObject StatsPanel;
    public GameObject StatRowPrefab;

    public Text CharacterLabel;
    public Text CharacterClass;

    public Button SelectLeftButton;
    public Button SelectRightButton;

    //public GameObject m_characterMeshHolderGO;

    public ModelWindowHolder m_modelWindowHolder;
    public CharacterMeshHolder m_characterMeshHolder;



    private CharacterDataList m_characterDataList;

    public CharacterDataList CharacterDataList
    {
        get { return m_characterDataList; }
        set
        {
            if (m_characterDataList != null)
            {
                m_characterDataList.CharacterChanged -= M_characterDataList_CharacterChanged;
                m_characterDataList = null;
            }
            m_characterDataList = value;
            if (m_characterDataList != null)
            {
                m_characterDataList.CharacterChanged += M_characterDataList_CharacterChanged;
            }
        }
    }

    public void Start()
    {
       
        float offset = 0;
        statComponents = new GameObject[names.Length];
        for(int i=0;i<names.Length;++i)
        {
            statComponents[i] = Instantiate(StatRowPrefab);
            statComponents[i].transform.SetParent(StatsPanel.transform);
            statComponents[i].transform.Find("Label").GetComponent<Text>().text = names[i];
        }

        SelectLeftButton.onClick.AddListener(()=> { CharacterPanel_ClickLeft(); });
        SelectRightButton.onClick.AddListener(() => { CharacterPanel_ClickRight(); });

    }

    

    private void M_characterDataList_CharacterChanged(object sender, EventArgs e)
    {
        UpdatePanel();
    }


    private void CharacterPanel_ClickLeft()
    {
        if (CharacterDataList != null)
        {
            CharacterDataList.CycleLeft();
        }
    }

    private void CharacterPanel_ClickRight()
    {
        if (CharacterDataList != null)
        {
            CharacterDataList.CycleRight();
        }
    }

    public void UpdatePanel()
    {
        if(CharacterDataList != null && CharacterDataList.CurrentCharacter != null)
        {
            CharacterLabel.text = CharacterDataList.CurrentCharacter.Name;
            CharacterClass.text = CharacterDataList.CurrentCharacter.ActorClass.ToString();

            for (int i = 0; i < names.Length; ++i)
            {
                statComponents[i].transform.FindChild("Value").GetComponent<Text>().text = CharacterDataList.CurrentCharacter.ValFromName(names[i]);
            }

            if (m_characterMeshHolder != null)
            {
                m_characterMeshHolder.SetupCharacterData(CharacterDataList.CurrentCharacter, null);
                m_modelWindowHolder.AttachModelToWindow(m_characterMeshHolder.m_gameModel);
            }
        }
    }

    public GameObject MainModel
    {
        get
        {
            return GetComponent<CharacterMeshHolder>().m_gameModel;
        }
        set
        {
            GetComponent<CharacterMeshHolder>().m_gameModel = value;
        }
    }

    public void OnDisable()
    {
        Debug.LogWarning("Character Panel Disabled");
    }

    public void OnEnable()
    {
        Debug.LogWarning("Character Panel Enabled");
    }


}
