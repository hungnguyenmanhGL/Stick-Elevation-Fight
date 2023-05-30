using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelFactory : MonoBehaviour
{
    [Header("Home Scene Refs")]
    [SerializeField] private Panel homePanel;
    [SerializeField] private Panel spinPanel;

    [Header("Game Scene Refs")]
    [SerializeField] private Panel pausePanel;
    [SerializeField] private Panel inventoryPanel;
    [SerializeField] private Panel resultPanel;

    [Header("Recycled Refs")]
    private Panel reHomePanel;
    private Panel reSpinPanel;

    private Panel rePausePanel;
    private Panel reInventoryPanel;
    private Panel reResultPanel;

    public Panel GetPanel(PanelEnum panelEnum) {
        switch (panelEnum) {
            case PanelEnum.home:
                Panel home = reHomePanel;
                if (!home) home = Instantiate(homePanel);
                return home;

            case PanelEnum.spin:
                Panel spin = reSpinPanel;
                if (!spin) spin = Instantiate(spinPanel);
                return spin;

            case PanelEnum.result:
                Panel result = reResultPanel;
                if (!result) result = Instantiate(resultPanel);
                return result;

            case PanelEnum.pause:
                Panel pause = rePausePanel;
                if (!pause) pause = Instantiate(pausePanel);
                return pause;

            case PanelEnum.inventory:
                Panel inventory = reInventoryPanel;
                if (!inventory) inventory = Instantiate(inventoryPanel);
                return inventory;

            default:
                return null;
        }
    }

    public void RecyclePanel(Panel panel) {
        if (panel is PausePanel pause) {
            rePausePanel = pause;
            return;
        }

        if (panel is InventoryPanel inventory) {
            reInventoryPanel = inventory;
            return;
        }
    }
}
