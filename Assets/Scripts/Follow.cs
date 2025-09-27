using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform followTransform;
    public BoxCollider2D mapBounds;
    public float smoothTime = 0.15f;

    private float xMin, xMax, yMin, yMax;
    private Camera cam;
    private Vector3 velocity;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        var b = mapBounds.bounds;
        xMin = b.min.x; xMax = b.max.x;
        yMin = b.min.y; yMax = b.max.y;

        // Snap once on start so there’s no initial drift.
        Vector3 target = GetClampedTarget();
        transform.position = new Vector3(target.x, target.y, transform.position.z);
    }

    void LateUpdate()
    {
        if (!followTransform || !cam) return;

        Vector3 target = GetClampedTarget();
        transform.position = Vector3.SmoothDamp(transform.position, 
                                                new Vector3(target.x, target.y, transform.position.z),
                                                ref velocity, 
                                                smoothTime);
    }

    private Vector3 GetClampedTarget()
    {
        float halfH = cam.orthographicSize;                 // half height in world units
        float halfW = halfH * cam.aspect;                   // half width in world units

        // If the map is smaller than the camera view, lock to the map center on that axis.
        float mapW = xMax - xMin;
        float mapH = yMax - yMin;

        float targetX = followTransform.position.x;
        float targetY = followTransform.position.y;

        if (mapW <= halfW * 2f)
            targetX = (xMin + xMax) * 0.5f;
        else
            targetX = Mathf.Clamp(targetX, xMin + halfW, xMax - halfW);

        if (mapH <= halfH * 2f)
            targetY = (yMin + yMax) * 0.5f;
        else
            targetY = Mathf.Clamp(targetY, yMin + halfH, yMax - halfH);

        return new Vector3(targetX, targetY, 0f);
    }
}
