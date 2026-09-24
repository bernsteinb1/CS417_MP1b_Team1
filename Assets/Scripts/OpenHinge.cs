using Unity.Mathematics;
using UnityEngine;

public class OpenHinge : MonoBehaviour
{
    private JointSpring s;
    private float velocity = 0;
    float prev_error = 0;
    public float acceleration;
    public float dampingForce;
    private AudioSource se;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        s = gameObject.GetComponent<HingeJoint>().spring;
        JointLimits l = gameObject.GetComponent<HingeJoint>().limits;
        l.min = -90f;
        gameObject.GetComponent<HingeJoint>().limits = l;
        se = gameObject.GetComponentInChildren<AudioSource>();
        if (se != null) se.Play();
    }

    void Update()
    {
        velocity += acceleration * (-90 - s.targetPosition) + dampingForce * (-s.targetPosition);
        velocity = math.min(velocity, -1);
        s.targetPosition += velocity * Time.deltaTime;

        if (s.targetPosition <= -88) {
            gameObject.GetComponent<HingeJoint>().useSpring = false;
            velocity = 0;
            if (se != null) se.Pause();
            enabled = false;
        }
        gameObject.GetComponent<HingeJoint>().spring = s;
    }
}
