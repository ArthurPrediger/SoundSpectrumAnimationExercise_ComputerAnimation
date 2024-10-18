using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private AudioSource audioSource;
    private int infiltrators = 0;
    public TMPro.TextMeshProUGUI infiltratorsInfo;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = FindObjectOfType<AudioSource>();
        UpdateInfiltratorsInfo();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                Destroy(enemy);
            };

            infiltrators = 0;
            UpdateInfiltratorsInfo();

            audioSource.Play();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void IncreaseInfiltrators()
    {
        infiltrators++;
        UpdateInfiltratorsInfo();
    }

    private void UpdateInfiltratorsInfo()
    {
        infiltratorsInfo.text = "Infiltrados: " + infiltrators.ToString();
    }
}
