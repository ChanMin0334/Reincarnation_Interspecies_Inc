using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealData
{
    public Entity Healer;
    public BigNumeric Value;

    public HealData(Entity healer, BigNumeric value)
    {
        Healer = healer;
        Value = value;
    }
}
