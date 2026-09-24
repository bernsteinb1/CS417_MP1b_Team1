using System.Collections;
using UnityEngine;

public class RotatingEasedMover : MonoBehaviour
{
    public Transform objectToRotate;
    public Vector3 targetLocalEulerAngles;
    public float duration = 0.9f;

    private bool opened;

    public void Open()
    {
        if (opened) return;
        opened = true;
        StartCoroutine(RotateRoutine());
    }

    private IEnumerator RotateRoutine()
    {
        if (objectToRotate == null)
            yield break;

        Quaternion start = objectToRotate.localRotation;
        Quaternion target = Quaternion.Euler(targetLocalEulerAngles);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t);
            objectToRotate.localRotation = Quaternion.Slerp(start, target, t);
            yield return null;
        }

        objectToRotate.localRotation = target;
    }
}
