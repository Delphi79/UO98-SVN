// TripleClick handler for JoinUO.UOSL Based on:
/* TripleClickMouseProcessor.cs
 * Copyright Noah Richards, licensed under the Ms-PL.
 * 
 * Check out blogs.msdn.com/noahric for more information about the Visual Studio 2010 editor.
 */

using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace JoinUO.UOSL.Package.MEF
{
    /// <summary>
    /// Bug workaround for VS2010:
    /// A fake mouse processor provider that forces word selection and drag drop to be in the correct order.
    /// Without this, the ordering can get reversed by the TripleClickMouseProcessorProvider below.
    /// </summary>
    [Export(typeof(IMouseProcessorProvider))]
    [Name("DragDropWordSelectionOrderingFix")]
    [Order(Before = "WordSelection", After = "DragDrop")]
    [ContentType("UOSL")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal sealed class DragDropWordSelectionOrderingFix : IMouseProcessorProvider
    {
        public IMouseProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            return null;
        }
    }

    [Export(typeof(IMouseProcessorProvider))]
    [Name("TripleClick")]
    [Order(Before = "DragDrop")]
    [ContentType("UOSL")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal sealed class TripleClickMouseProcessorProvider : IMouseProcessorProvider
    {
        [Import] internal INodeProviderBroker nodeProviderBroker { get; set; }

        public IMouseProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            return new TripleClickMouseProcessor(wpfTextView, nodeProviderBroker != null ? nodeProviderBroker.GetNodeProvider(wpfTextView.TextBuffer) : null);
        }
    }

    internal sealed class TripleClickMouseProcessor : MouseProcessorBase
    {
        private IWpfTextView _view;
        private bool _dragging;
        private SnapshotSpan? _originalLine;

        NodeProvider nodeProvider { get; set; }

        public TripleClickMouseProcessor(IWpfTextView view, NodeProvider nodeprovider=null)
        {
            _view = view;

            _view.LayoutChanged += (sender, args) =>
                {
                    if (_dragging && args.OldSnapshot != args.NewSnapshot)
                        StopDrag();
                };

            nodeProvider = nodeprovider;
        }

        #region Overrides

        public override void PreprocessMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.ClickCount != 3)
                return;

            var line = GetTextViewLineUnderPoint(e);
            if (line == null)
                return;

            VirtualSnapshotSpan? selected = _view.Selection.GetSelectionOnTextViewLine(line);
            if (nodeProvider != null && selected != null )
            {
                 //((VirtualSnapshotSpan)_view.Selection.GetSelectionOnTextViewLine).Start
                //SnapshotPoint triggerPoint= e.GetPosition(_view.VisualElement);
                Node node = nodeProvider.GetMostSpecificNode(((VirtualSnapshotSpan)selected).SnapshotSpan.Start+1);
                if (node!=null && node.astnode is Service.ASTNodes.InheritsNode)
                {
                    Service.ASTNodes.InheritsNode iNode = (Service.ASTNodes.InheritsNode)node.astnode;
                    string filename = null;
                    if (iNode.Parent is Service.ASTNodes.DeclarationsNode)
                        if (((Service.ASTNodes.DeclarationsNode)iNode.Parent).Depends.TryGetValue(iNode.Filename, out filename))
                        {
                            var dest = GetIVsTextView(filename,true);
                        }
                }
            }

            SnapshotSpan extent = line.ExtentIncludingLineBreak;
            
            SelectSpan(extent);
            StartDrag(extent);

            e.Handled = true;
        }

        /// <summary>
        /// Returns an IVsTextView for the given file path, if the given file is open in Visual Studio.
        /// </summary>
        /// <param name="filePath">Full Path of the file you are looking for.</param>
        /// <returns>The IVsTextView for this file, if it is open, null otherwise.</returns>
        internal static IVsTextView GetIVsTextView(string filePath, bool setfocus = false)
        {
            var dte2 = (EnvDTE80.DTE2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE));
            var sp = (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)dte2;
            var serviceProvider = new Microsoft.VisualStudio.Shell.ServiceProvider(sp);
            try
            {
                IVsUIHierarchy uiHierarchy;
                uint itemID;
                IVsWindowFrame windowFrame;

                if (!VsShellUtilities.IsDocumentOpen(serviceProvider, filePath, Guid.Empty, out uiHierarchy, out itemID, out windowFrame))
                    VsShellUtilities.OpenDocument(serviceProvider, filePath);

                if (VsShellUtilities.IsDocumentOpen(serviceProvider, filePath, Guid.Empty, out uiHierarchy, out itemID, out windowFrame))
                {
                    // Get the IVsTextView from the windowFrame.  
                    IVsTextView view = VsShellUtilities.GetTextView(windowFrame);
                    if (setfocus)
                        windowFrame.Show();
                    return view;

                }
                return null;
            }
            finally
            {
                serviceProvider.Dispose();
            }
        }

        public override void PreprocessMouseMove(MouseEventArgs e)
        {
            if (!_dragging || e.LeftButton != MouseButtonState.Pressed)
                return;

            // If somehow the drag wasn't set up correctly or the view has changed, stop the drag immediately
            if (_originalLine == null || _originalLine.Value.Snapshot != _view.TextSnapshot)
            {
                StopDrag();
                return;
            }

            var line = GetTextViewLineUnderPoint(e);
            if (line == null)
                return;

            SnapshotSpan newExtent = line.ExtentIncludingLineBreak;

            // Calculate the new extent, using the original span
            int start = Math.Min(newExtent.Start, _originalLine.Value.Start);
            int end = Math.Max(newExtent.End, _originalLine.Value.End);

            SelectSpan(new SnapshotSpan(_view.TextSnapshot, Span.FromBounds(start, end)));

            e.Handled = true;
        }

        public override void PostprocessMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            StopDrag();
        }

        #endregion

        #region Helpers

        ITextViewLine GetTextViewLineUnderPoint(MouseEventArgs e)
        {
            Point viewPoint = RelativeToView(e.GetPosition(_view.VisualElement));

            return _view.TextViewLines.GetTextViewLineContainingYCoordinate(viewPoint.Y);
        }

        void SelectSpan(SnapshotSpan extent)
        {
            if (!extent.IsEmpty)
            {
                _view.Selection.Select(extent, true);
                _view.Caret.MoveTo(_view.Selection.ActivePoint);
            }
            else
            {
                _view.Selection.Clear();
                _view.Caret.MoveTo(extent.Start.TranslateTo(_view.TextSnapshot, PointTrackingMode.Negative));
            }
        }

        void StartDrag(SnapshotSpan extent)
        {
            _dragging = _view.VisualElement.CaptureMouse();

            if (_dragging)
            {
                _originalLine = extent;
            }
        }

        void StopDrag()
        {
            if (_dragging)
            {
                _view.VisualElement.ReleaseMouseCapture();
                _originalLine = null;
                _dragging = false;
            }
        }

        Point RelativeToView(Point position)
        {
            return new Point(position.X + _view.ViewportLeft, position.Y + _view.ViewportTop);
        }

        #endregion
    }

    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("UOSL")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    internal class KeyBindingCommandFilterProvider : IVsTextViewCreationListener
    {
        [Import(typeof(IVsEditorAdaptersFactoryService))]
        internal IVsEditorAdaptersFactoryService editorFactory = null;

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            IWpfTextView textView = editorFactory.GetWpfTextView(textViewAdapter);
            if (textView == null)
                return;

            //AddCommandFilter(textViewAdapter, new KeyBindingCommandFilter(textView));
        }


    }
}
