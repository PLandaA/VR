using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenDoor : MonoBehaviour
{
    public Text uiText;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Key")
        {
            if (other.gameObject.name == "RedKey")
            {
                GameManager.doorOpen = true;
            }
            else
            {
                uiText.text = "Necesitas una llave roja";
            }
        }
        else if (other.gameObject.tag == "Player")
        {
            uiText.text = "Necesitas una llave para salir";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        StartCoroutine(WaitForDelete());
    }

    IEnumerator WaitForDelete()
    {
        yield return new WaitForSeconds(3.5f);
        uiText.text = "";
    }
}
