using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Mover : MonoBehaviour
{
    public Vector3 movement;
    public Vector3 speed;
    public float waitingTime;


    Vector3 initialPosition;
    Vector3 targetPosition;
    bool moveTowardsTargetPosition = true;
    bool isPausing = false;

    private void OnEnable()
    {
        initialPosition = transform.position;
        targetPosition = initialPosition + movement;
    }

    new Rigidbody rigidbody;

    Rigidbody GetRigidbody()
    {
        if (!rigidbody)
            rigidbody = GetComponent<Rigidbody>();

        return rigidbody;
    }


    private void FixedUpdate()
    {
        if (!isPausing)
        {
            if (moveTowardsTargetPosition)
            {
                MoveTowards(targetPosition);

                if (transform.position == targetPosition)
                {
                    moveTowardsTargetPosition = false;
                    StartCoroutine("Pause");
                }
            } else
            {
                MoveTowards(initialPosition);

                if (transform.position == initialPosition)
                {
                    moveTowardsTargetPosition = true;
                    StartCoroutine("Pause");
                }
            }
        }
    }

    IEnumerator Pause()
    {
        isPausing = true;
        yield return new WaitForSeconds(waitingTime);
        isPausing = false;
    }

    void MoveTowards(Vector3 position)
    {
        Vector3 offset = position - transform.position;

        Vector3 nextPosition = transform.position + ClampVector(offset, -speed * Time.fixedDeltaTime, speed * Time.fixedDeltaTime);

        GetRigidbody().MovePosition(nextPosition);
    }

    Vector3 ClampVector(Vector3 vectorToClamp, Vector3 lowerBounds, Vector3 upperBounds)
    {
        Vector3 clampedVector;

        clampedVector.x = Mathf.Clamp(vectorToClamp.x, lowerBounds.x, upperBounds.x);
        clampedVector.y = Mathf.Clamp(vectorToClamp.y, lowerBounds.y, upperBounds.y);
        clampedVector.z = Mathf.Clamp(vectorToClamp.z, lowerBounds.z, upperBounds.z);

        return clampedVector;
    }
}
