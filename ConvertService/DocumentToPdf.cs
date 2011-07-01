using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

// For InitOpenOfficeEnvironment
using System.Runtime.InteropServices;
using Microsoft.Win32;

using unoidl.com.sun.star.lang;
using unoidl.com.sun.star.uno;
using unoidl.com.sun.star.bridge;
using unoidl.com.sun.star.frame;
using unoidl.com.sun.star.beans;
using uno.util;

namespace ConvertService
{
	public class DocumentToPdf : MarshalByRefObject, ConvertInterface.IConverter
	{
		protected EventLog _log;

		public DocumentToPdf()
		{
			_log = new EventLog();
			_log.Source = "Teztech Document Conversion";

			_log.WriteEntry("DocumentToPdf object constructed");
		}

		public void Convert(string srcDocument, string destPdf)
		{
			_log.WriteEntry(string.Format("Converting '{0}' bytes to '{1}'", srcDocument, destPdf));

			XComponent wordDocument = null;

			try
			{
				if (!_isOpenOfficeEnvironmentInit)
				{
					InitOpenOfficeEnvironment();
					_isOpenOfficeEnvironmentInit = true;
				}

				//Call the bootstrap method to get a new ComponentContext
				//object. If OpenOffice isn't already started this will
				//start it and then return the ComponentContext.
				XComponentContext localContext = Bootstrap.bootstrap();

				//Get a new service manager of the MultiServiceFactory type
				//we need this to get a desktop object and create new CLI
				//objects.
				XMultiServiceFactory multiServiceFactory = (XMultiServiceFactory)localContext.getServiceManager();

				//Create a new Desktop instance using our service manager
				//Notice: We cast our desktop object to XComponent loader
				//so that we could load or create new documents.
				XComponentLoader componentLoader = (XComponentLoader)multiServiceFactory.createInstance("com.sun.star.frame.Desktop");

				PropertyValue hiddenProperty = new PropertyValue();
				hiddenProperty.Name = "Hidden";
				hiddenProperty.Value = new uno.Any(true);
				PropertyValue[] loadProperties = new PropertyValue[] { hiddenProperty };

				//Create a new blank writer document using our component
				//loader object.
				wordDocument = componentLoader.loadComponentFromURL(ConvertPath(srcDocument), "_blank", 0, loadProperties);

				PropertyValue storeProperty = new PropertyValue();
				storeProperty.Name  = "FilterName";
				storeProperty.Value = new uno.Any("writer_pdf_Export");
				PropertyValue[] storeProperties = new PropertyValue[] { storeProperty };

				((XStorable)wordDocument).storeToURL(ConvertPath(destPdf), storeProperties);

				_log.WriteEntry("Document Converstion Complete");
			}
			catch (System.Exception ex)
			{
				_log.WriteEntry("Cannot convert document. ERROR: " + ex.Message);
				throw ex;
			}
			finally
			{
				if (wordDocument != null)
					wordDocument.dispose();
			}
		}

		private static string ConvertPath(string file)
		{
			file = file.Replace(@"\", "/");
			return "file:///" + file;
		}

		private bool _isOpenOfficeEnvironmentInit = false;

		// See http://blog.nkadesign.com/2008/net-working-with-openoffice-3/
		private void InitOpenOfficeEnvironment() 
		{
			string baseKey;
			// OpenOffice being a 32 bit app, its registry location is different in a 64 bit OS
			if (Marshal.SizeOf(typeof(IntPtr)) == 8)
				baseKey = @"SOFTWARE\Wow6432Node\OpenOffice.org\";	
			else
				baseKey = @"SOFTWARE\OpenOffice.org\";  

			// Get the URE directory
			string key = baseKey + @"Layers\URE\1";
			RegistryKey reg = Registry.CurrentUser.OpenSubKey(key);
			if (reg == null) reg = Registry.LocalMachine.OpenSubKey(key);
			string urePath = reg.GetValue("UREINSTALLLOCATION") as string;
			reg.Close();
			urePath = Path.Combine(urePath, "bin");

			// Get the UNO Path
			key = baseKey + @"UNO\InstallPath";
			reg = Registry.CurrentUser.OpenSubKey(key);
			if (reg == null) reg = Registry.LocalMachine.OpenSubKey(key);
			string unoPath = reg.GetValue(null) as string;
			reg.Close();

			string path;
			path = string.Format ("{0};{1}", System.Environment.GetEnvironmentVariable("PATH"), urePath);
			System.Environment.SetEnvironmentVariable("PATH", path);
			System.Environment.SetEnvironmentVariable("UNO_PATH", unoPath);
		}
	}
}
