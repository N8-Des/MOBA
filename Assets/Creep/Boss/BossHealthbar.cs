using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthbar : MonoBehaviour
{
    public Image healthbar;
    public Text bossName;
    public void takeDamage(int health, int maxHealth)
    {
        float div = (float)health / (float)maxHealth;
        healthbar.fillAmount = div;
    }
}
