using System.Collections;
using UnityEngine;

public class EasedMover : MonoBehaviour
{
    [Header("Movement")]
    public Transform objectToMove;

    public Vector3 targetLocalPosition;

    [Header("Timing")]
    public float duration = 1.0f;

    private bool hasMoved;

    public void MoveToTarget()
    {
        if (hasMoved)
            return;

        hasMoved = true;
        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        if (objectToMove == null)
            yield break;

        Vector3 startPosition = objectToMove.localPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            // SmoothStep ease-in/ease-out.
            t = t * t * (3f - 2f * t);

            objectToMove.localPosition =
                Vector3.Lerp(
                    startPosition,
                    targetLocalPosition,
                    t
                );

            yield return null;
        }

        objectToMove.localPosition = targetLocalPosition;
    }
}