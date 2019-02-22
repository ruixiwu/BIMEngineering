namespace BIM.Lmv.Revit.Utility
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    internal static class FormHelper
    {
        public static void ShowMessageBox(this Form form, string message)
        {
            MessageBox.Show(message, form.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
    }
}

