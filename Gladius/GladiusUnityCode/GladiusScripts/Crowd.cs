using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gladius
{
    public class Crowd : MonoBehaviour
	{
        List<dfProgressBar> m_teamBarList = new List<dfProgressBar>();

        public void Update()
        {
            foreach (string teamName in GladiusGlobals.TurnManager.AllTeams)
            {
                UpdateBar(teamName);
            }
        }

        public int GetValueForTeam(String teamName)
        {
            return m_crowdRankings[teamName];
        }

        public void UpdateBar(String teamName)
        {
            int val = 0;
            if (m_crowdRankings.TryGetValue(teamName, out val))
            {
                m_teamBars[teamName].Value = 0;
            }
        }

        public void Start()
        {
            //m_team1Bar = GameObject.Find("CrowdBar1").GetComponent<dfProgressBar>();
            //m_team2Bar = GameObject.Find("CrowdBar2").GetComponent<dfProgressBar>();
            //m_team3Bar = GameObject.Find("CrowdBar3").GetComponent<dfProgressBar>();
            //m_team4Bar = GameObject.Find("CrowdBar4").GetComponent<dfProgressBar>();

            //m_teamBars[GladiusGlobals.PlayerTeam] = m_team1Bar;
            //m_teamBars[GladiusGlobals.EnemyTeam1] = m_team2Bar;
            //m_teamBars[GladiusGlobals.EnemyTeam2] = m_team3Bar;
            //m_teamBars[GladiusGlobals.EnemyTeam3] = m_team4Bar;

            GladiusGlobals.Crowd = this;
        }


        public void UpdateTeamScore(String team, int value)
        {
            m_crowdRankings[team] += value;
        }

        public void RoundStarted()
        {
            int teamCounter = 0;
            foreach (string teamName in GladiusGlobals.TurnManager.AllTeams)
            {
                if (!m_crowdRankings.ContainsKey(teamName))
                {
                    m_crowdRankings[teamName] = 0;
                    if(teamCounter >= m_teamBarList.Count)
                    {
                        GameObject go = GameObject.Find("CrowdBar"+(teamCounter+1));
                        if(go != null)
                        {
                            dfProgressBar bar = go.GetComponent<dfProgressBar>();
                            m_teamBarList.Add(bar);
                            m_teamBars[teamName] = bar;
                        }
                    }


                }

                int teamVal = m_crowdRankings[teamName];
                if (teamVal >= 10 && teamVal < 20)
                {
                    teamVal -= 5;
                }
                else if (teamVal >= 20 && teamVal < 30)
                {
                    teamVal -= 10;
                }
                else if (teamVal >= 60 && teamVal < 100)
                {
                    teamVal -= 15;
                }
                else if (teamVal >= 100)
                {
                    teamVal -= 20;
                }

                teamVal = Math.Min(0, Math.Max(teamVal, 100));
                m_crowdRankings[teamName] = teamVal;
                teamCounter++;
            }
        }



        Dictionary<String, dfProgressBar> m_teamBars = new Dictionary<string, dfProgressBar>();
        Dictionary<String, int> m_crowdRankings = new Dictionary<string, int>();
	}
}
