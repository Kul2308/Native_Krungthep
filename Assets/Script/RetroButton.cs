using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class RetroButton : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TMP_Text label;
    public GameObject arrow;
    public Color normalColor = new Color(0.7f, 0.7f, 0.7f);
    public Color hoverColor = Color.white;
    public Color blinkColor = Color.yellow;
    public int blinkCount = 4;
    public float blinkInterval = 0.07f;
    public UnityEvent onConfirmed;

    bool busy;

    void Awake()
    {
        if (label == null) label = GetComponent<TMP_Text>();
        label.color = normalColor;
        if (arrow != null) arrow.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData e)
    {
        if (busy) return;
        label.color = hoverColor;
        if (arrow != null) arrow.SetActive(true);
    }

    public void OnPointerExit(PointerEventData e)
    {
        if (busy) return;
        label.color = normalColor;
        if (arrow != null) arrow.SetActive(false);
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (!busy) StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        busy = true;
        for (int i = 0; i < blinkCount; i++)
        {
            label.color = blinkColor;
            yield return new WaitForSeconds(blinkInterval);
            label.color = hoverColor;
            yield return new WaitForSeconds(blinkInterval);
        }
        busy = false;
        onConfirmed.Invoke();
    }
}