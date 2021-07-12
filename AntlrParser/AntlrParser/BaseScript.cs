using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BaseScript
{
    public InterfaceGlobals InterfaceGlobals()
    {
        return null;
    }
    public InterfaceWorldMap InterfaceWorldMap()
    {
        return null;
    }

    public InterfaceMapCursor InterfaceMapCursor()
    {
        return null;
    }

    public InterfaceMath InterfaceMath()
    {
        return null;
    }

    public ReferenceVector3 NewVec3()
    {
        return new ReferenceVector3();
    }

    public InterfaceBattle InterfaceBattle()
    {
        return null;
    }


    public void EnterCriticalSection()
    {
    }

    public void LeaveCriticalSection()
    {
    }

    public void Sleep(float f)
    {
    }

    public ScriptActor self;

}

public class InterfaceGlobals
{
    public ScriptSchool GetSchool()
    {
        return null;
    }
}

public class InterfaceWorldMap
{
    public float GetDistanceToHero(ReferenceVector3 pos)
    {
        return 0;
    }

    public void GetHeroPosition(ReferenceVector3 pos)
    {

    }

    public ScriptActor SummonEntity(string name)
    {
        return null;
    }

}

public class InterfaceMath
{
    public float RandomNumber(float val)
    {
        return 0;
    }
}

public class InterfaceBattle
{
    public void PauseCountdownTimer()
    {
    }

    public void UnpauseCountdownTimer()
    {
    }


}

public class InterfaceInput
{

}


public class ScriptSchool
{

}

public class InterfaceMapCursor
{
}

public class ScriptWorldMap
{
    public ScriptActor SummonEntity(string name)
    {
        return null;
    }

    public void DestroyEntity(ScriptActor eo)
    {

    }

    public void GetHeroPosition(ReferenceVector3 v3)
    {

    }

    public ScriptActor GetHero()
    {
        return null;
    }

    public ScriptActor GetWorldProp(string name)
    {
        return null;
    }
    public ScriptActor RequestShop(string name)
    {
        return null;
    }
    public void RequestSceneChange(string region,string prop,string a,string b)
    {

    }

    public void enableclosecamera(int val)
    {

    }


}

//public class ScriptActor
public class ScriptActor
{
    public void LoadAnimRef(string a,string b)
    {

    }
    public void PlayAnimRef(string a, bool loop)
    {

    }


    public void SetPositionXYZ(float x,float y,float z)
    {

    }

    public void SetTargetPosXYZ(float x, float y, float z)
    {

    }
    public void FacePosXYZ(float x, float y, float z)
    {

    }

    public void LookAtMe(float val)
    {

    }
    

    public void GetPosition(ReferenceVector3 v3)
    {
        // this needs a vector3 class or a ref...
    }

    public float GetDistanceToHero()
    {
        return 0;
    }

    public void CutSceneComplete()
    {

    }

    public void CreateGridTrigger(string name,float val1,float val2,float val3,string name2,float val4, string name3)
    {

    }

    public Object CreateObject()
    {
        return null;
    }

    public void SetLeagueTips(int val)
    {

    }

    public void RecruitChoiceMade(int val)
    {

    }

    public Object CreatePinCamera(string name)
    {
        return null;
    }

    public Object CreateLookAtCamera()
    {
        return null;
    }

    public string GetState()
    {
        return "";
    }

    public void LockLeagues()
    {

    }

    public void LockHistory()
    {

    }

    public void LockRecruiting()
    { }

    public Object LoadWorldCamTrack(string path)
    {
        return null;
    }
    public Object LoadCharCamTrack(string path)
    {
        return null;
    }

    public void FaceUnit(ScriptActor unit)
    {

    }

    public void AddEntity(ScriptActor unit)
    {

    }

    public void Move(int x,int y)
    {

    }

    public void OrderNamedSupportSkill(string name, int x, int y)
    {
    }

    public void OrderUseSkill(int x,int y)
    {

    }
    
    public bool HasNamedSkill(string name)
    {
        return false;
    }

    public void CardinalTurnRight()
    {

    }

    public void OrderPass()
    {

    }

    public void Rotate(float pi)
    {

    }

    public void SetNewScript(string name)
    {

    }

    public void WalkWayPointsLoop(int val)
    {

    }

    public void WalkWayPointsOneShot(int val)
    {

    }

    public string GetBestAffinitySkill()
    {
        return "";
    }

    public string GetBestSkillForEnemy(ScriptActor enemy)
    {
        return "";
    }

    public string GetClassName()
    {
        return "";
    }

    public ScriptActor GetClosestEnemy()
    {
        return null;
    }

    public void AddWayPointXYZ(float x,float y,float z)
    {

    }

    public float AirDistanceToUnit(string name)
    {
        return 0;
    }

    public bool mCutSceneComplete;
    public string mOnExecName;
    public int mScriptState;



    public List<ScriptActor> mUnits = new List<ScriptActor>();

}




