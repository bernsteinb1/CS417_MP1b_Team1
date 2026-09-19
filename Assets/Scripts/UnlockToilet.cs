using UnityEngine;

public class UnlockToilet : MonoBehaviour
{
    public HingeJoint h;
    public void OpenToilet()
    {
        JointLimits jl = h.limits;
        jl.min = -90;
        h.limits = jl;
    }
}
