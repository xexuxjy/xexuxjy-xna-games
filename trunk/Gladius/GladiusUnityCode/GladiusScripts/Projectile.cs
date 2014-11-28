using UnityEngine;
using System.Collections;
using Gladius;

public class Projectile : MonoBehaviour
{
    public float Speed = 3f;
    GameObject m_modelObject;

    public void Start()
    {
        m_modelObject = Instantiate(Resources.Load(GladiusGlobals.ModelsRoot + "bow_arrow")) as GameObject;
        if (m_modelObject != null)
        {
            m_modelObject.transform.parent = transform;
            m_modelObject.transform.localPosition = Vector3.zero;
            m_modelObject.transform.localRotation = Quaternion.Euler(new Vector3(90, 90, 0));
            m_modelObject.transform.localScale = new Vector3(100f, 100f, 100f);
        }
    }


    public void Update()
    {
        float elapsed = Time.deltaTime;
        //base.VariableUpdate(gameTime);
        Position += m_velocity * elapsed;
        Vector3 diff = Target.Position - Position;
        // don't care about height check.
        diff.y = 0;
        float closeEnough = 0.1f;
        float len = diff.sqrMagnitude;
        if (len < closeEnough)
        {
            // do damage.
            // end state.
            GladiusGlobals.CombatEngine.ResolveAttack(Owner, Target, AttackSKill);
            //Owner.Attacking = false;
            Owner.StopAttack();
            gameObject.SetActive(false);
            Debug.Log("Projectile stop attack.");
        }

        gameObject.transform.position = Position;
    }

    private BaseActor m_target;
    private Vector3 m_velocity;

    public BaseActor Target
    {
        get
        {
            return m_target;
        }
        set
        {
            m_target = value;
            Vector3 diff = m_target.Position - Position;
            diff.y = 0;
            diff.Normalize();
            transform.rotation = Quaternion.LookRotation(diff) * modelRotation;

            m_velocity = diff * Speed;
        }


    }

    Quaternion modelRotation = Quaternion.Euler(90, 0, 0);

    public AttackSkill AttackSKill
    {
        get;
        set;
    }

    public BaseActor Owner
    {
        get;
        set;
    }

    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
        set
        {
            transform.position = value;
        }
    }
}



