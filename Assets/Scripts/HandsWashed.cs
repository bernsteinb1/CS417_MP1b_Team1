using UnityEngine;

public class HandsWashed : MonoBehaviour
{
     // Occurs the exact frame another collider enters the trigger
    private bool firstTime = false;

    private void OnTriggerEnter(Collider other)
    {
        if (firstTime) return;
        GameObject pref = Resources.Load<GameObject>("RedKey");
        Instantiate(pref, new Vector3(-3.81509995f,1.07589996f,3.6078999f), Quaternion.identity);
        firstTime = false;
    }

}
