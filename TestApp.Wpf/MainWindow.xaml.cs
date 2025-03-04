using System.Diagnostics;
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
        int port = 8000;

        // Start UDP Listener
        // Explicitly cast to HandleOscPacket to resolve ambiguity

        HandleOscPacket cb = delegate (OscPacket packet)
        {
            var msg = packet;
        };

        var l1 = new UDPListener(8000, cb);

        //// Wait a moment for listener to start
        //Thread.Sleep(1000);

        //var sender = new UDPSender("127.0.0.1", 7000);
        //var bundle = new OscBundle(Utils.DateTimeToTimetag(DateTime.UtcNow), new OscMessage("/test", 47, "asdfasd", 0.2f));

        ////var parms = new object[] { 51, "huhu", 3.1f};
        ////var message = new OscMessage("/test", new object[] { 54, "miau", (float)3.1 });

        ////var message = new OscMessage("/test", parms);


        ////sender.Send(message);
        //sender.Send(bundle);


        //// Wait for response
        //Thread.Sleep(2000);

        ////listener.Close();
        //Console.WriteLine("Test finished.");

    }
}