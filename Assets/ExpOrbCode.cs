using UnityEngine;

public class ExpOrbCode : MonoBehaviour
{
    private int expAmount;
    
    public void SetExpAmount(int amount)
    {
        expAmount = amount;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerScript playerLevel = other.GetComponent<PlayerScript>();
            if (playerLevel != null)
            {
                playerLevel.AddExperience(expAmount);
                Destroy(gameObject);
            }
        }
    }
}