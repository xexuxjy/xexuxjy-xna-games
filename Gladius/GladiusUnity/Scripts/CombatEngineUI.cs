using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StringLeakTest;
using Gladius.util;
using UnityEngine;
using UnityEngine.UI;

public class CombatEngineUI : MonoBehaviour
{

    public bool DrawHealthBars = true;
    public bool DrawCombatText = true;
    public float NearBound = 0f;
    public float FarBound = 30f;
    public float Multiplier = 3;
    public const float TextFloatSpeed = 3f;

    public GameObject CombatTextPrefab;
    public TurnManager TurnManager;

    public UnityEngine.Sprite RedSprite;
    public UnityEngine.Sprite GreenSprite;
    public UnityEngine.Sprite EnemyIndicator;
    public UnityEngine.Sprite FriendIndicator;
    private Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();



    public void Start()
    {
        LoadSpriteDictionary();
        RedSprite = GetSpriteByName("red");
        GreenSprite = GetSpriteByName("green");
        EnemyIndicator = GetSpriteByName("enemyindicator");
        FriendIndicator = GetSpriteByName("friendindicator");
    }

    private void LoadSpriteDictionary()
    {
        Sprite[] SpritesData = Resources.LoadAll<Sprite>(GladiusGlobals.ArenaUIRoot+"ArenaUIAtlas");

        for (int i = 0; i < SpritesData.Length; i++)
        {
            Sprites.Add(SpritesData[i].name.ToLower(), SpritesData[i]);
        }
    }

    public Sprite GetSpriteByName(string name)
    {
        if (Sprites.ContainsKey(name))
            return Sprites[name];
        else
            return null;
    }

    public void DrawFloatingText(Vector3 initialPosition, Color textColor, StringBuilder text, float age)
    {
        if (DrawCombatText)
        {
            FloatingText ft = GetFloatingText();
            ft.Initialise(initialPosition, text, textColor, age);
            m_activeFloatingText.Add(ft);
        }
    }

    public void DrawFloatingText(Vector3 initialPosition, Color textColor, String text, float age)
    {
        if (DrawCombatText)
        {
            FloatingText ft = GetFloatingText();
            ft.Initialise(initialPosition, text, textColor, age);
            m_activeFloatingText.Add(ft);
        }
    }


    public void Update()
    {
        foreach (FloatingText ft in m_activeFloatingText)
        {
            ft.Update();
            if (ft.Complete)
            {
                FreeFloatingText(ft);
            }
        }

        int removed = m_activeFloatingText.RemoveAll(t => t.Complete);


        if (DrawHealthBars)
        {
            if (TurnManager != null)
            {
                foreach (BaseActor baseActor in TurnManager.AllActors)
                {
                    DrawNameHealthIndicators(baseActor);
                }
            }
        }


    }


    private FloatingText GetFloatingText()
    {
        FloatingText ft = null;
        if (m_floatingTextPool.Count == 0)
        {
            GameObject ftObject = new GameObject();
            ft = ftObject.AddComponent<FloatingText>();
            // store them under the combat engine ui 'group' rather than in the top level
            ft.transform.parent = transform;
            m_floatingTextPool.Push(ft);
        }
        ft = m_floatingTextPool.Pop();
        ft.gameObject.SetActive(true);
        return ft;
    }

    private void FreeFloatingText(FloatingText ft)
    {
        ft.Reset();
        ft.gameObject.SetActive(false);
        m_floatingTextPool.Push(ft);
    }

    public void LoadContent()
    {
        EventManager.BaseActorChanged += new EventManager.BaseActorSelectionChanged(EventManager_BaseActorChanged);
    }

    //public static Vector3 WorldToScreenAdjusted(Vector3 world)
    //{
    //    Vector3 result = GladiusGlobals.Camera.WorldToScreenPoint(world);
    //    result.y = Screen.height - result.y;
    //    return result;
    //}

    public void DrawNameHealthIndicators(BaseActor actor)
    {
        // *** Update this to use daikon followobject3d class along with helpers.
        if (actor.m_healthBar != null)
        {
            GameObject uiPanel = actor.m_healthBar;

            Text rtl = uiPanel.transform.Find("Panel/Name").GetComponent<Text>();
            rtl.text = actor.Name;

            Progress progress = uiPanel.transform.Find("Panel/ProgressBar").GetComponent<Progress>();
            progress.MaxValue = actor.MaxHealth;
            progress.Value = actor.Health;
            if (actor.MaxHealth != 0)
            {
                float healthVal = (actor.Health / actor.MaxHealth);
                //progress.Value = healthVal;
                //dfp.ProgressColor = CombatEngine.IsNearDeath(actor) ? Color.red : Color.green;
                Sprite progressSprite = CombatEngine.IsNearDeath(actor) ? RedSprite : GreenSprite;
                progress.SetSprite(progressSprite);
            }
            // Draw an enemy indicator based on current player.
            if (TurnManager != null)
            {
                //Transform child = GladiusGlobals.FindChild("Indicators/Image", uiPanel.transform);
                Transform child = uiPanel.transform.Find("Panel/Indicators/Image");


                Image indicator = child.GetComponent<Image>();

                UnityEngine.Sprite markerName = EnemyIndicator;
                if (TurnManager.CurrentActor != null)
                {
                    if (TurnManager.CurrentActor.TeamName == actor.TeamName)
                    {
                        markerName = FriendIndicator;
                    }
                }
                indicator.sprite = markerName;
            }
        }
    }


    void EventManager_BaseActorChanged(object sender, BaseActorChangedArgs e)
    {
        //CurrentActor = e.New;
    }


    private List<FloatingText> m_activeFloatingText = new List<FloatingText>();
    private Stack<FloatingText> m_floatingTextPool = new Stack<FloatingText>();
}

public class FloatingText : MonoBehaviour
{
    private CombatEngineUI m_combatEngineUI;
    private Text m_combatText;
    private GameObject m_textCopy;
    private Camera m_camera;
    private bool m_initialised;
    private static int s_counter;

    public void Start()
    {
        name = "CombatText" + (s_counter++);
    }

    public void Reset()
    {
        m_initialised = false;
    }

    public void Update()
    {
        if (m_textCopy == null)
        {

            GameObject combatTextPrefab = Resources.Load<GameObject>("Prefabs/CombatTextCanvasPrefab");
            if (combatTextPrefab != null)
            {
                m_textCopy = Instantiate(combatTextPrefab);
                m_combatText = m_textCopy.transform.Find("MainPanel/Text").GetComponent<Text>();
            }
        }

        if (m_textCopy != null && !m_initialised)
        {
            m_combatText.text = CombatText;
            m_combatText.color = TextColor;
            m_textCopy.transform.position = StartPos;
            m_initialised = true;
        }

        if (m_camera == null)
        {
            m_camera = GameObject.Find("MainCamera").GetComponent<Camera>();
        }
        if (m_camera != null)
        {
            m_textCopy.transform.LookAt(m_textCopy.transform.position + m_camera.transform.rotation * Vector3.forward,
                m_camera.transform.rotation * Vector3.up);
        }
        float elapsed = Time.deltaTime;
        float update = CombatEngineUI.TextFloatSpeed * elapsed;
        Age += elapsed;

        Vector3 delta = new Vector3(0, update, 0);
        m_textCopy.transform.position += delta;

    }

    public String CombatText
    { get; set; }

    public Vector3 StartPos
    {get;set;}

    public Color TextColor
    {
        get; set;
    }
    public float Age
    {
        get; set;
    }
    public float MaxAge
    {
        get; set;
    }

    public bool Complete
    {
        get { return (Age > MaxAge); }
    }

    public void Initialise(Vector3 worldPosition, StringBuilder textToCopy, Color color, float maxAge)
    {
        Age = 0f;
        TextColor = color;
        MaxAge = maxAge;
        StartPos = worldPosition;
        CombatText = textToCopy.ToString();
    }

    public void Initialise(Vector3 worldPosition, String textToCopy, Color color, float maxAge)
    {
        Age = 0f;
        TextColor = color;
        MaxAge = maxAge;
        StartPos = worldPosition;
        CombatText = textToCopy;
    }

    #region IComparable Members


    #endregion
}

