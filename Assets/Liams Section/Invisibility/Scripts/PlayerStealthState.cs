using UnityEngine;

public class PlayerStealthState : MonoBehaviour
{
    // variables for invisibility 
    private bool isStealthed = false;
    private float currentCooldown = 0f;
    private float stealthTimer = 0f;
    // reference to stealth controller script
    private StealthEffectController effectController;
    // coroutines for stealth cooldowns 
    private Coroutine stealthCountdownCoroutine;
    private Coroutine cooldownCountdownCoroutine;

    // get reference on awake to the stealth controller 
    private void Awake()
    {
        effectController = GetComponent<StealthEffectController>();
        if (!effectController)
            Debug.LogWarning("No StealthEffectController found on player.");
    }
    // on update if the target is stealthed  run a timer, when it hits 0 end stealth and start cooldown 
    private void Update()
    {
        if (isStealthed)
        {
            stealthTimer -= Time.deltaTime;
            if (stealthTimer <= 0f)
            {
                ExitStealth();
            }
        }
        else if (currentCooldown > 0f)
        {
            currentCooldown -= Time.deltaTime;
        }
    }
    // a function for checking if stealth is available 
    public bool CanEnterStealth()
    {
        return !isStealthed && currentCooldown <= 0f;
    }
    // a function for entering stealth 
    public void EnterStealth(float duration, float cooldown)
    {
        if (!CanEnterStealth()) return;

        isStealthed = true;
        stealthTimer = duration;
        currentCooldown = cooldown;

        effectController?.StartStealthEffects();

        Debug.Log("Player entered stealth.");

        // start countdown log for the coroutines for stealth time
        if (stealthCountdownCoroutine != null) StopCoroutine(stealthCountdownCoroutine);
        stealthCountdownCoroutine = StartCoroutine(StealthCountdownLog());
    }
    // a function for exiting stealth 
    public void ExitStealth()
    {
        if (!isStealthed) return;

        isStealthed = false;

        effectController?.EndStealthEffects();

        Debug.Log("Player exited stealth.");

        // Start cooldown countdown log coroutine
        if (cooldownCountdownCoroutine != null) StopCoroutine(cooldownCountdownCoroutine);
        cooldownCountdownCoroutine = StartCoroutine(CooldownCountdownLog());
    }
    // a function for if the player is stealthed 
    public bool IsStealthed()
    {
        return isStealthed;
    }
    // a function for remaining cooldown
    public float CooldownRemaining()
    {
        return Mathf.Max(currentCooldown, 0f);
    }
    // a debug log countdown of the time left in stealth 
    private System.Collections.IEnumerator StealthCountdownLog()
    {
        int secondsLeft = Mathf.CeilToInt(stealthTimer);
        while (secondsLeft > 0)
        {
            Debug.Log($"Player stealth ends in {secondsLeft}...");
            yield return new WaitForSeconds(1f);
            secondsLeft--;
        }
    }
    // a countdown that lets you know when stealth is available again
    private System.Collections.IEnumerator CooldownCountdownLog()
    {
        int secondsLeft = Mathf.CeilToInt(currentCooldown);
        while (secondsLeft > 0)
        {
            yield return new WaitForSeconds(1f);
            secondsLeft--;
        }
        Debug.Log("Stealth spell is available.");
    }
}
