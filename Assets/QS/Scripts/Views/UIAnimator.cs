using DG.Tweening;
using UnityEngine;

namespace QualiumSystems
{
    public class UIAnimator : MonoBehaviour
    {
        private const float AppearDuration = 1f;
        private const float ShakeDuration = 1f;
        private const float ShakeStrength = 10f;
        private const float MinScaleMultiplier = 0.95f;
        private const float ShrinkTime = 0.4f;

        // Teleport element off screen and then appear in the specified time
        public static void ElementAppear(Transform element)
        {
            float positionX = element.position.x;
            element.position -= Vector3.right * Screen.width;
            element.DOMoveX(positionX, AppearDuration);
        }

        // Shake element with indicated force during the specified time
        public static void ElementShake(Transform element)
        {
            element.DOShakeRotation(ShakeDuration, ShakeStrength);
        }

        // Shrink then enlarge the object
        public static void ElementTapResponse(Transform element)
        {
            var scale = element.localScale;
            element.DOScale(scale * MinScaleMultiplier, ShrinkTime).OnComplete(() => element.DOScale(scale, ShrinkTime / 2f));
        }
    }
}
