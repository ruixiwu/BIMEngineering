using System.Windows.Forms;

namespace BIM.Lmv.Revit.Utility
{
    internal static class FormHelper
    {
        public static void ShowMessageBox(this Form form, string message)
        {
            MessageBox.Show(message, form.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
    }
}