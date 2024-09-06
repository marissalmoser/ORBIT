using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Tile currentTile;
    [SerializeField] private int currentFacingDirection;
    [SerializeField] private Transform raycastPoint;
    public static UnityAction ReachedDestination;
    public Tile tile1;
    public Tile tile2;
    public Tile tile3;
    [SerializeField] private AnimationCurve moveEaseCurve;
    [SerializeField] private AnimationCurve jumpEaseCurve;
    [SerializeField] private AnimationCurve fallEaseCurve;
    public float fallTime = 1f;
    public float checkMoveInterval;
    public float jumpArcHeight;

    private Tile previousTile;
    private Coroutine currentCoroutine;

    public void Start()
    {

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(JumpPlayer(tile1, tile2));
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            TurnPlayer(true);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            TurnPlayer(false);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(JumpPlayer(tile2, tile3));
        }
    }


    private IEnumerator MovePlayer(Tile originTile, Tile targetTile)
    {
        Vector3 origin = originTile.GetPlayerSnapPosition();
        Vector3 target = targetTile.GetPlayerSnapPosition();
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

            //if (checkTimeElapsed >= checkMoveInterval)
            //{
            //    ScanTile(GetTileWithPlayerRaycast());

            //    // Reset the check timer
            //    checkTimeElapsed = 0f;
            //}
            yield return null;
        }
        //transform.position = target;
        //if (targetTile.IsHole())
        //{
        //    StartCoroutine(Fall(targetTile));
        //}
    }
    private IEnumerator FallPlayer(Tile originTile)
    {
        float timeElapsed = 0f;
        float totalTime = fallEaseCurve.keys[moveEaseCurve.length - 1].time;

        Vector3 origin = originTile.GetPlayerSnapPosition();
        Vector3 target = new Vector3(origin.x, origin.y - 10f, origin.z);

        while (timeElapsed < totalTime)
        {
            float time = timeElapsed / totalTime;
            transform.position = Vector3.Lerp(origin, target, time);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator JumpPlayer(Tile originTile, Tile targetTile)
    {
        float timeElapsed = 0f;
        float checkTimeElapsed = 0f;
        Vector3 origin = originTile.GetPlayerSnapPosition();
        Vector3 target = targetTile.GetPlayerSnapPosition();

        //calculate the midpoint by using both A and B and getting halfway at the archeight
        Vector3 controlPoint = (origin + target) / 2 + Vector3.up * jumpArcHeight;
        float totalDuration = jumpEaseCurve.keys[moveEaseCurve.length - 1].time;

        while (timeElapsed < totalDuration)
        {
            float time = timeElapsed / totalDuration;

            // Calculate the current position on the Bezier curve
            Vector3 currentPos = CalculateQuadraticBezierPoint(time, origin, controlPoint, target);
            transform.position = currentPos;

            timeElapsed += Time.deltaTime;
            checkTimeElapsed += Time.deltaTime;

            //if (checkTimeElapsed >= checkMoveInterval)
            //{
            //    ScanTile(GetTileWithPlayerRaycast());

            //    // Reset the check timer
            //    checkTimeElapsed = 0f;
            //}

            yield return null;
        }
        //transform.position = target; //double check final position
        //if (targetTile.IsHole())
        //{
        //    StartCoroutine(Fall(targetTile));
        //}
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
    public void TurnPlayer(bool turningLeft)
    {
        if (turningLeft)
        {
            switch (currentFacingDirection)
            {
                case 1: currentFacingDirection = 3; break;
                case 3: currentFacingDirection = 7; break;
                case 7: currentFacingDirection = 5; break;
                case 5: currentFacingDirection = 1; break;
            }
        }
        else
        {
            switch (currentFacingDirection)
            {
                case 1: currentFacingDirection = 5; break;
                case 5: currentFacingDirection = 7; break;
                case 7: currentFacingDirection = 3; break;
                case 3: currentFacingDirection = 1; break;
            }
        }

        SetFacingDirection(currentFacingDirection); //updating rotation
    }
    public Tile GetTileWithPlayerRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(raycastPoint.position, -Vector3.up, out hit, 1f))
        {
            if (hit.collider.GetComponent<Tile>() != null)
            {
                return hit.collider.GetComponent<Tile>();
            }
        }
        return null;
    }
    public Tile GetCurrentTile()
    {
        if (currentTile != null)
        {
            return currentTile;
        }
        Debug.LogError("Player controller's reference to the tile it is on is null");
        return null;
    }
    public int GetCurrentFacingDirection()
    {
        return currentFacingDirection;
    }
    public void SetFacingDirection(int direction)
    {
        currentFacingDirection = direction;

        float newYRotation = 0f;  // Default rotation for North
        switch (currentFacingDirection)
        {
            case 0: newYRotation = 315f; break;  // Northwest
            case 1: newYRotation = 0f; break;    // North
            case 2: newYRotation = 45f; break;   // Northeast
            case 3: newYRotation = 270f; break;  // West
            case 5: newYRotation = 90f; break;   // East
            case 6: newYRotation = 225f; break;  // Southwest
            case 7: newYRotation = 180f; break;  // South
            case 8: newYRotation = 135f; break;  // Southeast
        }
        transform.rotation = Quaternion.Euler(0f, newYRotation, 0f);
    }
    public void ScanTile(Tile target)
    {
        Tile currentTile = target;
        if (previousTile == null || currentTile != previousTile)
        {
            previousTile = currentTile;
            if (previousTile.GetObstacleClass() != null)
            {
                //there is an obstacle, 
                ReachedDestination?.Invoke();
            }
        }
    }
    public void StartMoveCoroutine(Tile origin, Tile target)
    {
        StartCoroutine(MovePlayer(origin, target));
    }
    public void StopMoveCoroutine()
    {
        StopCoroutine(MovePlayer(currentTile, currentTile));
    }
    public void StartJumpCoroutine(Tile origin, Tile target)
    {
        StartCoroutine(JumpPlayer(origin, target));
    }
    public void StopJumpCoroutine()
    {
        StopCoroutine(JumpPlayer(currentTile, currentTile));
    }
    public void StartFallCoroutine(Tile origin)
    {
        StartCoroutine(FallPlayer(origin));
    }
    public void StopFallCoroutine()
    {
        StopCoroutine(FallPlayer(currentTile));
    }
}
