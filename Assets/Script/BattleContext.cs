using UnityEngine;
using UnityEngine.SceneManagement;

public enum BattleResult { None, Won, Lost }

// ตัวกลางส่งข้อมูลข้ามซีน: เกมหลัก <-> ซีนต่อสู้
public static class BattleContext
{
    public const string BattleSceneName = "Battle";   // ชื่อซีนต่อสู้ (ต้องอยู่ใน Build Settings)

    // ขาเข้า (เกมหลักตั้ง)
    public static BossData Boss;
    public static string ReturnScene;
    public static int PlayerStartHP = -1;             // -1 = เลือดเต็ม

    // ขาออก (ซีนต่อสู้ตั้ง เกมหลักอ่าน)
    public static BattleResult Result = BattleResult.None;
    public static int PlayerHPLeft;

    // เกมหลักเรียกอันนี้เพื่อเริ่มสู้
    public static void Begin(BossData boss, string returnScene, int playerHP = -1)
    {
        Boss = boss;
        ReturnScene = returnScene;
        PlayerStartHP = playerHP;
        Result = BattleResult.None;
        SceneManager.LoadScene(BattleSceneName);
    }

    // เกมหลักเรียกหลังอ่านผลเสร็จ
    public static void Clear()
    {
        Boss = null;
        ReturnScene = null;
        PlayerStartHP = -1;
        Result = BattleResult.None;
    }
}
