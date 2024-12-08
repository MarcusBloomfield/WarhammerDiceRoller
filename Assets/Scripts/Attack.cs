using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack
{
    public List<RollModifier> rollModifiers;
    public string owner;
    public string name;
    public int attacks;
    public int skill;
    public int strength;
    public int armorPenetration;
    public int damage;
}
