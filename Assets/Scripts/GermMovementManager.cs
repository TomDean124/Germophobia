using UnityEngine;
using System.Collections.Generic;

public class GermMovementManager : MonoBehaviour
{
    public GameObject player;

    public float moveToCenterStrength;
    public float distanceToStartCenterCalculations;
    public float staynearRadius;

    public float AvoidStrength;
    public float avoidDistanceRequirement;

    public float alignmentdist;

    public List<Boid> TotalGermCount = new List<Boid>();

    public Vector2 target;
    public bool noTarget = true;

    public bool enabled = true;

    void Update()
    {
        if (!enabled) return;

        // Clean out destroyed boids before doing anything
        TotalGermCount.RemoveAll(boid => boid == null);

        if (TotalGermCount.Count == 0) return;

        if (noTarget && player != null)
        {
            target = player.transform.position;
        }

        MoveToCenter();
        AvoidOtherBoids();
        Align();

        
    }

    public void DisableFlocking()
    {
    enabled = false;
    }

    public void MoveToCenter()
    {
        Vector2 positionSum = Vector2.zero;
        int count = 0;

        foreach (Boid boid in TotalGermCount)
        {
            if (boid == null) continue;

            float distance = Vector2.Distance(boid.transform.position, target);
            if (distance < distanceToStartCenterCalculations)
            {
                positionSum += (Vector2)boid.transform.position;
                count++;
            }
        }

        if (count == 0) return;

        Vector2 positionAvr = positionSum / count;

        foreach (Boid boid in TotalGermCount)
        {
            if (boid == null) continue;

            float distanceToPlayer = Vector2.Distance(boid.transform.position, target);
            if (distanceToPlayer > staynearRadius)
            {
                Vector2 directionToPlayer = (target - (Vector2)boid.transform.position).normalized;
                Vector2 deltaTimeStrength = directionToPlayer * moveToCenterStrength * Time.deltaTime;
                boid.ApplyForce(deltaTimeStrength);
            }
        }
    }

    public void AvoidOtherBoids()
    {
        foreach (Boid boid in TotalGermCount)
        {
            if (boid == null) continue;

            Vector2 SeperationForce = Vector2.zero;
            int nearbyBoids = 0;

            foreach (Boid otherBoid in TotalGermCount)
            {
                if (otherBoid == null || boid == otherBoid) continue;

                float distance = Vector2.Distance(boid.transform.position, otherBoid.transform.position);
                if (distance < avoidDistanceRequirement && distance > 0)
                {
                    Vector2 faceAwayDirection = ((Vector2)boid.transform.position - (Vector2)otherBoid.transform.position).normalized;
                    SeperationForce += faceAwayDirection / distance;
                    nearbyBoids++;
                }
            }

            if (nearbyBoids > 0)
            {
                SeperationForce /= nearbyBoids;
                SeperationForce = SeperationForce.normalized * AvoidStrength * Time.deltaTime;
                boid.ApplyForce(SeperationForce);
            }
        }
    }

    public void Align()
    {
        foreach (Boid boid in TotalGermCount)
        {
            if (boid == null) continue;

            Vector2 avrVelocity = Vector2.zero;
            int nearbyBoids = 0;

            foreach (Boid otherBoid in TotalGermCount)
            {
                if (otherBoid == null || boid == otherBoid) continue;

                float distance = Vector2.Distance(boid.transform.position, otherBoid.transform.position);
                if (distance < alignmentdist)
                {
                    avrVelocity += otherBoid.velocity;
                    nearbyBoids++;
                }
            }

            if (nearbyBoids > 0)
            {
                avrVelocity /= nearbyBoids;
                Vector2 alignmentForce = (avrVelocity - boid.velocity).normalized;
                boid.ApplyForce(alignmentForce);
            }
        }
    }
}
