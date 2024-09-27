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
    public static UnityAction WallInterruptAnimation;
    public static UnityAction SpikeCollision;
    public static UnityAction ReachedDestination;
    public static UnityAction<Card> AddCard;

    [SerializeField] private int _currentFacingDirection;
    [SerializeField] private float _jumpArcHeight;
    [SerializeField] private float _checkInterval;
    [SerializeField] private float _rayCastDistance;

    [SerializeField] private Transform _raycastPoint;
    [SerializeField] private Tile _currentTile;

    [SerializeField] private AnimationCurve _moveEaseCurve;
    [SerializeField] private AnimationCurve _jumpEaseCurve;
    [SerializeField] private AnimationCurve _fallEaseCurve;
    [SerializeField] private AnimationCurve _turnEaseCurve;
    private Tile _previousTile;
    private Coroutine _currentMovementCoroutine;

    public void Start()
    {

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

        Tile nextTile = _currentTile;

        //get the last key in the curve
        while (timeElapsed < _moveEaseCurve.keys[_moveEaseCurve.length - 1].time)
        {
            float curvePosition = _moveEaseCurve.Evaluate(timeElapsed);

            // Interpolate the player's position based on the curve's output
            transform.position = Vector3.Lerp(originTileLoc, targetTileLoc, curvePosition);

            timeElapsed += Time.deltaTime;
            checkTimeElapsed += Time.deltaTime;

            if (checkTimeElapsed >= _checkInterval)
            {
                if (GetTileWithPlayerRaycast().GetCoordinates() != nextTile.GetCoordinates() && GetTileWithPlayerRaycast() != null)
                {
                    nextTile = GetTileWithPlayerRaycast();
                    if (nextTile.IsHole()) //player needs to fall down
                    {
                        StopCoroutine(_currentMovementCoroutine);
                        Vector3 newV = nextTile.GetPlayerSnapPosition();
                        StartFallCoroutine(transform.position, new Vector3(newV.x, newV.y - 10, newV.z));
                    }
                    else if (nextTile.GetElevation() < _currentTile.GetElevation()) // going down
                    {
                        StopCoroutine(_currentMovementCoroutine);
                        StartFallCoroutine(transform.position, nextTile.GetPlayerSnapPosition());
                        print(transform.position + " " + nextTile.GetPlayerSnapPosition());
                    }
                    else if (nextTile.GetObstacleClass() != null && nextTile.GetObstacleClass().IsActive())
                    {
                        StopCoroutine(_currentMovementCoroutine);

                        SetCurrentTile(TileManager.Instance.GetTileByCoordinates(nextTile.GetCoordinates()));

                        nextTile.GetObstacleClass().PerformObstacleAnim();
                        
                       
                        var card = TileManager.Instance.GetObstacleWithTileCoordinates(nextTile.GetCoordinates()).GetCard();

                        if (card != null)
                        {
                            AddCard?.Invoke(card);
                        }
                        ReachedDestination?.Invoke();
                    }
                }
                // Reset the check timer
                checkTimeElapsed = 0f;
            }
            yield return null;
        }
        print("down here");
        transform.position = targetTileLoc; //double check final position
        SetCurrentTile(TileManager.Instance.GetTileByCoordinates(new Vector2((int)targetTileLoc.x, (int)targetTileLoc.z)));

        if (_currentMovementCoroutine != null)
        {
            ReachedDestination?.Invoke();
        }
    }
    private IEnumerator FallPlayer(Vector3 originTileLoc, Vector3 target)
    {
        //GetComponent<SphereCollider>().enabled = false; 
        float timeElapsed = 0f;
        float totalTime = _fallEaseCurve.keys[_moveEaseCurve.length - 1].time;

        //Vector3 target = new Vector3(originTileLoc.x, originTileLoc.y + directionMagnitude, originTileLoc.z);

        while (timeElapsed < totalTime)
        {
            float time = timeElapsed / totalTime;
            transform.position = Vector3.Lerp(originTileLoc, target, time);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = target;
        SetCurrentTile(TileManager.Instance.GetTileByCoordinates(new Vector2((int)target.x, (int)target.z)));
        if (_currentTile.GetObstacleClass() != null && _currentTile.GetObstacleClass().IsActive())
        {
            _currentTile.GetObstacleClass().PerformObstacleAnim();
            var card = TileManager.Instance.GetObstacleWithTileCoordinates(_currentTile.GetCoordinates()).GetCard();
            if (card != null)
            {
                AddCard?.Invoke(card);
            }
        }
        //GetComponent<SphereCollider>().enabled = true;
        ReachedDestination?.Invoke();
    }

    private IEnumerator TurnPlayer(bool turningLeft)
    {
        float timeElapsed = 0f;
        float totalDuration = _turnEaseCurve.keys[_turnEaseCurve.length - 1].time;

        float startRotationY = transform.eulerAngles.y;

        //first calculate the target Y rotation (90 degrees to the left or right)
        float targetRotationY = turningLeft ? startRotationY - 90f : startRotationY + 90f;
        //then we need to normalize the angle to prevent values greater than 360 or less than 0
        if (targetRotationY < 0f)
            targetRotationY += 360f;
        else if (targetRotationY >= 360f)
            targetRotationY -= 360f;

        while (timeElapsed < totalDuration)
        {
            float t = _turnEaseCurve.Evaluate(timeElapsed);

            float newRotationY = Mathf.LerpAngle(startRotationY, targetRotationY, t);

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, newRotationY, transform.eulerAngles.z);

            timeElapsed += Time.deltaTime;

            yield return null;
        }
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, targetRotationY, transform.eulerAngles.z);
        UpdateFacingDirection(turningLeft);
        ReachedDestination?.Invoke();
    }
    private IEnumerator JumpPlayer(Vector3 originTileLoc, Vector3 targetTileLoc)
    {
        float timeElapsed = 0f;
        //float checkTimeElapsed = 0f;

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
            //checkTimeElapsed += Time.deltaTime;

            yield return null;
        }
        if (GetTileWithPlayerRaycast().IsHole()) //player needs to fall down
        {
            StopCoroutine(_currentMovementCoroutine);
            Vector3 newV = GetTileWithPlayerRaycast().GetPlayerSnapPosition();
            StartFallCoroutine(transform.position, new Vector3(newV.x, newV.y - 10, newV.z));
        }

        else
        {
            transform.position = targetTileLoc; //double check final position
            SetCurrentTile(TileManager.Instance.GetTileByCoordinates(new Vector2((int)targetTileLoc.x, (int)targetTileLoc.z)));
            if (_currentTile.GetObstacleClass() != null && _currentTile.GetObstacleClass().IsActive())
            {
                _currentTile.GetObstacleClass().PerformObstacleAnim();
                var card = TileManager.Instance.GetObstacleWithTileCoordinates(_currentTile.GetCoordinates()).GetCard();
                if (card != null)
                {
                    AddCard?.Invoke(card);
                }
            }

            ReachedDestination?.Invoke();
        }
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
        if (Physics.Raycast(_raycastPoint.position, -Vector3.up, out hit, _rayCastDistance))
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
    public Coroutine GetCurrentMovementCoroutine()
    {
        return _currentMovementCoroutine;
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
    public void UpdateFacingDirection(bool turningLeft)
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

        //SetFacingDirection(_currentFacingDirection); //updating rotation
    }

    /// <summary>
    /// Will be using this to detect collisions with walls and spikes
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<Spike>() != null)
        {
            SpikeCollision?.Invoke();
            Vector3 newV = GetTileWithPlayerRaycast().GetPlayerSnapPosition();
            StartFallCoroutine(transform.position, new Vector3(newV.x, newV.y + 10, newV.z));
        }
    }

    #region Starts
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
        _currentMovementCoroutine = StartCoroutine(MovePlayer(origin, target));
    }
    public void StartTurnCoroutine(bool turningLeft)
    {
        _currentMovementCoroutine = StartCoroutine(TurnPlayer(turningLeft));
    }
    public void StartJumpCoroutine(Vector3 origin, Vector3 target)
    {
        _currentMovementCoroutine = StartCoroutine(JumpPlayer(origin, target));
    }
    public void StartFallCoroutine(Vector3 origin, Vector3 target)
    {
        _currentMovementCoroutine = StartCoroutine(FallPlayer(origin, target));
    }
    #endregion
}

