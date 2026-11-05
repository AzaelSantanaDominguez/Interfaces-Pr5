using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRBaseInteractable))]
public class UIOnGrab : MonoBehaviour
{
    public TMP_Text label;
    XRBaseInteractable interactable;
    int grabs;

    void Awake() => interactable = GetComponent<XRBaseInteractable>();

    void OnEnable()
    {
        interactable.selectEntered.AddListener(OnGrab);
        interactable.selectExited.AddListener(OnRelease);
    }

    void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnGrab);
        interactable.selectExited.RemoveListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        grabs++;
        label.text = $"Agarrado: {name}\nVeces: {grabs}";
    }

    void OnRelease(SelectExitEventArgs args)
    {
        label.text = $"Soltado: {name}\nVeces: {grabs}";
    }
}