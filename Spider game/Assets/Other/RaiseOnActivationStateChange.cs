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
    GameEvent eventToRaiseOnEnable;

    [SerializeField]
    GameObjectVariable variableToOverwriteWithThisGameObjectOnEnable;

    [Header("On Disable")]
    [SerializeField]
    GameEvent eventToRaiseOnDisable;

    [SerializeField]
    GameObjectVariable variableToOverwriteWithThisGameObjectOnDisable;

    private void OnEnable()
    {
        if (variableToOverwriteWithThisGameObjectOnEnable)
            variableToOverwriteWithThisGameObjectOnEnable.RuntimeValue = gameObject;

        if (eventToRaiseOnEnable != null)
            eventToRaiseOnEnable.Raise();
    }

    private void OnDisable()
    {
        if (variableToOverwriteWithThisGameObjectOnDisable)
            variableToOverwriteWithThisGameObjectOnDisable.RuntimeValue = gameObject;

        if (eventToRaiseOnDisable != null)
            eventToRaiseOnDisable.Raise();
    }
}
