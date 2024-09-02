// +-----------------------------------------------------------------------------------------------+
// @author - Ryan Herwig
// @Last Modified - August 31 2024
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
    public bool clicked;

    public enum CardName
    {
        Move,
        Turn,
        Jump,
        Clear,
        Switch,
        BackToIt
    }

    public static Card CreateInstance(CardName name, Sprite sprite, bool clicked)
    {
        Card data = ScriptableObject.CreateInstance<Card>();

        data.name = name;
        data.cardSprite = sprite;
        data.clicked = clicked;

        return data;
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
        text += name.ToString() + " | " + "Clicked = " + clicked;

        return text;
    }
}