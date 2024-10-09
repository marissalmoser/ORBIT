using UnityEngine;

public class ArrowControls : MonoBehaviour
{
    private int index;
    private int maxIndex;

    #region Singleton
    private static ArrowControls instance;
    public static ArrowControls Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType(typeof(ArrowControls)) as ArrowControls;
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    #endregion

    void Start()
    {
        index = -1;
    }

    public void UpdateMaxIndex(int maxIndex)
    {
        this.maxIndex = maxIndex;
    }

    public void RightArrowClicked()
    {
        //Connects the end of the list to the beginning of the list
        if (index >= maxIndex)
            index = 0;
        else
            index++;
        //TODO
    }

    public void LeftArrowClicked()
    {
        //Connects the beginning of the list to the end of the list
        if (index < 1)
            index = maxIndex;
        else
            index--;
        //TODO
    }
}
