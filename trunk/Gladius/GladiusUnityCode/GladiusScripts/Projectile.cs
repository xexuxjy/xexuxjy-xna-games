using UnityEngine;
using System.Collections;
using Gladius;

public class Projectile : MonoBehaviour
{
    public float Speed = 3f;

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

    //public override void Draw(GameTime gameTime, ICamera camera)
    //{
    //    m_modelData.Draw(camera,World);
    //}

    //public override void LoadContent()
    //{
    //    base.LoadContent();
    //    ModelName = "Models/Shapes/UnitCylinder";
    //    m_modelData = new ModelData(ContentManager.Load<Model>(ModelName), new Vector3(0.05f,0.5f,0.05f),0f, ContentManager.GetColourTexture(Color.Magenta));
    //    m_modelData.ModelRotation = Matrix.CreateFromYawPitchRoll(0, MathHelper.PiOver2, 0);
    //    DrawOrder = Globals.CharacterDrawOrder;
    //}


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

    //public Vector3 Velocity
    //{
    //    get;
    //    set;
    //}

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
//}
