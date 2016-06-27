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
            mainManager.SetAdJustedOffset(CurrentPage * 0xFA0);
            mainManager.UpdateMemoryBytes();
            byte[] MemBytes = mainManager.GetMemoryBytes();

            byte[] FilteredBytes = new byte[0x2000];


            /* TEST OUTPUT */

            String s2 = " ÚName: ÄÄÄÄÄÄÄÄÄÄÄÄÄÄÂBase: ÄÄÄÄÄÄÂÄÄÄÄÄÄÄÂBlock: ÄÂAlert: ÂBorrowedÂSmartiesÂÄÄÄÄ¿" +
                         "³                   ³           ³       ³       ³      ³        ³        ³    ³" +
                         "ÃÄÄÄÄÄÄÄÄÄÄÄÄÄÄÂNÂYÂBScan: ÄÄÂUScan: ÄÄÂIScan: ÄÄÂAScan: ÄÄÅUnits: ÄÄÁÄÂBuildings: Ä´" +
                         "³Active........³ ³ ³        ³        ³        ³        ³          ³           ³" +
                         "³Human.........³ ³ ÃTiber: ÄÂCreds: ÄÂICreds: ÂCap: ÄÄÄÂPower: ÄÂDrain: ÁÂInfantry: Ä´" +
                         "³Prod.Started.³ ³ ³       ³       ³       ³       ³       ³       ³          ³" +
                         "³Alerted.......³ ³ ÃÄÄÄÄÄÄÄÁÄÄÄÄÄÄÄÁÄÄÄÄÄÄÄÁÄÄÄÄÄÄÄÁÄÄÄÄÄÄÄÁÄÄÄÄÄÄÄÁÄÄÄÄÄÄJust´" +
                         "³Discovered....³ ³ ³Radius: Struct³  ³" +
                         "³Tib.Maxed....³ ³ ³                                                   Unit³  ³" +
                         "³Defeated......³ ³ ÃNorth: ÄÄÄÄÄÄÄÄÄÄÂSouth: ÄÄÄÄÄÄÄÄÄÄ¿                  Inf³  ³" +
                         "³To Die........³ ³ ³Air:            ³Air:            ³                  Air³  ³" +
                         "³To Win........³ ³ ³Arm:            ³Arm:            ³                     ÀÄÄ´" +
                         "³To Lose.......³ ³ ³Inf:            ³Inf:            ³                        ³" +
                         "³Civ.Evac.....³ ³ ³                ³                ³Globals:                ³" +
                         "³Recalc Needed.³ ³ ÃWest: ÄÄÄÄÄÄÄÄÄÄÄÅEast: ÄÄÄÄÄÄÄÄÄÄÄÅCore: ÄÄÄÄÄÄÄÄÄÄÄ¿       ³" +
                         "³Visionary.....³ ³ ³Air:            ³Air:            ³Air:            ³       ³" +
                         "³              ³ ³ ³Arm:            ³Arm:            ³Arm:            ³       ³" +
                         "³Tib.Short....³ ³ ³Inf:            ³Inf:            ³Inf:            ³       ³" +
                         "³              ³ ³ ³                ³                ³                ³       ³" +
                         "ÀÄÄÄÄÄÄÄÄÄÄÄÄÄÄÁÄÁÄÁÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÁÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÁÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÁÄÄÄÄÄÄÄÙ";

            byte[] bytes = GetBytes(s2);
            s2 = System.Text.Encoding.GetEncoding(437).GetString(bytes);
            textBoxOutput.Text = s2;

            /* TEST OUTPUT */


            /* for (int i = 0, j = 0, count = 0; i < 0x2000; i += 2, j++, count++)
             {
                 if (count == 80)
                 {
                     FilteredBytes[j] = (byte)'\r';
                     j++;
                     FilteredBytes[j] = (byte)'\n';
                     j++;
                     count = 0;
                 }

                 if (MemBytes[i] == 0xB3 || MemBytes[i] == 0xDA || MemBytes[i] == 0xBF || MemBytes[i] == 0xB4)
                 {
                     FilteredBytes[j] = (byte)'|';
                 }
                 else if (MemBytes[i] == 0xC2 || MemBytes[i] == 0xC4)
                 {
                     FilteredBytes[j] = (byte)'-';
                 }
                 else {
                     FilteredBytes[j] = MemBytes[i];
                 }
             } */

            for (int i = 0, j = 0, count = 0; i < 0x2000; i += 2, j++, count++)
            {
                if (count == 80)
                {
                    FilteredBytes[j] = (byte)'\r';
                    j++;
                    FilteredBytes[j] = (byte)'\n';
                    j++;
                    count = 0;
                }

                if (MemBytes[i] == 0x00)
                {
                    FilteredBytes[j] = (byte)' ';
                }
                else
                {
                    FilteredBytes[j] = MemBytes[i];
                }

            }


            //String s = System.Text.Encoding.UTF8.GetString(FilteredBytes);
            String s = System.Text.Encoding.GetEncoding(437).GetString(FilteredBytes);

            //Suspend(textBoxOutput);
            textBoxOutput.Text = s;
            //Resume(textBoxOutput);
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
