using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "New Monster Data", menuName = "Monster/New Monster Data")]

[System.Serializable]
public class MonsterStat
{
    public string statName;
    public float statValue;
    public MonsterStat(string name, float value)
    {
        statName = name;
        statValue = value;
    }
}
[CreateAssetMenu(fileName = "New Monster Data", menuName = "Monster/New Monster Data")]
public class MonsterData : ScriptableObject
{
    [Header("Dynamic Stats (Custom Values)")]
    public List<MonsterStat> customStats = new List<MonsterStat>();

    // Reset จะทำงานอัตโนมัติเมื่อสร้าง ScriptableObject ใหม่ใน Project
    private void Reset()
    {
        InitializeDefaultStats();
    }

    private void InitializeDefaultStats()
    {
        customStats.Clear();

        // ใส่ค่า Base Values ที่คุณต้องการไว้ในลิสต์
        customStats.Add(new MonsterStat("MaxHP", 100f));
        customStats.Add(new MonsterStat("MoveSpeed", 3f));
        customStats.Add(new MonsterStat("AttackDamage", 10f));
        customStats.Add(new MonsterStat("StopDistance", 1f));
    }

    public float GetStatValue(string name)
    {
        var stat = customStats.FirstOrDefault(s => s.statName == name);

        if (stat != null)
        {
            return stat.statValue;
        }

        Debug.LogWarning($"Stat {name} not found in {this.name}!");
        return 0;
    }
}
