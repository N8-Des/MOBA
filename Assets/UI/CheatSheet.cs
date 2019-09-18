using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatSheet : MonoBehaviour {
    public PlayerBase character;
    public void addLevel()
    {
        character.abilityLevelNum += 1;
        character.level += 1;
        character.LevelUp();
    }
}
