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

        void Start()
        {
            slotQ.SetSpell(invisibilityIcon, "Q", invisibilityCooldown);
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
