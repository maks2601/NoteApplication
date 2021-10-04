using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimator : MonoBehaviour
{
    public static void ElementAppear(Transform element)
    {
        float positionX = element.position.x;
        element.position = element.position - Vector3.right * 2000;
        element.DOMoveX(positionX, 1);
    }

    public static void ElementShake(Transform element)
    {
        element.DOShakeRotation(1f, 10);
    }

    public static void ElementTapResponse(Transform element)
    {
        element.DOScale(Vector3.one * 0.95f, 0.4f).OnComplete(()=>element.DOScale(Vector3.one, 0.2f));
    }
}
