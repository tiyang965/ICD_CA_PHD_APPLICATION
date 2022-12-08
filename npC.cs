// Based on https://learn.microsoft.com/en-us/dotnet/api/system.io.pipes.namedpipeclientstream?view=net-7.0
// Tianjiao Yang, 08.12.2022, Stuttgart

using System;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Runtime.CompilerServices;

class PipeClient
{
	static void Main(string[] args)
	{
		using (NamedPipeClientStream pipeClient =
			new NamedPipeClientStream(".", "icdca", PipeDirection.In))
		{

			// Connect to the pipe or wait until the pipe is available.
			Console.Write("Attempting to connect to pipe...");
			pipeClient.Connect();

			Console.WriteLine("Connected to pipe.");
			Console.WriteLine("There are currently {0} pipe server instances open.",
			   pipeClient.NumberOfServerInstances);
			using (StreamReader sr = new StreamReader(pipeClient))
			{
				int[] arrdim = { -1, -1 };
				string temp;
				int i = 0;

				while ((temp = sr.ReadLine()) != null && i < 2)
				{

					arrdim[i] = Int16.Parse(temp);
					i++;
			
				}

				float[,] arrrec = new float[arrdim[0], arrdim[1]];
				Console.WriteLine("The shape of the array is {0}*{1}", arrdim[0], arrdim[1]);
				Console.WriteLine("Array received from server, line by line:");
				Console.WriteLine(" {0}", temp);
				List<string> elements = new List<string>(temp.Split(','));

				for (int j = 0; j < arrdim[1]; j++)
				{
					arrrec[0, j] = float.Parse(elements[j]);
				}

				int line = 1;

				while ((temp = sr.ReadLine()) != null)
				{
					Console.WriteLine(" {0}", temp);
					List<string> next = new List<string>(temp.Split(','));
					for (int k = 0; k < arrdim[1]; k++)
					{
						arrrec[line, k] = float.Parse(next[k]);
					}
					line++;
				}

				Console.WriteLine("\nArray received from server, repeat:\n");

				for (int m = 0; m < arrrec.GetLength(0); m++)
				{
					for (int n = 0; n < arrrec.GetLength(1); n++)
					{
						Console.Write(arrrec[m, n] + "\t");
					}
					Console.WriteLine();
				}
			}
		}
		
		Console.Write("Press Enter to continue...");
		Console.ReadLine();
	}
}