using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUI : MonoBehaviour
{
    [Header("Player bars (bottom left)")]
    public Slider playerHP;
    public Slider playerStamina;
    public Slider playerUlti;

    [Header("Boss (top center)")]
    public TMP_Text bossName;
    public Slider bossHP;
    public TMP_Text intentText;

    [Header("Center text (battle log + win/lose)")]
    public TMP_Text resultText;

    [Header("Skill buttons")]
    public Button attackButton;
    public Button heavyButton;
    public Button restoreButton;
    public Button ultiButton;
    public Button guardButton;

    TMP_Text hpLabel, stmLabel, ultiLabel, bossHpLabel;

    void Awake()
    {
        // ตั้งสีแถบ + ซ่อนปุ่มกลมๆ (handle) + สร้างตัวเลขบนแถบ
        hpLabel = SetupBar(playerHP, new Color(0.25f, 0.8f, 0.3f));
        stmLabel = SetupBar(playerStamina, new Color(1f, 0.8f, 0.1f));
        ultiLabel = SetupBar(playerUlti, new Color(0.7f, 0.3f, 0.9f));
        bossHpLabel = SetupBar(bossHP, new Color(0.85f, 0.15f, 0.15f));
    }

    TMP_Text SetupBar(Slider s, Color fillColor)
    {
        s.interactable = false;
        if (s.handleRect != null) s.handleRect.gameObject.SetActive(false);
        if (s.fillRect != null) s.fillRect.GetComponent<Image>().color = fillColor;

        var t = s.GetComponentInChildren<TMP_Text>();
        if (t != null) return t;

        var go = new GameObject("ValueLabel", typeof(RectTransform));
        go.transform.SetParent(s.transform, false);
        var rt = (RectTransform)go.transform;
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;

        t = go.AddComponent<TextMeshProUGUI>();
        t.alignment = TextAlignmentOptions.Center;
        t.color = Color.white;
        t.enableAutoSizing = true;
        t.fontSizeMin = 10; t.fontSizeMax = 28;
        t.outlineWidth = 0.25f;
        t.outlineColor = new Color32(0, 0, 0, 255);
        t.raycastTarget = false;
        return t;
    }

    public void Refresh(Fighter player, Fighter boss, string intent, BattleState state)
    {
        playerHP.maxValue = player.maxHP; playerHP.value = player.HP;
        playerStamina.maxValue = player.maxStamina; playerStamina.value = player.Stamina;
        playerUlti.maxValue = player.maxUlti; playerUlti.value = player.Ulti;
        bossHP.maxValue = boss.maxHP; bossHP.value = boss.HP;

        hpLabel.text = $"HP {player.HP}/{player.maxHP}";
        stmLabel.text = $"STM {player.Stamina}/{player.maxStamina}";
        ultiLabel.text = player.Ulti >= player.maxUlti ? "ULTI READY!" : $"ULTI {player.Ulti}/{player.maxUlti}";
        bossHpLabel.text = $"{boss.HP}/{boss.maxHP}";

        bossName.text = boss.displayName;
        intentText.text = intent;

        if (state == BattleState.Won) resultText.text = "YOU WIN!";
        else if (state == BattleState.Lost) resultText.text = "YOU LOSE... Try again!";
    }

    public void Log(string message) => resultText.text = message;

    public void SetButtons(bool attack, bool heavy, bool restore, bool ulti, bool guard)
    {
        attackButton.interactable = attack;
        heavyButton.interactable = heavy;
        restoreButton.interactable = restore;
        ultiButton.interactable = ulti;
        guardButton.interactable = guard;
    }

    public void SetSkillLabels(string attack, string heavy, string restore, string ulti, string guard)
    {
        SetButtonText(attackButton, attack);
        SetButtonText(heavyButton, heavy);
        SetButtonText(restoreButton, restore);
        SetButtonText(ultiButton, ulti);
        SetButtonText(guardButton, guard);
    }

    void SetButtonText(Button b, string text)
    {
        var t = b.GetComponentInChildren<TMP_Text>();
        t.text = text;
        t.enableAutoSizing = true;
        t.fontSizeMin = 10; t.fontSizeMax = 24;
    }
}