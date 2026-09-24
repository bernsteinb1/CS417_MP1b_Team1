using UnityEngine;

public class KeyIdentity : MonoBehaviour
{
    public enum KeyType
    {
        IDCard,
        USBDrive,
        OverrideToken
    }

    public KeyType keyType;
}