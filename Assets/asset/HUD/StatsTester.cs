using UnityEngine;
using UnityEngine.InputSystem;

public class StatsTester : MonoBehaviour
{
    public PlayerStats stats;

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.hKey.wasPressedThisFrame) stats.OnHit();   // H = get hit
        if (Keyboard.current.jKey.wasPressedThisFrame)                  // J = jump
            Debug.Log("Jump allowed: " + stats.TryJump());
    }
}