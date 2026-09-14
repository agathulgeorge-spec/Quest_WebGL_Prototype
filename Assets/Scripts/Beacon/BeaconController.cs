using System.Collections.Generic;
using UnityEngine;

public class BeaconController : MonoBehaviour
{
    [Header("Ray Origin")]
    public Transform rayOrigin;

    [Header("Light Ray")]
    public float rayMaxDistance = 12f;
    public float coreRayWidth = 0.04f;
    public float glowRayWidth = 0.22f;
    public Color coreRayColor = Color.white;
    public Color glowRayColor = new Color(1f, 0.95f, 0.55f, 0.35f);

    [Header("Beacon Visual")]
    public Color inactiveBeaconColor = Color.white;
    public Color activeBeaconColor = Color.white;
    public float activeBrightness = 2f;

    [Header("Feel")]
    public float startAimAngle = 190f;
    public float aimRotateSpeed = 90f;
    public float lightFadeTime = 1.25f;
    public float lightReachSmoothSpeed = 12f;

    [Header("Reflection")]
    public int maxReflections = 5;
    public float reflectionOffset = 0.12f;

    [Header("Layers")]
    public LayerMask reflectiveLayer;
    public LayerMask obstacleLayer;
    public LayerMask solidLayer;
    public LayerMask jumpOrbLayer;

    [Header("PC Beacon Control Note")]
    public BeaconControlNote beaconControlNote;

    private LineRenderer coreRay;
    private LineRenderer glowRay;

    private bool heroInRange;
    private bool isActive;

    // Prevents mobile E being triggered every frame
    private bool previousEState = false;

    private float lightFadeTimer;
    private float aimAngle;
    private Vector2 aimDirection;

    private List<Vector3> currentVisualPoints =
        new List<Vector3>();

    private List<Vector3> targetRayPoints =
        new List<Vector3>();

    private SpriteRenderer beaconRenderer;

    // Orbs currently being lit by this beacon
    private HashSet<JumpOrb> litOrbs =
        new HashSet<JumpOrb>();


    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        CreateLightRays();
        SetupBeaconVisual();

        if (rayOrigin == null)
        {
            rayOrigin = transform;
        }

        aimAngle = startAimAngle;
        aimDirection = AngleToDirection(aimAngle);

        currentVisualPoints.Clear();

        currentVisualPoints.Add(rayOrigin.position);
        currentVisualPoints.Add(rayOrigin.position);
    }


    // =========================================================
    // UPDATE
    // =========================================================

    void Update()
    {
        UpdateLightFade();

        // PC + Mobile interaction
        CheckEInteraction();

        // Remove this beacon's light from old orbs
        ClearPreviousOrbHits();

        UpdateRay();
        UpdateBeaconVisual();
    }


    // =========================================================
    // PC + MOBILE INTERACTION
    // =========================================================

    void CheckEInteraction()
    {
        // If hero is not near beacon,
        // reset the previous state.
        if (!heroInRange)
        {
            previousEState = false;
            return;
        }

        // PC keyboard OR mobile combined joystick/interact button
        bool currentEState =
            Input.GetKey(KeyCode.E) ||
            MobileInput.eHeld;

        // Trigger only once when the button is initially pressed.
        if (currentEState && !previousEState)
        {
            Interact();
        }

        previousEState = currentEState;
    }


    // =========================================================
    // MOBILE + UNIVERSAL INTERACTION
    // =========================================================
    // This method can also be called directly by a UI button
    // if needed.

    public void Interact()
    {
        if (!heroInRange)
        {
            return;
        }

        // Activate beacon
        lightFadeTimer = lightFadeTime;

        // Show control note
        if (beaconControlNote != null)
        {
            beaconControlNote.ShowNote();
        }
        else
        {
            Debug.LogWarning(
                "BeaconControlNote is NOT assigned on " +
                gameObject.name
            );
        }
    }


    // =========================================================
    // OPTIONAL DIRECT MOBILE METHOD
    // =========================================================
    // You don't need to assign this if MobileInput is being used.
    // It is here as a backup if you ever want the UI button
    // to call the beacon directly.

    public void MobileInteract()
    {
        Interact();
    }


    // =========================================================
    // ANGLE TO DIRECTION
    // =========================================================

    Vector2 AngleToDirection(float angle)
    {
        return new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ).normalized;
    }


    // =========================================================
    // CREATE LIGHT RAYS
    // =========================================================

    void CreateLightRays()
    {
        glowRay = CreateRay(
            "SoftGlowRay",
            glowRayWidth,
            glowRayColor,
            0
        );

        coreRay = CreateRay(
            "CoreLightRay",
            coreRayWidth,
            coreRayColor,
            1
        );
    }


    LineRenderer CreateRay(
        string rayName,
        float width,
        Color color,
        int sortingOrder
    )
    {
        GameObject rayObject =
            new GameObject(rayName);

        rayObject.transform.SetParent(transform);

        LineRenderer lr =
            rayObject.AddComponent<LineRenderer>();

        lr.material =
            new Material(
                Shader.Find("Sprites/Default")
            );

        lr.startWidth = width;
        lr.endWidth = width;

        lr.startColor = color;
        lr.endColor = color;

        lr.positionCount = 2;

        lr.numCapVertices = 12;
        lr.numCornerVertices = 12;

        lr.sortingOrder = sortingOrder;

        lr.enabled = false;

        return lr;
    }


    // =========================================================
    // BEACON VISUAL
    // =========================================================

    void SetupBeaconVisual()
    {
        beaconRenderer =
            GetComponent<SpriteRenderer>();

        if (beaconRenderer == null)
        {
            beaconRenderer =
                gameObject.AddComponent<SpriteRenderer>();
        }

        beaconRenderer.color =
            inactiveBeaconColor;

        if (GetComponent<Collider2D>() == null)
        {
            CircleCollider2D col =
                gameObject.AddComponent<CircleCollider2D>();

            col.isTrigger = true;
            col.radius = 0.5f;
        }
    }


    // =========================================================
    // HERO CONTROL
    // =========================================================

    public void SetHeroControl(
        bool inRange,
        Transform hero
    )
    {
        heroInRange = inRange;
    }


    // =========================================================
    // HERO INPUT
    // =========================================================
    // Compatible with your existing Player script.
    //
    // directionInput comes from your mobile beacon joystick
    // through MobileInput.

    public void HandleHeroInput(
        bool fPressed,
        bool gPressed,
        Vector2 directionInput,
        Vector2 mouseDirection = default
    )
    {
        if (!heroInRange)
        {
            return;
        }


        // =====================================================
        // BEACON DIRECTION
        // =====================================================

        if (directionInput.magnitude > 0.1f)
        {
            float targetAngle =
                Mathf.Atan2(
                    directionInput.y,
                    directionInput.x
                ) * Mathf.Rad2Deg;

            aimAngle =
                Mathf.MoveTowardsAngle(
                    aimAngle,
                    targetAngle,
                    aimRotateSpeed *
                    Time.deltaTime
                );

            aimDirection =
                AngleToDirection(aimAngle);
        }
    }


    // =========================================================
    // LIGHT FADE
    // =========================================================

    void UpdateLightFade()
    {
        if (lightFadeTimer > 0f)
        {
            lightFadeTimer -= Time.deltaTime;
            isActive = true;
        }
        else
        {
            lightFadeTimer = 0f;
            isActive = false;
        }
    }


    // =========================================================
    // CLEAR OLD ORB HITS
    // =========================================================

    void ClearPreviousOrbHits()
    {
        foreach (JumpOrb orb in litOrbs)
        {
            if (orb != null)
            {
                orb.RemoveBeacon(this);
            }
        }

        litOrbs.Clear();
    }


    // =========================================================
    // UPDATE RAY
    // =========================================================

    void UpdateRay()
    {
        if (!isActive)
        {
            coreRay.enabled = false;
            glowRay.enabled = false;

            return;
        }

        coreRay.enabled = true;
        glowRay.enabled = true;

        float alpha =
            Mathf.Clamp01(
                lightFadeTimer /
                lightFadeTime
            );

        Color coreColor = coreRayColor;
        coreColor.a *= alpha;

        Color glowColor = glowRayColor;
        glowColor.a *= alpha;

        coreRay.startColor = coreColor;
        coreRay.endColor = coreColor;

        glowRay.startColor = glowColor;
        glowRay.endColor = glowColor;

        targetRayPoints =
            BuildRayPath();

        SmoothRayVisual();

        coreRay.positionCount =
            currentVisualPoints.Count;

        glowRay.positionCount =
            currentVisualPoints.Count;

        coreRay.SetPositions(
            currentVisualPoints.ToArray()
        );

        glowRay.SetPositions(
            currentVisualPoints.ToArray()
        );
    }


    // =========================================================
    // BUILD RAY PATH
    // =========================================================

    List<Vector3> BuildRayPath()
    {
        List<Vector3> points =
            new List<Vector3>();

        Vector2 currentPosition =
            rayOrigin.position;

        Vector2 currentDirection =
            aimDirection.normalized;

        points.Add(currentPosition);

        Collider2D lastCollider = null;

        int reflections =
            Mathf.Max(1, maxReflections);


        for (int i = 0; i < reflections; i++)
        {
            RaycastHit2D hit =
                GetClosestValidHit(
                    currentPosition,
                    currentDirection,
                    lastCollider
                );


            // =================================================
            // NOTHING HIT
            // =================================================

            if (hit.collider == null)
            {
                points.Add(
                    currentPosition +
                    currentDirection *
                    rayMaxDistance
                );

                break;
            }


            points.Add(hit.point);


            // =================================================
            // JUMP ORB
            // =================================================

            JumpOrb orb =
                hit.collider.GetComponentInParent<JumpOrb>();

            if (orb != null)
            {
                orb.SetLitByBeacon(this);

                litOrbs.Add(orb);

                Vector2 normal =
                    hit.normal;

                if (normal == Vector2.zero)
                {
                    normal =
                        (
                            (Vector2)hit.point -
                            (Vector2)orb.transform.position
                        ).normalized;
                }

                currentDirection =
                    Vector2.Reflect(
                        currentDirection,
                        normal
                    ).normalized;

                currentPosition =
                    hit.point +
                    currentDirection *
                    reflectionOffset;

                lastCollider =
                    hit.collider;

                continue;
            }


            // =================================================
            // OBSTACLE
            // =================================================

            if (IsInLayerMask(
                hit.collider.gameObject.layer,
                obstacleLayer))
            {
                LightAbsorbingObstacle obstacle =
                    hit.collider.GetComponent<LightAbsorbingObstacle>();

                if (obstacle != null)
                {
                    obstacle.OnLightRayHit(
                        hit.point,
                        currentDirection
                    );
                }

                break;
            }


            // =================================================
            // REFLECTION
            // =================================================

            if (IsInLayerMask(
                hit.collider.gameObject.layer,
                reflectiveLayer))
            {
                Vector2 normal =
                    hit.normal;

                if (normal == Vector2.zero)
                {
                    normal = Vector2.up;
                }

                currentDirection =
                    Vector2.Reflect(
                        currentDirection,
                        normal
                    ).normalized;

                currentPosition =
                    hit.point +
                    currentDirection *
                    reflectionOffset;

                lastCollider =
                    hit.collider;

                continue;
            }


            break;
        }

        return points;
    }


    // =========================================================
    // GET CLOSEST VALID HIT
    // =========================================================

    RaycastHit2D GetClosestValidHit(
        Vector2 origin,
        Vector2 direction,
        Collider2D ignoredCollider
    )
    {
        RaycastHit2D[] hits =
            Physics2D.RaycastAll(
                origin,
                direction,
                rayMaxDistance,
                reflectiveLayer |
                obstacleLayer |
                solidLayer |
                jumpOrbLayer
            );

        RaycastHit2D closestHit =
            new RaycastHit2D();

        float closestDistance =
            float.MaxValue;


        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null)
            {
                continue;
            }

            if (hit.collider == ignoredCollider)
            {
                continue;
            }

            if (hit.distance <= 0.02f)
            {
                continue;
            }

            if (hit.distance < closestDistance)
            {
                closestHit = hit;

                closestDistance =
                    hit.distance;
            }
        }

        return closestHit;
    }


    // =========================================================
    // SMOOTH RAY VISUAL
    // =========================================================

    void SmoothRayVisual()
    {
        while (
            currentVisualPoints.Count <
            targetRayPoints.Count
        )
        {
            currentVisualPoints.Add(
                currentVisualPoints[
                    currentVisualPoints.Count - 1
                ]
            );
        }


        while (
            currentVisualPoints.Count >
            targetRayPoints.Count
        )
        {
            currentVisualPoints.RemoveAt(
                currentVisualPoints.Count - 1
            );
        }


        for (
            int i = 0;
            i < targetRayPoints.Count;
            i++
        )
        {
            currentVisualPoints[i] =
                Vector3.Lerp(
                    currentVisualPoints[i],
                    targetRayPoints[i],
                    Time.deltaTime *
                    lightReachSmoothSpeed
                );
        }
    }


    // =========================================================
    // BEACON VISUAL UPDATE
    // =========================================================

    void UpdateBeaconVisual()
    {
        if (beaconRenderer == null)
        {
            return;
        }


        if (isActive)
        {
            beaconRenderer.color =
                activeBeaconColor *
                activeBrightness;
        }
        else
        {
            beaconRenderer.color =
                inactiveBeaconColor;
        }
    }


    // =========================================================
    // LAYER CHECK
    // =========================================================

    bool IsInLayerMask(
        int layer,
        LayerMask layerMask
    )
    {
        return (
            layerMask.value &
            (1 << layer)
        ) != 0;
    }


    // =========================================================
    // PUBLIC HELPERS
    // =========================================================

    public bool IsBeaconActive()
    {
        return isActive;
    }


    public bool IsDirectionalMode()
    {
        return isActive;
    }


    public bool IsHeroInRange()
    {
        return heroInRange;
    }


    public Vector2 GetBeaconDirection()
    {
        return aimDirection;
    }
}