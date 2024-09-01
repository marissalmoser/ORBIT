using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    #region Singleton
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType(typeof(UIManager)) as UIManager;
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    #endregion


    [SerializeField] CardDisplay card1;
    [SerializeField] CardDisplay card2;
    [SerializeField] CardDisplay card3;
    [SerializeField] CardDisplay card4;

    [SerializeField] Card moveCard;
    [SerializeField] Card jumpCard;
    [SerializeField] Card turnCard;
    [SerializeField] Card switchCard;
    [SerializeField] Card clearCard;

    public void Init()
    {
        card1.UpdateCard(clearCard);
        card2.UpdateCard(clearCard);
        card3.UpdateCard(clearCard);
        card4.UpdateCard(clearCard);
    }
}
