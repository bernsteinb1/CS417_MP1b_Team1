using UnityEngine;

public class FlipSwitch : MonoBehaviour
{
    public Transform s;
    public Light l;

    public void Flip()
    {
        s.rotation = Quaternion.identity;
        l.color = new(0, 1f, 0);
    }
}
