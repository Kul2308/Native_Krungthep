using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("HP")]
    public float maxHP = 100f;
    public float hitDamage = 10f;      // HP lost per hit
    public float hitCooldown = 0.5f;   // seconds of safety after a hit
    public Image hpFill;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float jumpStaminaCost = 5f; // stamina lost per jump
    public float staminaRegen = 15f;   // refill per second
    public Image staminaFill;

    float hp, stamina;
    float lastHitTime = -99f;

    void Start()
    {
        hp = maxHP;
        stamina = maxStamina;
        UpdateUI();
    }

    void Update()
    {
        if (stamina < maxStamina)
        {
            stamina = Mathf.Min(maxStamina, stamina + staminaRegen * Time.deltaTime);
            UpdateUI();
        }
    }

    // Call this when the player gets hit
    public void OnHit()
    {
        Debug.Log("OnHit called");
        if (Time.time - lastHitTime < hitCooldown) return;   // ignore repeat hits
        lastHitTime = Time.time;
        TakeDamage(hitDamage);
    }

    // Call this when the player jumps. Returns false if not enough stamina.
    public bool TryJump()
    {
        return UseStamina(jumpStaminaCost);
    }

    public void TakeDamage(float amount)
    {
        hp = Mathf.Max(0f, hp - amount);
        Debug.Log("HP now: " + hp + "  fill: " + (hp / maxHP));
        UpdateUI();
        if (hp <= 0f) GameOverScene.Trigger();
    }

    public void Heal(float amount)
    {
        hp = Mathf.Min(maxHP, hp + amount);
        UpdateUI();
    }

    public bool UseStamina(float amount)
    {
        if (stamina < amount) return false;
        stamina -= amount;
        UpdateUI();
        return true;
    }

    void UpdateUI()
    {
        hpFill.fillAmount = hp / maxHP;
        staminaFill.fillAmount = stamina / maxStamina;
    }
}