using UnityEngine;

public class InvisInstructionsUI : MonoBehaviour
{
    [Header("UI Panel for instructions")]
    public GameObject instructionsPanel;

    private void Start()
    {
        // show instructions at the start
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(true);
            // pause the game while instructions are visible
            Time.timeScale = 0f;
        }
    }

    private void Update()
    {
        // hide instructions and resume game when player presses X
        if (instructionsPanel.activeSelf && Input.GetKeyDown(KeyCode.X))
        {
            instructionsPanel.SetActive(false);
            // resume the game
            Time.timeScale = 1f;
        }
    }
}
