using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using com.xexuxjy.magiccarpet.gameobjects;
using BulletXNA;
using BulletXNA.LinearMath;
using com.xexuxjy.magiccarpet.interfaces;
using com.xexuxjy.magiccarpet.util.debug;

namespace com.xexuxjy.magiccarpet.util.console
{
    // acts as a command processor?
    public class SimpleConsole : DebugWindow , IKeyboardCallback
    {
        public SimpleConsole(IDebugDraw debugDraw)
            : base("SimpleConsole",Globals.Game,debugDraw)
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
            String[] tokens = command.Split(s_commandSplitChars);
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
                            Vector3 spawnPosition = Vector3.Zero;
                            GameObjectType objectType = GetGameObjectType(args, 0);
                            Dictionary<String, String> properties = null;

                            if (args.Length > 1)
                            {
                                Vector3? position = GetVector(args[1]);
                                if (position.HasValue)
                                {
                                    spawnPosition = position.Value;
                                }
                            }

                            if (args.Length > 2)
                            {
                                properties = GetProperties(args[2]);
                            }

                            result = SpawnEntity(objectType, spawnPosition,properties);

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
                                Vector3? position = GetVector(args[1]);
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
                            Vector3? position = GetVector(args[1]);
                            if (position.HasValue)
                            {
                                float radius = float.Parse(args[2]);
                                float height = float.Parse(args[3]);
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
                            result = LoadScript(args[0]);
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
                        else if (commandDetails.Name.Equals("droprandommanaballs"))
                        {
                            int number = int.Parse(args[0]);
                            Vector3? position = GetVector(args[1]);
                            if (position.HasValue)
                            {
                                float range  = float.Parse(args[2]);
                                DropRandomManaBalls(number,position.Value,range);
                            }
                        }
                        else if (commandDetails.Name.Equals("camerafollow"))
                        {
                            if (Globals.Camera.CurrentBehavior != Dhpoware.Camera.Behavior.Follow)
                            {
                                Globals.Camera.CurrentBehavior = Dhpoware.Camera.Behavior.Follow;
                            }

                            if (args.Length > 0)
                            {
                                String followObjectId = args[0];
                                GameObject gameObject = Globals.GameObjectManager.GetObject(followObjectId);
                                if (gameObject != null)
                                {
                                    Globals.Camera.FollowTarget = gameObject;
                                }
                            }
                            else
                            {
                                Globals.Camera.FollowTarget = null;
                            }
                        }
                        else if (commandDetails.Name.Equals("cameraclip"))
                        {
                            Globals.Camera.ClipToWorld = true;
                        }
                        else if (commandDetails.Name.Equals("camerafree"))
                        {
                            Globals.Camera.ClipToWorld = false;
                        }
                        else if (commandDetails.Name.Equals("buildrandomlandscape"))
                        {
                            Globals.Terrain.BuildRandomLandscape();
                        }
                        else if(commandDetails.Name.Equals("buildfractallandscape"))
                        {
                            Globals.Terrain.BuildFractalLandscape();
                        }
                        else if (commandDetails.Name.Equals("setglobalplayer"))
                        {
                            String globalPlayerId = args[0];
                            GameObject gameObject = Globals.GameObjectManager.GetObject(globalPlayerId);
                            if (gameObject != null)
                            {
                                Globals.Player = gameObject as Magician;
                                if (Globals.MiniMap != null)
                                {
                                    Globals.MiniMap.SetTrackedObject(Globals.Player);
                                }
                            }

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

        public String LoadScript(String name)
        {
            String result = "";
            try
            {
                List<String> commands = new List<String>();
                using (StreamReader reader = new StreamReader("../../../Scripts/" + name + ".mc"))
                {
                    while (reader.EndOfStream == false)
                    {
                        String dataLine = reader.ReadLine().ToLower();
                        if (dataLine.StartsWith("#") || String.IsNullOrWhiteSpace(dataLine))
                        {
                            continue;
                        }
                        commands.Add(dataLine);
                    }
                    foreach (String commandLine in commands)
                    {
                        ProcessCommand(commandLine);
                    }
                }
            }
            catch (System.Exception ex)
            {
                result = "unable to load script.";
            }
            return result;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private string SpawnEntity(GameObjectType entityType, Vector3 position,Dictionary<String,String> properties)
        {
            GameObject GameObject = null;
            try
            {
                GameObject = Globals.GameObjectManager.CreateAndInitialiseGameObject(entityType, position,properties);
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
            m_texture = new Texture2D(Game.GraphicsDevice, m_consoleWidth, m_consoleHeight);
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


                //DebugDraw.DrawTexture(m_texture, m_screenPosition, Color.White.ToVector3());
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
            SpawnEntity(GameObjectType.manaball, position,null);
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void DropRandomManaBalls(int number,IndexedVector3 centerPosition,float range)
        {
            for (int i = 0; i < number; ++i)
            {
                IndexedVector3 ballPosition = Globals.Terrain.GetRandomWorldPositionXZWithRange(centerPosition, range);
                SpawnEntity(GameObjectType.manaball, ballPosition, null);
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public IndexedVector3? GetVector(String args)
        {
            String[] tokens = args.Split(s_subcommandSplitChars);

            IndexedVector3? result = null;
            if (tokens.Length == 3)
            {
                float x = float.Parse(tokens[0]);
                float y = float.Parse(tokens[1]);
                float z = float.Parse(tokens[2]);
                result = new IndexedVector3(x, y, z);
            }
            return result;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public Dictionary<String,String> GetProperties(String args)
        {
            String[] tokens = args.Split(s_subcommandSplitChars);
            Dictionary<String, String> result = new Dictionary<String, String>();
            for (int i = 0; i < tokens.Length; ++i)
            {
                int equalsIndex = tokens[i].IndexOf('=');
                String key = tokens[i].Substring(0,equalsIndex);
                String value = tokens[i].Substring(equalsIndex+1,tokens[i].Length - equalsIndex -1);

                result[key] = value;
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

        public virtual void KeyboardCallback(Keys key, bool released, ref KeyboardState newState, ref KeyboardState oldState)
        {
            char c = (char)key;
            // hacky
            if (key == Keys.OemComma)
            {
                c = ',';
            }
            
            
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
            m_commandDetailsMap.Add(id,new CommandDetails(id,new int[]{1,2,3}));
            id = "setcastlelevel";
            m_commandDetailsMap.Add(id,new CommandDetails(id,new int[]{2}));
            id = "getposition";
            m_commandDetailsMap.Add(id,new CommandDetails(id,new int[]{1}));
            id = "setposition";
            m_commandDetailsMap.Add(id,new CommandDetails(id,new int[]{2}));
            id = "setdebug";
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
            id = "droprandommanaballs";
            m_commandDetailsMap.Add(id, new CommandDetails(id, new int[] { 3 }));
            id = "camerafollow";
            m_commandDetailsMap.Add(id, new CommandDetails(id, new int[] { 1 }));
            id = "cameraclip";
            m_commandDetailsMap.Add(id, new CommandDetails(id, new int[] { 1 }));
            id = "camerafree";
            m_commandDetailsMap.Add(id, new CommandDetails(id, new int[] { 0 }));
            id = "buildrandomlandscape";
            m_commandDetailsMap.Add(id, new CommandDetails(id, new int[] { 0 }));
            id = "buildfractallandscape";
            m_commandDetailsMap.Add(id, new CommandDetails(id, new int[] { 0 }));
            id = "setglobalplayer";
            m_commandDetailsMap.Add(id, new CommandDetails(id, new int[] { 1 }));
            
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
        
        private static char[] s_commandSplitChars = { ' ' };
        private static char[] s_subcommandSplitChars = { ',' };

        private static String SUCCESS = "Ok";
        private static String s_commandLinePrefix = ">>";
    }
}
