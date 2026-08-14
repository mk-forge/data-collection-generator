namespace TupleGeneratorGUI.Services {
    public class LDAPAuthService {
        public bool Login(string username, string password) {
            try {
                string trailingContext = username[^1].ToString();
                string userDN = $"cn={username},ou={trailingContext},ou=USERS,o=VSB";
                LdapDirectoryIdentifier identifier = new LdapDirectoryIdentifier("ldap.vsb.cz", 636, true, false);

                using (LdapConnection connection = new LdapConnection(identifier)) {
                    connection.AuthType = AuthType.Basic;
                    connection.SessionOptions.ProtocolVersion = 3;
                    connection.SessionOptions.ReferralChasing = ReferralChasingOptions.None;
                    connection.SessionOptions.SecureSocketLayer = true;
                    connection.SessionOptions.VerifyServerCertificate += (conn, cert) => true;
                    connection.Timeout = TimeSpan.FromSeconds(3);

                    NetworkCredential credential = new NetworkCredential(userDN, password);
                    connection.Bind(credential);
                }

                return true;
            } catch {
                return false;
            }
        }
    }
}