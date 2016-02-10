﻿using System.ComponentModel.Design;
using System.Windows;

namespace Microsoft.R.Components.View {
    /// <summary>
    /// Represents UI element that holds visual component
    /// (typically a tool window)
    /// </summary>
    public interface IVisualComponentContainer<out T> where T : IVisualComponent {
        T Component { get; }
        bool IsOnScreen { get; }

        void Show(bool focus);
        void ShowContextMenu(CommandID commandId, Point position);
        void UpdateCommandStatus(bool immediate);
    }
}

