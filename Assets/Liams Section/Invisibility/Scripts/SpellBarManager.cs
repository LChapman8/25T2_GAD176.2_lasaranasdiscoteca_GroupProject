using UnityEngine;

namespace PeekabooPro.UI
{
    // UI spell manager for stealth mechanics being added
    public class SpellBarManager : MonoBehaviour
    {
        public SpellSlot slotQ;
        public SpellSlot slotE;
        public SpellSlot slotF;

        public Sprite invisibilityIcon;
        public float invisibilityCooldown = 10f;

        public Sprite disarmIcon;
        public float disarmCooldown = 12f;

        void Start()
        {
            slotQ.SetSpell(invisibilityIcon, "Q", invisibilityCooldown);
            slotE.SetSpell(disarmIcon, "E", disarmCooldown);

            // add in E and R spells respectively for your own abilities 
        }

        // function for triggering the "Q" cooldown 
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                slotQ.TriggerCooldown();
            }
        }
    }
}
