using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StringLeakTest;
using Gladius.util;
using UnityEngine;

    public class CombatEngineUI : MonoBehaviour
    {

        public bool DrawHealthBars = true;
        public bool DrawCombatText = true;
        public float NearBound = 0f;
        public float FarBound = 30f;
        public float Multiplier = 3;
        public float TextFloatSpeed = 3f;

        public GameObject CombatTextPrefab;

        String atlasPath = "GladiusUI/Arena/ArenaUIAtlas";



        public void Start()
        {
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
                if (GladiusGlobals.GameStateManager.ArenaStateCommon.TurnManager != null)
                {
                    foreach (BaseActor baseActor in GladiusGlobals.GameStateManager.ArenaStateCommon.TurnManager.AllActors)
                    {
                        DrawNameHealthIndicators(baseActor);
                    }
                }
            }


        }


        private FloatingText GetFloatingText()
        {
            if (m_floatingTextPool.Count == 0)
            {
                m_floatingTextPool.Push(new FloatingText(this));
            }
            FloatingText ft = m_floatingTextPool.Pop();
            ft.SetActive(true);
            return ft;
        }

        private void FreeFloatingText(FloatingText ft)
        {
            ft.SetActive(false);
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

            GameObject uiPanel = actor.m_healthBar;

            dfLabel rtl = GladiusGlobals.FindChild("Name", uiPanel.transform).GetComponent<dfLabel>();
            rtl.Text = actor.Name;

            dfProgressBar dfp = GladiusGlobals.FindChild("ProgressBar", uiPanel.transform).GetComponent<dfProgressBar>();
            if (actor.MaxHealth != 0)
            {
                float healthVal = (actor.Health / actor.MaxHealth);

                dfp.Value = healthVal;
                dfp.ProgressColor = dfp.Value < 0.2 ? Color.red : Color.green;
            }
            // Draw an enemy indicator based on current player.
            if (GladiusGlobals.GameStateManager.ArenaStateCommon.TurnManager != null)
            {
                dfSprite indicator = GladiusGlobals.FindChild("Indicator", uiPanel.transform).GetComponent<dfSprite>();
                String markerName = "EnemyIndicator";
                if (GladiusGlobals.GameStateManager.ArenaStateCommon.TurnManager.CurrentActor != null)
                {
                    if (GladiusGlobals.GameStateManager.ArenaStateCommon.TurnManager.CurrentActor.TeamName == actor.TeamName)
                    {
                        markerName = "FriendIndicator";
                    }
                }
                indicator.SpriteName = markerName;
            }
        }


        void EventManager_BaseActorChanged(object sender, BaseActorChangedArgs e)
        {
            //CurrentActor = e.New;
        }


        private List<FloatingText> m_activeFloatingText = new List<FloatingText>();
        private Stack<FloatingText> m_floatingTextPool = new Stack<FloatingText>();
    }

    public class FloatingText : IComparable
    {
        private CombatEngineUI m_combatEngineUI;
        private GameObject m_labelGameObject;
        private dfRichTextLabel m_label;
        private dfFollowObject m_follow;

        private GameObject m_dummyObject = new GameObject();

        private FloatingText() { }
        public FloatingText(CombatEngineUI combatEngineUI)
        {
            m_combatEngineUI = combatEngineUI;

            GameObject playerUIRoot = GameObject.Find("PlayerUIRoot");
            GameObject prefab = (GameObject)GameObject.Find("CombatTextPrefab");
            m_labelGameObject = (GameObject)GameObject.Instantiate(prefab);
            m_labelGameObject.transform.parent = playerUIRoot.transform;
            m_label = m_labelGameObject.GetComponent<dfRichTextLabel>();
            m_follow = m_labelGameObject.GetComponent<dfFollowObject>();
            m_follow.attach = m_dummyObject;
            m_follow.enabled = false;
            m_follow.enabled = true;

        }
        

        public Vector3 CameraPosition
        {
            get;
            set;
        }

        public Color TextColor
        {
            get;set;
        }
        public float Age
        {
            get;set;
        }
        public float MaxAge
        {
            get;set;
        }

        public bool Complete
        {
            get { return (Age > MaxAge); }
        }

        public void SetActive(bool value)
        {
            m_labelGameObject.SetActive(value);
        }

        public void Initialise(Vector3 worldPosition, StringBuilder textToCopy, Color color,float maxAge)
        {
            Age = 0f;
            m_dummyObject.transform.position = worldPosition;
            TextColor = color;
            MaxAge = maxAge;
            m_label.Text = textToCopy.ToString();
            //Content.Remove(0, Content.Length);
            //Content.Append(textToCopy);
        }

        public void Initialise(Vector3 worldPosition, String textToCopy, Color color, float maxAge)
        {
            Age = 0f;
            m_dummyObject.transform.position = worldPosition;
            TextColor = color;
            MaxAge = maxAge;
            m_label.Text = textToCopy;
        }


        public void Update()
        {
            float elapsed = Time.deltaTime;
            float update = m_combatEngineUI.TextFloatSpeed * elapsed;
            Age += elapsed;

            Vector3 delta = new Vector3(0,update,0);
            m_dummyObject.transform.position += delta;
            //CameraPosition = CombatEngineUI.WorldToScreenAdjusted(WorldPosition);
            // to do? fade text as it reaches end
        }


   
#region IComparable Members

public int  CompareTo(object obj)
{
 	return (int)(this.CameraPosition.z - ((FloatingText)obj).CameraPosition.z);
}

#endregion
}

