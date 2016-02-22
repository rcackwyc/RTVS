﻿using System.Windows.Input;
using Microsoft.Languages.Editor.Controller.Constants;
using Microsoft.VisualStudio.Text.Editor;

namespace Microsoft.R.Editor.Commands {
    public sealed class RMouseProcessor : MouseProcessorBase {
        private IWpfTextView _wpfTextView;
        public RMouseProcessor(IWpfTextView wpfTextView) {
            _wpfTextView = wpfTextView;
        }

        public override void PreprocessMouseLeftButtonDown(MouseButtonEventArgs e) {
            if ((e.ClickCount == 1 && ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)) ||
                e.ClickCount == 2){
                // If this is a Ctrl+Click or double-click then post the select word command.
                var controller = RMainController.FromTextView(_wpfTextView);
                if(controller != null) {
                    object o = new object();
                    controller.Invoke(typeof(VSConstants.VSStd2KCmdID).GUID, (int)VSConstants.VSStd2KCmdID.SELECTCURRENTWORD, null, ref o);
                    e.Handled = true;
                    return;
                }
            }
            base.PreprocessMouseLeftButtonDown(e);
        }
    }
}