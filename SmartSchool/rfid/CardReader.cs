using System;
using System.Runtime.InteropServices;
using System.Threading;
using static System.Text.Encoding;

namespace SmartSchool.rfid {
    internal static class CardReader {
        [DllImport("R-DT-EVO-HF-USB-SDK.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr OpenReader(uint nPort, uint nSpeed);

        [DllImport("R-DT-EVO-HF-USB-SDK.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint GetReaderCaps(IntPtr hReader, uint nRdFeat, byte[] pData, ref uint dwCount);

        [DllImport("R-DT-EVO-HF-USB-SDK.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint SetReaderCaps(IntPtr hReader, uint nRdFeat, byte[] pData, uint dwCount);

        [DllImport("R-DT-EVO-HF-USB-SDK.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint ReadData(IntPtr hReader, byte[] pData, ref uint dwCount);

        [DllImport("R-DT-EVO-HF-USB-SDK.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CloseReader(IntPtr hReader);

        private enum Error {
            ER_OK = 0x00,
            ER_NOT_IMPLEMENTED,
            ER_INVALID_POINTER,
            ER_FEATURE,
            ER_NO_PROTOCOL,
            ER_OPEN,
            ER_CLOSE,
            ER_WRITE,
            ER_READ,
            ER_SET_SPEED,
            ER_SET_TIMEOUT,
            ER_INVALID_DATA,
            ER_SIZE,
            ER_NO_TRANSPORT,
            ER_BCC,
            ER_MORE_DATA,
            ER_CHECK_PROTOCOL,
            ER_TAG_START,
            ER_TAG_LEN,
            ER_TAG_CMD,
            ER_TAG_DATA,
            ER_TAG_BCC,
            ER_EXEC_CMD,
            ER_COM,
            ER_SOH,
            ER_STX,
            ER_ETX,
            ER_CR,
            ER_ADDRESS,
            ER_NAK,
            ER_DATA_BIT_STOP,
            ER_DATA_BIT_SIZE,
            ER_DATA_SPEED,
            ER_NO_TAG,
            ER_WRONG_ANTENA,
            ER_OTHER_ERROR,
            ER_UNKNOWN = -1,
        };

        private enum ReaderFeature {
            ERD_SERIAL_NUMER = 0x00,
            ERD_FIRMWARE,
            ERD_LED,
            ERD_BUZZER,
            ERD_SPEED,
            ERD_CARD,
            ERD_CARD_INFO_14443A,
            ERD_CARD_INFO_15693,
            ERD_AFI,
            ERD_DSFID,
            ERD_LOCK_AFI,
            ERD_LOCK_DSFID,
        };


        public static bool IsCardPresent(uint comPort) {
            var hReader = OpenReader(comPort, 9600);
            uint nCount = 200;
            var arrData = new byte[nCount];
            var response = GetReaderCaps(hReader, (uint) ReaderFeature.ERD_CARD, arrData, ref nCount);
            CloseReader(hReader);
            Thread.Sleep(100);
            return response == (uint) Error.ER_OK;
        }

        public static string ReadId(uint nPort, uint nBlock, string sKey) {
            var hReader = OpenReader(nPort, 9600);
            var clsConv = new Convertor();

            var arrData = clsConv.GetIso14443AInfo(nBlock, 0x01, sKey, 0x00);
            var nError = SetReaderCaps(hReader, (uint) ReaderFeature.ERD_CARD_INFO_14443A, arrData,
                (uint) arrData.Length);

            if (nError == (uint) Error.ER_OK) {
                uint nCount = 200;
                var arrReadData = new byte[nCount];
                if (ReadData(hReader, arrReadData, ref nCount) == (uint) Error.ER_OK) {
                    CloseReader(hReader);
                    return ASCII.GetString(arrReadData).Split('\n')[0];
                }
            }

            CloseReader(hReader);
            
            return "WTF";
        }
    }
}