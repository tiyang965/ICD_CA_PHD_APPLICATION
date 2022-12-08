// Based on https://stackoverflow.com/questions/71391206/named-pipe-c-sharp-server-python-client
// Tianjiao Yang, 08.12.2022, Stuttgart

using System;
using System.IO;
using System.IO.Pipes;


public class Server
{
	private NamedPipeServerStream PipeServer { get; set; }

	static void Main()
	{

		NamedPipeServerStream pipeServer = 
			new NamedPipeServerStream("acdci", PipeDirection.InOut, 10, PipeTransmissionMode.Message, PipeOptions.WriteThrough, 1024, 1024);

		StreamReader sr = new StreamReader(pipeServer);
		StreamWriter sw = new StreamWriter(pipeServer);
		Console.WriteLine("NamedPipeServerStream object created.\n");

		// initialization of the 2d array
		double[,] exchange = new double[10, 10];
		Random rand = new Random();
		for (int x = 0; x < exchange.GetLength(0); x++)
		{
			for (int y = 0; y < exchange.GetLength(1); y++)
			{
				exchange[x, y] = rand.NextDouble();
			}
		}

		// get dimensions
		int rowsOrHeight = exchange.GetLength(0);
		int colsOrWidth = exchange.GetLength(1);
		int size = rowsOrHeight * colsOrWidth;

		var count = 0;
		var rowVector = new double[colsOrWidth];

		while (count < rowsOrHeight)
		{
			for (int el = 0; el < colsOrWidth; el++)
			{
				rowVector[el] = exchange[count, el];
			}
			try
			{
				Console.Write("Waiting for client connection...\n");
				pipeServer.WaitForConnection();
				string instruction;

				//Receive instruction message
				instruction = sr.ReadLine();
				Console.WriteLine(instruction);
				var str = String.Join(",", rowVector);
				Console.WriteLine("Sending line {0} of the double array", count + 1);
				Console.WriteLine(str);
				sw.WriteLine(str);
				sw.Flush();
				pipeServer.WaitForPipeDrain();

			}
			catch (Exception ex) 
			{ 
				throw ex; 
			}
			finally
			{
				pipeServer.WaitForPipeDrain();
				if (pipeServer.IsConnected) { pipeServer.Disconnect(); }
			}

			count++;
		}


		Console.Write("\nData transimission now complete.\n");
		pipeServer.WaitForConnection();

	}
}