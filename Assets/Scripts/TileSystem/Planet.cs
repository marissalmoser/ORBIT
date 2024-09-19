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
            //sound effect caller
            GameObject manager = GameObject.Find("SfxManager");
            SfxManager function_call = (SfxManager)manager.GetComponent(typeof(SfxManager));
            function_call.PlaySFX(8346);

            GameManager.Instance.AddCollectable(this);
            Destroy(gameObject);
        }
    }
}
