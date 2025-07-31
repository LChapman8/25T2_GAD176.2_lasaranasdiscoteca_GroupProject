using UnityEngine;

namespace Peekaboopro.Invisibility
{
    public class InvisibilitySpell : MonoBehaviour
    {
        // variables for turning invisible/cooldowns
        [Header("Invisibility Settings")]
        public float invisibilityDuration = 5f;
        public float cooldownTime = 10f;

        private bool isOnCooldown = false;
        // reference to my stealth state script
        private PlayerStealthState stealthState;

        // on awake get the player stealth state scrupt/make sure it exists 
        private void Awake()
        {
            stealthState = GetComponent<PlayerStealthState>();
            if (stealthState == null)
            {
                Debug.LogError("InvisibilitySpell: No PlayerStealthState found on the GameObject.");
            }
        }
        // on update check if the player press "Q" if they do, cast invisibility spell
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q) && !isOnCooldown)
            {
                ActivateInvisibility();
            }
        }
        // function for casting invisibility
        private void ActivateInvisibility()
        {
            isOnCooldown = true;
            stealthState.EnterStealth(invisibilityDuration, cooldownTime);
            Invoke(nameof(EndInvisibility), invisibilityDuration);
            Invoke(nameof(ResetCooldown), cooldownTime);
        }
        // function for ending invisibility
        private void EndInvisibility()
        {
            stealthState.ExitStealth();
        }
        // function for reseting the cooldown
        private void ResetCooldown()
        {
            isOnCooldown = false;
        }
    }
}
