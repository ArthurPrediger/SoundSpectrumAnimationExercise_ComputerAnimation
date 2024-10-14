using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private GameObject spawnArea;
    [SerializeField]
    private GameObject enemyTarget;

    private AudioSource audioSource;
    private float[] spectrumSamples = new float[256];
    private float[] lastSpectrumSamples = new float[256];

    private readonly float spawnRate = 2.0f;
    private float spawnDelta = 0.0f;

    private float lastAverageFreq = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = FindObjectOfType<AudioSource>();

        for (int i = 0; i < lastSpectrumSamples.Length; i++)
            lastSpectrumSamples[i] = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.GetSpectrumData(spectrumSamples, 0, FFTWindow.Hamming);

        float maxAmpDiff = 0.0f;

        for (int i = 0; i < spectrumSamples.Length; i++)
        {
            float ampDiff = Mathf.Abs(spectrumSamples[i] - lastSpectrumSamples[i]);

            if (ampDiff > maxAmpDiff)
            {
                maxAmpDiff = ampDiff;
            }
        }

        System.Array.Copy(spectrumSamples, lastSpectrumSamples, spectrumSamples.Length);

        maxAmpDiff = Mathf.Pow(maxAmpDiff, 4.0f);
        maxAmpDiff *= Mathf.Pow(230.0f, 4.0f);
        spawnDelta += maxAmpDiff * Time.deltaTime;

        int numEnemies = 0;
        while (spawnDelta >= spawnRate && numEnemies++ < 32)
        {
            spawnDelta -= spawnRate;
            GameObject newEnemy = Instantiate(enemyPrefab, GenerateRandomPosWithinSpawnArea(), Quaternion.identity);
        }

        if (numEnemies > 0)
        {
            spawnDelta = 0.0f;
        }

        //float ampSum = 0.0f;
        //foreach (var amp in spectrumSamples)
        //{
        //    ampSum += amp;
        //}

        //int sampleRate = AudioSettings.outputSampleRate;
        //int fftSize = spectrumSamples.Length;

        //float oneOverAmpSum = (1.0f / ampSum);
        //float avgFrequency = 0.0f;
        //for (int i = 0; i < spectrumSamples.Length; i++)
        //{
        //    float frequency = (i / (float)spectrumSamples.Length) * (sampleRate / 2);
        //    avgFrequency += frequency * spectrumSamples[i] * oneOverAmpSum; 
        //}

        //float avgFreqDiff = lastAverageFreq - avgFrequency;
        //lastAverageFreq = avgFrequency;
    }

    Vector3 GenerateRandomPosWithinSpawnArea()
    {
        Bounds areaBounds = spawnArea.GetComponent<Renderer>().bounds;
        Bounds prefabBounds = enemyPrefab.GetComponent<Renderer>().bounds;

        float offsetX = prefabBounds.extents.x;
        float offsetZ = prefabBounds.extents.z;
        float yPos = prefabBounds.extents.y;

        float minX = areaBounds.min.x + offsetX;
        float maxX = areaBounds.max.x - offsetX;
        float minZ = areaBounds.min.z + offsetZ;
        float maxZ = areaBounds.max.z - offsetZ;

        float randomX = Random.Range(minX, maxX);
        float randomZ = Random.Range(minZ, maxZ);

        return new Vector3(randomX, yPos, randomZ);
    }
}
