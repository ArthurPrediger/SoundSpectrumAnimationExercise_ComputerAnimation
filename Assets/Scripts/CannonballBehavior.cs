using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CannonballBehavior : MonoBehaviour
{
    public GameObject explosionPrefab;
    public bool isHeavyType = false;
    private float speed = 1.0f;
    private List<Vector3> bezierPoints = new List<Vector3>();
    private const int numBezierPoints = 100;
    private double bezierCurveLength;
    private float tParam = 0.0f;

    private float lightExplosionSpeed = 16f;
    private float lightExplosionScale = 8f;
    private float heavyExplosionSpeed = 32f;
    private float heavyExplosionScale = 22f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (tParam < 1.0f && bezierPoints.Count == numBezierPoints)
        {
            tParam += Time.deltaTime * speed;
            tParam = Mathf.Clamp01(tParam);

            //float easeT = EaseOutQuart(tParam);

            PathInfo pathInfo = GetPathInfoAt(tParam);
            gameObject.transform.position = pathInfo.pos;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Static") || collision.collider.CompareTag("Enemy"))
        {
            GameObject explosion = Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity);
            if(isHeavyType)
            {
                explosion.GetComponent<ExplosionBehavior>().SetExplosionSpeedScale(heavyExplosionSpeed, heavyExplosionScale);
                explosion.GetComponent<Renderer>().material.color = Color.black;
            }
            else
            {
                explosion.GetComponent<ExplosionBehavior>().SetExplosionSpeedScale(lightExplosionSpeed, lightExplosionScale);
            }

            Destroy(gameObject);
        }
    }

    public void CalculateBezierPoints(List<Vector3> controlPoints)
    {
        speed = (128f / Mathf.Max((controlPoints.First() - controlPoints.Last()).magnitude, 16f));
        float t = 0.0f;
        for (int i = 0; i < numBezierPoints; i++)
        {
            bezierPoints.Add(CalculateBezierPoint(t,
                controlPoints[3],
                controlPoints[2],
                controlPoints[1],
                controlPoints[0]));

            t += (float)(1) / (float)(numBezierPoints - 1);
        }

        bezierCurveLength = 0.0f;
        for (int i = 0; i < bezierPoints.Count - 1; i++)
        {
            bezierCurveLength += (bezierPoints[i + 1] - bezierPoints[i]).magnitude;
        }
    }

    private class PathInfo
    {
        public Vector3 pos;
        public Vector3 dir;
    }

    private PathInfo GetPathInfoAt(float t)
    {
        double distFromStart = bezierCurveLength * t;

        int i;
        for (i = 0; i < bezierPoints.Count - 1 && distFromStart > 0.0f; i++)
        {
            distFromStart -= (bezierPoints[i + 1] - bezierPoints[i]).magnitude;
        }

        PathInfo pathInfo = new PathInfo();
        Vector3 lastDir = gameObject.transform.forward;
        if (i > 0)
        {
            lastDir = (bezierPoints[i] - bezierPoints[i - 1]);
            pathInfo.dir = lastDir.normalized;
        }
        pathInfo.pos = bezierPoints[i] + ((float)(distFromStart / lastDir.magnitude) * lastDir);
        return pathInfo;
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3)
    {
        // Parametric form of cubic Bezier
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 point = uuu * P0; // (1 - t)^3 * P0
        point += 3 * uu * t * P1; // 3 * (1 - t)^2 * t * P1
        point += 3 * u * tt * P2; // 3 * (1 - t) * t^2 * P2
        point += ttt * P3;        // t^3 * P3

        return point;
    }
}
