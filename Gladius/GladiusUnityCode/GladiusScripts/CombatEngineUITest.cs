using UnityEngine;
using System.Collections;
using Gladius;

public class CombatEngineUITest : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.fixedDeltaTime;
        if(Timer > Frequency)
        {
            Timer = 0;
            int deltaX = (int)(GladiusGlobals.Random.NextDouble() * RandomX);
            if (GladiusGlobals.Random.NextDouble() < 0.5)
            {
                deltaX *= -1;
            }
            int deltaZ = (int)(GladiusGlobals.Random.NextDouble() * RandomZ);
            if (GladiusGlobals.Random.NextDouble() < 0.5)
            {
                deltaZ *= -1;
            }

            GladiusGlobals.CombatEngineUI.DrawFloatingText(new Vector3(X+deltaX, Y, Z+deltaZ), Color.red, "Some Test TExt", Age);
        }
    }


    public float Timer = 0;
    public float Frequency = 1f;
    public float Age = 3;
    public int X = 0;
    public int Y = 3;
    public int Z = 0;
    public int RandomX = 10;
    public int RandomZ = 10;
}
