using UnityEngine;

public class OpenHinge : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        JointLimits l = gameObject.GetComponent<HingeJoint>().limits;
        l.min = -90f;
        gameObject.GetComponent<HingeJoint>().limits = l;
    }
}
