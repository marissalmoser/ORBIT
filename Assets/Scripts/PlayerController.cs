using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Tile currentTile;
    public Tile tile1;
    public Tile tile2;
    public AnimationCurve moveEaseCurve;
    public float fallTime = 1f;
    public float checkMoveInterval;
    public float jumpArcHeight;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(MovePlayer(tile1, tile2));
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(MoveAlongArc(tile1, tile2));
        }
    }
    IEnumerator MovePlayer(Tile originTile, Tile targetTile)
    {
        Vector3 origin = originTile.GetPlayerSnap().position;
        Vector3 target = targetTile.GetPlayerSnap().position;
        float timeElapsed = 0f;
        float checkTimeElapsed = 0f;

        //get the last key in the curve
        while (timeElapsed < moveEaseCurve.keys[moveEaseCurve.length - 1].time)
        {
            float curvePosition = moveEaseCurve.Evaluate(timeElapsed);

            // Interpolate the player's position based on the curve's output
            transform.position = Vector3.Lerp(origin, target, curvePosition);

            timeElapsed += Time.deltaTime;
            checkTimeElapsed += Time.deltaTime;

            if (checkTimeElapsed >= checkMoveInterval)
            {
                //GetTileWithRaycast(); //Update whatever tile 

                // Reset the check timer
                checkTimeElapsed = 0f;
            }
            print(timeElapsed);
            yield return null;
        }
        transform.position = target;
        if (targetTile.IsHole())
        {
            StartCoroutine(Fall(targetTile));
        }
    }
    IEnumerator Fall(Tile originTile)
    {
        float timeElapsed = 0f;
        float totalTime = moveEaseCurve.keys[moveEaseCurve.length - 1].time;

        Vector3 origin = originTile.GetPlayerSnap().position;
        Vector3 target = new Vector3(origin.x, origin.y - 10f, origin.z);

        while (timeElapsed < totalTime)
        {
            float time = timeElapsed / totalTime;
            transform.position = Vector3.Lerp(origin, target, time);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator MoveAlongArc(Tile originTile, Tile targetTile)
    {
        float timeElapsed = 0f;
        float checkTimeElapsed = 0f;
        Vector3 origin = originTile.GetPlayerSnap().position;
        Vector3 target = targetTile.GetPlayerSnap().position;

        //calculate the midpoint by using both A and B and getting halfway at the archeight
        Vector3 controlPoint = (origin + target) / 2 + Vector3.up * jumpArcHeight;
        float totalDuration = moveEaseCurve.keys[moveEaseCurve.length - 1].time;

        while (timeElapsed < totalDuration)
        {
            float time = timeElapsed / totalDuration;

            // Calculate the current position on the Bezier curve
            Vector3 currentPos = CalculateQuadraticBezierPoint(time, origin, controlPoint, target);
            transform.position = currentPos;

            timeElapsed += Time.deltaTime;
            checkTimeElapsed += Time.deltaTime;

            print(timeElapsed);
            yield return null;
        }
        transform.position = target; //double check final position
        if (targetTile.IsHole())
        {
            StartCoroutine(Fall(targetTile));
        }
    }
    /// <summary>
    /// Calculations for BezierCurve
    /// </summary>
    /// <param name="t"></param>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        // Calculate the point on the curve
        Vector3 p = uu * p0; // (1-t)^2 * P0
        p += 2 * u * t * p1; // 2(1-t)t * P1
        p += tt * p2;        // t^2 * P2

        return p;
    }
    public Tile GetTileWithRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 1f))
        {
            if (hit.collider.GetComponent<Tile>() != null)
            {
                return hit.collider.GetComponent<Tile>();
            }
        }
        return null;
    }
}
