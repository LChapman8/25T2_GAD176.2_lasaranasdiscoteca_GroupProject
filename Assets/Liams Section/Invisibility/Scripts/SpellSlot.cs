using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PeekabooPro.UI;

public class SpellSlot : MonoBehaviour
{
    // vairables for images and texts of the spell 
    public Image iconImage;
    public TextMeshProUGUI hotkeyText;
    public Image cooldownOverlay;
    public TextMeshProUGUI cooldownText;
    // variables to manage cooldowns 
    private float cooldownTime;
    private float cooldownTimer;
    private bool isOnCooldown;

    // function for setting the spells icon, hotkey and cooldown
    public void SetSpell(Sprite icon, string hotkey, float cooldown)
    {
        iconImage.sprite = icon;
        hotkeyText.text = hotkey.ToUpper();
        cooldownTime = cooldown;

        cooldownOverlay.fillAmount = 0f;
        cooldownText.gameObject.SetActive(false);
        isOnCooldown = false;
    }

    // starts the cooldown if it's not already running
    public void TriggerCooldown()
    {
        // prevent triggering again during cooldown
        if (isOnCooldown) return; 

        isOnCooldown = true;
        cooldownTimer = cooldownTime;
        cooldownOverlay.fillAmount = 1f;
        cooldownText.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!isOnCooldown)
            return;

        cooldownTimer -= Time.deltaTime;
        cooldownOverlay.fillAmount = cooldownTimer / cooldownTime;
        cooldownText.text = Mathf.Ceil(cooldownTimer).ToString();

        if (cooldownTimer <= 0f)
        {
            isOnCooldown = false;
            cooldownOverlay.fillAmount = 0f;
            cooldownText.gameObject.SetActive(false);
        }
    }

    // exposes the cooldown status to other scripts
    public bool IsOnCooldown()
    {
        return isOnCooldown;
    }
}
