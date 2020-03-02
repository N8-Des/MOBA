using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class AbilityIndicator : MonoBehaviour {
    public Image picture;
    public float timeLeft;
    public float maxTime;
    public int levelNum;
    public GameObject levelInd;
    public GameObject abilityDescription;
    public Text AbilityName;
    public Text AbilityCooldown;
    public PlayerBase player;
    public Text cooldownSec;
    float cooldownForText;
    void Start()
    {
        picture = gameObject.GetComponent<Image>();
        player = gameObject.transform.parent.gameObject.GetComponentInParent<PlayerBase>();
    }

    public void StartCooldown(float cooldown, PlayerBase player, int abnum)
    {
        maxTime = cooldown;
        picture.fillAmount = 0;
        maxTime = cooldown;
        StartCoroutine(Cooldown(player, abnum));
    }
    IEnumerator Cooldown(PlayerBase player, int abnum)
    {
        timeLeft = 0;
        while (timeLeft < maxTime || picture.fillAmount != 1)
        {
            cooldownSec.gameObject.SetActive(true);
            cooldownForText = maxTime - timeLeft;
            cooldownForText = Mathf.Round(cooldownForText * 10) / 10;
            cooldownSec.text = cooldownForText.ToString();
            timeLeft += Time.deltaTime;
            float ratio = timeLeft / maxTime;
            picture.fillAmount = ratio;
            yield return new WaitForEndOfFrame();
        }
        cooldownSec.gameObject.SetActive(false);
        if (abnum == 1)
        {
            player.CooldownQ = true;
        } else if (abnum == 2)
        {
            player.CooldownW = true;
        } else if (abnum == 3)
        {
            player.CooldownE = true;
        }
        else if (abnum == 4)
        {
            player.CooldownR = true;
        }
    }
    public void chooseLevelUp(int abilNum)
    {
        if (levelNum >= 5)
        {
            //AAAAAHHH
        }
        else
        {
            levelNum += 1;
            player.LevelUp();
            player.updateDamage(abilNum, levelNum);
        }
    }
    public void updateCooldownNum(float cooldown)
    {
        if (cooldown > 0)
        {
            AbilityCooldown.text = cooldown.ToString() + "s cooldown";
        }
        else
        {
            AbilityCooldown.text = "Passive";
        }
    }
    public void updateAbilityName(string name)
    {
        AbilityName.text = name;
    }
    public void MouseEnter()
    {
        abilityDescription.SetActive(true);
    }
    public void MouseExit()
    {
        abilityDescription.SetActive(false);
    }
    public void updateAbilityDescription(string description)
    {
        abilityDescription.GetComponentInChildren<Text>().text = description;
    }
    public void levelUp()
    {
        levelInd.SetActive(true);
    }
    public void endLevelUp()
    {
        levelInd.SetActive(false);
    }
}
