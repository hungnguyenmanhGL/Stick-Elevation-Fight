using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static HUD instance;

    [SerializeField] private PanelFactory panelFactory;

    private Stack<Panel> panelStack = new Stack<Panel>();

    private void Awake() {
        if (!instance) instance = this;
        else Destroy(this.gameObject);

        if (!panelFactory) {
            panelFactory = GetComponent<PanelFactory>();
            if (!panelFactory) Debug.LogWarning("Can't find PanelHolder on this!");
        }
    }

    public void HidePanel(Panel panel) {
        if (!panel) return;
        if (panelStack.Peek() != panel) {
            Debug.LogError("Hidden Panel is not on top");
            return;
        }
        panelStack.Pop();
        panelFactory.RecyclePanel(panel);
    }

    public Panel Push(PanelEnum panelEnum) {
        Panel panel = panelFactory.GetPanel(panelEnum);
        panelStack.Push(panel);

        panel.RectTransform.SetParent(this.transform, false);
        panel.gameObject.SetActive(true);
        return panel;
    } 

    public Panel Peek() {
        if (panelStack.Count > 0) return panelStack.Peek();
        else return null;
    }
}

