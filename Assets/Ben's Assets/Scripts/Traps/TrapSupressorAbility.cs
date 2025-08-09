using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PeekabooPro.UI;

public class TrapSupressorAbility : MonoBehaviour
{
    [Header("UI hookup (optional — can auto-find)")]
    [SerializeField] private SpellBarManager spellBar;

    [Header("Input")]
    [SerializeField] private KeyCode key = KeyCode.E;

    [Header("Effect")]
    [SerializeField] private float radius = 6f;
    [SerializeField] private float suppressDuration = 5f;
    [SerializeField] private LayerMask trapMask = ~0;

    private bool TryResolveSpellBar()
    {
        if (spellBar != null) return true;

        // If spell bar not set in inspector, find by tag
        var byTag = GameObject.FindWithTag("SpellBar");
        if (byTag) spellBar = byTag.GetComponentInChildren<SpellBarManager>();

        // Last resort scan for spell bar if not tagged or added to inspector
        if (!spellBar) spellBar = FindObjectOfType<SpellBarManager>(true);

        return spellBar != null;
    }

    void Update()
    {
        if (Input.GetKeyDown(key)) TryCast();
    }

    private void TryCast()
    {
        if (!TryResolveSpellBar())
        {
            Debug.LogWarning("TrapSuppressorAbility: No SpellBarManager found.");
            return;
        }

        var eSlot = spellBar.slotE; // Uses existing E slot
        if (eSlot == null)
        {
            Debug.LogWarning("TrapSuppressorAbility: SpellBarManager.slotE is not assigned.");
            return;
        }

        if (eSlot.IsOnCooldown()) return;  // Gate on cooldown (from SpellSlot API)

        // Suppress nearby traps
        var hits = Physics.OverlapSphere(transform.position, radius, trapMask);
        var processed = new HashSet<ITrap>();
        foreach (var h in hits)
        {
            var trap = h.GetComponentInParent<ITrap>();
            if (trap != null && processed.Add(trap))
                trap.Suppress(suppressDuration);
        }

        // Start E slot cooldown so the UI updates
        eSlot.TriggerCooldown();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.3f, 0.9f, 1f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
