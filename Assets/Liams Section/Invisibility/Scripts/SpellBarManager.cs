using UnityEngine;

namespace PeekabooPro.UI
{
    /// <summary>
    /// This script is responsible for managing the spellslots on UI spell bar.
    /// </summary>
    
    public class SpellBarManager : MonoBehaviour
    {
        public SpellSlot slotQ;
        public SpellSlot slotE;
        public SpellSlot slotR;

        public Sprite invisibilityIcon;
        public float invisibilityCooldown = 10f;

        public Sprite disarmIcon;
        public float disarmCooldown = 12f;

        public Sprite throwItemIcon;
        public float throwItemCooldown = 5f;

        // on start, set all spell slots
        void Start()
        {
            slotQ.SetSpell(invisibilityIcon, "Q", invisibilityCooldown);
            slotE.SetSpell(disarmIcon, "E", disarmCooldown);
            slotR.SetSpell(throwItemIcon, "R", throwItemCooldown);
        }

        // function for triggering the "Q, and R" cooldowns
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                slotQ.TriggerCooldown();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                slotE.TriggerCooldown();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                slotR.TriggerCooldown();
            }
        }
    }
}
