using System;
using UnityEngine;

// RPG is the main namespace
// for our game
namespace RPG.Example
{
    public class Robot : MonoBehaviour
    {
        private BatteryRegulations includedBattery;

        public Robot()
        {
            includedBattery = new Battery(80f);
            includedBattery.CheckHealth();
            Charger.ChargeBattery(includedBattery);
            includedBattery.CheckHealth();
            print(Charger.chargerInUse);
        }
    }

    public class Battery : BatteryRegulations
    {
        public Battery(float newHealth) : base(newHealth) { }

        public override void CheckHealth()
        {
            Debug.Log(health);
        }
    }

    static class Charger
    {
        public static bool chargerInUse = false;

        public static void ChargeBattery(BatteryRegulations batteryToCharge)
        {
            chargerInUse = true;
            batteryToCharge.health = 100f;
        }
    }

    public abstract class BatteryRegulations
    {
        public float health;

        public BatteryRegulations(float newHealth)
        {
            health = newHealth;
            Debug.Log("New battery created!");
        }

        public abstract void CheckHealth();
    }
}