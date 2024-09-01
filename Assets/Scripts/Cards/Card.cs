// +-----------------------------------------------------------------------------------------------+
// @author - Ryan Herwig
// @Last Modified - August 31 2024
// @ Description - A scriptable object of a card to use. The scriptable object will contain a name,
//                 sprite, and if it has been clicked or not. It also has a ToString override.
// +-----------------------------------------------------------------------------------------------+

using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Card")]
public class Card : ScriptableObject
{
    [Tooltip("The name of the card")]
    [SerializeField] private CardName cardName;

    [SerializeField] private Image cardSprite;

    [NonSerialized] public bool clicked;

    public enum CardName
    {
        Move,
        Turn,
        Jump,
        Clear,
        Switch
    }

    /**
     * Overrides the ToString method
     * When object is printed, prints text instead
     * @return string - the string to print into the console
     */
    public override string ToString()
    {
        string text = "";
        text += cardName.ToString() + " | " + "Clicked = " + clicked;

        return text;
    }
}