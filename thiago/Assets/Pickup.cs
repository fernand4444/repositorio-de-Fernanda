using UnityEngine;

public class Pickup : MonoBehaviour
{
    [Tooltip("Value awarded when collected")]
    public int scoreValue = 1;
    [Tooltip("Optional effect to instantiate on collect")]
    public GameObject collectEffect;

    void Reset()
    {
        // Ensure this object has a trigger collider by default
        var col = GetComponent<Collider>();
        if (col == null)
        {
            gameObject.AddComponent<BoxCollider>().isTrigger = true;
        }
        else
        {
            col.isTrigger = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (collectEffect != null)
                Instantiate(collectEffect, transform.position, Quaternion.identity);

            // Simple behavior: deactivate the pickup
            gameObject.SetActive(false);

            // Optionally notify a GameManager via messaging (if exists)
            // GameObject.FindObjectOfType<GameManager>()?.AddScore(scoreValue);
        }
    }
}


