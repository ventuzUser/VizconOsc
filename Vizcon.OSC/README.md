# Vizcon.OSC 🎶

### About This Fork 🚀  
This project is a **modernized fork of SharpOSC** by **Valdemar Örn Erlingsson**. I didn't reinvent the wheel—I just gave it a fresh coat of .NET 8 paint, sprinkled in some newer language features, and made sure it's ready for the future.  

### **Original Work & Credits** 🎩  
All the heavy lifting was done by Valdemar back in 2012, and this project proudly carries his work forward. It remains **free and open** under the original MIT License, so everyone can continue using, modifying, and sharing it however they see fit.  

### **What’s New?** ✨  
- Upgraded to **.NET 8** for modern compatibility  
- Improved performance & language features  
- Bug fixes and refinements  
- Proper OSC message formatting for **Ventuz compatibility**  
- Fixed **float encoding issues** with correct Big Endian conversion  
- Ensured **4-byte alignment** for OSC messages   

### **Installation** 📦  
You can install Vizcon.OSC via NuGet:
```sh
 dotnet add package Vizcon.OSC
```
Or use the NuGet Package Manager in Visual Studio.

### **Usage** 📖  
#### **Sending an OSC Message**
```csharp
using Vizcon.OSC;

var sender = new OscSender("127.0.0.1", 9000);
var message = new OscMessage("/test", new object[] { 55, "hello", 3.1f });
sender.Send(message);
```

#### **Receiving OSC Messages**
```csharp
var listener = new OscListener(9000);
listener.MessageReceived += (msg) => 
{
    Console.WriteLine($"Received: {msg.Address}");
};
listener.Start();
```

### **Ventuz Compatibility Fixes** 🛠️  
- **Fixed Float Encoding:** Floats are now correctly converted to **Big Endian** for Ventuz.
- **Ensured Message Alignment:** All strings and type tags are properly **padded to 4 bytes**.
- **Handled Special Cases:** Improved parsing to prevent "Unknown Type Tag" errors.

### **Disclaimer** ⚠️  
I claim no ownership over the original work—this is just an evolution. If you like it, great! If something breaks, well... you know the drill. 🔧  

### **License** 📝  
This project follows the original **MIT License** from SharpOSC. See `LICENSE` for details.

---

🚀 Happy coding!

