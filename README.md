# BankCardData NFC Reader

BankCardData NFC Reader is a C# project that demonstrates how to read and process bank card data via NFC. The application leverages Windows smart card APIs (WinScard.dll) to establish communication with NFC-enabled bank cards, extract critical information such as the Primary Account Number (PAN), expiry date, and card type, and display this data. The project is designed with modularity, extensibility, and robust error handling in mind.

---

## Features

- **NFC Communication:**  
  Uses Windows smart card APIs to connect with NFC-enabled cards and transmit commands.

- **Multi-Card Support:**  
  Supports Visa, MasterCard, and American Express through a dictionary of known Application Identifiers (AIDs). Easily extendable for additional card types.

- **Data Extraction:**  
  Reads card records and extracts key details like PAN and expiry date using regular expressions and marker-based parsing.

- **Robust Error Handling:**  
  Implements try-catch-finally blocks to ensure proper resource management and reliable operation.

- **Utility Functions:**  
  Includes helper methods for hexadecimal string conversion, data formatting, and retrieving the card's UID.

---

## Prerequisites

- **Operating System:** Windows (with access to smart card reader hardware)
- **Development Environment:** Visual Studio or any compatible C# IDE
- **.NET Framework:** .NET Framework or .NET Core/5+ (depending on your setup)
- **Hardware:** A compatible NFC-enabled smart card reader with necessary drivers installed

---

## Installation

1. **Clone the Repository:**

   ```bash
   git clone https://github.com/KhanbalaRashidov/BankCardData.git
   ```

2. **Open the Project:**
   
   Open the solution in Visual Studio.

3. **Restore Dependencies:**

   Restore any NuGet packages if necessary.

4. **Build the Project:**

   Build the solution to ensure all dependencies are properly resolved.

---

## Usage

1. **Connect the NFC Reader:**

   Plug in your NFC-enabled smart card reader to your computer.

2. **Run the Application:**

   Start the application from Visual Studio or your preferred IDE.

3. **Automatic Card Detection:**

   The application will:
   - Establish a connection with the NFC reader.
   - Select the appropriate bank card application (Visa, MasterCard, or Amex).
   - Read the card records and extract the PAN, expiry date, and card type.

4. **View Output:**

   The extracted card details will be displayed in the console output.

5. **Further Integration:**

   Utilize the provided methods to integrate with other systems or develop a graphical user interface if desired.

---

## Code Structure

- **BankCardReader.cs:**  
  Contains high-level logic for selecting a bank card application, reading records, and extracting card information.

- **SmartCardNFC.cs:**  
  Implements low-level NFC communication using P/Invoke with WinScard.dll. Includes methods for connecting, transmitting commands, and disconnecting from the smart card.

- **CardInfo.cs:**  
  Defines a simple data class that holds card details (card number, expiry date, and card type).

- **Utility Methods:**  
  Various helper functions for converting hexadecimal strings to byte arrays, formatting data, and more.

---

## Contributing

Contributions are welcome! If you have suggestions for improvements, bug fixes, or additional features, please follow these steps:

1. **Fork the Repository**
2. **Create a Feature Branch:**

   ```bash
   git checkout -b feature/my-feature
   ```

3. **Commit Your Changes:**

   ```bash
   git commit -am "Add new feature"
   ```

4. **Push to the Branch:**

   ```bash
   git push origin feature/my-feature
   ```

5. **Open a Pull Request**

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## Disclaimer

This project is intended for educational and experimental purposes only. When handling sensitive data such as bank card information, always ensure you are operating in a secure and authorized environment.

---

Happy Coding!
