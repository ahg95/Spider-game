using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Raises the respective <c>gameEvent</c> on enabling or disabling and, if specified, writes this gameObject into the respective <c>gameVariable</c>.
/// </summary>
public class RaiseOnActivationStateChange : MonoBehaviour
{
    [Header("On Enable")]
    [SerializeField]
    GameEvent wasEnabled;

    [SerializeField]
    GameObjectVariable setOnEnable;

    [Header("On Disable")]
    [SerializeField]
    GameEvent wasDisabled;

    [SerializeField]
    GameObjectVariable setOnDisable;

    private void OnEnable()
    {
        if (setOnEnable)
            setOnEnable.RuntimeValue = gameObject;

        if (wasEnabled != null)
            wasEnabled.Raise();
    }

    private void OnDisable()
    {
        if (setOnDisable)
            setOnDisable.RuntimeValue = gameObject;

        if (wasDisabled != null)
            wasDisabled.Raise();
    }
}
