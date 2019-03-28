using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Xml;
using System.Xml.XPath;
using log4net;
using Encryption.Security;

namespace GenuinaBI.Service
{
    public static class SecurityProviderService
    {
        private static String getGCSConfig(String host, String port) {
            Genuina.Central.Service.Contracts.ICentralServiceContract server;
            String address = "net.tcp://{0}:{1}/Genuina/";
            String mac = "";
            String result = String.Empty;
            GenuinaCentralService channel = null;
            NetTcpBinding bnd = null;

            address = String.Format(address, host, port);

            try {
                bnd = new NetTcpBinding(SecurityMode.None);
                channel = new GenuinaCentralService(bnd, new System.ServiceModel.EndpointAddress(address));
                server = channel.ChannelFactory.CreateChannel();

                result = server.getConfig(mac);
            }
            catch (Exception e) {
                throw e;
            }

            return result.Replace("\r\n", string.Empty);
        }

        public static String getConnectionString(String host, String port, String dbConnectionStringTemplate)
        {
            ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            String config = String.Empty;
            String dbServerName = String.Empty;
            String dbName = String.Empty;
            String dbUserId = String.Empty;
            String dbEncryptedPassword = String.Empty;
            String dbConnectionString = String.Empty;
            String dbPassword = "gnsys";

            try
            {
                XmlDocument doc = new XmlDocument();
                config = getGCSConfig(host, port);
                doc.LoadXml(config);

                dbServerName = doc.SelectSingleNode("/GNSysConfig/DatabaseConfig/DataBase/DatabaseServerName").InnerText;
                dbName = doc.SelectSingleNode("/GNSysConfig/DatabaseConfig/DataBase/DatabaseName").InnerText;
                dbUserId = doc.SelectSingleNode("/GNSysConfig/DatabaseConfig/DataBase/UserId").InnerText;
                dbEncryptedPassword = doc.SelectSingleNode("/GNSysConfig/DatabaseConfig/DataBase/Password").InnerText;

                Encrypt encrypt = new Encrypt();
                dbPassword = encrypt.decryptPassword(dbEncryptedPassword, "T.04ynYKhj8W");
            }
            catch (Exception e)
            {
                logger.Error(e.StackTrace);
            }

            return String.Format(dbConnectionStringTemplate, dbServerName, dbName, dbUserId, dbPassword);
        }
    }

    class GenuinaCentralService : ClientBase<Genuina.Central.Service.Contracts.ICentralServiceContract>
    {
        public GenuinaCentralService() { }
        public GenuinaCentralService(System.ServiceModel.Channels.Binding binding, EndpointAddress serverAddress) : base(binding, serverAddress) { }
    }
}