using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private Image sprite;
    void Start()
    {
        sprite.sprite = card.cardSprite;

    }

    public void UpdateCard(Card card)
    {
        this.card = card;
    }
}