using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HatProjectile : MonoBehaviour
{
    public float speed = 15f;
    public GameObject hit;
    public GameObject flash;
    public GameObject[] Detached;
    private Vector3 attackPos;
    private Vector3 targetOffset;

    [Space]
    [Header("PROJECTILE PATH")]
    private float randomUpAngle;
    private float randomSideAngle;
    public float sideAngle = 25;
    public float upAngle = 20;

    void Start()
    {
        FlashEffect();
        NewRandom();
    }

    private void NewRandom()
    {
        randomUpAngle = Random.Range(0, upAngle);
        randomSideAngle = Random.Range(-sideAngle, sideAngle);
    }

    private Vector3 MakeRandomOffset()
    {
        float randOffsetX = Random.Range(-0.5f, 0.5f);
        float randOffsetY = Random.Range(-0.5f, 0.5f);
        float randOffsetZ = Random.Range(-0.5f, 0.5f);
        return new Vector3(randOffsetX, randOffsetY, randOffsetZ);
    }

    //Link from movement controller
    //TARGET POSITION + TARGET OFFSET
    public void UpdateTarget(Transform targetPosition , Vector3 Offset)
    {
        attackPos = targetPosition.position + MakeRandomOffset();
        targetOffset = Offset;
    }

    void Update()
    {
        Vector3 forward = ((attackPos + targetOffset) - transform.position);
        Vector3 crossDirection = Vector3.Cross(forward, Vector3.up);
        Quaternion randomDeltaRotation = Quaternion.Euler(0, randomSideAngle, 0) * Quaternion.AngleAxis(randomUpAngle, crossDirection);
        Vector3 direction = randomDeltaRotation * ((attackPos + targetOffset) - transform.position);

        float distanceThisFrame = Time.deltaTime * speed;

        if (direction.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void FlashEffect()
    {
        if (flash != null)
        {
            var flashInstance = Instantiate(flash, transform.position, Quaternion.identity);
            flashInstance.transform.forward = gameObject.transform.forward;
            var flashPs = flashInstance.GetComponent<ParticleSystem>();
            if (flashPs != null)
            {
                Destroy(flashInstance, flashPs.main.duration);
            }
            else
            {
                var flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(flashInstance, flashPsParts.main.duration);
            }
        }
    }

    void HitTarget()
    {
        if (hit != null)
        {
            var hitInstance = Instantiate(hit, attackPos + targetOffset, transform.rotation);
            var hitPs = hitInstance.GetComponent<ParticleSystem>();
            if (hitPs != null)
            {
                Destroy(hitInstance, hitPs.main.duration);
            }
            else
            {
                var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitInstance, hitPsParts.main.duration);
            }
        }
        foreach (var detachedPrefab in Detached)
        {
            if (detachedPrefab != null)
            {
                detachedPrefab.transform.parent = null;
            }
        }
        Destroy(gameObject);
    }
}
