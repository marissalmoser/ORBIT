// +-----------------------------------------------------------------------------------------------+
// @author - Ryan Herwig
// @Contributers - Elijah Vroman
// @Last Modified - August 4 2024
// @ Description - A scriptable object of a card to use. The scriptable object will contain a name,
//                 sprite, and if it has been clicked or not. It also has a ToString override.
// +-----------------------------------------------------------------------------------------------+

using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Card")]

public class Card : ScriptableObject
{
    [Tooltip(" North is positive on the z axis. East is positive on the x axis")] [SerializeField] private Direction direction;
    public new CardName name;
    public Sprite cardSprite;

    [SerializeField] private int distance;
    public int ID { get; private set; }

    public enum Direction
    {
        None, Northwest, North, Northeast, West, East, Southwest, South, Southeast
    }
    public enum CardName
    {
        Move,
        Turn,
        TurnLeft,
        TurnRight,
        Jump,
        Clear,
        Switch,
        BackToIt
    }

    /// <summary>
    /// Changes the ID of the card
    /// </summary>
    /// <param name="id">The ID of the card</param>
    public void SetID(int id)
    {
        ID = id;
    }

    /// <summary>
    /// Getter for the private variable i made - E.v.
    /// </summary>
    /// <returns></returns>
    public int GetDistance()
    {
        return distance;
    }

    public int GetDirection()
    {
        switch (direction)
        {
            case Direction.Northwest:
                return 0;
            case Direction.North:
                return 1;
            case Direction.Northeast:
                return 2;
            case Direction.West:
                return 3;
            case Direction.East:
                return 5;
            case Direction.Southwest:
                return 6;
            case Direction.South:
                return 7;
            case Direction.Southeast:
                return 8;
            default:
                return 4;
        }
    }

    /// <summary>
    /// Overrides the ToString method
    /// When object is printed, prints text instead
    /// </summary>
    /// <returns>
    /// @return string - the string to print into the console
    /// </returns>
    public override string ToString()
    {
        string text = "";
        text += name.ToString() + " | ID: " + ID;

        return text;
    }
}