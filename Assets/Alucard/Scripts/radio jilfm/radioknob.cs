using UnityEngine;
using UnityEngine.EventSystems;

public class RadioKnob : MonoBehaviour, IDragHandler
{
    public float rotationSpeed = 0.5f;
    public System.Action<float> onRotationChanged;

    public void OnDrag(PointerEventData eventData)
    {
        float deltaX = eventData.delta.x;
        float rotAmount = deltaX * rotationSpeed * Mathf.Cos(Mathf.PI);

        if (onRotationChanged != null)
        {
            onRotationChanged(rotAmount);
        }
    }
}