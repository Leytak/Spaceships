using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Weapon : MonoBehaviour
{
    [Serializable]
    public struct Stats
    {
        public int Damage;
        public float Cooldown;
    }
    
    [SerializeField] private Stats baseStats;
    [SerializeField] private float shotTrailTime;

    private Stats currentsStats;
    private IEnumerator shootingRoutine;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
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
            StartCoroutine(AnimateShot(target.transform.position));
            yield return cooldown;
        }
    }

    private IEnumerator AnimateShot(Vector3 targetPosition)
    {
        lineRenderer.SetPosition(1, transform.InverseTransformPoint(targetPosition));
        yield return new WaitForSeconds(shotTrailTime);
        lineRenderer.SetPosition(1, lineRenderer.GetPosition(0));
    }
}
