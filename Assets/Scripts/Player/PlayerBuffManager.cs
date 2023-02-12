using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffManager : MonoBehaviour
{
    public PlayerController playerController;
    public PlayerFireControl fireControl;

    private float defaultSpeed;

    private Coroutine speedBuff;
    private Coroutine chargeBuff;

    public enum PlayerBuffs
    {
        moveSpeed,
        chargeRate
    }

    public void BuffPlayer(PlayerBuffs type, float duration, float multiplier)
    {
        switch (type)
        {
            case PlayerBuffs.moveSpeed:
                if (speedBuff != null)
                {
                    StopCoroutine(speedBuff);
                }
                speedBuff = StartCoroutine(SpeedBuff(duration, multiplier));
                break;

            case PlayerBuffs.chargeRate:
                if (chargeBuff != null)
                {
                    StopCoroutine(chargeBuff);
                }
                chargeBuff = StartCoroutine(ChargeRateBuff(duration, multiplier));
                break;

            default:
                break;
        }
    }

    private IEnumerator SpeedBuff(float duration, float multiplier)
    {
        playerController.moveSpeed = defaultSpeed * multiplier;
        yield return new WaitForSecondsRealtime(duration);
        playerController.moveSpeed = defaultSpeed;
        speedBuff = null;
        yield return null;
    }

    private IEnumerator ChargeRateBuff(float duration, float multiplier)
    {
        fireControl.chargeRate = multiplier;
        yield return new WaitForSecondsRealtime(duration);
        fireControl.chargeRate = 1.0f;
        chargeBuff = null;
        yield return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        defaultSpeed = playerController.moveSpeed;

        playerController = GetComponent<PlayerController>();
        fireControl = GetComponent<PlayerFireControl>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
