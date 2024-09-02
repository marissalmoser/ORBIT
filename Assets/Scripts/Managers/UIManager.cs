using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] Image card1;
    [SerializeField] GameObject canvas;

    [SerializeField] Sprite moveSprite;
    [SerializeField] Sprite jumpSprite;
    [SerializeField] Sprite turnSprite;
    [SerializeField] Sprite switchSprite;
    [SerializeField] Sprite clearSprite;

    [SerializeField] Card moveCard;
    [SerializeField] Card jumpCard;
    [SerializeField] Card turnCard;
    [SerializeField] Card switchCard;
    [SerializeField] Card clearCard;

    public void Init()
    {
        Image newImage = Instantiate(card1, Vector3.zero, Quaternion.identity);
        newImage.transform.SetParent(canvas.transform, false);
        newImage.rectTransform.anchoredPosition = Vector3.zero;

        //createImage.transform.SetParent(canvas.transform, false);
        //Instantiate(card1, new Vector3(0, 0, 0), Quaternion.identity, canvas);
    }
}
