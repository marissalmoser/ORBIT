/******************************************************************
 *    Author:  
 *    Contributors: 
 *    Date Created: 
 *    Description: Contains functionality for the planet collectable
 *******************************************************************/
using UnityEngine;
using UnityEngine.SceneManagement;

public class Planet : Collectable
{
    GameManager gameManager;
    private CollectibleManager _collectibleManager;
    private UIManager _uiManager;

    [SerializeField] private Material _collectedMaterial;

    private void Start()
    {
        gameManager = GameManager.Instance;
        _collectibleManager = CollectibleManager.Instance;
        _uiManager = UIManager.Instance;
        
        //if is collected, update the material
        if (CollectibleManager.Instance.GetIsCollected())
        {
            GetComponent<MeshRenderer>().material = _collectedMaterial;
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //sound effect caller
            SfxManager.Instance.PlaySFX(8346);

            //plays particle effect
            other.gameObject.GetComponentInChildren<PlayerAnimatorController>().SpawnParticle(8);

            //collect
            GameManager.Instance.SetCollectableCollected(true);
            Destroy(gameObject);
        }
    }
}
