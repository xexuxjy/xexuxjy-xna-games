using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Text;
using StringLeakTest;

namespace IlluminatiEngine.Utilities
{

    public class SimpleProfiler 
    {
        static uint whitepv = 0;
        static uint graypv = 0;
        static uint redpv = 0;
        static uint bluepv = 0;
        static uint yellowpv = 0;
        static uint greenpv = 0;


        public void Initialize(bool millisecondTimer, Game game, SpriteFont spriteFont)
        {
            m_game = game;
            m_millisecondTimer = millisecondTimer;
            m_rootNode = new ProfileInformation(m_totalID, m_millisecondTimer);
            m_currentNode = m_rootNode;

            m_texture = new Texture2D(game.GraphicsDevice, s_textureWidth, s_textureHeight);
            ScreenPosition = new Vector2(30, 30);
            m_spriteBatch = new SpriteBatch(game.GraphicsDevice);
            m_spriteFont = spriteFont;

            ProfileInformation updateNode = new ProfileInformation(m_update,m_millisecondTimer);
            updateNode.Parent = m_rootNode;
            updateNode.Depth = 1;
            m_rootNode.Children[m_update] = updateNode;

            ProfileInformation drawNode = new ProfileInformation(m_draw, m_millisecondTimer);
            drawNode.Parent = m_rootNode;
            drawNode.Depth = 1;
            m_rootNode.Children[m_draw] = drawNode;

            TotalBlocks = 3;

            SetColorPV(Color.White, ref whitepv);
            SetColorPV(Color.Gray, ref graypv);
            SetColorPV(Color.Red, ref redpv);
            SetColorPV(Color.Blue, ref bluepv);
            SetColorPV(Color.Yellow, ref yellowpv);
            SetColorPV(Color.Green, ref greenpv);
        }

        public static void SetColorPV(Color c, ref uint pv)
        {
            Color cc = c;
            cc.A = 128;
            pv = cc.PackedValue;
        }

        public int TotalBlocks
        {
            get;
            set;
        }

        private List<String> m_includeBlocks = new List<String>();
        public void SetIncludeBlocks(String[] includes)
        {
            for (int i = 0; i < includes.Length; ++i)
            {
                m_includeBlocks.Add(includes[i]);
            }
        }

        public bool ShouldInclude(String blockName)
        {
            if (blockName == m_totalID)
            {
                return true;
            }

            if (m_includeBlocks.Count == 0)
            {
                return true;
            }

            return m_includeBlocks.Contains(blockName);
        }

        public void Cleanup()
        {
            Enabled = false;
        }

        private static SimpleProfiler s_instance;
        public  static SimpleProfiler Instance
        {
            get { return s_instance; }
            set { s_instance = value; }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	


        public void Draw(GameTime gameTime)
        {
            if (Enabled)
            {
                //SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState, Matrix.Identity);
                int numEntries = TotalBlocks;
                int ystep = s_textureHeight / numEntries;
                int xstep = 50;

                int counter = 0;

                //int fontHeight = m_spriteFont.

                ProfileInformation totalPI = m_rootNode;

                //unsafe
                {
                    Array.Clear(textureData, 0, textureData.Length);

                    //m_texture.GetData<uint>(textureData);

                    //m_recCounter = 0;
                    //DrawProfileInformation(m_rootNode, textureData,null, 0, ystep, 0,false);
                    //m_texture.SetData(textureData);
                    m_recCounter = 0;
                    SpriteBatch.Begin();
                    //SpriteBatch.Draw(m_texture, ScreenPosition, Color.White);
                    DrawProfileInformation(m_rootNode, null, SpriteBatch, 0, ystep, 0,true);

                    SpriteBatch.End();
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        private int m_recCounter = 0;

        StringBuilder m_tempBuilder = new StringBuilder();


        private void DrawProfileInformation(ProfileInformation pi, uint[] textureData,SpriteBatch spriteBatch,int xstep,int ystep,int yoffset,bool setCleanup)
        {
            int xpos = (int)ScreenPosition.X;
            int ypos = (int)ScreenPosition.Y + (m_recCounter * ystep);
            int xoffset = 20 * pi.Depth;
            m_tempBuilder.Length = 0;
            pi.debugInfo(m_tempBuilder);
            Vector2 lineDims = m_spriteFont.MeasureString(m_tempBuilder);
            //float scaler = m_rootNode.LastTime != 0 ? (float)((double)pi.LastTime / (double)m_rootNode.LastTime) : 1.0f;

            //ProfileInformation parent = pi.Parent != null ? pi.Parent : m_rootNode;
            ProfileInformation parent = m_rootNode;
            float scaler = parent.LastTime != 0 ? (float)((double)pi.LastTime / (double)parent.LastTime) : 1.0f;


            // seems to have a bug where individual values exceed total so clamp.
            scaler = Math.Min(1f, scaler);

            int scaledWidth = (int)(scaler * s_textureWidth);

            uint colour = whitepv;
            Color ccolour = Color.White ;
            switch (m_recCounter % 3)
            {
                case 0:
                    colour = redpv;
                    ccolour = Color.Red;
                    break;
                case 1:
                    colour = yellowpv;
                    ccolour = Color.Yellow;
                    break;
                case 2:
                    colour = greenpv;
                    ccolour = Color.Green;
                    break;
            }

            scaledWidth += xoffset;
            scaledWidth = Math.Min(scaledWidth, s_textureWidth);

            ++m_recCounter;

            if (spriteBatch != null)
            {
                spriteBatch.Draw(GetTexture(Color.LightGray), new Rectangle(xpos, ypos, xoffset, ystep), Color.White);
                spriteBatch.Draw(GetTexture(ccolour), new Rectangle(xpos + xoffset, ypos, scaledWidth - xoffset, ystep), Color.White);
                spriteBatch.Draw(GetTexture(Color.LightGray), new Rectangle(xpos + scaledWidth, ypos, (s_textureWidth - scaledWidth), ystep), Color.White);
                spriteBatch.DrawString(m_spriteFont, m_tempBuilder, new Vector2(xpos + xoffset, ypos+(ystep/2f)), Color.Black);
            }


            foreach (ProfileInformation cpi in pi.Children.Values)
            {
                DrawProfileInformation(cpi, textureData, spriteBatch,xstep, ystep, yoffset + 1,setCleanup);
            }
            if (setCleanup)
            {
                pi.postDrawCleanup();
            }
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void Start_Profile(String id)
        {
            if (id == m_update && m_currentNode == null)
            {
                m_currentNode = m_rootNode;
            }

            if (Enabled && ShouldInclude(id) && m_currentNode != null)
            {
                ProfileInformation info;
                
                m_currentNode.Children.TryGetValue(id, out info);
                if (info == null)
                {
                    info = new ProfileInformation(id, m_millisecondTimer);
                    m_currentNode.Children[id] = info;
                    info.Parent = m_currentNode;
                    info.Depth = info.Parent.Depth + 1;
                    TotalBlocks++;
                }
                m_currentNode = info;
                info.start();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void Stop_Profile(String id)
        {
            if (Enabled && m_currentNode != null)
            {
                m_currentNode.stop();
                m_currentNode = m_currentNode.Parent;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        //private ProfileInformation FindProfileBlock(String id)
        //{
        //    ProfileInformation result = null;
        //    if (m_profileDictionary.ContainsKey(id))
        //    {
        //        result = m_profileDictionary[id];
        //    }
        //    return result;
        //}



        public Vector2 ScreenPosition
        {
            get { return m_screenPosition; }
            set { m_screenPosition = value; }
        }

        public bool Enabled
        {
            get { return m_enabled; }
            set { m_enabled = value; }
        }

        public string Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        public SpriteBatch SpriteBatch
        {
            get { return m_spriteBatch; }
        }

        private string m_id;
        private bool m_enabled;
        private Vector2 m_screenPosition;
        private  SpriteBatch m_spriteBatch;
        private  SpriteFont m_spriteFont;

        private  bool m_millisecondTimer;
        //private  Dictionary<String, ProfileInformation> m_profileDictionary;
        private  const int s_textureWidth = 600;
        private const int s_textureHeight = 600;
        uint[] textureData = new uint[s_textureWidth * s_textureHeight];
        private Texture2D m_texture;
        public static String m_totalID = "--TOTAL--";
        public static String m_update = "Update";
        public static String m_draw = "Draw";

        private ProfileInformation m_rootNode;
        private ProfileInformation m_currentNode;

        private Game m_game;

        private Texture2D GetTexture(Color color)
        {
            if (!m_colorMap.ContainsKey(color))
            {
                Texture2D newTexture = new Texture2D(m_game.GraphicsDevice, 1, 1);
                Color[] colorData = new Color[1];
                newTexture.GetData<Color>(colorData);
                colorData[0] = new Color(color.ToVector3());
                colorData[0].A = 128;
                newTexture.SetData(colorData);
                m_colorMap[color] = newTexture;
            }
            return m_colorMap[color];
        }

        private Dictionary<Color, Texture2D> m_colorMap = new Dictionary<Color, Texture2D>();


        #region IProfileManager Members


        public void CleanupMemory()
        {
        }

        public void Reset()
        {
        }

        public void Increment_Frame_Counter()
        {
        }

        public int Get_Frame_Count_Since_Reset()
        {
            return 0;
        }

        public float Get_Time_Since_Reset()
        {
            return 0.0f;
        }


        public void DumpAll()
        {
        }

        #endregion
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////	
    // Simple class to keep track of number of calls to see how certain sections are behaving.
    class ProfileInformation
    {
        public ProfileInformation(String id, bool millisecondTimer)
        {
            m_id = id;
            m_minTime = long.MaxValue;
            m_maxTime = long.MinValue;
            m_lastTime = 0;
            m_avgTime = 0;
            m_numCalls = 0;
            m_open = false;
            m_stopWatch = new Stopwatch();
            m_millisecondTimer = millisecondTimer;
        }

        public void postDrawCleanup()
        {
            m_lastTime = 0;
        }

        public void start()
        {
            System.Diagnostics.Debug.Assert(m_open == false, "Trying to re-open a currently open block");
            m_open = true;
            m_stopWatch.Start();
        }

        public void stop()
        {
            //System.Diagnostics.Debug.Assert(m_open == true, "Trying to close a currently closed block");
            m_stopWatch.Stop();
            //update(m_stopWatch.ElapsedMilliseconds);
            update(m_millisecondTimer ? m_stopWatch.ElapsedMilliseconds : m_stopWatch.ElapsedTicks);
            m_stopWatch.Reset();
            m_open = false;
        }

        private void update(long elapsed)
        {
            m_lastTime += elapsed;
            m_totalTime += elapsed;
            m_minTime = Math.Min(m_minTime, elapsed);
            m_maxTime = Math.Max(m_maxTime, elapsed);
            m_numCalls++;
            m_avgTime = m_totalTime / m_numCalls;
        }

        public void debugInfo(StringBuilder builder)
        {
            //builder.Append(m_id);
            //builder.Append(" Last[");
            //builder.Append(LastTime);
            //builder.Append("] Min[");
            //builder.Append(MinTime);
            //builder.Append("] Avg[");
            //builder.Append(AvgTime);
            //builder.Append("] Max[");
            //builder.Append(MaxTime);
            //builder.Append("] NC[");
            //builder.Append(NumCalls);
            //builder.Append("]\n");
            builder.ConcatFormat("{0} Last[{1}] Min[{2}] Avg[{3}] ", m_id, LastTime, MinTime, AvgTime);
            builder.ConcatFormat("Max [{0}] NC [{2}]\n",  MaxTime, NumCalls);
            //builder.AppendFormat("{0} Last[{1}] Min[{2}] Avg[{3}] Max [{4}] NC [{5}]\n", m_id, LastTime, MinTime, AvgTime, MaxTime, NumCalls);
        }

        public long LastTime
        {
            get 
            { 
                if(Parent == null)
                {
                    return m_children[SimpleProfiler.m_update].LastTime + m_children[SimpleProfiler.m_draw].LastTime;
                }
                return m_lastTime; 
            }
        }

        public long MinTime
        {
            get 
            {
                if (Parent == null)
                {
                    return m_children[SimpleProfiler.m_update].MinTime+ m_children[SimpleProfiler.m_draw].MinTime;
                }

                return m_minTime; 
            }
        }

        public long MaxTime
        {
            get 
            {
                if (Parent == null)
                {
                    return m_children[SimpleProfiler.m_update].MaxTime+ m_children[SimpleProfiler.m_draw].MaxTime;
                }

                return m_maxTime; 
            }
        }

        public long AvgTime
        {
            get 
            {
                if (Parent == null)
                {
                    return m_children[SimpleProfiler.m_update].AvgTime+ m_children[SimpleProfiler.m_draw].AvgTime;
                }

                return m_avgTime; 
            }
        }

        public long NumCalls
        {
            get 
            {
                if (Parent == null)
                {
                    return m_children[SimpleProfiler.m_update].NumCalls;
                }

                return m_numCalls; 
            }
        }

        public ProfileInformation Parent
        {
            get { return m_parent; }
            set { m_parent = value; }
        }

        public Dictionary<String,ProfileInformation> Children
        {
            get { return m_children; }
        }

        public int Depth
        {
            get;
            set;
        }

        String m_id;
        long m_lastTime;
        long m_minTime;
        long m_maxTime;
        long m_avgTime;
        long m_numCalls;
        long m_totalTime; // yep this could loop but don't mind too much.

        Stopwatch m_stopWatch;
        bool m_open;  // Till I get a better solution this will enforce non recursive calls.
        bool m_millisecondTimer;
        ProfileInformation m_parent;
        Dictionary<String,ProfileInformation> m_children = new Dictionary<String,ProfileInformation>();

    }

}


