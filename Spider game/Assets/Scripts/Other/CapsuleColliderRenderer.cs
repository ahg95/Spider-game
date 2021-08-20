using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component renders a capsule collider it is attached to.
/// </summary>
[RequireComponent(typeof(CapsuleCollider))]
public class CapsuleColliderRenderer : MonoBehaviour
{
    public Transform topSphere;
    public Transform cylinder;
    public Transform bottomSphere;

    CapsuleCollider capsuleCollider;

    CapsuleCollider GetCapsuleCollider()
    {
        if (!capsuleCollider)
            capsuleCollider = GetComponent<CapsuleCollider>();

        return capsuleCollider;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTransformsToMatchCapsuleDimensions();
    }

    void UpdateTransformsToMatchCapsuleDimensions()
    {
        ScaleTransformsToMatchCapsuleDimensions();

        PositionTransformsToMatchCapsuleDimensions();
    }

    void ScaleTransformsToMatchCapsuleDimensions()
    {
        topSphere.localScale = Vector3.one * GetCapsuleCollider().radius * 2;
        bottomSphere.localScale = Vector3.one * GetCapsuleCollider().radius * 2;

        float scaleY = GetCapsuleCollider().height - 2 * GetCapsuleCollider().radius;
        // The default height of a cylinder is 2 units. Therefore, the cylinder also has to be scaled down in height by half.
        scaleY *= 0.5f;
        float scaleXZ = GetCapsuleCollider().radius * 2;

        cylinder.localScale = new Vector3(scaleXZ, scaleY, scaleXZ);
    }

    void PositionTransformsToMatchCapsuleDimensions()
    {
        float sphereDistanceFromOrigin = (GetCapsuleCollider().height - 2 * GetCapsuleCollider().radius) / 2;

        topSphere.localPosition = Vector3.up * sphereDistanceFromOrigin;

        bottomSphere.localPosition = Vector3.down * sphereDistanceFromOrigin;

        cylinder.localPosition = Vector3.zero;
    }
}
