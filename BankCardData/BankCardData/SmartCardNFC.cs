using System.Runtime.InteropServices;
using System.Text;

namespace BankCardData
{
    public abstract class SmartCardNFC
    {
        [DllImport("WinScard.dll")]
        protected static extern int SCardConnect(IntPtr hContext, string cReaderName, uint dwShareMode,
            uint dwPreferredProtocols, ref IntPtr phCard, ref uint ActiveProtocol);

        [DllImport("WinScard.dll")]
        protected static extern int SCardTransmit(IntPtr hCard, ref SmartCardNFC.SCARD_IO_REQUEST pioSendPci,
            byte[] pbSendBuffer, int cbSendLength, ref SmartCardNFC.SCARD_IO_REQUEST pioRecvPci, byte[] pbRecvBuffer,
            ref int pcbRecvLength);

        [DllImport("WinScard.dll")]
        protected static extern int SCardDisconnect(IntPtr hCard, int Disposition);

        [DllImport("WinScard.dll")]
        protected static extern int SCardEstablishContext(uint dwScope, IntPtr nNotUsed1, IntPtr nNotUsed2,
            ref IntPtr phContext);

        [DllImport("WinScard.dll")]
        protected static extern int SCardListReaders(IntPtr hContext, byte[] mszGroups, byte[] mszReaders,
            ref UInt32 pcchReaders);

        [StructLayout(LayoutKind.Sequential)]
        protected struct SCARD_IO_REQUEST
        {
            public int dwProtocol;
            public int cbPciLength;
        }

        static protected IntPtr hContext = new IntPtr();
        static protected int retCode = SCardEstablishContext(2, IntPtr.Zero, IntPtr.Zero, ref hContext);
        static protected IntPtr hCard = IntPtr.Zero;


        public bool Connect()
        {
            if (retCode != 0)
                throw new Exception("SCardEstablishContext error: " + retCode);

            UInt32 pcchReaders = 0;
            retCode = SCardListReaders(hContext, null, null, ref pcchReaders);
            if (retCode != 0)
                throw new Exception("SCardListReaders error: " + retCode);


            byte[] mszReaders = new byte[pcchReaders];
            retCode = SCardListReaders(hContext, null, mszReaders, ref pcchReaders);
            if (retCode != 0)
                throw new Exception("SCardListReaders error: " + retCode);


            string readerName = System.Text.Encoding.ASCII.GetString(mszReaders);

            #region Connect
            uint activeProtocol = 0;
            retCode = SCardConnect(hContext, readerName, 2, 2, ref hCard, ref activeProtocol);
            if (retCode != 0)
                throw new Exception("SCardConnect error: " + retCode);
            #endregion
            return true;
        }



        protected bool Disconnect()
        {
            retCode = SCardDisconnect(hCard, 2);
            if (retCode != 0)
                throw new Exception("SCardDisconnect error: " + retCode);

            return false;
        }

        protected string SetCommand(byte[] command)
        {
            string response = String.Empty;

            byte[] specCommand = command;
            int recvLength = short.MaxValue;
            byte[] recvBuffer = new byte[recvLength];
            SCARD_IO_REQUEST sendPci = new SCARD_IO_REQUEST();
            SCARD_IO_REQUEST recvPci = new SCARD_IO_REQUEST();
            sendPci.cbPciLength = (int)Marshal.SizeOf(typeof(SCARD_IO_REQUEST));
            recvPci.cbPciLength = (int)Marshal.SizeOf(typeof(SCARD_IO_REQUEST));

            retCode = SCardTransmit(hCard, ref sendPci, specCommand, specCommand.Length,
                ref recvPci, recvBuffer, ref recvLength);

            if (retCode != 0)
                throw new Exception("Command executed error! RetCode: " + retCode);

            response = BitConverter.ToString(recvBuffer, 0, recvLength).Replace("-", "");

            return response;
        }


        public string GetCardUId()
        {
            Connect();
            string CardUID = SetCommand(new byte[] { 0xFF, 0xCA, 0x00, 0x00, 0x00 });
            Disconnect();
            return CardUID.Substring(0, CardUID.Length - 4);

        }

        protected static byte[] ConvertHexToBytes(string hexString)
        {
            hexString = hexString.Replace(" ", ""); // Remove spaces if present
            int length = hexString.Length;
            byte[] bytes = new byte[length / 2];

            for (int i = 0; i < length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }

            return bytes;
        }

        protected static List<List<byte>> ConvertStringToListToHex(List<string> hexList)
        {
            List<List<byte>> listOfLists = new List<List<byte>>();

            foreach (var hexString in hexList)
            {
                List<byte> innerList = new List<byte>();
                for (int i = 0; i < hexString.Length; i += 2)
                {
                    var replace = hexString;
                    replace = hexString.Replace(" ", "");
                    string byteString = replace.Substring(i, 2);
                    byte b = Convert.ToByte(byteString, 16);
                    innerList.Add(b);
                }
                listOfLists.Add(innerList);
            }

            return listOfLists;
        }

        protected static string AddSpaces(string input, int spacing)
        {

            string spacedText = "";

            for (int i = 0; i < input.Length; i++)
            {
                spacedText += input[i];
                if ((i + 1) % spacing == 0 && i != input.Length - 1)
                {
                    spacedText += " ";
                }
            }

            return spacedText;
        }

        protected static List<string> ConvertListOfListsToHex(List<List<byte>> listOfLists)
        {
            List<string> hexList = new List<string>();

            foreach (var innerList in listOfLists)
            {
                StringBuilder hexStringBuilder = new StringBuilder();
                foreach (byte b in innerList)
                {
                    hexStringBuilder.AppendFormat("{0:X2}", b);
                }
                hexList.Add(hexStringBuilder.ToString());
            }

            return hexList;
        }

        //public abstract string ReadData();
        //public abstract string WriteData(string data);

    }
}
