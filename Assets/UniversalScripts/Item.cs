using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour {
    public GameManager gameManager;
    public string itemName;
    public int abilityPower;
    public int attackSpeed;
    public int attackDamage;
    public int cooldownReduction;
    public int health;
    public int armor;
    public int magicResist;
    public int moveSpeed;
    public int gold;
    public List<Item> build = new List<Item>();
    public string passiveEffects;
    public string lore;
    public GameObject description;
    public GameObject Tree;
    public Text itName;
    public Text statsText;
    public Text passiveText;
    public Text loreText;
    public Text goldText;
    public ShopUI shopControl;
    bool oneClick;
    float doubleTime;
    bool usingDouble;
    int newGold;
    public bool isChecked;
    public void MouseEnter()
    {
        description.SetActive(true);
        itName.text = itemName;
        statsText.text = "";
        if (abilityPower != 0)
        {
            statsText.text += "\n+" + abilityPower.ToString() + " Ability Power";
        }
        if (attackSpeed != 0)
        {
            statsText.text += "\n+" + attackSpeed.ToString() + "% Attack Speed";
        }
        if (attackDamage != 0)
        {
            statsText.text += "\n+" + attackDamage.ToString() + " Attack Damage";
        }
        if (cooldownReduction != 0)
        {
            statsText.text += "\n+" + cooldownReduction.ToString() + "% CDR";
        }
        if (health != 0)
        {
            statsText.text += "\n+" + health.ToString() + " Health";
        }
        if (armor != 0)
        {
            statsText.text += "\n+" + armor.ToString() + " Armor";
        }
        if (magicResist != 0)
        {
            statsText.text += "\n+" + magicResist.ToString() + " Magic Resist";
        }
        if (moveSpeed != 0)
        {
            statsText.text += "\n+" + moveSpeed.ToString() + " Movespeed";
        }
        if (passiveEffects != null)
        {
            passiveText.text = passiveEffects;
        }
        if (lore != null)
        {
            loreText.text = lore;
        }
        checkPrice();
    }
    public void MouseExit()
    {
        description.SetActive(false);
    }

    void checkPrice()
    {
        newGold = gold;
        foreach (Item item in build)
        {
            foreach (Item playerItem in gameManager.acplayer.items)
            {
                if (string.Equals(item.gameObject.name + "(Clone)", playerItem.gameObject.name) && !playerItem.isChecked)
                {
                    playerItem.isChecked = true;
                    newGold -= playerItem.gold;
                }
            }
        }
        foreach (Item playerItem in gameManager.acplayer.items)
        {
            playerItem.isChecked = false;
        }
        goldText.text = newGold.ToString() + "G";
    }
    IEnumerator treeDelay()
    {
        yield return new WaitForSeconds(0.1f);
        if (Tree != null && !usingDouble)
        {
            Tree.SetActive(true);
            shopControl.openTree();
            usingDouble = false;
            oneClick = false;
            if (gameObject.activeInHierarchy== true)
            {
                gameObject.SetActive(false);
            }
        }
    }
    public void activateTree()
    {
        if(gameObject.activeInHierarchy == false)
        {
            gameObject.SetActive(true);
        }
        if (!oneClick)
        {
            oneClick = true;
            usingDouble = false;
            StartCoroutine(treeDelay());
        }
        else
        {
            checkPrice();
            gameManager.acplayer.buyItem(this, newGold);
            oneClick = false;
            usingDouble = true;
        }
    }
}
