using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum HitStrength { Light, Heavy, Ultimate }

// เอฟเฟกต์ตอนโดนตี: กล้องสั่น + ตัวกระพริบขาว/สั่น + ตัวเลขดาเมจลอย
public class HitEffects : MonoBehaviour
{
    [Header("References")]
    public Canvas canvas;      // ลาก Canvas ใส่
    public Camera cam;         // ว่างไว้ = ใช้ Main Camera
    public RectTransform uiRoot; // Panel ที่ครอบ UI ทั้งหมด (ให้ UI สั่นตาม) ว่างไว้ = UI ไม่สั่น

    [Header("Camera shake (หน่วย world)")]
    public float lightShake = 0.08f;
    public float heavyShake = 0.2f;
    public float ultiShake = 0.4f;

    [Header("UI shake (หน่วย canvas pixel, 0 = ไม่สั่น)")]
    public float lightUiShake = 0f;
    public float heavyUiShake = 8f;
    public float ultiUiShake = 30f;

    [Header("Popup")]
    public float popupHeight = 120f;   // ลอยขึ้นกี่หน่วย canvas
    public float popupTime = 0.9f;

    Vector3 camOrigin;
    Vector2 uiOrigin;
    Coroutine shakeRoutine;
    readonly Dictionary<SpriteRenderer, Color> originalColors = new Dictionary<SpriteRenderer, Color>();
    readonly Dictionary<SpriteRenderer, Vector3> originalPos = new Dictionary<SpriteRenderer, Vector3>();

    void Awake()
    {
        if (cam == null) cam = Camera.main;
        camOrigin = cam.transform.position;
        if (uiRoot != null) uiOrigin = uiRoot.anchoredPosition;
    }

    // ---------- เรียกจาก BattleManager ----------
    public void Hit(Fighter target, int damage, HitStrength strength = HitStrength.Light)
    {
        float shake = strength == HitStrength.Light ? lightShake
                    : strength == HitStrength.Heavy ? heavyShake : ultiShake;
        float shakeTime = strength == HitStrength.Light ? 0.15f : strength == HitStrength.Heavy ? 0.3f : 0.5f;
        float uiShake = strength == HitStrength.Light ? lightUiShake
                      : strength == HitStrength.Heavy ? heavyUiShake : ultiUiShake;
        Shake(shake, shakeTime, uiShake);
        Flash(target);

        Color color = strength == HitStrength.Light ? Color.white
                    : strength == HitStrength.Heavy ? new Color(1f, 0.85f, 0.2f)
                    : new Color(1f, 0.45f, 0.1f);
        float size = strength == HitStrength.Light ? 56f
                   : strength == HitStrength.Heavy ? 76f : 100f;
        Popup(target, damage.ToString(), color, size);
    }

    public void Popup(Fighter target, string text, Color color, float size = 56f)
    {
        var sr = target.GetComponentInChildren<SpriteRenderer>();
        Vector3 worldTop = sr != null
            ? new Vector3(sr.bounds.center.x, sr.bounds.max.y, 0f)
            : target.transform.position + Vector3.up;

        Vector2 start = WorldToCanvas(worldTop);
        start.x += Random.Range(-30f, 30f);   // กันตัวเลขทับกันพอดี

        var go = new GameObject("Popup", typeof(RectTransform), typeof(CanvasGroup));
        go.transform.SetParent(uiRoot != null ? (Transform)uiRoot : canvas.transform, false);
        var rt = (RectTransform)go.transform;
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(500f, 120f);
        rt.anchoredPosition = start;

        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = text;
        t.color = color;
        t.fontSize = size;
        t.fontStyle = FontStyles.Bold;
        t.alignment = TextAlignmentOptions.Center;
        t.outlineWidth = 0.3f;
        t.outlineColor = new Color32(0, 0, 0, 255);
        t.raycastTarget = false;

        StartCoroutine(PopupRoutine(rt, go.GetComponent<CanvasGroup>(), start));
    }

    public void Shake(float intensity, float duration, float uiPixels = 0f)
    {
        if (shakeRoutine != null) StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(ShakeRoutine(intensity, duration, uiPixels));
    }

    // ---------- ข้างใน ----------
    void Flash(Fighter target)
    {
        var sr = target.GetComponentInChildren<SpriteRenderer>();
        if (sr == null) return;
        if (!originalColors.ContainsKey(sr))
        {
            originalColors[sr] = sr.color;
            originalPos[sr] = sr.transform.localPosition;
        }
        StartCoroutine(FlashRoutine(sr));
    }

    IEnumerator FlashRoutine(SpriteRenderer sr)
    {
        Color orig = originalColors[sr];
        Vector3 pos = originalPos[sr];
        float t = 0f, dur = 0.2f;
        while (t < dur)
        {
            t += Time.deltaTime;
            sr.color = t < dur * 0.5f ? Color.white : orig;   // ขาวครึ่งแรก
            sr.transform.localPosition = pos + (Vector3)(Random.insideUnitCircle * 0.06f);
            yield return null;
        }
        sr.color = orig;
        sr.transform.localPosition = pos;
    }

    IEnumerator ShakeRoutine(float intensity, float duration, float uiPixels)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float fade = 1f - t / duration;
            Vector2 o = Random.insideUnitCircle * intensity * fade;
            cam.transform.position = camOrigin + new Vector3(o.x, o.y, 0f);
            if (uiRoot != null && uiPixels > 0f)
                uiRoot.anchoredPosition = uiOrigin + Random.insideUnitCircle * uiPixels * fade;
            yield return null;
        }
        cam.transform.position = camOrigin;
        if (uiRoot != null) uiRoot.anchoredPosition = uiOrigin;
    }

    IEnumerator PopupRoutine(RectTransform rt, CanvasGroup group, Vector2 start)
    {
        float time = 0f;
        while (time < popupTime)
        {
            time += Time.deltaTime;
            float k = time / popupTime;
            float ease = 1f - (1f - k) * (1f - k);                       // ลอยเร็วแล้วช้าลง
            rt.anchoredPosition = start + Vector2.up * popupHeight * ease;
            float pop = k < 0.15f ? Mathf.Lerp(1.6f, 1f, k / 0.15f) : 1f; // เด้งใหญ่แล้วหดลง
            rt.localScale = Vector3.one * pop;
            group.alpha = k < 0.6f ? 1f : 1f - (k - 0.6f) / 0.4f;        // จางตอนท้าย
            yield return null;
        }
        Destroy(rt.gameObject);
    }

    Vector2 WorldToCanvas(Vector3 world)
    {
        Vector2 screen = cam.WorldToScreenPoint(world);
        var canvasRect = (RectTransform)canvas.transform;
        Camera uiCam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screen, uiCam, out Vector2 local);
        return local;
    }
}
