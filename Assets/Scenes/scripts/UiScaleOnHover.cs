using Lean.Transition;
using Lean.Transition.Method;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiScaleOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scaleFactor = 0.1f;
    public float duration = 0.1f;
    private Vector3 actualScale;
    private bool state = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (state)
        {
            return;
        }
        state = true;
        actualScale = transform.localScale;
        transform.localScaleTransition(actualScale * (1 + scaleFactor), duration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!state)
        {
            return;
        }
        state = false;
        transform.localScaleTransition(actualScale, duration);
    }
}
