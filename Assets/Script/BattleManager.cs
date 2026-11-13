using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum BattleState { PlayerTurn, Busy, Won, Lost }

public class BattleManager : MonoBehaviour
{
    [Header("References")]
    public Fighter player;
    public Fighter boss;
    public BattleUI ui;
    public HitEffects fx;

    [Header("Testing / fallback")]
    public BossData defaultBoss;          // ใช้ตอนเปิดซีนนี้ตรงๆ (ไม่ได้มาจากเกมหลัก)
    public string fallbackReturnScene;    // ว่างไว้ = สู้จบแล้วค้างอยู่หน้าเดิม
    public float endDelay = 2f;           // รอกี่วิหลังจบก่อนกลับเกมหลัก

    [Header("Player skill values (tune balance here)")]
    public int attackCost = 10; public int attackDmg = 10; public int attackUlti = 15;
    public int heavyCost = 30; public int heavyDmg = 28; public int heavyUlti = 25;
    public int restoreAmount = 40;
    public int ultiDmg = 60;
    public int guardCost = 10;
    [Range(0f, 1f)] public float guardReduction = 0.6f;

    [Header("Boss shared values")]
    public int ultiGainWhenHit = 10;
    public int chargeMultiplier = 2;

    public BattleState State { get; private set; }

    BossData bossData;
    BossMove move;       // ท่าที่บอสจะใช้เทิร์นหน้า (intent)
    bool bossCharged;
    bool guarding;

    void Start()
    {
        bossData = BattleContext.Boss != null ? BattleContext.Boss : defaultBoss;
        if (bossData == null || bossData.moves.Count == 0)
        {
            Debug.LogError("BattleManager: no BossData (or it has no moves). Assign Default Boss or start via BattleContext.Begin().");
            enabled = false;
            return;
        }
        BattleContext.Result = BattleResult.None;

        boss.Setup(bossData.bossName, bossData.maxHP);
        var sr = boss.GetComponentInChildren<SpriteRenderer>();
        if (bossData.sprite != null && sr != null) sr.sprite = bossData.sprite;

        if (BattleContext.PlayerStartHP >= 0) player.SetHP(BattleContext.PlayerStartHP);

        ui.SetSkillLabels(
            $"Normal Attack\nDMG {attackDmg} | STM -{attackCost}",
            $"Heavy Attack\nDMG {heavyDmg} | STM -{heavyCost}",
            $"Re_Stamina\nSTM +{restoreAmount}",
            $"Ultimate\nDMG {ultiDmg} | Needs full gauge",
            $"Guard\nBlock {Mathf.RoundToInt(guardReduction * 100)}% | STM -{guardCost}");

        PickBossMove();
        State = BattleState.PlayerTurn;
        RefreshUI();
        ui.Log("Fight!");
    }

    // ---------- Player buttons ----------
    public void OnAttack()
    {
        if (State != BattleState.PlayerTurn || player.Stamina < attackCost) return;
        player.UseStamina(attackCost);
        boss.TakeDamage(attackDmg);
        fx.Hit(boss, attackDmg, HitStrength.Light);
        player.AddUlti(attackUlti);
        ui.Log($"{player.displayName} used Normal Attack  -{attackDmg} HP");
        StartCoroutine(EndPlayerTurn());
    }

    public void OnHeavy()
    {
        if (State != BattleState.PlayerTurn || player.Stamina < heavyCost) return;
        player.UseStamina(heavyCost);
        boss.TakeDamage(heavyDmg);
        fx.Hit(boss, heavyDmg, HitStrength.Heavy);
        player.AddUlti(heavyUlti);
        ui.Log($"{player.displayName} used Heavy Attack  -{heavyDmg} HP");
        StartCoroutine(EndPlayerTurn());
    }

    public void OnRestore()
    {
        if (State != BattleState.PlayerTurn) return;
        player.AddStamina(restoreAmount);
        fx.Popup(player, $"+{restoreAmount} STM", new Color(1f, 0.85f, 0.2f));
        ui.Log($"{player.displayName} rested  STM +{restoreAmount}");
        StartCoroutine(EndPlayerTurn());
    }

    public void OnUlti()
    {
        if (State != BattleState.PlayerTurn || player.Ulti < player.maxUlti) return;
        player.ResetUlti();
        boss.TakeDamage(ultiDmg);
        fx.Hit(boss, ultiDmg, HitStrength.Ultimate);
        ui.Log($"{player.displayName} used ULTI!!  -{ultiDmg} HP");
        StartCoroutine(EndPlayerTurn());
    }

    public void OnGuard()
    {
        if (State != BattleState.PlayerTurn || player.Stamina < guardCost) return;
        player.UseStamina(guardCost);
        guarding = true;
        fx.Popup(player, "GUARD", new Color(0.4f, 0.8f, 1f));
        ui.Log($"{player.displayName} is guarding!  Damage -{Mathf.RoundToInt(guardReduction * 100)}% this turn");
        StartCoroutine(EndPlayerTurn());
    }

    // ---------- Turn flow ----------
    IEnumerator EndPlayerTurn()
    {
        State = BattleState.Busy;
        RefreshUI();
        yield return new WaitForSeconds(0.9f);

        if (boss.IsDead) { Finish(BattleState.Won); yield break; }

        DoBossAction();
        RefreshUI();
        yield return new WaitForSeconds(0.9f);

        if (player.IsDead) { Finish(BattleState.Lost); yield break; }

        PickBossMove();
        State = BattleState.PlayerTurn;
        RefreshUI();
    }

    void PickBossMove()
    {
        int total = 0;
        foreach (var m in bossData.moves) total += Mathf.Max(0, m.weight);
        int r = Random.Range(0, total);
        foreach (var m in bossData.moves)
        {
            r -= Mathf.Max(0, m.weight);
            if (r < 0) { move = m; return; }
        }
        move = bossData.moves[0];
    }

    void DoBossAction()
    {
        if (move.type == MoveType.Charge)
        {
            bossCharged = true;
            guarding = false;
            fx.Popup(boss, "CHARGING!", new Color(1f, 0.5f, 0.1f));
            ui.Log($"{boss.displayName} is charging up! Next hit x{chargeMultiplier}");
            return;
        }

        int dmg = move.damage;
        bool wasCharged = bossCharged;
        if (bossCharged) { dmg *= chargeMultiplier; bossCharged = false; }

        string blockText = "";
        if (guarding)
        {
            int blocked = Mathf.RoundToInt(dmg * guardReduction);
            dmg -= blocked;
            blockText = $" (Guard blocked {blocked})";
            guarding = false;
        }
        player.TakeDamage(dmg);
        fx.Hit(player, dmg, wasCharged ? HitStrength.Ultimate : move.strength);
        player.AddUlti(ultiGainWhenHit);
        ui.Log($"{boss.displayName} used {move.moveName}  You lose {dmg} HP{blockText} (ULTI +{ultiGainWhenHit})");
    }

    void Finish(BattleState result)
    {
        State = result;
        BattleContext.Result = result == BattleState.Won ? BattleResult.Won : BattleResult.Lost;
        BattleContext.PlayerHPLeft = player.HP;
        RefreshUI();
        StartCoroutine(ReturnToMain());
    }

    IEnumerator ReturnToMain()
    {
        yield return new WaitForSeconds(endDelay);

        if (State == BattleState.Lost)
        {
            // แพ้ = เล่นใหม่ (โหลดซีนต่อสู้เดิม ใช้บอสกับเลือดเริ่มต้นเดิมจาก BattleContext)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            yield break;
        }

        // ชนะ = ไปต่อ (กลับซีนเกมหลัก)
        string scene = !string.IsNullOrEmpty(BattleContext.ReturnScene) ? BattleContext.ReturnScene : fallbackReturnScene;
        if (!string.IsNullOrEmpty(scene)) SceneManager.LoadScene(scene);
    }

    // ---------- Feed UI ----------
    string IntentText()
    {
        if (move.type == MoveType.Charge)
            return $"NEXT: {move.moveName} (next hit x{chargeMultiplier})";
        int dmg = bossCharged ? move.damage * chargeMultiplier : move.damage;
        return $"NEXT: {move.moveName} {dmg} dmg";
    }

    void RefreshUI()
    {
        bool myTurn = State == BattleState.PlayerTurn;
        ui.Refresh(player, boss, IntentText(), State);
        ui.SetButtons(
            myTurn && player.Stamina >= attackCost,
            myTurn && player.Stamina >= heavyCost,
            myTurn,
            myTurn && player.Ulti >= player.maxUlti,
            myTurn && player.Stamina >= guardCost);
    }
}