using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IPlayerInputListener {
    //already translated to world position
    void OnClicked(Vector3 position);
    
}

public class PlayerInputListener : MonoBehaviour, IPointerClickHandler {
    private IPlayerInputListener listener;
    private Camera mainCamera;

    private void Awake() {
        mainCamera = Camera.main;
    }

    public void SetListener(IPlayerInputListener listener) {
        this.listener = listener;
    }

    //only called when clicked on game objects with collider
    public void OnPointerClick(PointerEventData eventData) {
        if (listener != null) {
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(eventData.position);
            listener.OnClicked(worldPosition);
        }
    }
}
