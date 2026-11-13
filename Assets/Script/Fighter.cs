using UnityEngine;

// ติดที่ตัวผู้เล่นและบอส เก็บค่า HP / Stamina / Ulti
public class Fighter : MonoBehaviour
{
    public string displayName = "Uncle";
    public int maxHP = 100;
    public int maxStamina = 100;
    public int maxUlti = 100;

    public int HP { get; private set; }
    public int Stamina { get; private set; }
    public int Ulti { get; private set; }
    public bool IsDead => HP <= 0;

    void Awake()
    {
        HP = maxHP;
        Stamina = maxStamina;
        Ulti = 0;
    }

    public void Setup(string name, int newMaxHP)
    {
        displayName = name;
        maxHP = newMaxHP;
        HP = maxHP;
    }

    public void SetHP(int value) => HP = Mathf.Clamp(value, 0, maxHP);

    public void TakeDamage(int amount) => HP = Mathf.Max(0, HP - amount);
    public void UseStamina(int amount) => Stamina = Mathf.Max(0, Stamina - amount);
    public void AddStamina(int amount) => Stamina = Mathf.Min(maxStamina, Stamina + amount);
    public void AddUlti(int amount) => Ulti = Mathf.Min(maxUlti, Ulti + amount);
    public void ResetUlti() => Ulti = 0;
}