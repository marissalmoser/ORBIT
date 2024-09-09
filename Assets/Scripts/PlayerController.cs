/******************************************************************
*    Author: Elijah Vroman
*    Contributors: 
*    Date Created: 9/02/24
*    Description: This script is mainly focused on moving the player
*    object from point A to point B, and turning, using coroutines
*******************************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public static UnityAction ReachedDestination;

    [SerializeField] private int _currentFacingDirection;
    [SerializeField] private float _fallTime = 1f;
    [SerializeField] private float _checkMoveInterval;
    [SerializeField] private float _jumpArcHeight;

    [SerializeField] private Transform _raycastPoint;
    [SerializeField] private Tile _currentTile;

    [SerializeField] private AnimationCurve _moveEaseCurve;
    [SerializeField] private AnimationCurve _jumpEaseCurve;
    [SerializeField] private AnimationCurve _fallEaseCurve;

    private Tile _previousTile;
    private Coroutine _currentCoroutine;

    public void Start()
    {
        _currentTile = TileManager.Instance.GetTileByCoordinates(new Vector2(0, 0));
        transform.position = _currentTile.GetPlayerSnapPosition();
        //TODO : replace this with a more concrete way to set the starting position
    }
    void Update()
    {

    }

    #region LiteralMovement
    /// <summary>
    /// These are all private because there are public callers below they all 
    /// use Lerp to update the player gameobject between two points
    /// </summary>
    /// <param name="originTileLoc"></param>
    /// <param name="targetTileLoc"></param>
    /// <returns></returns>
    private IEnumerator MovePlayer(Vector3 originTileLoc, Vector3 targetTileLoc)
    {
        float timeElapsed = 0f;
        float checkTimeElapsed = 0f;

        //get the last key in the curve
        while (timeElapsed < _moveEaseCurve.keys[_moveEaseCurve.length - 1].time)
        {
            float curvePosition = _moveEaseCurve.Evaluate(timeElapsed);

            // Interpolate the player's position based on the curve's output
            transform.position = Vector3.Lerp(originTileLoc, targetTileLoc, curvePosition);

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
        transform.position = targetTileLoc; //double check final position
        SetCurrentTile(TileManager.Instance.GetTileByCoordinates(new Vector2((int)targetTileLoc.x, (int)targetTileLoc.z)));
        ReachedDestination?.Invoke();
    }
    private IEnumerator FallPlayer(Vector3 originTileLoc)
    {
        float timeElapsed = 0f;
        float totalTime = _fallEaseCurve.keys[_moveEaseCurve.length - 1].time;

        Vector3 target = new Vector3(originTileLoc.x, originTileLoc.y - 10f, originTileLoc.z);

        while (timeElapsed < totalTime)
        {
            float time = timeElapsed / totalTime;
            transform.position = Vector3.Lerp(originTileLoc, target, time);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator JumpPlayer(Vector3 originTileLoc, Vector3 targetTileLoc)
    {
        float timeElapsed = 0f;
        float checkTimeElapsed = 0f;

        //calculate the midpoint by using both A and B and getting halfway at the archeight
        Vector3 controlPoint = (originTileLoc + targetTileLoc) / 2 + Vector3.up * _jumpArcHeight;
        float totalDuration = _jumpEaseCurve.keys[_moveEaseCurve.length - 1].time;

        while (timeElapsed < totalDuration)
        {
            float time = timeElapsed / totalDuration;

            // Calculate the current position on the Bezier curve
            Vector3 currentPos = CalculateQuadraticBezierPoint(time, originTileLoc, controlPoint, targetTileLoc);
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
        transform.position = targetTileLoc; //double check final position
        SetCurrentTile(TileManager.Instance.GetTileByCoordinates(new Vector2((int)targetTileLoc.x, (int)targetTileLoc.z)));
        ReachedDestination?.Invoke();
    }
    /// <summary>
    /// Calculations for BezierCurve, all math stuff 
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
    #endregion

    #region Getters
    public Tile GetTileWithPlayerRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(_raycastPoint.position, -Vector3.up, out hit, 1f))
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
        if (_currentTile != null)
        {
            return _currentTile;
        }
        Debug.LogError("Player controller's reference to the tile it is on is null");
        return null;
    }
    public int GetCurrentFacingDirection()
    {
        return _currentFacingDirection;
    }
    #endregion

    #region Setters
    public void SetCurrentTile(Tile tileToBeAt)
    {
        _currentTile = tileToBeAt;
    }
    /// <summary>
    /// Literally updates the playerobject rotation
    /// </summary>
    /// <param name="direction"></param>
    public void SetFacingDirection(int direction)
    {
        _currentFacingDirection = direction;

        float newYRotation = 0f;  // Default rotation for North
        switch (_currentFacingDirection)
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
    #endregion

    /// <summary>
    /// Updates the facing direction in code only, but also calls SetFacingDir
    /// </summary>
    /// <param name="turningLeft"></param>
    public void TurnPlayer(bool turningLeft)
    {
        if (turningLeft)
        {
            switch (_currentFacingDirection)
            {
                case 1: _currentFacingDirection = 3; break;
                case 3: _currentFacingDirection = 7; break;
                case 7: _currentFacingDirection = 5; break;
                case 5: _currentFacingDirection = 1; break;
            }
        }
        else
        {
            switch (_currentFacingDirection)
            {
                case 1: _currentFacingDirection = 5; break;
                case 5: _currentFacingDirection = 7; break;
                case 7: _currentFacingDirection = 3; break;
                case 3: _currentFacingDirection = 1; break;
            }
        }

        SetFacingDirection(_currentFacingDirection); //updating rotation
    }

    /// <summary>
    /// Will be using this to reset the players position + add new anim after
    /// they collide with an obstacle
    /// </summary>
    /// <param name="target"></param>
    public void ScanTile(Tile target)
    {
        Tile currentTile = target;
        if (_previousTile == null || currentTile != _previousTile)
        {
            _previousTile = currentTile;
            if (_previousTile.GetObstacleClass() != null)
            {
                //there is an obstacle, 
                ReachedDestination?.Invoke();
            }
        }
    }

    /// <summary>
    /// Will be using this to detect collisions with walls and spikes
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Collider>() != null)
        {
            print("HERE");
        }
    }

    #region StartsAndStops
    /// <summary>
    /// Below are all the coroutines for moving the playercontainer. We will a
    /// nimate the player itself, not the container. However, we still need to 
    /// move the container, because the animations will be animated in place
    /// I dont know a better way to stop a coroutine from another script
    /// without doing string comparison, so thats why there are so many methods
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    public void StartMoveCoroutine(Vector3 origin, Vector3 target)
    {
        _currentCoroutine = StartCoroutine(MovePlayer(origin, target));
    }
    public void StopMoveCoroutine()
    {
        StopCoroutine(_currentCoroutine);
    }
    public void StartJumpCoroutine(Vector3 origin, Vector3 target)
    {
        _currentCoroutine = StartCoroutine(JumpPlayer(origin, target));
    }
    public void StopJumpCoroutine()
    {
        StopCoroutine(_currentCoroutine);
    }
    public void StartFallCoroutine(Vector3 origin)
    {
        _currentCoroutine = StartCoroutine(FallPlayer(origin));
    }
    public void StopFallCoroutine()
    {
        StopCoroutine(_currentCoroutine);
    }
    #endregion
}
