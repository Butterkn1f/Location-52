using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PhotoAnomalyChecker : MonoBehaviour
{
	const float maxDistance = 2000;
    List<Renderer> anomalyRenderers = new();

    private void Awake()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Object");
        // Merge arrays and sort by closest distance to camera => so it detects the most front anomaly first
        GameObject[] anomalies = monsters.Concat(objects).OrderBy(a => Vector3.Distance(Camera.main.transform.position, a.transform.position)).ToArray();
        foreach (var anomaly in anomalies)
        {
            anomalyRenderers.InsertRange(anomalyRenderers.Count, anomaly.GetComponentsInChildren<Renderer>());
        }
    }

    /// <summary>
    /// Checks if anomaly is visible in camera's view
    /// Returns the amount of points visible (see comments in function), to be used for photo
    /// If it returns 0, no points are detected => anomaly is not visible
    /// </summary>
    /// <param name="planes">Frustum planes</param>
    /// <param name="renderer">Renderer (anomaly) to test</param>
    /// <returns>Points visible</returns>
    private int IsVisible(Plane[] planes, Renderer renderer)
    {
        if (GeometryUtility.TestPlanesAABB(planes, renderer.bounds))
        {
            // Distance from renderer to camera
            Vector3 distance = (Camera.main.transform.position - renderer.bounds.center);

			if (distance.magnitude > maxDistance)
            {
				Debug.LogWarning(renderer.name + " anomaly too far! It is " + distance.magnitude + " units away.");
                return 0;
            }

            // Begin linecasting key points of anomaly
            // 1) Center of object
            // 2) Each corner of anomaly
            // --3) Center of each edge-- nvm not needed maybe? TODO if camera feels inaccurate still
            // If either of these points hit, anomaly is seen (although lesser points if not center :/)
            int pointsDetected = 0;
            // Test (1) Center
            if (Physics.Linecast(Camera.main.transform.position, renderer.bounds.center, out var hit))
            {
                Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red, 5);
                if (hit.collider.gameObject == renderer.gameObject)
                    pointsDetected++;
            }

            // Test (2) Corners
            Vector3 c = renderer.bounds.center;
            Vector3 e = renderer.bounds.extents;
            Vector3[] worldCorners = new[] {
                new Vector3( c.x + e.x, c.y + e.y, c.z + e.z ),
                new Vector3( c.x + e.x, c.y + e.y, c.z - e.z ),
                new Vector3( c.x + e.x, c.y - e.y, c.z + e.z ),
                new Vector3( c.x + e.x, c.y - e.y, c.z - e.z ),
                new Vector3( c.x - e.x, c.y + e.y, c.z + e.z ),
                new Vector3( c.x - e.x, c.y + e.y, c.z - e.z ),
                new Vector3( c.x - e.x, c.y - e.y, c.z + e.z ),
                new Vector3( c.x - e.x, c.y - e.y, c.z - e.z ),
            };

            foreach (var corner in worldCorners)
            {
                if (Physics.Linecast(Camera.main.transform.position, corner, out var hit2)) {
                    Debug.DrawLine(Camera.main.transform.position, hit2.point, Color.red, 5);
                    if (hit2.collider.gameObject == renderer.gameObject)
                        pointsDetected++;
                }
            }

            return pointsDetected;
        }
        else
        {
            return 0;
        }
    }

    public Renderer CheckAnomaliesInView(out int pointsDetected)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        foreach (var anomaly in anomalyRenderers)
        {
            pointsDetected = IsVisible(planes, anomaly);
            if (pointsDetected > 0)
            {
				Debug.Log(anomaly.name + " anomaly detected.");
				return anomaly;
            }
        }

        pointsDetected = 0;
		return null;
    }
}
