using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gladius
{
	public class Crowd
	{

        public void Update()
        {

        }


        public void UpdateTeamScore(String team, int value)
        {
            m_crowdRankings[team] += value;
        }




        Dictionary<String, int> m_crowdRankings = new Dictionary<string, int>();
	}
}
