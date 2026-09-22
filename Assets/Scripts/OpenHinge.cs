using UnityEngine;

public class OpenHinge : MonoBehaviour
{
    private JointSpring s;
    private float velocity = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        s = gameObject.GetComponent<HingeJoint>().spring;
        JointLimits l = gameObject.GetComponent<HingeJoint>().limits;
        l.min = -90f;
        gameObject.GetComponent<HingeJoint>().limits = l;
    }

    void Update()
    {
        s.targetPosition += velocity * Time.deltaTime;
        velocity -= 50 * Time.deltaTime; // acceleration term
        velocity -= 120 * s.targetPosition / 90 * Time.deltaTime; // dampening term
        if (s.targetPosition <= -90) {
            gameObject.GetComponent<HingeJoint>().useSpring = false;
            enabled = false;
        }
        gameObject.GetComponent<HingeJoint>().spring = s;
    }
}
