using UnityEngine;

public class SimpleTwoButtonPuzzle : MonoBehaviour
{
    [Header("Sequence")]
    public int firstValue = 0;
    public int secondValue = 1;

    [Header("Key Reveal")]
    public GameObject keyObject;
    public Transform revealPoint;

    private int step;
    private bool solved;

    public bool IsSolved => solved;

    public void Press(int value)
    {
        if (solved) return;

        int expected = step == 0 ? firstValue : secondValue;
        if (value != expected)
        {
            step = value == firstValue ? 1 : 0;
            return;
        }

        step++;
        if (step < 2) return;

        solved = true;
        RevealKey();
    }

    private void RevealKey()
    {
        if (keyObject == null) return;

        keyObject.SetActive(true);
        keyObject.transform.SetParent(null, true);
        if (revealPoint != null)
        {
            keyObject.transform.position = revealPoint.position;
            keyObject.transform.rotation = revealPoint.rotation;
        }

        Rigidbody rb = keyObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
