using UnityEngine;

public class CarHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;

    private void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        Debug.Log("Car HP: " + currentHP);

        if (currentHP <= 0)
        {
            Debug.Log("Car destroyed!");
        }
    }
}
