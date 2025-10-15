using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class QuadButton : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent onClick;

    void Reset() // called when component is added or via context menu
    {
        if (!TryGetComponent<BoxCollider2D>(out var col))
            col = gameObject.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
    }

    public void OnPointerClick(PointerEventData eventData) => onClick?.Invoke();
}