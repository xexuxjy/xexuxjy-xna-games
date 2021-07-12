using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace AntlrParser
//{

    public interface Type
    {
        string Name { get; }
    }

    public interface Scope
    {
        string GetScopeName();
        Scope GetEnclosingScope();
        Scope GetParentScope();
        void Define(Symbol symbol);
        Symbol Resolve(string name);
    }


public abstract class BaseScope : Scope
{
    Scope enclosingScope; // null if global (outermost) scope
    Dictionary<String, Symbol> symbols = new Dictionary<string, Symbol>();

    public BaseScope(Scope parent) { this.enclosingScope = parent; }

    public Symbol Resolve(String name)
    {
        Symbol s = null;
        symbols.TryGetValue(name, out s);
        if (s != null) return s;
        // if not here, check any enclosing scope
        if (GetParentScope() != null)
            return GetParentScope().Resolve(name);
        return null; // not found
    }

    public void Define(Symbol sym)
    {
        symbols[sym.Name] = sym;
        sym.Scope = this; // track the scope in each symbol
    }

    public Scope GetParentScope() { return GetEnclosingScope(); }
    public Scope GetEnclosingScope() { return enclosingScope; }

    public String toString()
    {
        String result = "{\n";
        foreach (Symbol s in symbols.Values)
        {
            result += s.ToString();
            result += "\n";
        }
        result += "}\n";
        return result;
    }//return symbols.keySet().toString(); }

    public abstract string GetScopeName();

    }

    public class GlobalScope : ScopedSymbol
    {
        public GlobalScope() : base("Global",null)
        {
            
        }

        public override Dictionary<string, Symbol> GetMembers()
        {
            return members;
        }

        public override string ToString()
        {

            String result = "Global : \n{\n";
            foreach (Symbol s in GetMembers().Values)
            {
                result += s.ToString();
                result += "\n";
            }
            result += "\n}\n";
            return result;
        }



        public Dictionary<String, Symbol> members = new Dictionary<String, Symbol>();

    }



    public class VariableSymbol : Symbol
    {
        public VariableSymbol(string name, Type type) : base(name, type)
        {
            
        }


        public bool IsGlobal { get; set; }
        public bool IsShared { get; set; }
    }

    public class FunctionSymbol : ScopedSymbol, Scope
    {

        public FunctionSymbol(String name, Type retType, Scope parent) : base(name, retType, parent)
        {
        }

        public override Dictionary<String, Symbol> GetMembers()
        {
            if (IsInit)
            {
                ScriptScope ss = GetParentScope() as ScriptScope;
                return ss.GetMembers();
            }
            return m_orderedArgs;
        }

        public override Symbol Resolve(string name)
        {
            VariableSymbol vs = m_arguments.Find(x => x.Name == name);
            if (vs != null)
            {
                return vs;
            }
            return base.Resolve(name);
        }


    public override string ToString()
        {

            String result = "Function : " + Type + " : " + Name;
            result+= "(";

            foreach (VariableSymbol vs in m_arguments)
            {
                result += vs.Type;
                result += " ";
                result += vs.Name;
                result += ",";
            }

            result+=")\n{\n";
            if (!IsInit)
            {
                foreach (Symbol s in GetMembers().Values)
                {
                    result += s.ToString();
                    result += "\n";
                }
            }
            result += "\n}\n";
            return result;
        }

        public void AddArgument(VariableSymbol vs)
        {
        // go through and make sure we don't have arg of same name?
            if (!m_arguments.Exists(x => x.Name == vs.Name))
            {
                m_arguments.Add(vs);
            }
            else
        {
            int ibreak = 0;
        }
        }

        public VariableSymbol GetVariableAtIndex(int index)
    {
        if (index < m_arguments.Count)
        {
            return m_arguments[index];
        }
        return null;
    }

        public bool IsInit = false;
        public List<VariableSymbol> m_arguments = new List<VariableSymbol>();
        Dictionary<String, Symbol> m_orderedArgs = new Dictionary<string, Symbol>();

    }

    public class BuiltInSymbol : Symbol, Type
    {
        public BuiltInSymbol(string name) : base(name)
        {

        }

        public BuiltInSymbol(string name, VariableType vtype) : base(name)
        {
            VariableType = vtype;
        }
        public VariableType VariableType;
    }


    public class SymbolTable
    {
        public GlobalScope globals = new GlobalScope();
        ScriptScope objectRoot;
        Dictionary<String, Symbol> m_symbols = new Dictionary<string, Symbol>();
        public SymbolTable()
        {
            InitTypeSystem();
        }

        public void Define(Symbol symbol)
        {
            m_symbols[symbol.Name] = symbol;
        }

        public Symbol Resolve(string name)
        {
            Symbol result = null;
            m_symbols.TryGetValue(name, out result);
            return result;
        }

        public FunctionSymbol AddFunction(string name,Type retType, ScriptScope owner)
        {
            FunctionSymbol symbol = new FunctionSymbol(name, retType, owner);
            owner.Define(symbol);
            return symbol;
        }

        public void InitTypeSystem()
        {
            ScriptScope globalScript = new ScriptScope("InterfaceGlobals", _GLOBALS, globals);
            globals.Define(globalScript);
            AddFunction("GetSchool", _SCHOOL, globalScript);

            {
                FunctionSymbol createTalkBox = AddFunction("CreateTalkBox", _TALKBOX, globalScript);
                createTalkBox.AddArgument(new VariableSymbol("col", _INTEGER));
                createTalkBox.AddArgument(new VariableSymbol("row", _INTEGER));
                createTalkBox.AddArgument(new VariableSymbol("val", _STRING));
                createTalkBox.AddArgument(new VariableSymbol("image", _STRING));
            }


            {
                FunctionSymbol createSubTitleTalkBox = AddFunction("CreateSubTitleTalkBox", _TALKBOX, globalScript);
                createSubTitleTalkBox.AddArgument(new VariableSymbol("col", _INTEGER));
                createSubTitleTalkBox.AddArgument(new VariableSymbol("row", _INTEGER));
                createSubTitleTalkBox.AddArgument(new VariableSymbol("val", _STRING));
                createSubTitleTalkBox.AddArgument(new VariableSymbol("image", _STRING));
            }



            {
                FunctionSymbol getHero = AddFunction("GetHero", _STRING, globalScript);
            }

            FunctionSymbol loadTalkboxSoundTable = AddFunction("LoadTalkboxSoundTable", _VOID, globalScript);
            FunctionSymbol loadSecondaryTalkboxSoundTable = AddFunction("LoadSecondaryTalkboxSoundTable", _VOID, globalScript);
            loadSecondaryTalkboxSoundTable.AddArgument(new VariableSymbol("name", _STRING));

            FunctionSymbol unloadTalkboxSoundTable = AddFunction("UnloadTalkboxSoundTable", _VOID, globalScript);
            FunctionSymbol unloadSecondaryTalkboxSoundTable = AddFunction("UnloadSecondaryTalkboxSoundTable", _VOID, globalScript);
            FunctionSymbol getGameDay = AddFunction("GetGameDay", _INTEGER, globalScript);
            FunctionSymbol triggerNewState = AddFunction("TriggerNewState", _VOID, globalScript);

            {
                FunctionSymbol setCurTown = AddFunction("SetCurTown", _VOID, globalScript);
                setCurTown.AddArgument(new VariableSymbol("name", _STRING));
            }

            {
                FunctionSymbol setCurLeague = AddFunction("SetCurLeague", _VOID, globalScript);
                setCurLeague.AddArgument(new VariableSymbol("name", _STRING));
            }

            {
                FunctionSymbol schoolAddCharacter = AddFunction("SchoolAddCharacter", _VOID, globalScript);
                schoolAddCharacter.AddArgument(new VariableSymbol("name", _STRING));
                schoolAddCharacter.AddArgument(new VariableSymbol("level", _INTEGER));
            }

            {
                FunctionSymbol schoolRemoveCharacter = AddFunction("SchoolRemoveCharacter", _VOID, globalScript);
                schoolRemoveCharacter.AddArgument(new VariableSymbol("name", _STRING));
            }


            FunctionSymbol schoolCanRecruit = AddFunction("SchoolCanRecruit", _BOOL, globalScript);

            {
                FunctionSymbol setCurrentRegion = AddFunction("SetCurrentRegion", _VOID, globalScript);
                setCurrentRegion.AddArgument(new VariableSymbol("name", _STRING));
            }
            {
                FunctionSymbol schoolLoad = AddFunction("SchoolLoad", _VOID, globalScript);
                schoolLoad.AddArgument(new VariableSymbol("name", _STRING));
            }

            {
                FunctionSymbol registerEventHandler = AddFunction("RegisterEventHandler", _VOID, globalScript);
                registerEventHandler.AddArgument(new VariableSymbol("name", _STRING));
                registerEventHandler.AddArgument(new VariableSymbol("actor", _STRING));
                registerEventHandler.AddArgument(new VariableSymbol("event", _STRING));
            }
            {
                FunctionSymbol unregisterEventHandler = AddFunction("UnregisterEventHandler", _VOID, globalScript);
                unregisterEventHandler.AddArgument(new VariableSymbol("name", _STRING));
                unregisterEventHandler.AddArgument(new VariableSymbol("actor", _STRING));
                unregisterEventHandler.AddArgument(new VariableSymbol("event", _STRING));
            }

            {
                FunctionSymbol testAndTriggerStoryEvent = AddFunction("TestAndTriggerStoryEvent", _VOID, globalScript);
                testAndTriggerStoryEvent.AddArgument(new VariableSymbol("name", _STRING));
            }
            {
                FunctionSymbol triggerStoryEvent = AddFunction("TriggerStoryEvent", _VOID, globalScript);
                triggerStoryEvent.AddArgument(new VariableSymbol("name", _STRING));
            }
            {
                FunctionSymbol setIntVar = AddFunction("SetIntVar", _VOID, globalScript);
                setIntVar.AddArgument(new VariableSymbol("name", _STRING));
                setIntVar.AddArgument(new VariableSymbol("val", _INTEGER));
            }

            {
                FunctionSymbol getIntVar = AddFunction("GetIntVar", _INTEGER, globalScript);
                getIntVar.AddArgument(new VariableSymbol("name", _STRING));
            }


            FunctionSymbol getCurArenaRefName = AddFunction("GetCurArenaRefName", _STRING, globalScript);

            FunctionSymbol getCurTownRefName = AddFunction("GetCurTownRefName", _STRING, globalScript);


            {
            FunctionSymbol setPreBattleCutScene = AddFunction("SetPreBattleCutScene", _VOID, globalScript);
                setPreBattleCutScene.AddArgument(new VariableSymbol("name", _STRING));
                setPreBattleCutScene.AddArgument(new VariableSymbol("val", _INTEGER));
            }
            {
                FunctionSymbol setPostBattleCutScene = AddFunction("SetPostBattleCutScene", _VOID, globalScript);
                setPostBattleCutScene.AddArgument(new VariableSymbol("name", _STRING));
                setPostBattleCutScene.AddArgument(new VariableSymbol("val", _INTEGER));
            }

            FunctionSymbol currentGameState = AddFunction("CurrentGameState", _INTEGER, globalScript);
            {
                FunctionSymbol gameStateName = new FunctionSymbol("GameStateName", _STRING, globalScript);
                gameStateName.AddArgument(new VariableSymbol("name", _STRING));
            }

            {
                FunctionSymbol requestFullScreenPic = AddFunction("RequestFullScreenPic", _VOID, globalScript);
                requestFullScreenPic.AddArgument(new VariableSymbol("name", _STRING));
            }
            {
                FunctionSymbol requestMovie = AddFunction("RequestMovie", _VOID, globalScript);
                requestMovie.AddArgument(new VariableSymbol("name", _STRING));
            }
            {
                FunctionSymbol requestState = AddFunction("RequestState", _VOID, globalScript);
                requestState.AddArgument(new VariableSymbol("name", _STRING));
            }

            FunctionSymbol requestSaveGame = AddFunction("RequestSaveGame", _VOID, globalScript);

            FunctionSymbol getChapter = AddFunction("GetChapter", _INTEGER, globalScript);
            {
                FunctionSymbol setChapter = AddFunction("SetChapter", _VOID, globalScript);
                setChapter.AddArgument(new VariableSymbol("val", _INTEGER));
            }
            {
                FunctionSymbol getUnitByName = AddFunction("GetUnitByName", _ACTOR, globalScript);
                getUnitByName.AddArgument(new VariableSymbol("name", _STRING));
            }
            {
                FunctionSymbol consoleExec = AddFunction("ConsoleExec", _VOID, globalScript);
                consoleExec.AddArgument(new VariableSymbol("name", _STRING));
            }
            FunctionSymbol exitShop = AddFunction("ExitShop", _VOID, globalScript);


            FunctionSymbol requestSplash = AddFunction("RequestSplash", _VOID, globalScript);
            FunctionSymbol resetTowns = AddFunction("ResetTowns", _VOID, globalScript);

            {
                FunctionSymbol requestLeague = AddFunction("RequestLeague", _VOID, globalScript);
                requestLeague.AddArgument(new VariableSymbol("name", _STRING));
            }

            {
                FunctionSymbol requestTownHold = AddFunction("RequestTownHold", _VOID, globalScript);
                requestTownHold.AddArgument(new VariableSymbol("name", _STRING));
                requestTownHold.AddArgument(new VariableSymbol("script", _STRING));
            }

            {
                FunctionSymbol requestLeagueWithScript = AddFunction("RequestLeagueWithScript", _VOID, globalScript);
                requestLeagueWithScript.AddArgument(new VariableSymbol("name", _STRING));
                requestLeagueWithScript.AddArgument(new VariableSymbol("script", _STRING));
            }

            {
                FunctionSymbol requestSceneAnimation = AddFunction("RequestSceneAnimation", _VOID, globalScript);
                requestSceneAnimation.AddArgument(new VariableSymbol("name", _STRING));
            }

            {
                FunctionSymbol isEncounterComplete = AddFunction("IsEncounterComplete", _BOOL, globalScript);
                isEncounterComplete.AddArgument(new VariableSymbol("name", _STRING));
            }

            {
                FunctionSymbol isLeagueComplete = AddFunction("IsLeagueComplete", _BOOL, globalScript);
                isLeagueComplete.AddArgument(new VariableSymbol("name", _STRING));
            }

            {
                FunctionSymbol addJournalItem = AddFunction("AddJournalItem", _VOID, globalScript);
                addJournalItem.AddArgument(new VariableSymbol("name", _STRING));
            }

            {
                FunctionSymbol worldMapScriptDisable = AddFunction("WorldMapScriptDisable", _VOID, globalScript);
                worldMapScriptDisable.AddArgument(new VariableSymbol("val", _INTEGER));
            }

            {
                FunctionSymbol getUnitList = AddFunction("GetUnitList", _UNITLIST, globalScript);
            }

            FunctionSymbol isMorning = AddFunction("IsMorning", _BOOL, globalScript);
            FunctionSymbol isNight = AddFunction("IsNight", _BOOL, globalScript);

            {
                FunctionSymbol setHourOfDay = AddFunction("SetHourOfDay", _VOID, globalScript);
                setHourOfDay.AddArgument(new VariableSymbol("hour", _INTEGER));
            }

                FunctionSymbol getRandomEncounter = AddFunction("GetRandomEncounter", _STRING, globalScript);
            FunctionSymbol loadLeagueOfficeData = AddFunction("LoadLeagueOfficeData", _VOID, globalScript);

            {
                FunctionSymbol requestEncounter = AddFunction("RequestEncounter", _STRING, globalScript);
                requestEncounter.AddArgument(new VariableSymbol("name", _STRING));
                requestEncounter.AddArgument(new VariableSymbol("val", _INTEGER));

            }


            {
                FunctionSymbol requestEncounterRetreat = AddFunction("RequestEncounterRetreat", _STRING, globalScript);
                requestEncounterRetreat.AddArgument(new VariableSymbol("name", _STRING));
                requestEncounterRetreat.AddArgument(new VariableSymbol("val", _INTEGER));
                requestEncounterRetreat.AddArgument(new VariableSymbol("val2", _INTEGER));
            }

            FunctionSymbol requestSaveGameOverlay = AddFunction("RequestSaveGameOverlay", _VOID, globalScript);

            {
                FunctionSymbol requestScenimation = AddFunction("RequestScenimation", _STRING, globalScript);
                requestScenimation.AddArgument(new VariableSymbol("name", _STRING));
            }

            {
                FunctionSymbol requestStoryEvent = AddFunction("RequestStoryEvent", _STRING, globalScript);
                requestStoryEvent.AddArgument(new VariableSymbol("name", _STRING));
            }

            FunctionSymbol requestTown = AddFunction("RequestRown", _VOID, globalScript);

            {
                FunctionSymbol requestWorldMap = AddFunction("RequestWorldMap", _STRING, globalScript);
                requestWorldMap.AddArgument(new VariableSymbol("name", _STRING));
            }

            {
                FunctionSymbol sendInput = AddFunction("SendInput", _STRING, globalScript);
                sendInput.AddArgument(new VariableSymbol("id", _INTEGER));
            }

            FunctionSymbol unloadLeagueOfficeDatasendInput = AddFunction("UnloadLeagueOfficeData", _VOID, globalScript);
            

            ScriptScope inputScript = new ScriptScope("InterfaceInput", _INPUT,globals);
            globals.Define(inputScript);

            ScriptScope mathScript = new ScriptScope("InterfaceMath", _MATH,globals);
            globals.Define(mathScript);
            {
                FunctionSymbol randomInRange = AddFunction("RandomInRange", _INTEGER, mathScript);
                randomInRange.AddArgument(new VariableSymbol("low", _INTEGER));
                randomInRange.AddArgument(new VariableSymbol("high", _INTEGER));
            }

            {
                FunctionSymbol floatToInt = AddFunction("floatToInt", _INTEGER, mathScript);
                floatToInt.AddArgument(new VariableSymbol("val", _INTEGER));
                
            }

            {
                FunctionSymbol randomNumber = AddFunction("RandomNumber", _INTEGER, mathScript);
                randomNumber.AddArgument(new VariableSymbol("val", _INTEGER));
            }

            {
                FunctionSymbol getVectorLength = AddFunction("GetVectorLength", _INTEGER, mathScript);
                getVectorLength.AddArgument(new VariableSymbol("vec3", _VEC3));
            }




            ScriptScope worldScript = new ScriptScope("InterfaceWorldMap", _WORLD, globals);
            globals.Define(worldScript);

            {
                FunctionSymbol getWorldProp = AddFunction("GetWorldProp", _VOID, worldScript);
                getWorldProp.AddArgument(new VariableSymbol("name", _STRING));
            }

            {
                FunctionSymbol getDistanceToHero= AddFunction("GetDistanceToHero", _INTEGER, worldScript);
                getDistanceToHero.AddArgument(new VariableSymbol("pos", _VEC3));
            }

            {
                FunctionSymbol summonCharacterEntity = AddFunction("SummonCharacterEntity", _ACTOR, worldScript);
                summonCharacterEntity.AddArgument(new VariableSymbol("name", _STRING));
                summonCharacterEntity.AddArgument(new VariableSymbol("level", _INTEGER));
            }

            {
                FunctionSymbol summonEntity = AddFunction("SummonEntity", _ACTOR, worldScript);
                summonEntity.AddArgument(new VariableSymbol("name", _STRING));
            }

            {
                FunctionSymbol summonSchoolEntity = AddFunction("SummonSchoolEntity", _ACTOR, worldScript);
                summonSchoolEntity.AddArgument(new VariableSymbol("name", _STRING));
            }

            {
                FunctionSymbol getHero = AddFunction("GetHero", _ACTOR, worldScript);
            }

            {
                FunctionSymbol checkScriptDisable = AddFunction("CheckScriptDisable", _BOOL, worldScript);
            }


            {
                FunctionSymbol isCivilized = AddFunction("IsCivilized", _BOOL, worldScript);
            }

            {
                FunctionSymbol destroyEntity = AddFunction("DestroyEntity", _VOID, worldScript);
                destroyEntity.AddArgument(new VariableSymbol("name", _STRING));
            }

            FunctionSymbol doRandomEncounterAudioAndVideo = AddFunction("DoRandomEncounterAudioAndVideo", _VOID, worldScript);

            {
                FunctionSymbol destroyEntity = AddFunction("EnableCloseCamera", _VOID, worldScript);
                destroyEntity.AddArgument(new VariableSymbol("val", _INTEGER));

            }

            {
                FunctionSymbol lookAtHero = AddFunction("LookAtHero", _VOID, worldScript);
                lookAtHero.AddArgument(new VariableSymbol("val", _INTEGER));
            }

            {
                FunctionSymbol requestRegionChange = AddFunction("RequestRegionChange", _VOID, worldScript);
                requestRegionChange.AddArgument(new VariableSymbol("region", _STRING));
                requestRegionChange.AddArgument(new VariableSymbol("props", _STRING));
                requestRegionChange.AddArgument(new VariableSymbol("zone", _STRING));
                requestRegionChange.AddArgument(new VariableSymbol("image", _STRING));
            }

            {
                FunctionSymbol requestShop = AddFunction("RequestShop", _VOID, worldScript);
                requestShop.AddArgument(new VariableSymbol("name", _STRING));
            }


            {
                FunctionSymbol getHeroPosition = AddFunction("GetHeroPosition", _VOID, worldScript);
                getHeroPosition.AddArgument(new VariableSymbol("pos", _VEC3));
                
            }


            globals.Define(new Symbol("NewVec3", _VEC3));


        ScriptScope mapCursorScript = new ScriptScope("InterfaceMapCursor", _MAPCURSOR, globals);
        globals.Define(mapCursorScript);
        {
            FunctionSymbol setLocation = AddFunction("SetLocation", _VOID, mapCursorScript);
            setLocation.AddArgument(new VariableSymbol("xpos", _INTEGER));
            setLocation.AddArgument(new VariableSymbol("ypos", _INTEGER));
            setLocation.AddArgument(new VariableSymbol("yesno", _BOOL));
        }


        ScriptScope battleScript = new ScriptScope("InterfaceBattle", _BATTLE, globals);
        globals.Define(battleScript);
        {
            FunctionSymbol pauseCountdownTimer = AddFunction("PauseCountdownTimer", _VOID, battleScript);
            FunctionSymbol unpauseCountdownTimer = AddFunction("UnpauseCountdownTimer", _VOID, battleScript);
            FunctionSymbol isCameraSettled = AddFunction("IsCameraSettled", _BOOL, battleScript);
            
        }


        ScriptScope battleTacticsScript = new ScriptScope("InterfaceBattleTactics", _BATTLETACTICS, globals);
        globals.Define(battleTacticsScript);
        {
            FunctionSymbol addUnitIgnoreUnit = AddFunction("AddUnitIgnoreUnit", _VOID, battleTacticsScript);
            addUnitIgnoreUnit.AddArgument(new VariableSymbol("UnitName", _STRING));
            addUnitIgnoreUnit.AddArgument(new VariableSymbol("UnitIgnoreName", _STRING));
        }
        {
            FunctionSymbol addTeamIgnoreUnit = AddFunction("AddTeamIgnoreUnit", _VOID, battleTacticsScript);
            addTeamIgnoreUnit.AddArgument(new VariableSymbol("TeamNumber", _INTEGER));
            addTeamIgnoreUnit.AddArgument(new VariableSymbol("IgnoreName", _STRING));
        }

        


    }
    public static ScriptScope GetEnclosingClass(Scope s)
        {
            while (s != null)
            { // walk upwards from s looking for a class
                if (s is ScriptScope) return (ScriptScope)s;
                s = s.GetParentScope();
            }
            return null;
        }

        public override string ToString()
        {
            return globals.ToString();
        }

        public static Type _NULL = new BuiltInSymbol("null");
        public static Type _STRING = new BuiltInSymbol("string");
        public static Type _INTEGER = new BuiltInSymbol("int");
        public static Type _FLOAT = new BuiltInSymbol("float");
        public static Type _BOOL = new BuiltInSymbol("bool");
        public static Type _METHODINVOKE = new BuiltInSymbol("METHODINVOKE");
        public static Type _FUNCTIONCALL= new BuiltInSymbol("FUNCTIONCALL");
        public static Type _VOID = new BuiltInSymbol("void",VariableType.vt_void);
        public static Type _INPUT = new BuiltInSymbol("InterfaceInput",VariableType.vt_interfaceInput);
        public static Type _MATH = new BuiltInSymbol("InterfaceMath", VariableType.vt_interfaceMath);
        public static Type _WORLD= new BuiltInSymbol("InterfaceWorldMap", VariableType.vt_interfaceWorldMap);
        public static Type _GLOBALS = new BuiltInSymbol("InterfaceGlobals", VariableType.vt_interfaceGlobals);
        public static Type _BATTLE = new BuiltInSymbol("InterfaceBattle", VariableType.vt_interfaceBattle);
        public static Type _BATTLETACTICS = new BuiltInSymbol("InterfaceBattleTactics", VariableType.vt_interfaceBattleTactics);
        public static Type _MAPCURSOR = new BuiltInSymbol("InterfaceMapCursor", VariableType.vt_interfaceMapCursor);

        public static Type _VEC3 = new BuiltInSymbol("ReferenceVector3", VariableType.vt_vec3);
        public static Type _ACTOR = new BuiltInSymbol("ScriptActor", VariableType.vt_actor);
        public static Type _SCHOOL = new BuiltInSymbol("ScriptSchool", VariableType.vt_school);
        public static Type _TALKBOX = new BuiltInSymbol("ScriptTalkBox", VariableType.vt_talkbox);
        public static Type _UNITLIST = new BuiltInSymbol("UnitList", VariableType.vt_unitList);
}

    public class Symbol
    {
        public Symbol(string name)
        {
            Name = name;
        }

        public Symbol(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public String Name { get; set; }
        public Type Type { get; set; }

        public Scope Scope { get; set; }

        public override string ToString()
        {
            String result = "";

            if (Type != null)
            {
                result = "<" + Name + ":" + Type + ">\n";
            }
            else
            {
                result = Name;
            }
            return result;
        }

        public bool Declared { get; set; }


    public static void GuessType(Symbol s)
    {
        //if (s.Type == null)
        //{
        //    if (s.Name.StartsWith("num", StringComparison.OrdinalIgnoreCase))
        //    {
        //        s.Type = SymbolTable._INTEGER;
        //    }
        //    else if (s.Name.EndsWith("num", StringComparison.OrdinalIgnoreCase))
        //    {
        //        s.Type = SymbolTable._INTEGER;
        //    }
        //    else if (s.Name.StartsWith("name", StringComparison.OrdinalIgnoreCase))
        //    {
        //        s.Type = SymbolTable._STRING;
        //    }
        //    else if (s.Name.EndsWith("name", StringComparison.OrdinalIgnoreCase))
        //    {
        //        s.Type = SymbolTable._STRING;
        //    }
        //}
    }



}

public abstract class ScopedSymbol : Symbol, Scope
    {
        protected Scope m_enclosingScope;
        bool haveGuessed = false;
        public ScopedSymbol(string name, Scope enclosingScope) : base(name)
        {
            m_enclosingScope = enclosingScope;
        }

        public ScopedSymbol(string name, Type type, Scope enclosingScope) : base(name, type)
        {
            m_enclosingScope = enclosingScope;
        }

    public virtual Symbol Resolve(string name)
    {
        Symbol s = null;
        if (name != null)
        {
        if (name != null)
        {
            GetMembers().TryGetValue(name, out s);
        }
            if (s != null)
            {
                return s;
            }
            // if not here, check any parent scope
            if (GetParentScope() != null)
            {
                return GetParentScope().Resolve(name);
            }
        }
        return null; // not found
    }

    public void Define(Symbol sym)
        {
            if (GetMembers() == null)
            {
                int ibreak = 0;
            }
            if (!GetMembers().ContainsKey(sym.Name))
            {
                GetMembers().Add(sym.Name, sym);
                sym.Scope = this; // track the scope in each symbol
            }
        }

        public Scope GetParentScope() { return GetEnclosingScope(); }
        public Scope GetEnclosingScope() { return m_enclosingScope; }

        public String GetScopeName() { return Name; }

        /** Indicate how subclasses store scope members. Allows us to
         *  factor out common code in this class.
         */
        public abstract Dictionary<String, Symbol> GetMembers();

    public void GuessTypes()
    {
        haveGuessed = true;
        foreach (Symbol s in GetMembers().Values)
        {
            Symbol.GuessType(s);
            if (s is ScopedSymbol)
            {
                ScopedSymbol ss = s as ScopedSymbol;
                if (!ss.haveGuessed)
                {
                    ss.GuessTypes();
                }
            }

        }

    }



}

public class ScriptScope : ScopedSymbol, Type
    {
        /** This is the superclass not enclosingScope field. We still record
         *  the enclosing scope so we can push in and pop out of class defs.
         */
        /** List of all fields and methods */
        public Dictionary<String, Symbol> members = new Dictionary<String, Symbol>();
        
        public ScriptScope(String name, Scope enclosingScope) : base(name, enclosingScope)
        {
            Name = (name != null)?name.ToLower():"";
        }

        public ScriptScope(String name, Type type,Scope enclosingScope) : base(name, type,enclosingScope)
        {
            Name = (name != null) ? name.ToLower() : "";
        }


        public Scope getParentScope()
        {
            return m_enclosingScope; // globals
        }

        /** For a.b, only look in a's class hierarchy to resolve b, not globals */
        public Symbol ResolveMember(String name)
        {
            Symbol s = null;
            members.TryGetValue(name,out s);
            if (s != null) return s;
            // if not here, check just the superclass chain
            //if (m_superClass != null)
            //{
            //    return m_superClass.ResolveMember(name);
            //}
            return null; // not found
        }

        public override Dictionary<String, Symbol> GetMembers() { return members; }
        public override String ToString()
        {
            String result = "class " + Name + "\n{\n";
            foreach (Symbol s in members.Values)
            {
                result += s.ToString();
                result += "\n";
            }
            result += "\n}\n";
            return result;
        }
    }




    public enum VariableScopeType
    {
        vst_static,
        vst_instance,
        vst_argument,
        vst_local,
        vst_function
    }

    public enum VariableType
    {
        vt_unknown,
        vt_int,
        vt_bool,
        vt_string,
        vt_float,
        vt_vec3,
        vt_interfaceMath,
        vt_interfaceGlobals,
        vt_interfaceInput,
        vt_interfaceWorldMap,
        vt_interfaceBattle,
        vt_interfaceMapCursor,
        vt_interfaceBattleTactics,
        vt_talkbox,
        vt_entityObject,
        vt_camera,
        vt_actor,
        vt_school,
        vt_void,
        vt_unitList
    }


//}