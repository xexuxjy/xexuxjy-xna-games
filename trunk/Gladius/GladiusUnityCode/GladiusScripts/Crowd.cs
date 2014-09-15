using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gladius
{
    public class Crowd : MonoBehaviour
	{
        dfProgressBar m_team1Bar;
        dfProgressBar m_team2Bar;
        dfProgressBar m_team3Bar;
        dfProgressBar m_team4Bar;


        public void Update()
        {
            UpdateBar(GladiusGlobals.PlayerTeam);
            UpdateBar(GladiusGlobals.EnemyTeam1);
            UpdateBar(GladiusGlobals.EnemyTeam2);
            UpdateBar(GladiusGlobals.EnemyTeam3);
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
            m_team1Bar = GameObject.Find("CrowdBar1").GetComponent<dfProgressBar>();
            m_team2Bar = GameObject.Find("CrowdBar2").GetComponent<dfProgressBar>();
            m_team3Bar = GameObject.Find("CrowdBar3").GetComponent<dfProgressBar>();
            m_team4Bar = GameObject.Find("CrowdBar4").GetComponent<dfProgressBar>();

            m_teamBars[GladiusGlobals.PlayerTeam] = m_team1Bar;
            m_teamBars[GladiusGlobals.EnemyTeam1] = m_team2Bar;
            m_teamBars[GladiusGlobals.EnemyTeam2] = m_team3Bar;
            m_teamBars[GladiusGlobals.EnemyTeam3] = m_team4Bar;


        }


        public void UpdateTeamScore(String team, int value)
        {
            m_crowdRankings[team] += value;
        }

        public void TurnStarted(String teamName)
        {
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
        }



        Dictionary<String, dfProgressBar> m_teamBars = new Dictionary<string, dfProgressBar>();
        Dictionary<String, int> m_crowdRankings = new Dictionary<string, int>();
	}
}
