using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRBaseInteractable))]
public class ColorOnSelect : MonoBehaviour
{
    Renderer rend;
    XRBaseInteractable interactable;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        interactable = GetComponent<XRBaseInteractable>();
    }

    void OnEnable()  => interactable.selectEntered.AddListener(OnSelect);
    void OnDisable() => interactable.selectEntered.RemoveListener(OnSelect);

    void OnSelect(SelectEnterEventArgs args)
    {
        rend.material.color = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.8f, 1f);
    }
}