using System;
using UnityEngine;

public class teleportTobeanbag : MonoBehaviour {
	[SerializeField] GameObject beanBag;
	[SerializeField] GameObject vrRig;
	public void TeleportToBeanBag() {
		vrRig.transform.position = beanBag.transform.position;
		vrRig.transform.rotation = beanBag.transform.rotation;
	}
}
