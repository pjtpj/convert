using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace ConvertClient
{
	class Program
	{
		static void Main(string[] args)
		{
			TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);

			string url = string.Format("tcp://localhost:{0}/{1}", ConvertInterface.ConverterEndpoints.Port, ConvertInterface.ConverterEndpoints.Uri);

			ConvertInterface.IConverter converter = (ConvertInterface.IConverter)Activator.GetObject(typeof(ConvertInterface.IConverter), url);

			converter.Convert(@"c:\temp\samples-Final\sample.docx", @"c:\temp\samples-Final\sample.pdf");
		}
	}
}
