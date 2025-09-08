using System;
using FancyScrollView;

namespace CharacterSelect
{
    public class Context
    {
        public int SelectedIndex = -1;
        public Action<int> OnCellClicked;
    }
}