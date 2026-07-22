using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerRuntimeData : EntityRuntimeData<PlayerBaseData>
{  
    public int Kp = 0;
    public float Gold = 0f;
    [ProgressBar("Xp", "XpToLevelUp", EColor.Violet)]
    public int Xp = 0;
    public int XpToLevelUp;

    public UnityAction<int, int> OnXpChanged;
    public UnityAction<int> OnKpChanged;
    public UnityAction<int> OnLevelUp;

    private int[] _xpTable;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        _xpTable = TypedBaseData.XpTable;
        XpToLevelUp = _xpTable[CurrentLevel-1];
    }

    public void AddXp(int recievedXp)
    {
        Xp += recievedXp;
        CheckLevelUp();
        OnXpChanged?.Invoke(Xp, XpToLevelUp);
    }

    private void CheckLevelUp()
    {
        while (CurrentLevel < _xpTable.Length && Xp >= XpToLevelUp)
        {
            LevelUp();
            OnLevelUp?.Invoke(CurrentLevel);
        }
    }

    private void LevelUp()
    {
        CurrentLevel++;
        Xp -= XpToLevelUp;
        XpToLevelUp = _xpTable[CurrentLevel-1];
        CurrentAttackDamage += 10;
    }

    public void AddKp(int recievedKp)
    {
        Kp += recievedKp;
        OnKpChanged?.Invoke(Kp);
    }
}
