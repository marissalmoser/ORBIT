using UnityEngine;

public class Planet : Collectable
{
    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameManager.AddCollectable(this);
            Destroy(gameObject);
        }
    }
}
