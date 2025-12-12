using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class LaserTurret : MonoBehaviour
{
    [SerializeField] LayerMask targetLayer;
    [SerializeField] GameObject crosshair;
    [SerializeField] float baseTurnSpeed = 3;
    [SerializeField] GameObject gun;
    [SerializeField] Transform turretBase;
    [SerializeField] Transform barrelEnd;
    [SerializeField] LineRenderer line;

    List<Vector3> laserPoints = new List<Vector3>();
    int bounceMax = 20;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        TrackMouse();
        TurnBase();

        laserPoints.Clear();
        laserPoints.Add(barrelEnd.position);

        Vector3 direction = barrelEnd.forward;
        Vector3 basePos = barrelEnd.position;

        for (int x = 0; x < bounceMax; x++)
        {
            if (Physics.Raycast(basePos, direction, out RaycastHit hit, 1000.0f, targetLayer))
            {
                laserPoints.Add(hit.point);
                Vector3 hitNormal = hit.normal.normalized;
                Vector3 projection = Vector3.Dot(direction, hitNormal) * hitNormal;

                direction = (direction - 2.0f * projection).normalized;
                basePos = hit.point + direction * 0.01f;
            }
            else
            {
                laserPoints.Add(basePos + direction * 2500f);
                break;
            }
        }

        line.positionCount = laserPoints.Count;
        for (int y = 0; y < line.positionCount; y++)
        {
            line.SetPosition(y, laserPoints[y]);
        }
    }

    void TrackMouse()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(cameraRay, out hit, 1000, targetLayer))
        {
            crosshair.transform.forward = hit.normal;
            crosshair.transform.position = hit.point + hit.normal * 0.1f;
        }
    }

    void TurnBase()
    {
        Vector3 directionToTarget = (crosshair.transform.position - turretBase.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, directionToTarget.y, directionToTarget.z));
        turretBase.transform.rotation = Quaternion.Slerp(turretBase.transform.rotation, lookRotation, Time.deltaTime * baseTurnSpeed);
    }
}
