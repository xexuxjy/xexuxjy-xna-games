using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Crowd : MonoBehaviour
{
    public TurnManager TurnManager;

    List<Progress> m_teamBarList = new List<Progress>();

    public void Update()
    {
        if (TurnManager != null)
        {
            foreach (GladiatorSchool school in TurnManager.AllSchools)
            {
                UpdateBar(school);
            }
        }
    }

    public int GetValueForSchool(GladiatorSchool school)
    {
        int val = 0;
        m_crowdRankings.TryGetValue(school, out val);
        return val;
    }

    public void UpdateBar(GladiatorSchool school)
    {
        int val = 0;
        if (m_crowdRankings.TryGetValue(school, out val))
        {
            //m_teamBars[school].Value = 0;
        }
    }

    public void Start()
    {
        //for (int i = 0; i < 4; ++i)
        //{
        //    dfProgressBar bar = GameObject.Find("CrowdBar" + (i + 1)).GetComponent<dfProgressBar>();
        //    bar.IsVisible = false;
        //}
        //m_team1Bar = GameObject.Find("CrowdBar1").GetComponent<dfProgressBar>();
        //m_team2Bar = GameObject.Find("CrowdBar2").GetComponent<dfProgressBar>();
        //m_team3Bar = GameObject.Find("CrowdBar3").GetComponent<dfProgressBar>();
        //m_team4Bar = GameObject.Find("CrowdBar4").GetComponent<dfProgressBar>();

        //m_teamBars[GladiusGlobals.PlayerTeam] = m_team1Bar;
        //m_teamBars[GladiusGlobals.EnemyTeam1] = m_team2Bar;
        //m_teamBars[GladiusGlobals.EnemyTeam2] = m_team3Bar;
        //m_teamBars[GladiusGlobals.EnemyTeam3] = m_team4Bar;
    }


    public void UpdateSchoolScore(GladiatorSchool school, int value)
    {
        m_crowdRankings[school] += value;
    }

    public void RoundStarted()
    {
        int teamCounter = 0;
        foreach (GladiatorSchool school in TurnManager.AllSchools)
        {
            //if (!m_crowdRankings.ContainsKey(school))
            //{
            //    m_crowdRankings[school] = 0;
            //    if (teamCounter >= m_teamBarList.Count)
            //    {
            //        GameObject go = GameObject.Find("CrowdBar" + (teamCounter + 1));
            //        if (go != null)
            //        {
            //            dfProgressBar bar = go.GetComponent<dfProgressBar>();
            //            bar.IsVisible = true;
            //            bar.Color = school.SchoolColour;
            //            m_teamBarList.Add(bar);
            //            m_teamBars[school] = bar;
            //        }
            //    }


            //}

            //int teamVal = m_crowdRankings[school];
            //if (teamVal >= 10 && teamVal < 20)
            //{
            //    teamVal -= 5;
            //}
            //else if (teamVal >= 20 && teamVal < 30)
            //{
            //    teamVal -= 10;
            //}
            //else if (teamVal >= 60 && teamVal < 100)
            //{
            //    teamVal -= 15;
            //}
            //else if (teamVal >= 100)
            //{
            //    teamVal -= 20;
            //}

            //teamVal = Math.Min(0, Math.Max(teamVal, 100));
            //m_crowdRankings[school] = teamVal;
            //teamCounter++;
        }
    }



    //Dictionary<GladiatorSchool, dfProgressBar> m_teamBars = new Dictionary<GladiatorSchool, dfProgressBar>();
    Dictionary<GladiatorSchool, int> m_crowdRankings = new Dictionary<GladiatorSchool, int>();
}
