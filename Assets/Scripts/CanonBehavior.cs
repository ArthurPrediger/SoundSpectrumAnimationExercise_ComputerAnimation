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

    private void Start()
    {
        firePoint = transform;

        for (int i = 0; i < 3; i++)
        {
            controlPoints.Add(Vector3.zero);
        }
        controlPoints.Add(transform.position);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown((int)MouseButton.Right))
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
            }
        }
    }
}