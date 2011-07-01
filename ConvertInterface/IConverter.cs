using System;
using System.Collections.Generic;
using System.Text;

namespace ConvertInterface
{
	public class ConverterEndpoints
	{
		public static int    Port = 7047;
		public static string Uri  = "DocumentToPdf";
	}

	public interface IConverter
	{
		void Convert(string srcDocument, string destPdf);
	}
}
