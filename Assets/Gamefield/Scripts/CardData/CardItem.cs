﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardItem
{
    public int index;
    public string cardName;
    public Sprite artwork;
    public Sprite background;
    public int health;
    public int defence;
    public int damage;
    public int energy;
    public bool isLegendary;

}
[System.Serializable]
public class CardTreit
{
    public int index;
    public string cardName;
    public Sprite artwork;
    public Sprite background;
    public int healthModifier;
    public int damageModifier;
    public int defenceModifier;
    public int energy;
}