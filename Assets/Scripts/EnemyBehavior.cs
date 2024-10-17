using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    private GameObject target;
    private Vector3 targetPos;
    private Rigidbody rb;
    private float speed = 12.0f;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Finish");
        targetPos = GenerateRandomPosWithinTargetArea();

        rb = GetComponent<Rigidbody>();

        Vector3 randomVec = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized;
        Vector3 startImpulseForce = 10f * randomVec + 3f * Vector3.up;
        rb.AddForce(startImpulseForce, ForceMode.VelocityChange);

        rb.maxLinearVelocity = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < -10.0f)
        {
            Destroy(gameObject);
        }

        rb.velocity += speed * Time.deltaTime * (targetPos - transform.position).normalized;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Finish"))
        {
            Destroy(gameObject);
        }
    }

    Vector3 GenerateRandomPosWithinTargetArea()
    {
        Bounds areaBounds = target.GetComponent<Renderer>().bounds;
        Bounds enemyBounds = GetComponent<Renderer>().bounds;

        float yPos = enemyBounds.extents.y;

        float minX = areaBounds.min.x + enemyBounds.extents.x;
        float maxX = areaBounds.max.x - enemyBounds.extents.x;
        float minZ = areaBounds.min.z + enemyBounds.extents.z;
        float maxZ = areaBounds.max.z - enemyBounds.extents.z;

        float randomX = Random.Range(minX, maxX);
        float randomZ = Random.Range(minZ, maxZ);

        return new Vector3(randomX, yPos, randomZ);
    }
}
