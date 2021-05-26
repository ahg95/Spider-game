using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateableConfigurableJoint : MonoBehaviour
{
    public ConfigurableJoint jointToInstantiate;

    private ConfigurableJoint instantiatedJoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {

        LoadJointValues();
    }

    private void LoadJointValues()
    {

    }

    public void Deactivate()
    {
        SaveJointValues();

        Destroy(this);
    }

    private void SaveJointValues()
    {

    }

    T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy as T;
    }
}
