using UnityEngine;
using UnityEngine.UI;


public class PlayerStats : MonoBehaviour
{
    [Header("HP")]
    public float maxHP = 100f;
    public Image hpFill;
    

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaRegen = 15f;   // per second
    public Image staminaFill;

    float hp, stamina;

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

    public void TakeDamage(float amount)
    {
        hp = Mathf.Max(0f, hp - amount);
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
        if (stamina < amount) return false;   // not enough stamina
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