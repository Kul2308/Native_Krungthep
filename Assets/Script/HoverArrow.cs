using UnityEngine;
using UnityEngine.EventSystems;

public class HoverArrow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject arrow;

    public void OnPointerEnter(PointerEventData e) { arrow.SetActive(true); }
    public void OnPointerExit(PointerEventData e) { arrow.SetActive(false); }
}