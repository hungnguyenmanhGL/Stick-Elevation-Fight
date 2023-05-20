using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClosePanel : MonoBehaviour
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private Panel panel;

    private void Awake() {
        panel = GetComponentInParent<Panel>();
        if (!closeBtn) {
            closeBtn = GetComponent<Button>();
        }
        if (closeBtn) closeBtn.onClick.AddListener(HidePanel);
    }

    public void HidePanel() {
        if (panel) {
            panel.Hide();
        }
    }
}
