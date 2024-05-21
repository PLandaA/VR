using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    static public float timeFlashLight = 100;
    static public bool doorOpen = false;
    public GameObject menu, creditos;
    public Material flashlightIndicator;
    public Light flashLightIntensity;
    public Text uiText;
    private bool bCreditos = false;
    static public bool game = false;
    void Start()
    {
        if (creditos)
        {
            bCreditos = false;
        }
        if (uiText)
        {
            uiText.text = "";
        }
        flashlightIndicator.EnableKeyword("_EMISSION");
        timeFlashLight = 120;
    }

    void Update()
    {
        if (game)
        {
            if (timeFlashLight > 0 && Flashlight_PRO.is_enabled)
            {
                timeFlashLight -= Time.deltaTime;
            }

            if (timeFlashLight > 50)
            {
                flashLightIntensity.GetComponent<Light>().intensity = 5;
                flashlightIndicator.SetColor("_EmissionColor", Color.green);
            }
            else if (timeFlashLight > 10)
            {
                flashLightIntensity.GetComponent<Light>().intensity = 2.5f;
                flashlightIndicator.SetColor("_EmissionColor", Color.yellow);
            }
            else
            {
                flashLightIntensity.GetComponent<Light>().intensity = .5f;
                flashlightIndicator.SetColor("_EmissionColor", Color.red);
            }

            if (doorOpen)
            {
                SceneManager.LoadScene(0);
                game = false;
            }
        }
    }
    public void NewGame()
    {
        game = true;
    }
    public void Creditos()
    {
        if (!bCreditos)
        {
            menu.SetActive(false);
            creditos.SetActive(true);
            bCreditos = true;
        }
        else
        {
            menu.SetActive(true);
            creditos.SetActive(false);
            bCreditos = false;
        }
    }
    public void Salir()
    {
        Application.Quit();
    }
}
