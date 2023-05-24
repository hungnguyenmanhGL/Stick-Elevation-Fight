using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IPlayerInputListener {
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

    //already translated to world position
    public void OnPointerClick(PointerEventData eventData) {
        if (listener != null) {
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(eventData.position);
            listener.OnClicked(worldPosition);
        }
    }
}
