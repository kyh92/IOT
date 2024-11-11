using System;
using System.Device.Gpio;
using System.Net;
using System.Threading;

class Program
{
    private static GpioController _gpioController;
    private const int RelayPin = 15; // GPIO pin number

    static void Main(string[] args)
    {
        _gpioController = new GpioController();
        _gpioController.OpenPin(RelayPin, PinMode.Output);

        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://*:8080/");
        listener.Start();
        Console.WriteLine("Listening for connections on http://*:8080/");

        while (true)
        {
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string responseString = "";
            if (request.Url.AbsolutePath == "/on")
            {
                _gpioController.Write(RelayPin, PinValue.High);
                responseString = "Light turned on";
            }
            else if (request.Url.AbsolutePath == "/off")
            {
                _gpioController.Write(RelayPin, PinValue.Low);
                responseString = "Light turned off";
            }
            else
            {
                responseString = "Invalid command";
            }

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
            Console.WriteLine();
        }
    }
}