/******************************************************************
*    Author: Marissa Moser
*    Contributors: 
*    Date Created: September 13, 2024
*    Description: This obstacle moves between the track obstacles. It is always active.
*******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWallController : Obstacle
{
    [SerializeField] private List<GameObject> _tracks = new List<GameObject>();

    [SerializeField] private float _wallMoveSpeed;

    private int _currentTrack;
    private bool _movingUp;
    private bool _isMoving = false;
    private int _direction;

    private void OnValidate()
    {
        if (_tracks.Count != 0 && _tracks[0] != null)
        {
            transform.position = _tracks[0].transform.position;

            //undo the previously set default state.
            for (int i = 0; i < _tracks.Count; i++)
            {
                _tracks[i].GetComponent<MovingWallTrack>().SetDefaultState(false);
            }

            _tracks[0].GetComponent<MovingWallTrack>().SetDefaultState(true);
        }
    }

    public override void PerformObstacleAnim()
    {
        //sound effect call
        SfxManager.Instance.PlaySFX(1191);

        MoveWall();
    }

    public override void SetToDefaultState()
    {
        transform.position = _tracks[0].transform.position;
        _tracks[_currentTrack].GetComponent<MovingWallTrack>().SetIsActive(false);
        _currentTrack = 0;
        _tracks[0].GetComponent<MovingWallTrack>().SetIsActive(true);
    }

    /// <summary>
    /// Called in place of an animation to move the wall.
    /// </summary>
    private void MoveWall()
    {
        //if current track is the first or last track, switch moving up bool
        if (_currentTrack >= _tracks.Count - 1 && _movingUp ||
            _currentTrack <= 0 && !_movingUp)
        {
            _movingUp = !_movingUp;
        }

        _tracks[_currentTrack].GetComponent<MovingWallTrack>().SetIsActive(false);

        int previousTrack = _currentTrack; // Store the current track
        //set current track
        if (_movingUp)
        {
            _currentTrack++;
        }
        else
        {
            _currentTrack--;
        }

        Vector3 movementDirection = (_tracks[_currentTrack].transform.position - _tracks[previousTrack].transform.position).normalized;
        _direction = GetTileDirectionFromMovement(movementDirection);


        //Move pos to current track and set current track active
        StartCoroutine(MoveWallToTrack(_tracks[_currentTrack].transform.position));
        _tracks[_currentTrack].GetComponent<MovingWallTrack>().SetIsActive(true);


    }

    private int GetTileDirectionFromMovement(Vector3 movementDirection)
    {
        if (movementDirection == Vector3.forward)
            return 1;
        else if (movementDirection == Vector3.back)
            return 7;
        else if (movementDirection == Vector3.left)
            return 3;
        else if (movementDirection == Vector3.right)
            return 5;
        else
            return 1;
    }
    /// <summary>
    /// Moves the wall along the track over time;
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    private IEnumerator MoveWallToTrack(Vector3 targetPos)
    {
        _isMoving = true;
        float timeElapsed = 0f;
        float totalTime = _wallMoveSpeed;
        Vector3 originalPos = transform.position;

        while (timeElapsed < totalTime)
        {
            float time = timeElapsed / totalTime;
            transform.position = Vector3.Lerp(originalPos, targetPos, time);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
        yield return new WaitForSeconds(.25f); //to prevent the player from hitting the box a second time
        _isMoving = false;
        GetComponent<BoxCollider>().enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (_isMoving)// && !_shovedPlayer) // wall is moving into the player
            {
                PlayerController pc = other.GetComponent<PlayerStateMachineBrain>().GetOriginalPlayerController();
                var tilePCIsOn = pc.GetCurrentTile();
                GetComponent<BoxCollider>().enabled = false;
                pc.StartMoveCoroutine(tilePCIsOn.GetPlayerSnapPosition(), TileManager.Instance.GetTileAtLocation(tilePCIsOn, _direction, 1).GetPlayerSnapPosition());
            }
        }
        else if (other.CompareTag("PlayerGhost") && _isMoving)
        {
            PlayerStateMachineBrain psmb = other.gameObject.GetComponentInParent<PlayerStateMachineBrain>(true);
            while (psmb.GetNextAction() != null) //removes all remaining actions
            {

            }
            PlayerController.ReachedDestination?.Invoke();
        }
    }
}
