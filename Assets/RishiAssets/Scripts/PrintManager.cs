using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PrintManager : MonoBehaviour {
	[SerializeField] List<GameObject> sheets;

	[SerializeField] AudioSource printerAudioSource;
	[SerializeField] AudioClip printerClip;
	[SerializeField] GameObject printArea;
	[SerializeField] TextMeshProUGUI computerText;

	public void keyPress(SelectEnterEventArgs args) {
		Debug.Log("Now Printing...");
		printerAudioSource.PlayOneShot(printerClip);
		computerText.text = "Printing...";
		Sleep(2);
		Instantiate(sheets[Random.Range(0, sheets.Count)], printArea.transform.position, printArea.transform.rotation);
		computerText.text = "PRESS ANY KEY\nTO PRINT";

	}

	IEnumerator Sleep(float seconds) {
		yield return new WaitForSeconds(seconds);
	}
}
