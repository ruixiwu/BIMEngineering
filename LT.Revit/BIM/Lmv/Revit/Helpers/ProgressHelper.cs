namespace BIM.Lmv.Revit.Helpers
{
    using BIM.Lmv.Revit.Helpers.Progress;
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    internal class ProgressHelper : IDisposable
    {
        private FormProgress _Form;
        private static ProgressHelper _Instance;

        public ProgressHelper(string title = null)
        {
            this._Form = new FormProgress(title);
            this._Form.StartPosition = FormStartPosition.CenterScreen;
            this._Form.Show();
            this._Form.Refresh();
            _Instance = this;
        }

        public ProgressHelper(Form owner, string title = null)
        {
            this._Form = new FormProgress(title);
            this._Form.StartPosition = FormStartPosition.CenterParent;
            this._Form.Show(owner);
            this._Form.Location = new Point(((owner.Width - this._Form.Width) / 2) + owner.Left, ((owner.Height - this._Form.Height) / 2) + owner.Top);
            this._Form.Refresh();
            _Instance = this;
        }

        public static void Close()
        {
            ProgressHelper helper = _Instance;
            if (helper != null)
            {
                helper.Dispose();
            }
        }

        public void Dispose()
        {
            if (this._Form != null)
            {
                this._Form.Close();
                this._Form = null;
                _Instance = null;
            }
        }
    }
}

