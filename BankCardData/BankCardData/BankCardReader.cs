using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BankCardData
{
    /// <summary>
    /// Reads and processes bank card data using NFC.
    /// </summary>
    internal partial class BankCardReader : SmartCardNFC
    {
        /// <summary>
        /// Known Application Identifiers (AIDs) for supported card types.
        /// </summary>
        private static readonly Dictionary<string, byte[]> KnownAIDs = new Dictionary<string, byte[]>()
        {
            { "Visa", new byte[] { 0x00, 0xA4, 0x04, 0x00, 0x07,
                                    0xA0, 0x00, 0x00, 0x00, 0x03, 0x10, 0x10, 0x00 } },
            { "MasterCard", new byte[] { 0x00, 0xA4, 0x04, 0x00, 0x07,
                                          0xA0, 0x00, 0x00, 0x00, 0x04, 0x10, 0x10, 0x00 } },
            { "Amex", new byte[] { 0x00, 0xA4, 0x04, 0x00, 0x07,
                                  0xA0, 0x00, 0x00, 0x00, 0x25, 0x01, 0x10, 0x00 } }
        };

        /// <summary>
        /// Contains card information such as Card Number, Expiry Date, and Card Type.
        /// </summary>
        public CardInfo CardInfo { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BankCardReader"/> class.
        /// Connects to the card, selects a bank card application, and extracts card information.
        /// </summary>
        public BankCardReader()
        {
            try
            {
                Connect();

                // Attempt to select a supported bank card application.
                if (!SelectBankApplication(out string cardType))
                {
                    Console.WriteLine("No bank card application selected.");
                    return;
                }

                // Search for the PAN record in the card.
                string record = FindPanRecord();
                if (record != null)
                {
                    // Extract the card number.
                    // The marker "5A08" is followed by 16 characters representing the card number.
                    int indexCard = record.IndexOf("5A08");
                    if (indexCard >= 0 && record.Length >= indexCard + "5A08".Length + 16)
                    {
                        string cardNumber = record.Substring(indexCard + "5A08".Length, 16);

                        // Extract the expiry date.
                        // The marker "5F2403" is followed by 4 characters, where the first 2 are the year and the next 2 are the month.
                        int indexExpiry = record.IndexOf("5F2403");
                        if (indexExpiry >= 0 && record.Length >= indexExpiry + "5F2403".Length + 4)
                        {
                            string datePart = record.Substring(indexExpiry + "5F2403".Length, 4);
                            string year = datePart.Substring(0, 2);
                            string month = datePart.Substring(2, 2);
                            string expiryDate = month + "/" + year;

                            // Set the card information.
                            CardInfo = new CardInfo
                            {
                                CardNumber = cardNumber,
                                ExpriyDate = expiryDate,
                                CardType = cardType,
                            };
                        }
                        else
                        {
                            Console.WriteLine("Expiry date information not found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Card number information not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while extracting PAN: {ex.Message}");
            }
            finally
            {
                // Ensure the connection is closed.
                Disconnect();
            }
        }

        /// <summary>
        /// Selects a supported bank card application by iterating over known AIDs.
        /// </summary>
        /// <param name="cardType">Outputs the card type that was successfully selected.</param>
        /// <returns>
        /// True if a supported bank card application was successfully selected; otherwise, false.
        /// </returns>
        public bool SelectBankApplication(out string cardType)
        {
            cardType = null;
            foreach (var kvp in KnownAIDs)
            {
                // Send the SELECT command for the current AID.
                string response = SetCommand(kvp.Value);
                // A response ending with "9000" indicates success.
                if (response.EndsWith("9000"))
                {
                    cardType = kvp.Key;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Reads a record from the smart card using the Short File Identifier (SFI) and record number.
        /// </summary>
        /// <param name="sfi">The Short File Identifier (range 1-31).</param>
        /// <param name="recordNumber">The record number (range 1-15).</param>
        /// <returns>The response string from the card.</returns>
        public string ReadRecord(int sfi, int recordNumber)
        {
            // Construct the parameter P2:
            // Left shift the SFI by 3 bits and set the 4th bit.
            byte p2 = (byte)((sfi << 3) | 4);
            byte[] command = new byte[] { 0x00, 0xB2, (byte)recordNumber, p2, 0x00 };
            string response = SetCommand(command);
            return response;
        }

        /// <summary>
        /// Searches through card records to find the PAN record using a regular expression.
        /// The PAN regex supports:
        /// - Visa (16 digits starting with 4)
        /// - MasterCard (16 digits starting with 51-55)
        /// - American Express (15 digits starting with 34 or 37)
        /// </summary>
        /// <returns>
        /// The data string containing the PAN record if found; otherwise, null.
        /// </returns>
        public string FindPanRecord()
        {
            // General PAN regex: Visa (16 digits starting with 4), MasterCard (16 digits starting with 51-55),
            // American Express (15 digits starting with 34 or 37).
            Regex panRegex = new Regex(@"(?:4[0-9]{15}|5[1-5][0-9]{14}|3[47][0-9]{13})");

            // Iterate through possible Short File Identifiers (SFIs) and record numbers.
            for (int sfi = 1; sfi < 32; sfi++)
            {
                for (int recordNumber = 1; recordNumber < 16; recordNumber++)
                {
                    try
                    {
                        string response = ReadRecord(sfi, recordNumber);
                        // Check if the response status word indicates success ("9000").
                        if (!response.EndsWith("9000"))
                            continue;

                        // Remove the status word (last 4 characters) to extract the actual data.
                        string data = response.Substring(0, response.Length - 4);
                        Match match = panRegex.Match(data);
                        if (match.Success)
                        {
                            return data;
                        }
                    }
                    catch (Exception)
                    {
                        // If an error occurs, skip to the next record.
                        continue;
                    }
                }
            }
            return null;
        }
    }
}
