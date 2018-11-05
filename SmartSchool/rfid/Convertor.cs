using System;
using System.Runtime.InteropServices;

namespace SmartSchool.rfid
{
    class Convertor
    {
        private struct ISO14443A_CARD_INFO
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] arrKey;

            [MarshalAs(UnmanagedType.U1)]
            public byte bIsKeyB;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 nStartSect;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 nNrSect;
        }

        public byte[] StringToHex(string strData)
        {
            byte[] arrData = new byte[strData.Length / 2];
            for (int iCount = 0; iCount <= strData.Length - 0x02; iCount += 0x02)
            {
                arrData[iCount / 0x02] = (byte)Int32.Parse(strData.Substring(iCount, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return arrData;
        }

        public string GetCardId(byte[] arrByte, UInt32 nSize)
        {
            return BitConverter.ToString(arrByte, 0x04, (int)(nSize - 0x04));
        }

        public byte[] GetIso14443AInfo(UInt32 nStartSect, UInt32 nNrSect, string strKey, byte bIsKeyB)
        {
            ISO14443A_CARD_INFO stCardInfo = new ISO14443A_CARD_INFO();
            stCardInfo.nStartSect = nStartSect;
            stCardInfo.nNrSect = nNrSect;
            stCardInfo.bIsKeyB = bIsKeyB;
            stCardInfo.arrKey = StringToHex(strKey);

            int iSize = Marshal.SizeOf(stCardInfo);
            byte[] arrByte = new byte[iSize];
            IntPtr pStruct = Marshal.AllocHGlobal(iSize);

            Marshal.StructureToPtr(stCardInfo, pStruct, true);
            Marshal.Copy(pStruct, arrByte, 0, iSize);
            Marshal.FreeHGlobal(pStruct);

            return arrByte;
        }
    }
}
