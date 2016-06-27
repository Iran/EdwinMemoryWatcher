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
        const int HEAP_ENTRY_SIZE = 0x17A8;

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

            SetCurrentPageToPlayerHouse();
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

        private void SetCurrentPageToPlayerHouse()
        {
            int PtrDiff = mainManager.GetPlayerPtrOffset() - mainManager.GetMemoryOffset();
            SetCurrentPage(PtrDiff / HEAP_ENTRY_SIZE);
        }

        private void SetMemoryBytesTextBoxOutput()
        {
            mainManager.SetAdJustedOffset(this.CurrentPage * HEAP_ENTRY_SIZE);
            mainManager.UpdateMemoryBytes();
            byte[] mb = mainManager.GetMemoryBytes();

            int AIDiff = mb[0x9];

            int Unknown01 = mb[0x41];

            bool IsActive = (mb[0x42] & 1) != 0;
            bool IsHuman = (mb[0x42] & 2) != 0;
            bool IsPlayerCtrl = (mb[0x42] & 4) != 0;
            bool ProductionStarted = (mb[0x42] & 8) != 0;
            bool Bit_16 = (mb[0x42] & 16) != 0;         //TriggersAreActive?
            bool Bit_32 = (mb[0x42] & 32) != 0;          //AutoBaseAI?
            bool Discovered = (mb[0x42] & 64) != 0;
            bool MaxCapacity = (mb[0x42] & 128) != 0;

            bool Defeated = (mb[0x43] & 1) != 0;
            bool ToDie = (mb[0x43] & 2) != 0;
            bool ToWin = (mb[0x43] & 4) != 0;
            bool ToLose = (mb[0x43] & 8) != 0;
            bool CivEvac = (mb[0x43] & 16) != 0;
            bool RecalcNeeded = (mb[0x43] & 32) != 0;
            bool Visionary = (mb[0x43] & 64) != 0;
            bool Bit2_128 = (mb[0x43] & 128) != 0;          //LowOre?

            bool Bit3_1 = (mb[0x44] & 1) != 0;          //Spied?
            bool Thieved = (mb[0x44] & 2) != 0;
            bool Bit3_4 = (mb[0x44] & 4) != 0;
            bool Bit3_8 = (mb[0x44] & 8) != 0;           //GPSActive?
            bool Bit3_16 = (mb[0x44] & 16) != 0;        //Production?
            bool Bit3_32 = (mb[0x44] & 32) != 0;        //Resigned?
            bool Bit3_64 = (mb[0x44] & 64) != 0;        //GaveUp?
            bool Paranoid = (mb[0x44] & 128) != 0;

            bool Bit4_1 = (mb[0x44] & 128) != 0;

            int CurrentIQ = BitConverter.ToInt32(mb, 0x46); // ALSO KNOWN AS 'Smarties'

            int Urgency = BitConverter.ToInt32(mb, 0x4A);

            byte JustStruct = mb[0x113];
            byte JustInfantry = mb[0x114];
            byte JustUnit = mb[0x115];
            byte JustAircraft = mb[0x116];

            int Blocks = BitConverter.ToInt32(mb, 0x118);

            int field_173 = BitConverter.ToInt32(mb, 0x173);       	//Spent?
            int field_177 = BitConverter.ToInt32(mb, 0x177);		//Harvested?
            int field_17B = BitConverter.ToInt32(mb, 0x17B);		//Stolen?

            int Power = BitConverter.ToInt32(mb, 0x1E3);
            int Drain = BitConverter.ToInt32(mb, 0x1E7);

            int BuildingsLost = BitConverter.ToInt32(mb, 0x2A9);
            int UnitsLost = BitConverter.ToInt32(mb, 0x255);

            int Radius = BitConverter.ToInt32(mb, 0x2B2);
            int CenterCell = BitConverter.ToInt32(mb, 0x2AE);

            int Tiberium = BitConverter.ToInt32(mb, 0x193);
            int Credits = BitConverter.ToInt32(mb, 0x197);
            int Cap = BitConverter.ToInt32(mb, 0x19B);

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
