using System;
using System.CodeDom.Compiler; // TempFileCollection
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Web;

using MySql.Data.MySqlClient;

public partial class Pdf : System.Web.UI.Page
{
	protected TcpChannel _channel;

	protected void Page_Load(object sender, EventArgs e)
	{
		try
		{
			if (Request["Username"] == null || Request["Password"] == null)
				throw new ApplicationException("Username and Password are required");

			HttpPostedFile postedFile = Request.Files["InputFile"];
			if (postedFile == null || postedFile.FileName == null || postedFile.FileName == "")
				throw new ApplicationException("InputFile was not supplied or file name is empty.");

			string inputFileExtension = Path.GetExtension(postedFile.FileName);
			if (inputFileExtension.Length < 2)
				throw new ApplicationException("InputFile does not have a valid file name extension.");
			inputFileExtension = inputFileExtension.Substring(1);

			string outputFileName = Request["OutputFileName"];
			if (outputFileName == null || outputFileName == "")
				outputFileName = Path.GetFileNameWithoutExtension(postedFile.FileName) + ".pdf";

			DbRequest.CheckAuthorization(Request["Username"], Request["Password"]);

			if (_channel == null)
			{
				Dictionary<string, string> channelProperties = new Dictionary<string,string>();
				channelProperties["name"] = "";
				_channel = new TcpChannel(channelProperties, null, null);
				ChannelServices.RegisterChannel(_channel, false);
			}

			string url = string.Format("tcp://localhost:{0}/{1}", ConvertInterface.ConverterEndpoints.Port, ConvertInterface.ConverterEndpoints.Uri);

			ConvertInterface.IConverter converter = (ConvertInterface.IConverter)Activator.GetObject(typeof(ConvertInterface.IConverter), url);

			string path = MapPath(".");
			path = Path.Combine(Path.GetDirectoryName(path), "Temp");

			using (TempFileCollection tempFiles = new TempFileCollection(path))
			{
				string inputFile  = tempFiles.AddExtension(inputFileExtension);
				string outputFile = tempFiles.AddExtension("pdf");

				postedFile.SaveAs(inputFile);
				converter.Convert(inputFile, outputFile);

				Response.Clear();
				Response.ContentType = "application/pdf";
				Response.AddHeader("content-disposition", string.Format("inline; filename={0}.pdf", "sample"));

				byte[] pdfBytes = File.ReadAllBytes(outputFile);

				Response.OutputStream.Write(pdfBytes, 0, pdfBytes.Length);
			}
		}		
		catch (Exception ex)
		{
			Response.Clear();
			Response.Write(string.Format("<HTML><HEAD></HEAD><BODY><H1>Conversion Error</H1><P>{0}</P></BODY></HTML>", ex.Message));
			Response.StatusCode = 500;
			Response.StatusDescription = ex.Message;
		}
	}
}
