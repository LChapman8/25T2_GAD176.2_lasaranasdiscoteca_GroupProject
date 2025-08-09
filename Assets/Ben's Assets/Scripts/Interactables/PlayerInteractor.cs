using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float scanRadius = 2.5f;
    [SerializeField] private LayerMask interactableMask = ~0;

    [Header("Input")]
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [Header("UI")]
    [SerializeField] private WorldPromptController promptPrefab;

    private IInteractable _current;
    private WorldPromptController _prompt;
    private InteractablePrompt _interactable;
    private Camera _cam;

    void Awake()
    {
        _cam = Camera.main; // or inject
        _prompt = Instantiate(promptPrefab);
        _prompt.Initialize(_cam);
        _interactable = _prompt.GetComponent<InteractablePrompt>();
        _prompt.gameObject.SetActive(false);
    }

    void Update()
    {
        FindBestInteractable();

        if (_current != null)
        {
            var mb = (_current as MonoBehaviour);
            var baseInteractable = mb as InteractableBase;
            Transform anchor = (baseInteractable != null) ? baseInteractable.PromptAnchor : mb.transform;

            // Attach & show
            _interactable.Attach(anchor, _cam);
            _prompt.Bind(anchor, _current.PromptText);
        }
        else
        {
            // Hide
            _prompt.Unbind();
            _interactable.Detach();
        }

        if (_current != null && Input.GetKeyDown(interactKey) && _current.CanInteract(gameObject))
        {
            _current.Interact(gameObject);
        }
    }

    void FindBestInteractable()
    {
        _current = null;

        var hits = Physics.OverlapSphere(transform.position, scanRadius, interactableMask);
        float bestScore = float.MaxValue;

        foreach (var col in hits)
        {
            if (!col) continue;
            var interactable = col.GetComponentInParent<IInteractable>();
            if (interactable == null) continue;

            if (!interactable.CanInteract(gameObject)) continue;

            // score by distance (closest wins)
            float dist = (col.transform.position - transform.position).sqrMagnitude;
            if (dist < bestScore)
            {
                bestScore = dist;
                _current = interactable;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, scanRadius);
    }
#endif
}
