using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace EdwinMemoryWatcher
{
    public partial class MemoryWatchUI : Form
    {
        MainManager mainManager;
        int CurrentPage = -1;

        public MemoryWatchUI()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                            ControlStyles.UserPaint |
                            ControlStyles.DoubleBuffer, true);

            mainManager = new MainManager();

            SetMemoryBytesTextBoxOutput();

            var OutputTextBoxRefreshTimer = new System.Windows.Forms.Timer();
            OutputTextBoxRefreshTimer.Tick += new EventHandler(RefreshOutput);
            OutputTextBoxRefreshTimer.Enabled = true;
            OutputTextBoxRefreshTimer.Interval = 40;

            SetCurrentPage(0);
        }

        // Fix stupid flickering
        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_COMPOSITED = 0x02000000;
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_COMPOSITED;
                return cp;
            }
        }

        private void RefreshOutput(object sender, EventArgs e)
        {
            SetMemoryBytesTextBoxOutput();
        }

        private void SetCurrentPage(int page)
        {
            CurrentPage = page;
            labelCurrentPage.Text = "" + CurrentPage;
        }

        private void SetMemoryBytesTextBoxOutput()
        {
            mainManager.UpdateMemoryBytes();
            byte[] mb = mainManager.GetMemoryBytes();

            bool IsActive = (mb[0x42] & 1) != 0;
            bool IsHuman = (mb[0x42] & 2) != 0;
            bool IsPlayerCtrl = (mb[0x42] & 4) != 0;

            bool ToDie = (mb[0x43] & 2) != 0;
            bool ToWin = (mb[0x43] & 4) != 0;
            bool ToLose = (mb[0x43] & 8) != 0;

            bool Thieved = (mb[0x43] & 2) != 0;

            int CurrentIQ = BitConverter.ToInt32(mb, 0x46); // ALSO KNOWN AS 'Smarties'

            int Struct = mb[0x113];
            int Infantry = mb[0x114];
            int Unit = mb[0x115];
            int Aircraft = mb[0x116];
            int Blocks = BitConverter.ToInt32(mb, 0x118);

            int Power = BitConverter.ToInt32(mb, 0x1E3);
            int Drain = BitConverter.ToInt32(mb, 0x1E7);

            int BuildingsLost = BitConverter.ToInt32(mb, 0x2a9);
            int UnitsLost = BitConverter.ToInt32(mb, 0x255);

            int Radius = BitConverter.ToInt32(mb, 0x2B2);
            int CenterCell = BitConverter.ToInt32(mb, 0x2AE);

            String Name = System.Text.Encoding.UTF8.GetString(mb, 0x001790, 12).Trim('\0');

            String s = String.Format("Name: {0}\r\n"
                + "IsActive: {1}\r\n"
                + "Drain: {2}\r\n"
                + "Power: {3}\r\n"
                
                , Name, IsActive, Drain, Power
                
                
                );

            textBoxOutput.Text = s;
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);

        public static void Suspend(Control control)
        {
            LockWindowUpdate(control.Handle);
        }

        public static void Resume(Control control)
        {
            LockWindowUpdate(IntPtr.Zero);
        }

        private void MainUI_Load(object sender, EventArgs e)
        {

        }

        private void textBoxPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                int page = int.Parse(textBoxPage.Text);
                SetCurrentPage(page);
            }
        }
    }
}
