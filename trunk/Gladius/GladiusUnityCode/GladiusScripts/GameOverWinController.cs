
using UnityEngine;
using System.Collections;
using Gladius;
using System.Collections.Generic;
using System;


public class GameOverWinController : MonoBehaviour
{
    private GameOverWinState CurrentState = GameOverWinState.Stage1;
    private Font m_largeFont;
    GUIStyle m_largeTextStyle;
    GUIStyle m_smallTextStyle;


    GUIText m_largeText;
    GUIText m_smallText;

    public Rect TextPos = new Rect(100, 100, 300, 50);

    // Use this for initialization
    void Start()
    {
        RegisterListeners();

        //GladiusGlobals.CurrentBattleData = BattleData.GenerateDummyBattleData();
        //GladiusGlobals.GladiatorSchool = new GladiatorSchool();
        //GladiusGlobals.AttackSkillDictionary = new AttackSkillDictionary();
        //GladiusGlobals.AttackSkillDictionary.Load(null);

        m_largeFont = Resources.Load<Font>("GladiusUI/Arena/TREBUC");

        m_largeTextStyle = new GUIStyle();
        m_largeTextStyle.font = m_largeFont;
        m_largeTextStyle.fontSize = 40;
        m_largeTextStyle.fontStyle = FontStyle.Bold;
        m_largeTextStyle.normal.textColor = Color.black;

    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentState)
        {
            case GameOverWinState.Stage1:
                break;
            case GameOverWinState.Stage2:
                break;
            case GameOverWinState.Stage3:
                break;

        }
    }




    public void RegisterListeners()
    {
        EventManager.ActionPressed += new EventManager.ActionButtonPressed(EventManager_ActionPressed);
        

    }

    void EventManager_ActionPressed(object sender, ActionButtonPressedArgs e)
    {
        if (e.ActionButton == ActionButton.ActionButton1)
        {

            switch (CurrentState)
            {
                case GameOverWinState.Stage1:
                    CurrentState = GameOverWinState.Stage2;
                    break;
                case GameOverWinState.Stage2:
                    CurrentState = GameOverWinState.Stage3;
                    break;
                case GameOverWinState.Stage3:
                    UnregisterListeners();
                    Application.LoadLevel("MainMenu");
                    break;

            }

        }
    
    }


    public void OnGUI()
    {
        switch (CurrentState)
        {
            case GameOverWinState.Stage1:
                DrawStage1();
                break;
            case GameOverWinState.Stage2:
                DrawStage2();
                break;
            case GameOverWinState.Stage3:
                DrawStage3();
                break;
        }

    }

    public void DrawStage1()
    {
        GUI.Label(TextPos, "Press " + UserControl.ActionButton1Name + " to continue.", m_largeTextStyle);
    }


    public void DrawStage2()
    {
        //Rect textPos = new Rect(100, 100, 300, 50);
        //Rect textPosCopy = textPos;
        //int lineSeparator = 20;
        //if (GladiusGlobals.CurrentBattleData != null)
        //{
        //    GUI.Label(textPosCopy, GladiusGlobals.CurrentBattleData.Name, m_largeTextStyle);
        //    textPosCopy.y += lineSeparator;
        //    GUI.Label(textPosCopy, "XP Gained " + GladiusGlobals.CurrentBattleData.XPReward, m_largeTextStyle);
        //    textPosCopy.y += lineSeparator;
        //    GUI.Label(textPosCopy, "Gold Gained " + GladiusGlobals.CurrentBattleData.GoldReward, m_largeTextStyle);
        //    textPosCopy.y += lineSeparator*2;
        //    List<CharacterData> partyData = GladiusGlobals.GladiatorSchool.CurrentParty;
        //    for (int i = 0; i < partyData.Count; ++i)
        //    {
        //        //if (ActorGenerator.CheckLevelUp(partyData[i], GladiusGlobals.CurrentBattleData.XPReward))
        //        //{
        //        //    GUI.Label(textPosCopy, String.Format("{0} leveled up!   {1} ---> {2}", partyData[i].Name, partyData[i].Level, partyData[i].Level + 1), m_largeTextStyle);
        //        //}
        //    }




        //}
    }

    public void ApplyXPGain()
    {
            //List<CharacterData> partyData = GladiusGlobals.GladiatorSchool.CurrentParty;
            //for (int i = 0; i < partyData.Count; ++i)
            //{
            //    partyData[i].XP += GladiusGlobals.GameStateManager.ArenaStateCommon.CurrentBattleData.XPReward;
            //    //if (ActorGenerator.CheckLevelUp(partyData[i], GladiusGlobals.CurrentBattleData.XPReward))
            //    //{
            //    //    partyData[i].XP += GladiusGlobals.CurrentBattleData.XPReward;
            //    //}
            //}

    }


    public void DrawStage3()
    {
        GUI.Label(TextPos, "Press " + UserControl.ActionButton1Name + " to continue.", m_largeTextStyle);
    }


    public void UnregisterListeners()
    {
        EventManager.ActionPressed -= new EventManager.ActionButtonPressed(EventManager_ActionPressed);

    }



    enum GameOverWinState
    {
        Stage1,
        Stage2,
        Stage3,
    }
}
