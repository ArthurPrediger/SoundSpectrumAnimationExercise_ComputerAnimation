using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBehavior : MonoBehaviour
{
    private float explosionSpeed = 20f;
    private float explosionScale = 14f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.localScale.x < explosionScale)
        {
            transform.localScale += explosionSpeed * Time.deltaTime * Vector3.one;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
        }
    }

    public void SetExplosionSpeedScale(float speed, float scale)
    {
        explosionSpeed = speed;
        explosionScale = scale;
    }
}
