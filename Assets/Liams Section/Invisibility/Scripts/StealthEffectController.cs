using UnityEngine;

public class StealthEffectController : MonoBehaviour
{
    // vairables/reference to my VFX for the invisibilty spell 
    [Header("VFX")]
    public GameObject stealthCastEffect;
    public float effectDuration = 2f;
    // controls in inspector for controling the alpha level
    [Header("Visual Fading")]
    [Range(0f, 1f)] public float invisibleAlpha = 0.2f;
    public float fadeSpeed = 5f;

    private Renderer[] renderers;
    private float targetAlpha = 1f;
    private bool isFading = false;
    // gets the render component to change alpha 
    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }
    // function to run the invisibility effects 
    public void StartStealthEffects()
    {
        // play VFX
        if (stealthCastEffect)
        {
            GameObject fx = Instantiate(stealthCastEffect, transform.position, Quaternion.identity);
            Destroy(fx, effectDuration);
        }

        // start fade out
        targetAlpha = invisibleAlpha;
        isFading = true;
    }
    // function for ending invisibility effects
    public void EndStealthEffects()
    {
        // start fade in
        targetAlpha = 1f;
        isFading = true;
    }
    // in updates check to see if stealths been active and if it has changes the alpha for all materials to make invis 
    private void Update()
    {
        if (!isFading) return;

        foreach (Renderer r in renderers)
        {
            foreach (Material mat in r.materials)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color color = mat.color;
                    float newAlpha = Mathf.Lerp(color.a, targetAlpha, Time.deltaTime * fadeSpeed);
                    mat.color = new Color(color.r, color.g, color.b, newAlpha);

                    // if close to targetAlpha, stop fading
                    if (Mathf.Abs(newAlpha - targetAlpha) < 0.01f)
                    {
                        mat.color = new Color(color.r, color.g, color.b, targetAlpha);
                        isFading = false;
                    }
                }
            }
        }
    }
}
