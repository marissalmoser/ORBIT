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
    [SerializeField] private float _forwardCastDistance;

    [SerializeField] private Transform _raycastPoint;
    [SerializeField] private Tile _currentTile;
    private Tile _previousTile; 

    [SerializeField] private AnimationCurve _moveEaseCurve;
    [SerializeField] private AnimationCurve _jumpEaseCurve;
    [SerializeField] private AnimationCurve _fallEaseCurve;
    [SerializeField] private AnimationCurve _turnEaseCurve;
    private Coroutine _currentMovementCoroutine;
    private Animator animator;

    public void Start()
    {
        _previousTile = GetTileWithPlayerRaycast();
    }
    void Update()
    {
        
    }
    /// <summary>
    /// This method condenses repiticious code into one spot, since animation
    /// triggers are called with strings they can be passed along no problem.
    /// Gets a random number from one to ten to allow for many different 
    /// animations to play for the same type in the animator as a parameter.
    /// For example, if there are 3 different jump anims to choose from, they 
    /// could be assigned weights like 1-5, 6-7, 8-9. 
    /// 
    /// If you pass in -1 for randomAnim, this method will pick a random value
    /// </summary>
    /// <param name="animationName"></param>
    /// <param name="randomAnim"></param>
    public void PlayAnimation(string animationName, int randomAnim)
    {      
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            if(animator == null)
            {
                Debug.LogError("Player or player ghost needs an animator component");
            }     
        }
        if(randomAnim < 0)
        {
            int ran = Random.Range(1, 10);
            animator.SetInteger("Random", ran);
            animator.SetTrigger(animationName);
        }
        else
        {
            animator.SetInteger("Random", randomAnim);
            animator.SetTrigger(animationName);
        }
        
        //animator.SetInteger("Random", -1); //ensuring no animation gets called again
    }

    /// <summary>
    /// Returns: 1: Forwards, 3: Sideways left, 5: sideways right, 7: backwards
    /// </summary>
    /// <param name="directionGoing"></param>
    /// <returns></returns>
    public int DetermineProperRollDirection(int directionGoing)
    {
        switch(directionGoing)
        {
            case 1: //going north
                switch(GetCurrentFacingDirection())
                {
                    case 1: //facing north
                        return 1;
                    case 3: //facing west
                        return 5;
                    case 5: //facing east
                        return 3;
                    case 7: //facing south
                        return 7;
                    default: //fallthrough
                        return 1;
                }
            
            case 3: //going west
                switch (GetCurrentFacingDirection())
                {
                    case 1: //facing north
                        return 3; //roll sideways left
                    case 3: //facing west
                        return 1; //roll forwards (player facing away)
                    case 5: //facing east
                        return 7; //roll backwards (player facing)
                    case 7: //facing south
                        return 5; //roll sideways right
                    default: //fallthrough
                        return 1;
                }
            case 5: //going east
                switch (GetCurrentFacingDirection())
                {
                    case 1: //facing north
                        return 5; //roll sideways right
                    case 3: //facing west
                        return 7; //roll backwards (player facing)
                    case 5: //facing east
                        return 1; //roll forwards (player facing away)
                    case 7: //facing south
                        return 1;
                    default: //fallthrough
                        return 1;
                }
            case 7: //going south
                switch (GetCurrentFacingDirection())
                {
                    case 1: //facing north
                        return 7; //roll  backwards (player facing)
                    case 3: //facing west
                        return 3; //roll sideways left
                    case 5: //facing east
                        return 5; //roll sideways right
                    case 7: //facing south
                        return 1; //roll forwards (player facing away)
                    default: //fallthrough
                        return 1;
                }
            default:
                return 1;
        }
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
        Card cardOnTile = null;
        Tile nextTile = _currentTile;

        //get the last key in the curve
        while (timeElapsed < _moveEaseCurve.keys[_moveEaseCurve.length - 1].time)
        {
            timeElapsed += Time.deltaTime;
            checkTimeElapsed += Time.deltaTime;

            float curvePosition = _moveEaseCurve.Evaluate(timeElapsed);

            // Interpolate the player's position based on the curve's output
            transform.position = Vector3.Lerp(originTileLoc, targetTileLoc, curvePosition);

            
            if (checkTimeElapsed >= _checkInterval)
            {
                if (GetTileWithPlayerRaycast() != null && GetTileWithPlayerRaycast().GetCoordinates() != nextTile.GetCoordinates())
                {
                    nextTile = GetTileWithPlayerRaycast();
                    if (nextTile.IsHole()) //player needs to fall down
                    {
                        StopCoroutine(_currentMovementCoroutine);
                        Vector3 newV = nextTile.GetPlayerSnapPosition();
                        StartFallCoroutine(transform.position, new Vector3(newV.x, newV.y - 10, newV.z));
                    }
                    else if (nextTile.GetElevation() < _currentTile.GetElevation() && !nextTile.IsHole()) // going down an elevation level
                    {
                        StopCoroutine(_currentMovementCoroutine);
                        StartFallCoroutine(transform.position, nextTile.GetPlayerSnapPosition());
                    }
                    else if (nextTile.GetObstacleClass() != null && nextTile.GetObstacleClass().IsActive()) //runs atop an active obstacle
                    {
                        
                        //SetCurrentTile(TileManager.Instance.GetTileByCoordinates(nextTile.GetCoordinates()));
                        targetTileLoc = nextTile.GetPlayerSnapPosition();
                        cardOnTile = TileManager.Instance.GetObstacleWithTileCoordinates(nextTile.GetCoordinates()).GetCard();
                        //if not a turntable

                    }
                }
                // Reset the check timer
                checkTimeElapsed = 0f;
            }
            yield return null;           
        }

        transform.position = targetTileLoc; //double check final position
        SetCurrentTile(TileManager.Instance.GetTileByCoordinates(new Vector2((int)targetTileLoc.x, (int)targetTileLoc.z)));

        //if we found an obstacle card under our path during this movement coroutine
        //  and checks if obstacle should fire
        if (cardOnTile != null)
        {
            if (nextTile.GetObstacleClass().IsActive()) //&& cardOnTile.name != Card.CardName.Jump)
            {
                StopCoroutine(_currentMovementCoroutine);
                AddCard?.Invoke(cardOnTile);
                nextTile.GetObstacleClass().PerformObstacleAnim();
            }
            //special case for the annoying springs >:)
            //else if (cardOnTile.name == Card.CardName.Jump && !nextTile.GetObstacleClass().IsActive())
            //{
            //    StopCoroutine(_currentMovementCoroutine);
            //    AddCard?.Invoke(cardOnTile);
            //    nextTile.GetObstacleClass().PerformObstacleAnim();
            //}
        }
        ReachedDestination?.Invoke();
    }
    private IEnumerator FallPlayer(Vector3 originTileLoc, Vector3 targetTileLoc)
    {
        //GetComponent<SphereCollider>().enabled = false; 
        float timeElapsed = 0f;
        float totalTime = _fallEaseCurve.keys[_moveEaseCurve.length - 1].time;

        PlayAnimation("Fall" , -1);

        while (timeElapsed < totalTime)
        {
            float time = timeElapsed / totalTime;
            transform.position = Vector3.Lerp(originTileLoc, targetTileLoc, time);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetTileLoc;
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
        //GetComponent<SphereCollider>().enabled = true;
        ReachedDestination?.Invoke();
    }
    private IEnumerator TurnPlayer(bool turningLeft)
    {
        float timeElapsed = 0f;
        float totalDuration = _turnEaseCurve.keys[_turnEaseCurve.length - 1].time;

        //float startRotationY = transform.eulerAngles.y;

        ////first calculate the target Y rotation (90 degrees to the left or right)
        //float targetRotationY = turningLeft ? startRotationY - 90f : startRotationY + 90f;
        ////then we need to normalize the angle to prevent values greater than 360 or less than 0
        //if (targetRotationY < 0f)
        //    targetRotationY += 360f;
        //else if (targetRotationY >= 360f)
        //    targetRotationY -= 360f;

        while (timeElapsed < totalDuration)
        {
            //float t = _turnEaseCurve.Evaluate(timeElapsed);

            //float newRotationY = Mathf.LerpAngle(startRotationY, targetRotationY, t);

            //transform.eulerAngles = new Vector3(transform.eulerAngles.x, newRotationY, transform.eulerAngles.z);

            timeElapsed += Time.deltaTime;

            yield return null;
        }
        //transform.eulerAngles = new Vector3(transform.eulerAngles.x, targetRotationY, transform.eulerAngles.z);
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
        if (GetTileWithPlayerRaycast() != null && GetTileWithPlayerRaycast().IsHole()) //player needs to fall down
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

    private IEnumerator SpikedPlayer(Vector3 originTileLoc, Vector3 targetTileLoc)
    {
        PlayAnimation("SpikeHit", -1);
        yield return new WaitForSeconds(.25f); //Delay for beginning of anim to play
        float timeElapsed = 0f;
        float totalTime = _fallEaseCurve.keys[_moveEaseCurve.length - 1].time;

        while (timeElapsed < totalTime)
        {
            float time = timeElapsed / totalTime;
            transform.position = Vector3.Lerp(originTileLoc, targetTileLoc, time);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        //No reached destination invoke because this spike should send the player into the deathbox
    }

    private IEnumerator SimpleMoveCoroutine(Vector3 originLoc, Vector3 targetLoc, float time)
    {
        float timeElapsed = 0;

        //get the last key in the curve
        while (timeElapsed < time)
        {
            float normalizedTime = timeElapsed / time;

            // Interpolate the player's position based on the curve's output
            transform.position = Vector3.Lerp(originLoc, targetLoc, normalizedTime);
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        transform.position = targetLoc; //double check final position
        SetCurrentTile(TileManager.Instance.GetTileByCoordinates(new Vector2((int)targetLoc.x, (int)targetLoc.z)));
        ReachedDestination?.Invoke();
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
    public Tile GetForwardTileWithRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(_raycastPoint.position, Vector3.forward, out hit, _forwardCastDistance))
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
    public Tile GetPreviousTile()
    {
        if (_previousTile != null)
        {
            return _previousTile;
        }
        Debug.LogError("Player controller's reference to the previous tile it was on is null");
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
    public void SetPreviousTile(Tile tileWasAt)
    {
        _previousTile = tileWasAt;
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

        SetFacingDirection(_currentFacingDirection); //updating rotation
    }
    #endregion



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
            StartSpikedCoroutine(transform.position, new Vector3(newV.x, newV.y + 10, newV.z));
        }
        //the player runs into a wall that is NOT a moving wall
        else if (other.gameObject.CompareTag("Wall") && other.gameObject.GetComponentInParent<MovingWallController>() == null)
        {
            WallInterruptAnimation?.Invoke();
        }
    }

    #region Starts
    /// <summary>
    /// Below are all the coroutines for moving the playercontainer. We will a
    /// nimate the player itself, not the container. However, we still need to 
    /// move the container, because the animations will be animated in place
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
    public void StartSpikedCoroutine(Vector3 origin, Vector3 target)
    {
        _currentMovementCoroutine = StartCoroutine(SpikedPlayer(origin, target));
    }
    /// <summary>
    /// No animation movement coroutine
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    public void StartSimpleMoveCoroutine(Vector3 origin, Vector3 target, float time)
    {
        _currentMovementCoroutine = StartCoroutine(SimpleMoveCoroutine(origin, target, time));
    }
    #endregion
}

