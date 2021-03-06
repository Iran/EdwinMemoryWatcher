﻿using ReadWriteMemory;
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
        byte[] FrameMemBytes;
        int MemoryOffset = -1;
        int AdjustedOffset = 0;
        const int PLAYER_PTR_ADDRESS = 0x00668958;
        const int HOUSECLASS_SIZE = 0x17A8;
        const int HOUSECLASS_HEAP_ADDRESS = 0x0065C994;
        const int HOUSECLASS_HEAP_OFFSET_Pointer = 0x10;
        int PlayerPtrOffset = -1;

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
            this.MemBytes = this.ProcMem.ReadMem(this.MemoryOffset + this.AdjustedOffset, HOUSECLASS_SIZE, false);
            this.FrameMemBytes = this.ProcMem.ReadMem(0x06670C4, 4/*sizeof(int32)*/, false);
        }

        public byte[] GetFrameMemoryBytes()
        {
            return this.FrameMemBytes;
        }

        public void SetAdJustedOffset(int offset)
        {
            this.AdjustedOffset = offset;
        }
        
        public int GetPlayerPtrOffset()
        {
            return this.PlayerPtrOffset;
        }

        public int GetMemoryOffset()
        {
            return this.MemoryOffset;
        }

        private void LoadMemoryOffset()
        {
            byte[] MemB = this.ProcMem.ReadMem(PLAYER_PTR_ADDRESS, 0x4, false); // read 32-bit pointer to HouseClass 
            this.PlayerPtrOffset = BitConverter.ToInt32(MemB, 0);

            byte[] MemB2 = this.ProcMem.ReadMem(HOUSECLASS_HEAP_ADDRESS + HOUSECLASS_HEAP_OFFSET_Pointer, 
                0x4, false); // read 32-bit pointer to HouseClass 
            this.MemoryOffset = BitConverter.ToInt32(MemB2, 0);



            //           MessageBox.Show(this.MemoryOffset.ToString());

            return;

//            MessageBox.Show("Can't find PlayerPtr offset, shutting down");
//            Environment.Exit(-1);
        }

        private void LoadProcessMemory()
        {
            this.ProcMem = new ProcessMemory("ra95_300");
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
