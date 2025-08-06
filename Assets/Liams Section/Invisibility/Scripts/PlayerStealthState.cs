using System;
using UnityEngine;
using Peekaboopro.Invisibility;

public class PlayerStealthState : MonoBehaviour
{
    // variables for invisibility
    private bool isStealthed = false;
    private float currentCooldown = 0f;
    private float stealthTimer = 0f;

    // reference to stealth effect controller script
    private StealthEffectController effectController;

    // coroutines for stealth cooldowns
    private Coroutine stealthCountdownCoroutine;
    private Coroutine cooldownCountdownCoroutine;

    // events to notify when stealth starts or ends
    public event Action OnStealthStarted;
    public event Action OnStealthEnded;

    // public properties for external access
    public bool IsStealthed => isStealthed;
    public float CooldownRemaining => Mathf.Max(currentCooldown, 0f);
    public float StealthTimeRemaining => Mathf.Max(stealthTimer, 0f);

    private void Awake()
    {
        effectController = GetComponent<StealthEffectController>();
        if (!effectController)
            Debug.LogWarning("No StealthEffectController found on player.");
    }

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

    // check if stealth can be entered
    public bool CanEnterStealth()
    {
        return !isStealthed && currentCooldown <= 0f;
    }

    // enter stealth with an optional duration and cooldown (it will default to last used if none there)
    public void EnterStealth(float duration = 5f, float cooldown = 10f)
    {
        if (!CanEnterStealth()) return;

        isStealthed = true;
        stealthTimer = duration;
        currentCooldown = cooldown;

        effectController?.StartStealthEffects();

        Debug.Log("Player entered stealth.");

        // Fire event
        OnStealthStarted?.Invoke();

        if (stealthCountdownCoroutine != null)
            StopCoroutine(stealthCountdownCoroutine);
        stealthCountdownCoroutine = StartCoroutine(StealthCountdownLog());
    }

    // exit stealth
    public void ExitStealth()
    {
        if (!isStealthed) return;

        isStealthed = false;

        effectController?.EndStealthEffects();

        Debug.Log("Player exited stealth.");

        
        OnStealthEnded?.Invoke();

        if (cooldownCountdownCoroutine != null)
            StopCoroutine(cooldownCountdownCoroutine);
        cooldownCountdownCoroutine = StartCoroutine(CooldownCountdownLog());
    }

    // clean up coroutines on disable
    private void OnDisable()
    {
        if (stealthCountdownCoroutine != null)
            StopCoroutine(stealthCountdownCoroutine);
        if (cooldownCountdownCoroutine != null)
            StopCoroutine(cooldownCountdownCoroutine);
    }

    // debug log countdown for stealth time left
    private System.Collections.IEnumerator StealthCountdownLog()
    {
        int secondsLeft = Mathf.CeilToInt(stealthTimer);
        while (secondsLeft > 0 && isStealthed)
        {
            Debug.Log($"Player stealth ends in {secondsLeft}...");
            yield return new WaitForSeconds(1f);
            secondsLeft--;
        }
    }

    // debug log countdown for cooldown time left
    private System.Collections.IEnumerator CooldownCountdownLog()
    {
        int secondsLeft = Mathf.CeilToInt(currentCooldown);
        while (secondsLeft > 0 && !isStealthed)
        {
            yield return new WaitForSeconds(1f);
            secondsLeft--;
            Debug.Log($"Stealth Spell Cooldown Remaning - {secondsLeft}");
        }
        Debug.Log("Stealth spell is available.");
    }
}

