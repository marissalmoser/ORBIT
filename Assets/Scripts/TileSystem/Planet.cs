using UnityEngine;

public class Planet : Collectable
{
    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.AddCollectable(this);
            Destroy(gameObject);
        }
    }
}
