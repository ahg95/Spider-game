using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionPlotter : MonoBehaviour
{
    public Transform startPosition;
    public Transform endPosition;

    public GameObject marker;
    public int nrOfMarkers;

    GameObject[] markers;

    // Start is called before the first frame update
    void Start()
    {
        markers = new GameObject[nrOfMarkers];

        for (int i = 0; i < nrOfMarkers; i++)
        {
            markers[i] = Instantiate(marker);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
