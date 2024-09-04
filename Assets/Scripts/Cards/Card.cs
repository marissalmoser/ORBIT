// +-----------------------------------------------------------------------------------------------+
// @author - Ryan Herwig
// @Contributers - 
// @Last Modified - August 4 2024
// @ Description - A scriptable object of a card to use. The scriptable object will contain a name,
//                 sprite, and if it has been clicked or not. It also has a ToString override.
// +-----------------------------------------------------------------------------------------------+

using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Card")]

public class Card : ScriptableObject
{
    public new CardName name;
    public Sprite cardSprite;
    public int ID {  get; private set; }

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