using System.Collections.Generic;
using UnityEngine;

public class MonsterRuntimeData : EntityRuntimeData<MonsterBaseData>
{   
    protected override void Start()
    {
        base.Start();
    }

    protected override void ResetToBaseData()
    {
        base.ResetToBaseData();
        CurrentXpReward = TypedBaseData.XpReward;
        CurrentKpReward = TypedBaseData.KpReward;
    }

    

}
