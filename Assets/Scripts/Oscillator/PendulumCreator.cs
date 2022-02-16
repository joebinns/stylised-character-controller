using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumCreator : MonoBehaviour
{
    [SerializeField] private GameObject pendulumPrefab;

    public int numPendulums = 1;

    [SerializeField] private float angle = 20f;

    public float incrementalTime = 0f;

    public List<GameObject> pendulums = new List<GameObject>();

    [HideInInspector] public Vector3 displacement;

    [HideInInspector] public float popInDistance = 10f; 

    void Start()
    {
        t = new List<float>(new float[numPendulums]);
        prevLerp = new List<float>(new float[numPendulums]);

        bool incrementally = true;
        if (incrementalTime == 0f)
        {
            incrementally = false;
        }

        displacement = transform.position;
        if (incrementally)
        {
            displacement += Vector3.back * popInDistance;
        }
        for (int i = 0; i < numPendulums; i++)
        {
            displacement += Vector3.back * 3f;
            GameObject pendulum = Instantiate(pendulumPrefab, displacement, Quaternion.identity, transform);
            pendulums.Add(pendulum);
            if (incrementally)
            {
                pendulums[i].transform.localScale = Vector3.zero;
                //pendulum.GetComponent<Renderer>().enabled = false;
            }

            TorsionalOscillator torsionalOscillator = pendulum.GetComponent<TorsionalOscillator>();
            torsionalOscillator.stiffness *= (1f + 1f * i / numPendulums);

            torsionalOscillator.transform.localRotation = Quaternion.Euler(0, 0, angle);
        }

        /*
        if (incrementally)
        {
            StartCoroutine(ActivateIncrementally(pendulums, incrementalTime));
        }
        */
    }

    private List<float> t;
    private List<float> prevLerp;
    private void Update()
    {
        for (int i = 0; i < numPendulums; i++)
        {
            if (t[i] < incrementalTime)
            {
                if (i > 0)
                {
                    if (t[i - 1] >= incrementalTime)
                    {
                        t[i] += Time.deltaTime / incrementalTime;
                        if (t[i] >= incrementalTime)
                        {
                            t[i] = incrementalTime;
                        }

                        float currLerp = Easing.Elastic.Out(t[i]);
                        float deltaLerp = currLerp - prevLerp[i];
                        pendulums[i].transform.localScale += Vector3.one * deltaLerp * 2f;
                        pendulums[i].transform.position += Vector3.forward * deltaLerp * popInDistance;
                        prevLerp[i] = currLerp;
                    }
                }
                
                else // i == 0
                {
                    t[i] += Time.deltaTime / incrementalTime;
                    if (t[i] >= incrementalTime)
                    {
                        t[i] = incrementalTime;
                    }

                    float currLerp = Easing.Elastic.Out(t[i]);
                    float deltaLerp = currLerp - prevLerp[i];
                    pendulums[i].transform.localScale += Vector3.one * deltaLerp * 2f;
                    pendulums[i].transform.position += Vector3.forward * deltaLerp * popInDistance;
                    prevLerp[i] = currLerp;
                }

            }
        }
    }

    /*
    IEnumerator ActivateIncrementally(List<GameObject> pendulums, float delay)
    {
        for (int i = 0; i < numPendulums; i++)
        {
            //pendulums[i].GetComponent<Renderer>().enabled = true;
            yield return new WaitForSeconds(delay);
        }
    }
    */
}
