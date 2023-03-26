using System.Collections;
using UnityEngine;

public partial class Spaceship
{
    [Header("Animations")]
    [SerializeField]
    private float idleMaxAmplitude = 0.5f;
    [SerializeField]
    private float idleMoveSpeed = 1f;
    [SerializeField]
    private float idleTurnSpeed = 20f;
    [SerializeField]
    private float idleTurnAngle = 3f;

    private IEnumerator idleRoutine;
    
    private void StartIdle()
    {
        CancelIdle();
        idleRoutine = Idle();
        StartCoroutine(idleRoutine);
    }

    private void CancelIdle()
    {
        if (idleRoutine is not null)
            StopCoroutine(idleRoutine);
        transform.position = new Vector3(transform.position.x, 0);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        
    }

    private IEnumerator Idle()
    {
        var position = transform.position;
        var eulerAngles = transform.eulerAngles;
        var direction = Random.value < 0.5f ? 1 : -1;
        while (IsAlive())
        {
            var idleAmplitude = Random.value * idleMaxAmplitude;
            while (position.y * direction < idleAmplitude)
            {
                eulerAngles.z = Mathf.Clamp(
                    eulerAngles.z + direction * idleTurnSpeed * Time.deltaTime,
                    -idleTurnAngle,
                    idleTurnAngle);
                var turnProgress = eulerAngles.z / idleTurnAngle;
                position.y += idleMoveSpeed * turnProgress * Time.deltaTime;
                transform.eulerAngles = eulerAngles;
                transform.position = position;
                yield return null;
            }
            direction *= -1;
            yield return null;
        }
    }
}