using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RunUOServerAdmin
{
    /// <summary>
    /// Interaction logic for ConsoleText.xaml
    /// </summary>
    public partial class ConsoleText : UserControl
    {
        const int DefaultCapacity = 60000;

        public event EventHandler OnTextChanged;
        void InvokeOnTextChanged(object sender, EventArgs e) { if (OnTextChanged != null) OnTextChanged(sender, e); }

        public int Capacity { get; set; }

        public ConsoleText()
        {
            InitializeComponent();
            Capacity = DefaultCapacity;
        }

        public void Append(string newData)
        {
            TextControl.AppendText(newData);
        }

        public void ScrollToEnd()
        {
            TextControl.ScrollToEnd();
        }

        public string Text { get { return TextControl.Text; } }
        public void Clear() { TextControl.Text = string.Empty; }

        private void Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextControl.ScrollToEnd();
            TrimToCapacity();
            InvokeOnTextChanged(sender, e);
        }

        void TrimToCapacity()
        {
            if(TextControl.Text.Length > Capacity)
            {
                int maxToRemove = TextControl.Text.Length / 2;
                int minToRemove = Math.Min(maxToRemove, TextControl.Text.Length - Capacity + (Capacity / 20));
                int amountToRemove= 0;
                while (amountToRemove < minToRemove)
                    amountToRemove = TextControl.Text.IndexOf('\n', amountToRemove) + 1;
                if (amountToRemove <= 0 || amountToRemove > maxToRemove)
                    amountToRemove = maxToRemove;
                TextControl.Text = TextControl.Text.Remove(0, amountToRemove);
            }
        }
    }
}
