// TextBoxPlaceholderHelper.cs
using System;
using System.Drawing;
using System.Windows.Forms;

namespace 实验耗材及设备物资管理系统
{
    public static class TextBoxPlaceholderHelper
    {
        private static readonly Color PlaceholderColor = Color.Gray;
        private static readonly Color NormalColor = SystemColors.WindowText;

        public static void SetPlaceholder(TextBox txt, string placeholder)
        {
            if (txt == null) return;

            txt.Tag = placeholder;
            if (string.IsNullOrEmpty(txt.Text))
            {
                ApplyPlaceholder(txt);
            }

            txt.GotFocus += RemovePlaceholder;
            txt.LostFocus += ShowPlaceholder;
        }

        private static void ApplyPlaceholder(TextBox txt)
        {
            txt.Text = txt.Tag?.ToString() ?? "";
            txt.ForeColor = PlaceholderColor;
        }

        private static void RemovePlaceholder(object sender, EventArgs e)
        {
            var txt = sender as TextBox;
            if (txt != null && txt.ForeColor == PlaceholderColor)
            {
                txt.Text = "";
                txt.ForeColor = NormalColor;
            }
        }

        private static void ShowPlaceholder(object sender, EventArgs e)
        {
            var txt = sender as TextBox;
            if (txt != null && string.IsNullOrEmpty(txt.Text))
            {
                ApplyPlaceholder(txt);
            }
        }
    }
}