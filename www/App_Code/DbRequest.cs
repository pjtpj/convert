using System;
using System.Collections;
using System.Data;
using System.Web.Configuration;

using MySql.Data.MySqlClient;

/// <summary>
/// Summary description for DbRequest
/// </summary>
public class DbRequest
{
	/*
	 * DatabasePassword and BlobPassword are stored in web.config like this:
	 * 
	 * <configSections>
	 *   <section name="DbRequest" type="System.Configuration.SingleTagSectionHandler" />
	 * </configSections>
	 * <DbRequest DatabasePassword="XXX"  />
	 * 
	 * To encrypt the DbRequest configuration use these commands:
	 *   
	 *   cd \Projects\convert\www
	 *   \windows\Microsoft.NET\Framework\v2.0.50727\aspnet_regiis -pef DbRequest . -prov DataProtectionConfigurationProvider
	 * 
	 * To decrypt:
	 * 
	 *   cd \Projects\convert\www
	 *   \windows\Microsoft.NET\Framework\v2.0.50727\aspnet_regiis -pdf DbRequest .
	 */
	static protected string _DatabasePassword = "";
	static public string DatabasePassword
	{ 
		get { return  _DatabasePassword != "" ? _DatabasePassword : (string)((IDictionary)WebConfigurationManager.GetSection("DbRequest"))["DatabasePassword"]; } 
		set { _DatabasePassword = value; }
	}
	static protected string _DatabaseServer = "";
	static public string DatabaseServer
	{ 
		get { return  _DatabaseServer != "" ? _DatabaseServer : WebConfigurationManager.AppSettings["DatabaseServer"]; } 
		set { _DatabaseServer = value; }
	}
	static protected string _DatabaseName = "";
	static public string DatabaseName
	{ 
		get { return  _DatabaseName != "" ? _DatabaseName : WebConfigurationManager.AppSettings["DatabaseName"]; } 
		set { _DatabaseName = value; }
	}
	static protected string _ConnectionStringPostfix = "";
	static public string ConnectionStringPostfix
	{ 
		get { return  _ConnectionStringPostfix != "" ? _ConnectionStringPostfix : WebConfigurationManager.AppSettings["ConnectionStringPostfix"]; } 
		set { _ConnectionStringPostfix = value; }
	}
	static protected string _OperationsContactEmail = "";
	static public string OperationsContactEmail
	{ 
		get { return  _OperationsContactEmail != "" ? _OperationsContactEmail : WebConfigurationManager.AppSettings["OperationsContactEmail"]; } 
		set { _OperationsContactEmail = value; }
	}
	static protected string _SmtpHost = "";
	static public string SmtpHost
	{ 
		get { return  _SmtpHost != "" ? _SmtpHost : WebConfigurationManager.AppSettings["SmtpHost"]; } 
		set { _SmtpHost = value; }
	}

	public static string ConnectionString
	{
		get
		{
			string connectionFormat = "Initial Catalog={1};Data Source={0};User ID={1};Password={2};";
			string connection       = string.Format(connectionFormat, DatabaseServer, DatabaseName, DatabasePassword);

			return connection + "Pooling=false;" + ConnectionStringPostfix;
		}
	}

	protected static object _commandLock = new object();
	protected static MySqlCommand _command;

	public static void CheckAuthorization(string username, string password)
	{
		lock (_commandLock)
		{
			if (_command == null)
			{
				MySqlConnection connection = new MySqlConnection(ConnectionString);
				connection.Open();
				MySqlCommand command = new MySqlCommand();
				command.Connection = connection;

				_command = command;
			}

			try
			{
				_command.CommandText = "SELECT Password FROM Users WHERE Username = @Username";
				_command.Parameters.Add(new MySqlParameter("Username", username));

				using (IDataReader reader = _command.ExecuteReader())
				{
					if (!reader.Read())
						throw new ApplicationException("Unknown username or bad password");

					if (reader.GetString(0) != password)
						throw new ApplicationException("Unknown username or bad password");
				}
			}
			finally
			{
				_command.Parameters.Clear();

			}
		}
	}
}
