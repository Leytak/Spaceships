using System;
using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Serializable]
    public struct Stats
    {
        public int Damage;
        public float Cooldown;
    }
    
    [SerializeField] private Stats baseStats;

    private Stats currentsStats;
    private IEnumerator shootingRoutine;

    private void Awake()
    {
        currentsStats = baseStats;
    }

    public void SetDecreasedCooldown(int percents)
    {
        currentsStats.Cooldown = Percents.Sub(baseStats.Cooldown, percents);
    }

    public string GetInfo() => $"Damage: {currentsStats.Damage}, Cooldown: {currentsStats.Cooldown}";

    public void StartShooting(Spaceship target)
    {
        CancelShooting();
        shootingRoutine = ShootingLoop(target);
        StartCoroutine(shootingRoutine);
    }
    
    public void CancelShooting()
    {
        if (shootingRoutine is not null)
            StopCoroutine(shootingRoutine);
    }

    private IEnumerator ShootingLoop(Spaceship target)
    {
        var cooldown = new WaitForSeconds(currentsStats.Cooldown);
        while (target.IsAlive())
        {
            target.Hit(currentsStats.Damage);
            yield return cooldown;
        }
    }
}
