using UnityEngine;
using System.Collections.Generic;
using System;
using Gladius;

public class OngoingSkillStatus
{
    private int m_duration = 0;
    private bool m_permanent;
    private SkillStatus m_status;
    private BaseActor m_source;
    private BaseActor m_target;
    public String m_name;

    public OngoingSkillStatus(SkillStatus status, BaseActor target)
    {
        if (m_status.statusDurationType == "Permanent")
        {
            Permanent = true;
        }
        else if (m_status.statusDurationType == "Turns Source")
        {

        }
        else if (m_status.statusDurationType == "Turns Target")
        {

        }
        else if (m_status.statusDurationType == "Until Negated")
        {

        }
        else if (m_status.statusDurationType == "Lapse on Failed Condition")
        {

        }
        else if (m_status.statusDurationType == "Lapse with order")
        {

        }
        else if (m_status.statusDurationType == "Stack Absolute")
        {

        }
        else if (m_status.statusDurationType == "Stack Source Mult")
        {

        }
        else if (m_status.statusDurationType == "Stack Target Mult")
        {

        }
    }
    
    

    public bool Permanent
    { get; set; }

    public void UpdateForSource(BaseActor actor)
    {
        if (m_status.statusDurationType == "Turns Source" && actor == m_source)
        {
            m_duration = -1;
        }
        else if (m_status.statusDurationType == "Turns Target" && actor == m_target)
        {
            m_duration = -1;

        }
    }

    public void Clear()
    {

    }

    public bool Expired()
    {
        return !m_permanent || m_duration > 0;
    }
}