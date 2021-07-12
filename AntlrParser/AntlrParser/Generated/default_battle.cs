public class default_battle : BaseScript
{
    public void BattleEnumSetup()
    {
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
    }

    public void DefaultCrowdUpdate(UNKNOWN theskirmish)
    {
        float;
        DamageCrowdVal;
        float;
        DamageCalc;
        int32;
        NumFlubs;
        int32;
        SPCost;
        int32;
        SkillAttributes;
        ;
        BattleEnumSetup();
        attacker=theskirmish.GetAttacker() ;
        ;
        DamageCrowdVal=0.0f ;
        ;
        int NumFlubs=0 ;
        ;
        targets=theskirmish.GetTargetList() ;
        ;
        skirmishNode=targets.GetFirst() ;
        ;
        while(skirmishNode!=){if(skirmishNode.mFlags& TARGET_RESULT_HIT){
        DamageCalc=skirmishNode.mResult_DamagePercent ;
        ;
        DamageCalc=skirmishNode.mCrowdBonusMult ;
        ;
        DamageCrowdVal+=DamageCalc ;
        ;
        }
        if(skirmishNode.mCrowdNoFlub==0){
        if(skirmishNode.mFlags& TARGET_RESULT_WEAKHIT){
        NumFlubs+=2 ;
        ;
        }
        else if(skirmishNode.mFlags&TARGET_RESULT_MISS){
        NumFlubs+=1 ;
        ;
        }
        ;
        }
        skirmishNode=targets.GetNext() ;
        ;};
        if(DamageCrowdVal>0){
        attacker.ChangeCrowd(DamageCrowdVal);
        }
        ;
        if(NumFlubs>0){
        attacker.ChangeCrowd(NumFlubs);
        }
        else if(!(theskirmish.mSFlags&KERO_SKIRMISH_NOCOST)){
        SkillAttributes=theskirmish.GetAttackSkillValueInt("mAttributes") ;
        ;
        SPCost=theskirmish.GetAttackSkillValueInt("mSPCost") ;
        ;
        attacker.ChangeCrowd(theskirmish.mCrowdBonusMult);
        if(SkillAttributes& SKILLATTR_AFFINITY){
        attacker.ChangeCrowd(theskirmish.mCrowdBonusMult);
        }
        ;
        }
        ;
    }

    public void OnSkirmishEnd(UNKNOWN theSkirmish)
    {
        skirmishList=theSkirmish.GetTargetList() ;
        ;
        skirmishNode=skirmishList.GetFirst() ;
        ;
        while(theSkirmish.DoAttackerClassChatter(skirmishNode)!=0){Sleep(0);};
        while(theSkirmish.DoDefenderClassChatter(skirmishNode)!=0){Sleep(0);};
        DefaultCrowdUpdate(theSkirmish);
    }
}
