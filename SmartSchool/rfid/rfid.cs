using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace access_control
{
    class rfid
    {
        [DllImport("R-DT-EVO-HF-USB-SDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 GetAvailableCom(UInt32 nComPos);

        [DllImport("R-DT-EVO-HF-USB-SDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr OpenReader(UInt32 nPort, UInt32 nSpeed);

        [DllImport("R-DT-EVO-HF-USB-SDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 GetReaderCaps(IntPtr hReader, UInt32 nRdFeat, byte[] pData, ref UInt32 dwCount);

        [DllImport("R-DT-EVO-HF-USB-SDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 SetReaderCaps(IntPtr hReader, UInt32 nRdFeat, byte[] pData, UInt32 dwCount);

        [DllImport("R-DT-EVO-HF-USB-SDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 ReadData(IntPtr hReader, byte[] pData, ref UInt32 dwCount);

        [DllImport("R-DT-EVO-HF-USB-SDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 WriteData(IntPtr hReader, byte[] pData, UInt32 dwCount);

        [DllImport("R-DT-EVO-HF-USB-SDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CloseReader(IntPtr hReader);

        private enum EERROR
        {
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

        private enum EREADER_FEATURE
        {
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

        enum ECARDTYPE
        {
            ECARD_ISO14443A = 0x00,
            ECARD_ISO15693,
        };

        public void showPorts()
        {
            UInt32 nComPos = 0x00;
            UInt32 nCom = 0xFFFFFFFF;
            while ((nCom = GetAvailableCom(nComPos)) != 0xFFFFFFFF)
            {
                string strPort = "Com "; strPort += nCom.ToString();
                System.Console.WriteLine(strPort);
                nComPos++;
            }
        }

        public void getInfo(UInt32 nPort)
        {
            IntPtr hReader = OpenReader(nPort, 9600);

            if (hReader != null)
            {
                UInt32 nCount = 200;
                byte[] bufData = new byte[nCount];

                System.Text.ASCIIEncoding clsEnc = new System.Text.ASCIIEncoding();
                if (GetReaderCaps(hReader, (UInt32)EREADER_FEATURE.ERD_SERIAL_NUMER, bufData, ref nCount) == (UInt32)EERROR.ER_OK)
                {
                    System.Console.WriteLine(clsEnc.GetString(bufData, 0x00, bufData.Length));
                }

                Thread.Sleep(100);
                nCount = 200;
                if (GetReaderCaps(hReader, (UInt32)EREADER_FEATURE.ERD_FIRMWARE, bufData, ref nCount) == (UInt32)EERROR.ER_OK)
                {
                    System.Console.WriteLine(clsEnc.GetString(bufData, 0x00, bufData.Length));
                }

                CloseReader(hReader);
            }
        }

        public int scanCards(UInt32 nPort)
        {
            IntPtr hReader = OpenReader(nPort, 9600);
            if (hReader != null)
            {
                UInt32 nCount = 200;
                byte[] arrData = new byte[nCount];
                arrData[0] = 0x00;
                UInt32 response = GetReaderCaps(hReader, (UInt32)EREADER_FEATURE.ERD_CARD, arrData, ref nCount);
                if (response == (UInt32)EERROR.ER_OK)
                {
                    Convertor clsConv = new Convertor();
                    System.Console.WriteLine("Korteliu skaicius: " + BitConverter.ToString(arrData, 0x00, 0x04));
                    System.Console.WriteLine(clsConv.GetCardId(arrData, nCount));
                }
                else if (response == (UInt32)EERROR.ER_NO_TAG)
                {
                    System.Console.WriteLine("Nera korteles");
                }
                else
                {
                    System.Console.WriteLine("Klaidos kodas: " + response.ToString());
                }

                CloseReader(hReader);
                return (int)response;
            }
            else
            {
                return 1;
            }
        }

        public byte[] readBlock(UInt32 nPort, UInt32 nBlock, string sKey)
        {
            byte[] arrReadData = null;
            IntPtr hReader = OpenReader(nPort, 9600);
            if (hReader != null)
            {
                UInt32 nError = (UInt32)EERROR.ER_OK;
                Convertor clsConv = new Convertor();

                byte[] arrData = clsConv.GetIso14443AInfo(nBlock, 0x01, sKey, 0x00);
                nError = SetReaderCaps(hReader, (UInt32)EREADER_FEATURE.ERD_CARD_INFO_14443A, arrData, (UInt32)arrData.Length);

                if (nError == (UInt32)EERROR.ER_OK)
                {
                    UInt32 nCount = 200;
                    arrReadData = new byte[nCount];
                    if (ReadData(hReader, arrReadData, ref nCount) == (UInt32)EERROR.ER_OK)
                    {
                        Console.WriteLine(BitConverter.ToString(arrReadData, 0, (int)nCount));
                        Console.WriteLine( System.Text.Encoding.ASCII.GetString(arrReadData));
                        Console.WriteLine("Blokas nuskaitytas");
                    }
                }

                CloseReader(hReader);
            }
            return arrReadData;
        }

        public int writeBlock(UInt32 nPort, UInt32 nBlock, string sKey, byte[] arrWriteData)
        {
            if (arrWriteData.Length > 16)
            {
                Console.WriteLine("Klaida. Per ilgas blokas.");
                return 1;
            }

            if ((nBlock + 1) % 4 == 0)
            {
                Console.WriteLine(" Klaida. Draudziamas blokas");
                return 1;
            }

            IntPtr hReader = OpenReader(nPort, 9600);
            if (hReader != null)
            {
                UInt32 nError = (UInt32)EERROR.ER_OK;
                Convertor clsConv = new Convertor();

                byte[] arrData = clsConv.GetIso14443AInfo(nBlock, 0x01, sKey, 0x00);
                nError = SetReaderCaps(hReader, (UInt32)EREADER_FEATURE.ERD_CARD_INFO_14443A, arrData, (UInt32)arrData.Length);

                if (nError == (UInt32)EERROR.ER_OK)
                {
                    nError = WriteData(hReader, arrWriteData, (UInt32)arrWriteData.Length);
                    /*if (nError == (UInt32)EERROR.ER_OK)
                    {
                        Console.WriteLine("Blokas irasytas");
                    }*/
                }

                CloseReader(hReader);
                return (int)nError;
            }
            else
            {
                return 1;
            }
        }
    }
}
