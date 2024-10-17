using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CanonBehavior : MonoBehaviour
{
    public GameObject cannonballPrefab;
    public Transform firePoint;
    
    private List<Vector3> controlPoints = new();

    private readonly float lightFireRate = 0.25f;
    private float timeSinceLastLight= 0.0f;
    private readonly float heavyFireRate = 5f;
    private float timeSinceLastHeavy = 0.0f;
    private bool heavyFireActive = false;
    public TMPro.TextMeshProUGUI projectileInfo;
    private List<String> projectileInfoTexts = new();


    private void Start()
    {
        firePoint = transform;

        for (int i = 0; i < 3; i++)
        {
            controlPoints.Add(Vector3.zero);
        }
        controlPoints.Add(transform.position);

        projectileInfoTexts.Add("Munição Ativa: Leve");
        projectileInfoTexts.Add("Munição Pesada: Carregando");
        UpdateProjectileInfo();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            heavyFireActive = !heavyFireActive;

            if (heavyFireActive)
            {
                projectileInfoTexts[0] = "Munição: Pesada";
            }
            else
            {
                projectileInfoTexts[0] = "Munição: Leve";
            }

            UpdateProjectileInfo();
        }

        timeSinceLastLight += Time.deltaTime;
        timeSinceLastLight = Mathf.Min(timeSinceLastLight, lightFireRate + 0.01f);
        timeSinceLastHeavy += Time.deltaTime;
        timeSinceLastHeavy = Mathf.Min(timeSinceLastHeavy, heavyFireRate + 0.01f);
        if(timeSinceLastHeavy > heavyFireRate)
        {
            projectileInfoTexts[1] = "Munição Pesada: Pronta!";
            UpdateProjectileInfo();
        }

        if (Input.GetMouseButton((int)MouseButton.Right) && CanFire())
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(r, out RaycastHit hit) && (hit.collider.CompareTag("Static") || hit.collider.CompareTag("Enemy")))
            {
                for (int i = 0; i < controlPoints.Count - 1; i++)
                {
                    controlPoints[i] = Vector3.Lerp(hit.point, transform.position, (float)i / controlPoints.Count);
                    if (i != 0)
                        controlPoints[i] += Mathf.Abs((hit.point - transform.position).y) * Vector3.up;
                }

                GameObject cannonball = Instantiate(cannonballPrefab);
                cannonball.GetComponent<CannonballBehavior>().CalculateBezierPoints(controlPoints);
                cannonball.GetComponent<CannonballBehavior>().isHeavyType = heavyFireActive;

                Vector3 diff = hit.point - transform.position;
                transform.up = (diff + 0.25f * diff.magnitude * Vector3.up).normalized;

                if(heavyFireActive)
                {
                    timeSinceLastHeavy = 0f;
                    projectileInfoTexts[1] = "Munição Pesada: Carregando";
                    UpdateProjectileInfo();
                }
                else
                {
                    timeSinceLastLight = 0f;
                }
            }
        }
    }

    private bool CanFire()
    {
        return (heavyFireActive && timeSinceLastHeavy >= heavyFireRate) ||
            (!heavyFireActive && timeSinceLastLight >= lightFireRate);
    }

    private void UpdateProjectileInfo()
    {
        projectileInfo.text = "";

        foreach (string str in projectileInfoTexts)
        {
            projectileInfo.text += str + "\t\t";
        }
    }
}