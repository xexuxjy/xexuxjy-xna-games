using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using com.xexuxjy.magiccarpet.debug;
using System.IO;
using com.xexuxjy.magiccarpet.gameobjects;
using BulletXNA;
using BulletXNA.LinearMath;

namespace com.xexuxjy.utils.console
{
    // acts as a command processor?
    public class SimpleConsole : DebugWindow
    {
        public SimpleConsole(Game game,IDebugDraw debugDraw)
            : base("SimpleConsole",game,debugDraw)
        {
            m_commandBuffer = new CommandBuffer(10);
            m_commandLine = new StringBuilder();
            m_outputLine = new StringBuilder();
            m_commandQueue = new Queue<string>();
            RegisterCommands();
            ClearCommandLine();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void LoadContent()
        {
            base.LoadContent();
            BuildTexture();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void AddCommand(String command)
        {
            m_commandQueue.Enqueue(command);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ProcessCommand(String command)
        {
            // commands are space delimited.
            String[] tokens = command.Split(s_splitChars);
            m_outputLine.Clear();
            m_outputLine.Append(DoProcess(tokens));
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        // very simple for now.
        private String DoProcess(String[] tokens)
        {
            String result = SUCCESS;
            String command = tokens[0];
            String[] args = new String[0];
            if (tokens.Length > 1)
            {
                args = new String[tokens.Length - 1];
                for (int i = 0; i < args.Length; ++i)
                {
                    args[i] = tokens[i + 1];
                }
            }
            if (m_commandDetailsMap.ContainsKey(command))
            {
                CommandDetails commandDetails = m_commandDetailsMap[command];
                if (commandDetails != null)
                {
                    int[] args2 = commandDetails.NumArgs;
                    bool match = false;
                    for (int i = 0; i < args2.Length; ++i)
                    {
                        if (args2[i] == args.Length)
                        {
                            match = true;
                            break;
                        }
                    }
                    if (!match)
                    {
                        result = String.Format("No matching command/args for {0} {1}.", command, args.Length);
                    }
                    else
                    {
                        if (commandDetails.Name.Equals("spawn"))
                        {
                            if (args.Length == 1)
                            {
                                result = SpawnEntity(GetGameObjectType(args, 0));
                            }
                            if (args.Length == 4)
                            {
                                GameObjectType entityType = GetGameObjectType(args, 0);
                                Vector3? position = GetVector(args, 1);
                                if (position.HasValue)
                                {
                                    result = SpawnEntity(entityType, position.Value);
                                }
                            }
                        }
                        else if (commandDetails.Name.Equals("setcastlelevel"))
                        {
                            Castle castle = (Castle)Globals.GameObjectManager.GetObject(args[0]);
                            int level = int.Parse(args[1]);
                            if (castle != null)
                            {
                                castle.Level = level;
                                result = SUCCESS;
                            }
                            else
                            {
                                result = String.Format("Unable to find castle [{0}].", args[0]);
                            }
                        }
                        else if (commandDetails.Name.Equals("getposition"))
                        {
                            GameObject GameObject = Globals.GameObjectManager.GetObject(args[0]);
                            if (GameObject != null)
                            {
                                Vector3 position = GameObject.Position;
                                result = String.Format("Object [{0}] at [{1}][{2}][{3}].", args[0], position.X, position.Y, position.Z);
                            }
                            else
                            {
                                result = String.Format("Unable to find entity [{0}].", args[0]);
                            }
                        }
                        else if (commandDetails.Name.Equals("setposition"))
                        {
                            GameObject GameObject = Globals.GameObjectManager.GetObject(args[0]);
                            if (GameObject != null)
                            {
                                Vector3? position = GetVector(args, 1);
                                if (position.HasValue)
                                {
                                    GameObject.Position = position.Value;
                                }
                                else
                                {
                                    result = "Unable to parse vector positions";
                                }
                            }
                            else
                            {
                                result = String.Format("Unable to find entity [{0}].", args[0]);
                            }
                        }
                        else if (commandDetails.Name.Equals("setdebug"))
                        {
                            GameObject gameObject = Globals.GameObjectManager.GetObject(args[0]);
                            if (gameObject != null)
                            {
                                Globals.DebugObjectManager.DebugObject = gameObject;
                            }
                            else
                            {
                                result = String.Format("Unable to find entity [{0}].", args[0]);
                            }

                        }
                        else if (commandDetails.Name.Equals("setcamerafollow"))
                        {
                            //GameObject GameObject = Globals.objectSpatialManager.GetObject(args[0]);
                            //if (GameObject != null)
                            //{
                            //    Globals.debugText.
                        }
                        else if (commandDetails.Name.Equals("viewprofiler"))
                        {
                            Globals.SimpleProfiler.Enabled = !Globals.SimpleProfiler.Enabled;
                        }
                        else if (commandDetails.Name.Equals("viewdebug"))
                        {
                            Globals.DebugObjectManager.Enabled = !Globals.DebugObjectManager.Enabled;
                        }
                        else if (commandDetails.Name.Equals("alterterrain"))
                        {
                            Vector3? position = GetVector(args, 1);
                            if (position.HasValue)
                            {
                                float radius = float.Parse(args[4]);
                                float height = float.Parse(args[5]);
                                //Globals.simpleTerrain.alterTerrain(position.Value, radius, height);
                            }
                        }
                        else if (commandDetails.Name.Equals("kill"))
                        {
                            GameObject GameObject = Globals.GameObjectManager.GetObject(args[0]);
                            if (GameObject != null)
                            {
                                GameObject.Die();
                            }
                            else
                            {
                                result = String.Format("Unable to kill entity [{0}].", args[0]);
                            }
                        }
                        else if (commandDetails.Name.Equals("killall"))
                        {
                            List<GameObject> results = new List<GameObject>();
                            Globals.GameObjectManager.FindObjects(GameObjectType.none, results);
                            foreach (GameObject entity in results)
                            {
                                //if (entity.KeyComponent == false)
                                {
                                    entity.Die();
                                }
                            }
                        }
                        else if (commandDetails.Name.Equals("list"))
                        {
                            GameObjectType entityType = GameObjectType.none;
                            if (args.Length == 1)
                            {
                                entityType = GetGameObjectType(args, 0);
                            }

                            List<GameObject> results = new List<GameObject>();
                            Globals.GameObjectManager.FindObjects(entityType, results);
                            foreach (GameObject GameObject in results)
                            {
                                result += GameObject.Id;
                                result += " ";
                            }
                        }
                        else if (commandDetails.Name.Equals("load"))
                        {
                            List<String> commands = new List<String>();
                            StreamReader reader = new StreamReader("../../../Scripts/" + args[0] + ".mc");
                            while (reader.EndOfStream == false)
                            {
                                commands.Add(reader.ReadLine().ToLower());
                            }
                            foreach (String commandLine in commands)
                            {
                                ProcessCommand(commandLine);
                            }
                        }
                        else if (commandDetails.Name.Equals("help"))
                        {
                            result = "Commands : ";
                            foreach (String key in m_commandDetailsMap.Keys)
                            {
                                result += key;
                                result += ",";
                            }
                        }
                        else if (commandDetails.Name.Equals("droprandommanaball"))
                        {
                            DropRandomManaBall();
                        }

                    }
                }
            }
            else
            {
                result = String.Format("Unable to find command {0}.", command);
            }
            return result;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private string SpawnEntity(GameObjectType entityType)
        {
            return SpawnEntity(entityType, new Vector3(0, 0, 0));
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private string SpawnEntity(GameObjectType entityType, Vector3 position)
        {
            GameObject GameObject = null;
            try
            {
                GameObject = Globals.GameObjectManager.CreateAndInitialiseGameObject(entityType, position);
            }
            catch (System.ArgumentException ae)
            {
                // unknown type
            }

            string result = "failed to spawn";
            if (GameObject != null)
            {
                result = String.Format("Spawned {0} at [{1},{2},{3}]", GameObject.Id, GameObject.Position.X, GameObject.Position.Y, GameObject.Position.Z);
            }
            else
            {
                int ibreak = 0;
            }
            return result;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////


        private void BuildTexture()
        {
            m_consoleHeight = (Globals.MCContentManager.DebugFont.LineSpacing * 2) + 3;
            m_texture = new Texture2D(GraphicsDevice, m_consoleWidth, m_consoleHeight);
            uint[] textureData = new uint[m_consoleWidth * m_consoleHeight];
            Array.Clear(textureData, 0, textureData.Length);
            m_texture.GetData<uint>(textureData);
            uint unpackedColour = Color.DarkGray.PackedValue;
            for (int i = 0; i < textureData.Length; i++)
            {
                textureData[i] = unpackedColour;
            }
            m_texture.SetData<uint>(textureData);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Draw(GameTime gameTime)
        {
            // we'll process a new command each draw (update?) call
            if (m_commandQueue.Count > 0)
            {
                ProcessCommand(m_commandQueue.Dequeue());
            }

            if (Enabled)
            {

                Rectangle bounds = Game.Window.ClientBounds;

                // have it at the bottom of the screen
                m_screenPosition.Y = bounds.Height - m_consoleHeight;
                m_screenPosition.X = 0;

                Vector3 commandLinePosition = new Vector3(0,bounds.Height - m_consoleHeight,0);
                Vector3 outputLinePosition = commandLinePosition;
                outputLinePosition.Y += (Globals.MCContentManager.DebugFont.LineSpacing) + 2;


                DebugDraw.DrawTexture(m_texture, m_screenPosition, Color.White.ToVector3());
                if (m_commandLine.Length > 0)
                {
                    DebugDraw.DrawText(m_commandLine.ToString(), commandLinePosition, Color.White.ToVector3());
                }
                if(m_outputLine.Length > 0)
                {
                    DebugDraw.DrawText(m_outputLine.ToString(), outputLinePosition, Color.White.ToVector3());
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        Boolean IsValidChar(char c)
        {
            return ((c >= '0' && c <= 'z') || c == ' ' || c == '.' || c == (char)Keys.Enter || c==(char)Keys.Back 
                ||c == (char)Keys.Up || c == (char)Keys.Down || c == '-' || c == ',');
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObjectType GetGameObjectType(String[] args, int startIndex)
        {
            GameObjectType returnValue = GameObjectType.none;
            try
            {
                returnValue = (GameObjectType)Enum.Parse(typeof(GameObjectType), args[startIndex]);
            }
            catch (Exception e)
            {
                returnValue = GameObjectType.none;
            }
            return returnValue;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void DropRandomManaBall()
        {
            Vector3 position = Globals.Terrain.GetRandomWorldPositionXZ();
            SpawnEntity(GameObjectType.manaball, position);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public Vector3? GetVector(String[] args, int startIndex)
        {
            Vector3? result = null;
            if (args.Length >= startIndex + 2)
            {
                float x = float.Parse(args[startIndex]);
                float y = float.Parse(args[startIndex + 1]);
                float z = float.Parse(args[startIndex + 2]);
                result = new Vector3(x, y, z);
            }
            return result;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ClearCommandLine()
        {
            m_commandLine.Clear();
            m_commandLine.Append(s_commandLinePrefix);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void KeyEvent(Keys key)
        {
            char c = (char)key;
            if (IsValidChar(c))
            {
                if (key == Keys.Up || key == Keys.Down)
                {
                    if (key == Keys.Up)
                    {
                        m_commandIndex = (m_commandIndex + 1) % m_commandBuffer.Size;
                    }
                    if (key == Keys.Down)
                    {
                        --m_commandIndex;
                        if (m_commandIndex < 0)
                        {
                            m_commandIndex += m_commandBuffer.Size;
                        }
                    }
                    ClearCommandLine();
                    m_commandLine.Append(m_commandBuffer.GetCommand(m_commandIndex));
                }
                else 
                {
                    // not a cursor key so reset command index
                    m_commandIndex = 0;
                    if (key == Keys.Enter)
                    {
                        String lowerString = m_commandLine.ToString(s_commandLinePrefix.Length, m_commandLine.Length - s_commandLinePrefix.Length).ToLower();
                        ProcessCommand(lowerString);
                        m_commandBuffer.AddCommand(lowerString);
                        ClearCommandLine();
                    }
                    else if (key == Keys.Back)
                    {
                        if (m_commandLine.Length > s_commandLinePrefix.Length)
                        {
                            m_commandLine.Length -=1;
                        }
                    }
                    else
                    {
                        m_commandLine.Append(c);
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RegisterCommands()
        {
            String id = "spawn";
            m_commandDetailsMap.Add(id,new CommandDetails(id,new int[]{1,4}));
            id = "setcastlelevel";
            m_commandDetailsMap.Add(id,new CommandDetails(id,new int[]{2}));
            id = "getposition";
            m_commandDetailsMap.Add(id,new CommandDetails(id,new int[]{1}));
            id = "setposition";
            m_commandDetailsMap.Add(id,new CommandDetails(id,new int[]{4}));
            id = "setdebug";
            m_commandDetailsMap.Add(id,new CommandDetails(id,new int[]{1}));
            id = "setcamerafollow";
            m_commandDetailsMap.Add(id,new CommandDetails(id,new int[]{1}));
            id = "viewprofiler";
            m_commandDetailsMap.Add(id,new CommandDetails(id,new int[]{0}));
            id = "viewdebug";
            m_commandDetailsMap.Add(id, new CommandDetails(id, new int[] { 0 }));
            id = "alterterrain";
            m_commandDetailsMap.Add(id,new CommandDetails(id,new int[]{5}));
            id = "pause";
            m_commandDetailsMap.Add(id, new CommandDetails(id, new int[] { 1 }));
            id = "kill";
            m_commandDetailsMap.Add(id, new CommandDetails(id, new int[] { 1 }));
            id = "killall";
            m_commandDetailsMap.Add(id, new CommandDetails(id, new int[] { 0 }));
            id = "list";
            m_commandDetailsMap.Add(id, new CommandDetails(id, new int[] { 0,1 }));
            id = "load";
            m_commandDetailsMap.Add(id, new CommandDetails(id, new int[] { 1 }));
            id = "help";
            m_commandDetailsMap.Add(id, new CommandDetails(id, new int[] { 0 }));
            id = "droprandommanaball";
            m_commandDetailsMap.Add(id, new CommandDetails(id, new int[] { 0 }));
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private class CommandBuffer
        {
            public CommandBuffer(int size)
            {
                m_commandHistory = new String[size];
                for (int i = 0; i < m_commandHistory.Length; ++i)
                {
                    AddCommand("Command " + i);
                }
            }

            public int Size
            {
                get { return m_commandHistory.Length; }
            }

            public void AddCommand(String command)
            {
                m_commandHistory[m_insertIndex] = command;
                m_insertIndex = (m_insertIndex+1) % m_commandHistory.Length;
            }

            public String GetCommand(int index)
            {
               
                int i = m_insertIndex - index;
                if(i<0)
                {
                    i += m_commandHistory.Length;
                }
                return m_commandHistory[i];
            }

            private int m_insertIndex;
            private String[] m_commandHistory;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private class CommandDetails
        {
            public CommandDetails(String name, int[] numArgs)
            {
                m_name = name;
                m_numArgs = numArgs;
            }

            public String Name
            {
                get { return m_name; }
            }
            public int[] NumArgs
            {
                get { return m_numArgs; }
            }

            private String m_name;
            private int[] m_numArgs;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private Dictionary<String, CommandDetails> m_commandDetailsMap = new Dictionary<string, CommandDetails>();
        private Texture2D m_texture;
        public Vector3 m_screenPosition;

        private CommandBuffer m_commandBuffer;
        private int m_commandIndex;
        private StringBuilder m_commandLine;
        private StringBuilder m_outputLine;
        private int m_consoleHeight = 40;
        private int m_consoleWidth = 800;
        private Queue<String> m_commandQueue;
        
        private static char[] s_splitChars = { ' ' };
        private static String SUCCESS = "Ok";
        private static String s_commandLinePrefix = ">>";
    }
}
