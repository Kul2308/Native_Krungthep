using System.Collections.Generic;
using UnityEngine;

public enum MoveType { Attack, Charge }

[System.Serializable]
public class BossMove
{
    public string moveName = "Normal hit";
    public MoveType type = MoveType.Attack;
    public int damage = 15;                          // ใช้เฉพาะ Attack
    [Min(0)] public int weight = 1;                  // ยิ่งสูงยิ่งใช้บ่อย
    public HitStrength strength = HitStrength.Light; // ระดับเอฟเฟกต์ตอนโดน
}

// สร้างบอสใหม่: คลิกขวาใน Project → Create → Battle → Boss Data
[CreateAssetMenu(menuName = "Battle/Boss Data", fileName = "NewBoss")]
public class BossData : ScriptableObject
{
    public string bossName = "Thug";
    public int maxHP = 200;
    public Sprite sprite;                            // ว่างไว้ = ใช้สี่เหลี่ยม placeholder เดิม
    public List<BossMove> moves = new List<BossMove>();
}
