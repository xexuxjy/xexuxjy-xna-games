using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StringLeakTest;
using Gladius.util;
using UnityEngine;
using Gladius.arena;
using Gladius;

    public class CombatEngineUI : MonoBehaviour
    {

        public bool DrawHealthBars = true;
        public bool DrawCombatText = true;
        public float NearBound = 0f;
        public float FarBound = 30f;
        public float Multiplier = 3;
        public float TextFloatSpeed = 3f;

        String atlasPath = "GladiusUI/Arena/ArenaUIAtlas";



        public void Start()
        {
            GladiusGlobals.CombatEngineUI = this;
            m_defaultGUIStyle = new GUIStyle();
            
            m_guiFont = Resources.Load<Font>("GladiusUI/Arena/TREBUC");
            //m_guiFont = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;

            m_defaultGUIStyle.font = m_guiFont;
            m_defaultGUIStyle.fontSize = 16;

            TPackManager.load(atlasPath);

            //m_defaultGUIStyle.fontStyle = FontStyle.Bold;
        }

        public void OnGUI()
        {
            //GUI.contentColor = Color.yellow;
            //GUI.Label(new Rect(20, 20, 100, 40), "Some Text",DefaultStyle);

            DrawElement();
        }

        public void DrawFloatingText(Vector3 initialPosition, Color textColor, StringBuilder text, float age)
        {
            if (DrawCombatText)
            {

                FloatingText ft = GetFloatingText();
                ft.Initialise(initialPosition, text, textColor, age);
                ft.LabelDims = m_defaultGUIStyle.CalcSize(ft.GUIContent);

                m_activeFloatingText.Add(ft);
            }
        }

        public void DrawFloatingText(Vector3 initialPosition, Color textColor, String text, float age)
        {
            if (DrawCombatText)
            {
                FloatingText ft = GetFloatingText();
                ft.Initialise(initialPosition, text, textColor, age);
                ft.LabelDims = m_defaultGUIStyle.CalcSize(ft.GUIContent);
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

        }

        public void DrawElement()
        {
            //Vector2 viewPortAdjust = new Vector2(graphicsDevice.Viewport.X,graphicsDevice.Viewport.Y);

            foreach (FloatingText ft in m_activeFloatingText)
            {
                Vector3 result = WorldToScreenAdjusted(ft.WorldPosition);
                // don't draw things behind?
                if (result.z < 0)
                {
                    continue;
                }
                
                Vector2 pos = new Vector2(result.x, result.y);

                pos.x -= (ft.LabelDims.x / 2f);
                int oldFontSize = DefaultStyle.fontSize;


                float t2 = 1f -  (Mathf.Clamp(result.z - NearBound,NearBound,FarBound) / FarBound - NearBound);

                float scale = t2;
                float fontScale = (((float)DefaultStyle.fontSize) * scale * Multiplier);
                DefaultStyle.fontSize = (int)fontScale;
                DefaultStyle.normal.textColor = ft.TextColor;
                
                GUI.Label(new Rect(pos.x,pos.y,0,0),ft.GUIContent,DefaultStyle);
                DefaultStyle.fontSize = oldFontSize;

            }

            if (DrawHealthBars)
            {
                if (GladiusGlobals.TurnManager != null)
                {
                    foreach (BaseActor baseActor in GladiusGlobals.TurnManager.AllActors)
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
            return m_floatingTextPool.Pop();
        }

        private void FreeFloatingText(FloatingText ft)
        {
            m_floatingTextPool.Push(ft);
        }

        public void LoadContent()
        {
            //base.LoadContent(manager, device);
            //m_spriteFont = manager.Load<SpriteFont>("UI/fonts/ShopFont");

            EventManager.BaseActorChanged += new EventManager.BaseActorSelectionChanged(EventManager_BaseActorChanged);
        }

        public static Vector3 WorldToScreenAdjusted(Vector3 world)
        {
            Vector3 result = GladiusGlobals.Camera.WorldToScreenPoint(world);
            result.y = Screen.height - result.y;
            return result;
        }

        public void DrawNameHealthIndicators(BaseActor actor)
        {
            Vector3 worldPoint = actor.Position;
            worldPoint.y += (actor.ModelHeight*1.2f);
            Vector3 result = WorldToScreenAdjusted(worldPoint);
            if (result.z >= 0)
            {
                // measure the text so it's centered - can only do at this point?
                GUIContent playerName = actor.GUIContentName;
                Vector2 textDims = m_defaultGUIStyle.CalcSize(playerName);

                textDims.x = 100;

                Vector2 pos = new Vector2(result.x, result.y);
                pos.x -= (textDims.x / 2f);

                DrawShadowedText(actor.Name, pos);

                //pos.y += textDims.y + 2;
                int barHeight = 16;
                DrawHealthBar(actor.Health, actor.MaxHealth, Color.green, Color.red, pos, (int)textDims.x, barHeight);

                // Draw an enemy indicator based on current player.
                if (GladiusGlobals.TurnManager != null)
                {
                    String markerName = "EnemyIndicator.png";
                    if (GladiusGlobals.TurnManager.CurrentActor.Team == actor.Team)
                    {
                        markerName = "FriendIndicator.png";
                    }
                    int xdims = 32;
                    Rect rect = new Rect(result.x, result.y, xdims, 32);
                    rect.x -= xdims / 2;
                    rect.y -= barHeight*2;
                    TPackManager.draw(rect, atlasPath, markerName);
        

                }

            }
        }

        private void DrawHealthBar(float value, float maxValue, Color colour1, Color colour2,Vector2 topLeft,int width,int height)
        {
            Color borderColour = Color.black;
            int inset = 2;

            Rect rect = new Rect((int)topLeft.x, (int)topLeft.y, width, height);
            Rect insetRect = GladiusGlobals.InsetRectangle(rect, inset);

            GUI.DrawTexture(rect,GladiusGlobals.ColourTextureHelper.GetColourTexture(borderColour));
            float scale = maxValue > 0 ? (value / maxValue) : 1;
            
            // draw bad health below 30%
            Color drawColour = scale > 0.3f? colour1:colour2;
            insetRect.width = (int)((float)insetRect.width*scale);
            GUI.DrawTexture(insetRect, GladiusGlobals.ColourTextureHelper.GetColourTexture(drawColour));
        }

        public void DrawShadowedText(String text, Point pos)
        {
            Vector2 vpos = new Vector2(pos.X, pos.Y);
            DrawShadowedText(text, vpos, Color.white);
        }

        public void DrawShadowedText(String text, Vector2 pos)
        {
            DrawShadowedText(text, pos, Color.white);
        }

        public void DrawShadowedText(String text, Point pos, Color textColor)
        {
            Vector2 vpos = new Vector2(pos.X, pos.Y);
            DrawShadowedText(text, vpos, textColor);
        }

        public static void DrawShadowedText(String text, Vector2 pos, Color textColor)
        {
            // Shadow text.
            //sb.DrawString(font, text, pos, Color.Black);
            //sb.DrawString(font, text, pos + new Vector2(1), textColor);
        }

        public void DrawShadowedText(GUIContent text, Vector2 pos, Color textColor)
        {
            // Shadow text.
            //sb.DrawString(font, text, pos, Color.Black);
            //sb.DrawString(font, text, pos + new Vector2(1), textColor);

            //GUI.contentColor = ft.TextColor;
            //GUI.color = ft.TextColor;
            //GUI.backgroundColor = Color.green;
            DefaultStyle.normal.textColor = textColor;
            GUI.Label(new Rect(pos.x, pos.y, 0, 0), text, DefaultStyle);

        
        }


        //public static void DrawCenteredText(String text, Vector2 centerPos, Color textColor)
        //{
        //    Vector2 textDims = font.MeasureString(text);
        //    Vector2 offsetCenter = centerPos - (textDims / 2f);
        //    sb.DrawString(font, text, offsetCenter, textColor);
        //}



        void EventManager_BaseActorChanged(object sender, BaseActorChangedArgs e)
        {
            //CurrentActor = e.New;
        }

        public GUIStyle DefaultStyle
        {
            get { return m_defaultGUIStyle; }
            set { m_defaultGUIStyle = value; }
        }


        private Font m_guiFont;
        private GUIStyle m_defaultGUIStyle;       
        private List<FloatingText> m_activeFloatingText = new List<FloatingText>();
        private Stack<FloatingText> m_floatingTextPool = new Stack<FloatingText>();
        //private SpriteFont m_spriteFont;
    }

    public class FloatingText : IComparable
    {
        private CombatEngineUI m_combatEngineUI;
        private FloatingText() { }
        public FloatingText(CombatEngineUI combatEngineUI)
        {
            m_combatEngineUI = combatEngineUI;
            GUIContent = new GUIContent();

        }
        
        public GUIContent GUIContent
        {
            get;
            set;
        }

        public Vector3 WorldPosition
        {
            get;set;
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

        public Vector2 LabelDims
        {
            get;
            set;
        }


        public void Initialise(Vector3 worldPosition, StringBuilder textToCopy, Color color,float maxAge)
        {
            Age = 0f;
            WorldPosition = worldPosition;
            TextColor = color;
            MaxAge = maxAge;
            GUIContent.text = textToCopy.ToString();
        }

        public void Initialise(Vector3 worldPosition, String textToCopy, Color color, float maxAge)
        {
            Age = 0f;
            WorldPosition = worldPosition;
            TextColor = color;
            MaxAge = maxAge;
            GUIContent.text = textToCopy.ToString();
        }


        public void Update()
        {
            float elapsed = Time.deltaTime;
            float update = m_combatEngineUI.TextFloatSpeed * elapsed;
            Age += elapsed;

            Vector3 delta = new Vector3(0,update,0);
            WorldPosition += delta;
            CameraPosition = CombatEngineUI.WorldToScreenAdjusted(WorldPosition);
            // to do? fade text as it reaches end
        }


   
#region IComparable Members

public int  CompareTo(object obj)
{
 	return (int)(this.CameraPosition.z - ((FloatingText)obj).CameraPosition.z);
}

#endregion
}

