using ReadWriteMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EdwinMemoryWatcher
{
    class MainManager
    {
        ProcessMemory ProcMem;
        byte[] MemBytes;
        int MemoryOffset = -1;
        int AdjustedOffset = 0;
        const int PLAYER_PTR_ADDRESS = 0X00668958;

        public MainManager()
        {
            LoadMemory();
        }

        private void LoadMemory()
        {
            LoadProcessMemory();
            LoadMemoryOffset();
            LoadMemoryBytes();
        }

        public void UpdateMemoryBytes()
        {
            LoadMemoryBytes();
        }

        public byte[] GetMemoryBytes()
        {
            return MemBytes;
        }

        private void LoadMemoryBytes()
        {
            this.MemBytes = this.ProcMem.ReadMem(this.MemoryOffset + AdjustedOffset, 0x8000, false);
        }

        public void SetAdJustedOffset(int offset)
        {
            this.AdjustedOffset = offset;
        }

        private void LoadMemoryOffset()
        {
            byte[] MemB = this.ProcMem.ReadMem(PLAYER_PTR_ADDRESS, 0x4, false); // read 32-bit pointer to HouseClass


            this.MemoryOffset = BitConverter.ToInt32(MemB, 0);

            MessageBox.Show(this.MemoryOffset.ToString());

            return;

            MessageBox.Show("Can't find PlayerPtr offset, shutting down");
            Environment.Exit(-1);
        }

        private void LoadProcessMemory()
        {
            this.ProcMem = new ProcessMemory("ra95");
            if (!this.ProcMem.OpenProcess())
            {
                Environment.Exit(-1);
            }
            
        }


        static private List<int> SearchBytePattern(byte[] pattern, byte[] bytes)
        {
            List<int> positions = new List<int>();
            int patternLength = pattern.Length;
            int totalLength = bytes.Length;
            byte firstMatchByte = pattern[0];
            for (int i = 0; i < totalLength; i++)
            {
                if (firstMatchByte == bytes[i] && totalLength - i >= patternLength)
                {
                    byte[] match = new byte[patternLength];
                    Array.Copy(bytes, i, match, 0, patternLength);
                    if (match.SequenceEqual<byte>(pattern))
                    {
                        positions.Add(i);
                        i += patternLength - 1;
                    }
                }
            }
            return positions;
        }
    }
}
