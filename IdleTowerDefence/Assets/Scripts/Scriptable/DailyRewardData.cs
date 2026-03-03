using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DailyRewardData", menuName = "DailyReward/Daily Reward Data")]
public class DailyRewardData : ScriptableObject
{
    public int startingGold = 50;
    public int startingDiamond = 0;
    public int startingCrystal = 0;
    
    public int goldPerDay = 100;
    public int diamondPerDay = 5;
    public int crystalPerDay = 10;
}
