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
        XpReward = BaseData.XpReward;
        KpReward = BaseData.KpReward;
    }
}
