using HslCommunication.ModBus;
using Opc.Ua;
using Opc.Ua.Server;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;

namespace NT.SPC.OPCUA.Server
{
    /// <summary>
    /// 一个数据中转服务器的实现
    /// </summary>
    public class CustomStandardServer : StandardServer
    {
        #region Private Fields
        private X509CertificateValidator m_certificateValidator;
        private Dictionary<string, string> user_Identity;
        private ModbusTcpServer modbus;
        #endregion
        #region Constructor
        public CustomStandardServer(ModbusTcpServer modbus)
        {
            user_Identity = new Dictionary<string, string>();
            this.modbus = modbus;
            AddUser();
        }
        public NtCustomNodeManager DevicesNodeManager { get; set; }
        #endregion
        /// <summary>
        /// 添加用户
        /// </summary>
        private void AddUser()
        {
            string name = "System";
            string password = "000000";
            string user = ConfigurationManager.AppSettings.Get("opcUser");
            if (string.IsNullOrWhiteSpace(user) || !user.Contains(",") || !user.Contains("name=") || !user.Contains("password="))
            {
                return;
            }
            string[] items = user.Split(',');
            if (items[0].Contains("name="))
            {
                name = items[0].Replace("name=", "").Trim();
                password = items[1].Replace("password=", "").Trim();
            }
            else if (items[1].Contains("name"))
            {
                name = items[1].Replace("name=", "").Trim();
                password = items[0].Replace("password=", "").Trim();
            }
            else
            {
                return;
            }
            user_Identity.Add(name, password);
        }
        #region Overridden Methods
        /// <summary>
        /// Creates the node managers for the server.
        /// </summary>
        /// <remarks>
        /// This method allows the sub-class create any additional node managers which it uses. The SDK
        /// always creates a CoreNodeManager which handles the built-in nodes defined by the specification.
        /// Any additional NodeManagers are expected to handle application specific nodes.
        /// </remarks>
        protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
        {
            Utils.Trace("Creating the Node Managers.");

            List<INodeManager> nodeManagers = new List<INodeManager>();

            DevicesNodeManager = new NtCustomNodeManager(server, configuration, modbus);
            nodeManagers.Add(DevicesNodeManager);

            // create master node manager.
            return new MasterNodeManager(server, configuration, null, nodeManagers.ToArray());
        }
        /// <summary>
        /// Initializes the server before it starts up.
        /// </summary>
        /// <remarks>
        /// This method is called before any startup processing occurs. The sub-class may update the 
        /// configuration object or do any other application specific startup tasks.
        /// </remarks>
        protected override void OnServerStarting(ApplicationConfiguration configuration)
        {
            base.OnServerStarting(configuration);
            // it is up to the application to decide how to validate user identity tokens.
            // this function creates validator for X509 identity tokens.
            CreateUserIdentityValidators(configuration);
        }

        /// <summary>
        /// Called after the server has been started.
        /// </summary>
        protected override void OnServerStarted(IServerInternal server)
        {
            base.OnServerStarted(server);
            // request notifications when the user identity is changed. all valid users are accepted by default.
            server.SessionManager.ImpersonateUser += new ImpersonateEventHandler(SessionManager_ImpersonateUser);
        }

        /// <summary>
        /// Cleans up before the server shuts down.
        /// </summary>
        /// <remarks>
        /// This method is called before any shutdown processing occurs.
        /// </remarks>
        protected override void OnServerStopping()
        {
            base.OnServerStopping();
        }

        /// <summary>
        /// Loads the non-configurable properties for the application.
        /// </summary>
        /// <remarks>
        /// These properties are exposed by the server but cannot be changed by administrators.
        /// </remarks>
        protected override ServerProperties LoadServerProperties()
        {
            ServerProperties properties = new ServerProperties();

            properties.ManufacturerName = "OPCUA";
            properties.ProductName = "OPCUA SERVER";
            properties.ProductUri = "http://localhost/UA/Server/v1.0";
            properties.SoftwareVersion = "V1.0.0";
            properties.BuildNumber = "1";
            properties.BuildDate = DateTime.Parse("2020-01-01 00:00:00");
            // TBD - All applications have software certificates that need to added to the properties.

            // for (int ii = 0; ii < certificates.Count; ii++)
            // {
            //    properties.SoftwareCertificates.Add(certificates[ii]);
            // }

            return properties;
        }

        /// <summary>
        /// Initializes the address space after the NodeManagers have started.
        /// </summary>
        /// <remarks>
        /// This method can be used to create any initialization that requires access to node managers.
        /// </remarks>
        protected override void OnNodeManagerStarted(IServerInternal server)
        {
            base.OnNodeManagerStarted(server);

        }
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            this.DevicesNodeManager.DisposeDevice();
            base.Dispose(disposing);
        }
        #endregion

        #region User Validation Functions
        /// <summary>
        /// Creates the objects used to validate the user identity tokens supported by the server.
        /// </summary>
        private void CreateUserIdentityValidators(ApplicationConfiguration configuration)
        {
            for (int ii = 0; ii < configuration.ServerConfiguration.UserTokenPolicies.Count; ii++)
            {
                UserTokenPolicy policy = configuration.ServerConfiguration.UserTokenPolicies[ii];

                // create a validator for a certificate token policy.
                if (policy.TokenType == UserTokenType.Certificate)
                {
                    // check if user certificate trust lists are specified in configuration.
                    if (configuration.SecurityConfiguration.TrustedUserCertificates != null &&
                        configuration.SecurityConfiguration.UserIssuerCertificates != null)
                    {
                        CertificateValidator certificateValidator = new CertificateValidator();
                        certificateValidator.Update(configuration.SecurityConfiguration).Wait();
                        certificateValidator.Update(configuration.SecurityConfiguration.UserIssuerCertificates,
                            configuration.SecurityConfiguration.TrustedUserCertificates,
                            configuration.SecurityConfiguration.RejectedCertificateStore);

                        // set custom validator for user certificates.
                        m_certificateValidator = certificateValidator.GetChannelValidator();
                    }
                }
            }
        }

        /// <summary>
        /// Called when a client tries to change its user identity.
        /// </summary>
        private void SessionManager_ImpersonateUser(Session session, ImpersonateEventArgs args)
        {
            // check for a WSS token.
            IssuedIdentityToken wssToken = args.NewIdentity as IssuedIdentityToken;

            // check for a user name token.
            UserNameIdentityToken userNameToken = args.NewIdentity as UserNameIdentityToken;

            if (userNameToken != null)
            {
                VerifyPassword(userNameToken.UserName, userNameToken.DecryptedPassword);
                args.Identity = new UserIdentity(userNameToken);
                Utils.Trace("UserName Token Accepted: {0}", args.Identity.DisplayName);
                return;
            }

            // check for x509 user token.
            X509IdentityToken x509Token = args.NewIdentity as X509IdentityToken;

            if (x509Token != null)
            {
                VerifyCertificate(x509Token.Certificate);
                args.Identity = new UserIdentity(x509Token);
                Utils.Trace("X509 Token Accepted: {0}", args.Identity.DisplayName);
                return;
            }
        }

        /// <summary>
        /// Validates the password for a username token.
        /// </summary>
        private void VerifyPassword(string userName, string password)
        {
            if (!this.user_Identity.ContainsKey(userName))
            {
                // construct translation object with default text.
                TranslationInfo info = new TranslationInfo(
                    "InvalidUserName",
                    "en-US",
                    "Specified userName is non-existent.");

                // create an exception with a vendor defined sub-code.
                throw new ServiceResultException(new ServiceResult(
                    StatusCodes.BadIdentityTokenRejected,
                    "InvalidUserName",
                    "http://localhost/UA/Server/",
                    new LocalizedText(info)));
            }
            else
            {
                if (!this.user_Identity[userName].Equals(password))
                {
                    // construct translation object with default text.
                    TranslationInfo info = new TranslationInfo(
                        "InvalidPassword",
                        "en-US",
                        "Specified password is not valid for user '{0}'.",
                        userName);

                    // create an exception with a vendor defined sub-code.
                    throw new ServiceResultException(new ServiceResult(
                        StatusCodes.BadIdentityTokenRejected,
                        "InvalidPassword",
                        "http://localhost/UA/Server/",
                        new LocalizedText(info)));
                }
            }
        }

        /// <summary>
        /// Verifies that a certificate user token is trusted.
        /// </summary>
        private void VerifyCertificate(X509Certificate2 certificate)
        {
            try
            {
                if (m_certificateValidator != null)
                {
                    m_certificateValidator.Validate(certificate);
                }
                else
                {
                    CertificateValidator.Validate(certificate);
                }
            }
            catch (Exception e)
            {
                TranslationInfo info;
                StatusCode result = StatusCodes.BadIdentityTokenRejected;
                ServiceResultException se = e as ServiceResultException;
                if (se != null && se.StatusCode == StatusCodes.BadCertificateUseNotAllowed)
                {
                    info = new TranslationInfo(
                        "InvalidCertificate",
                        "en-US",
                        "'{0}' is an invalid user certificate.",
                        certificate.Subject);

                    result = StatusCodes.BadIdentityTokenInvalid;
                }
                else
                {
                    // construct translation object with default text.
                    info = new TranslationInfo(
                        "UntrustedCertificate",
                        "en-US",
                        "'{0}' is not a trusted user certificate.",
                        certificate.Subject);
                }

                // create an exception with a vendor defined sub-code.
                throw new ServiceResultException(new ServiceResult(
                    result,
                    info.Key,
                    "http://localhost/UA/Server/",
                    new LocalizedText(info)));
            }
        }
        #endregion
    }
}
