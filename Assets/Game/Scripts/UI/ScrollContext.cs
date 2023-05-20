using System;

[System.Serializable]
public class ScrollContext {
    public int SelectedIndex = -1;
    public Action<int> OnCellClicked;
}
