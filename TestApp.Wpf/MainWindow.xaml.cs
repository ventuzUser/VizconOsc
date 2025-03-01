using System.Windows;
using Vizcon.OSC;

namespace TestApp.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        int port = 7000;

        // Start UDP Listener
        // Explicitly cast to HandleOscPacket to resolve ambiguity

        //var listener = new UDPListener(9000, (HandleOscPacket)(packet =>
        //{
        //    Console.WriteLine($"Received OSC Message: {packet}");
        //}));


        // Wait a moment for listener to start
        Thread.Sleep(1000);

        var sender = new UDPSender("127.0.0.1", 9000);
        var bundle = new OscBundle(DateTime.UtcNow, new OscMessage("/test", 43));

        sender.Send(bundle);


        // Wait for response
        Thread.Sleep(2000);

        //listener.Close();
        Console.WriteLine("Test finished.");

    }
}