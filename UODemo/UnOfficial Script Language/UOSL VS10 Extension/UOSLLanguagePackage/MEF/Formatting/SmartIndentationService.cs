using System;
using Microsoft.VisualStudio.Text.Editor;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;


namespace JoinUO.UOSL.Package.MEF
{
    [Export(typeof(ISmartIndentProvider))]
    [ContentType("UOSL")]
    class IndentProvider : ISmartIndentProvider
    {
        #region ISmartIndentProvider Members

        LineIndenter lineindenter = null;
        public ISmartIndent CreateSmartIndent(ITextView textView)
        {
            textView.TextBuffer.ChangedHighPriority += new EventHandler<Microsoft.VisualStudio.Text.TextContentChangedEventArgs>(TextBuffer_ChangedHighPriority);

            return lineindenter ?? (lineindenter = new LineIndenter(textView));
        }

        #endregion

        void TextBuffer_ChangedHighPriority(object sender, Microsoft.VisualStudio.Text.TextContentChangedEventArgs e)
        {
            foreach (var change in e.Changes)
            {
                if (change.NewText.EndsWith("{"))
                {
                    var line = e.After.GetLineFromPosition(change.NewPosition);
                    int linenum = line.LineNumber;

                    if (linenum > 0)
                    {
                        int? ind = lineindenter.GetDesiredIndentation(line);
                        int indentLevel;
                        if (ind == null)
                        {
                            int previousLine = LineIndenter.GetPreviousNonWhitespaceLine(e.After, linenum);
                            string previousLineText = e.After.GetLineFromLineNumber(previousLine).GetText();
                            indentLevel = LineIndenter.GetIndentLevel(e.After, previousLineText);
                        }
                        else
                            indentLevel=(int)ind;

                        Span s = new Span(line.Start, line.Length - 1);
                        if (string.IsNullOrWhiteSpace(e.After.GetText(s)))
                            e.After.TextBuffer.Replace(s, new string(' ', indentLevel));
                    }
                }
                else if (change.NewText.EndsWith("}"))
                {
                    var line = e.After.GetLineFromPosition(change.NewPosition);

                    FormatClosingBrace(e.After, line);
                }
            }
        }

        public static void FormatClosingBrace(ITextSnapshot Snapshot, ITextSnapshotLine line)
        {
            Span s = new Span(line.Start, line.Length - 1);
            if (string.IsNullOrWhiteSpace(Snapshot.GetText(s)))
            {
                SnapshotSpan pair;
                if (BraceMatchingTagger.FindMatchingOpenChar(line.End, '{', '}', Snapshot.LineCount, out pair,true))
                {
                    var previousLine = Snapshot.GetLineFromPosition(pair.Span.Start);
                    int previousLineNum = previousLine.LineNumber;
                    string previousLineText = previousLine.GetText();
                    int indentLevel = LineIndenter.GetIndentLevel(Snapshot, previousLineText);

                    Snapshot.TextBuffer.Replace(s, new string(' ', indentLevel));
                }
            }

        }

    }

    public sealed class LineIndenter : ISmartIndent
    {
        // matches comments on the end of a line
        private readonly Regex removeCommentsRegExp = new Regex(@".*((?:#|\/\/|\/\*).*)$", RegexOptions.Compiled);

        //private readonly Regex dedentRegExp = new Regex(@"(?:return|pass)(?:[\t ]?[\w]*[\t ]?(?<exp>(?:if|unless)))?", RegexOptions.Compiled);

        private readonly ITextView textView;
        private static readonly char IndentChar = ' ';


        public LineIndenter(ITextView view)
        {
            this.textView = view;
        }

        #region ISmartIndent Members

        /// <summary>
        /// Sets the indentation for the next lineNumber.
        /// </summary>
        public int? GetDesiredIndentation(Microsoft.VisualStudio.Text.ITextSnapshotLine line)
        {
            int tabsize = GetTabSize(textView.TextSnapshot);

            int previousLine = GetPreviousNonWhitespaceLine(textView.TextSnapshot, line.LineNumber);
            string previousLineText = PrepareLine(line.Snapshot.GetLineFromLineNumber(previousLine).GetText());
            int indentLevel = GetIndentLevel(textView.TextSnapshot, previousLineText);

            string curline=line.GetText();
            if (curline.TrimStart().StartsWith("{") && !previousLineText.EndsWith(";") && !previousLineText.EndsWith("{")) return indentLevel;
            if (PrepareLine(curline).EndsWith("}"))
            {
                IndentProvider.FormatClosingBrace(line.Snapshot, line);
                return null;
            }

            if (previousLineText.Contains("{"))
                indentLevel += tabsize;
            else if (!previousLineText.EndsWith(";") && !previousLineText.EndsWith("}"))
            {
                previousLine = GetPreviousNonWhitespaceLine(textView.TextSnapshot, previousLine);
                previousLineText = line.Snapshot.GetLineFromLineNumber(previousLine).GetText();
                if (previousLineText.EndsWith(";"))
                    indentLevel+=tabsize;
                if (previousLineText.Contains("{"))
                    indentLevel += tabsize;
            }
            else
            {
                previousLine = GetPreviousNonWhitespaceLine(textView.TextSnapshot, previousLine);
                previousLineText = line.Snapshot.GetLineFromLineNumber(previousLine).GetText();
                if (!previousLineText.EndsWith(";") && !previousLineText.Contains("{") && !previousLineText.EndsWith("}")) indentLevel -= tabsize;
            }

            return indentLevel;
        }

        #endregion

        #region IDisposable Members

        private bool m_isDisposed;
        public void Dispose()
        {
            if (!m_isDisposed)
            {
                GC.SuppressFinalize(this);
                m_isDisposed = true;
            }
        }

        #endregion

        public static int GetTabSize(ITextSnapshot TextSnapshot)
        {
            IEditorOptions ops;
            if (TextSnapshot.TextBuffer.Properties.TryGetProperty(typeof(IEditorOptions),out ops))
            {
                return ops.GetOptionValue<int>("Tabs/TabSize");
            }
            return 4;
        }

        public static int GetIndentLevel(ITextSnapshot TextSnapshot, string line)
        {
            var count = 0;
            int tabsize = GetTabSize(TextSnapshot);

            foreach (var c in line)
            {
                if (c == '\t')
                    count += tabsize;
                else if (c != IndentChar)
                    break; // got to the text on the line
                else
                    count++;
            }

            return count;
        }

        public static int GetPreviousNonWhitespaceLine(ITextSnapshot TextSnapshot, int startLine)
        {
            int prevLineIndex = startLine;

            Microsoft.VisualStudio.Text.ITextSnapshotLine line;
            string lineText;

            //searching for not blank line
            do
            {
                if (prevLineIndex == 0) return 0;
                prevLineIndex--;
                line = TextSnapshot.GetLineFromLineNumber(prevLineIndex);
                lineText = line.GetText();
            } while (prevLineIndex > 0 && string.IsNullOrEmpty(lineText));

            return prevLineIndex;
        }

        private string PrepareLine(string line)
        {
            // remove comments, makes parsing easier
            Match commentMatch = removeCommentsRegExp.Match(line);

            if (commentMatch != null && commentMatch.Groups.Count >= 2)
                line = line.Remove(line.IndexOf(commentMatch.Groups[1].Value));

            // get rid of trailing whitespace
            line = line.TrimEnd();

            return line;
        }
    }
}
    


