using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingScript : MonoBehaviour
{
    public GameObject objectToCheckActivationOf;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckActivation()
    {
        Debug.Log("The gameObject " + objectToCheckActivationOf.name + " has an activity status of " + objectToCheckActivationOf.activeInHierarchy);
    }
}
