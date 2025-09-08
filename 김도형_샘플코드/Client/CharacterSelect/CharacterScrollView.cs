using System;
using FancyScrollView;
using System.Collections.Generic;
using CharacterSelect;
using UnityEngine;
using EasingCore;

public class CharacterScrollView : FancyScrollView<CharacterData, Context>
{
    [SerializeField] Scroller scroller = default;
    [SerializeField] GameObject cellPrefab = default;
    
    public bool IsLocked { get; set; } = false;

    Action<int> onSelectionChanged;

    protected override GameObject CellPrefab => cellPrefab;

    protected override void Initialize()
    {
        base.Initialize();

        Context.OnCellClicked = index =>
        {
            if (!IsLocked)
            {
                SelectCell(index);
            }
        };
        scroller.OnValueChanged(UpdatePosition);
        scroller.OnSelectionChanged(UpdateSelection);
    }

    void UpdateSelection(int index)
    {
        if (IsLocked) return;
        if (Context.SelectedIndex == index)
        {
            return;
        }

        Context.SelectedIndex = index;
        Refresh();

        onSelectionChanged?.Invoke(index);
    }

    public void UpdateData(IList<CharacterData> items)
    {
        UpdateContents(items);
        scroller.SetTotalCount(items.Count);
    }

    public void OnSelectionChanged(Action<int> callback)
    {
        onSelectionChanged = callback;
    }

    public void SelectNextCell()
    {
        var next = (Context.SelectedIndex + 1) % ItemsSource.Count;
        SelectCell(next);
    }

    public void SelectPrevCell()
    {
        var prev = (Context.SelectedIndex - 1 + ItemsSource.Count) % ItemsSource.Count;
        SelectCell(prev);
    }

    public void SelectCell(int index)
    {
        if (IsLocked) return;
        if (index < 0 || index >= ItemsSource.Count || index == Context.SelectedIndex)
        {
            return;
        }

        UpdateSelection(index);
        scroller.ScrollTo(index, 0.35f, Ease.OutCubic);
    }
    
    public void SetScrollDragEnabled(bool enabled)
    {
        scroller.enabled = enabled; 
    }
}


