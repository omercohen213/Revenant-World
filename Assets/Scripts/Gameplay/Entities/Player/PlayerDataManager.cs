using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerDataManager : EntityDataManager
{
    public int Score = 0;
    public float Gold = 0f;
    public int Xp = 0;
    public int XpToLevelUp;

    public UnityAction<int, int> OnXpChanged;
    public UnityAction<int> OnLevelUp;
    public UnityAction<int> OnScoreChanged;

    private int[] _xpTable;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        PlayerBaseData playerBaseData = (PlayerBaseData)baseData;
        _xpTable = playerBaseData.XpTable;
        XpToLevelUp = _xpTable[Level-1];
    }

    public void AddXP(int recievedXp)
    {
        Xp += recievedXp;
        CheckLevelUp();
        OnXpChanged?.Invoke(Xp, XpToLevelUp);
    }

    private void CheckLevelUp()
    {
        while (Level < _xpTable.Length && Xp >= XpToLevelUp)
        {
            LevelUp();
            OnLevelUp?.Invoke(Level);
        }
    }

    private void LevelUp()
    {
        Level++;
        Xp -= XpToLevelUp;
        XpToLevelUp = _xpTable[Level-1];
        AttackDamage += 10;
    }

    public void AddScore(int recievedScore)
    {
        Score += recievedScore;
        OnScoreChanged?.Invoke(Score);
    }
}
