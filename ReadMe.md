# Ldap-Lab

This is a web-application for laborating with LDAP.

## 1 Development

### 1.1 Connection-string

You can put your secret connection-strings in secrets.json (Manage User Secrets):

- C:\Users\{USER_NAME}\AppData\Roaming\Microsoft\UserSecrets\48ca523a-9c79-4742-905f-d4a4cb4b18ff

#### 1.1.1 Simple

	AuthenticationType=Anonymous;Servers=example

#### 1.1.2 Full

	AuthenticationType=Kerberos;CredentialDomain=EXAMPLE;CredentialPassword=**********;CredentialUserName=username;Port=636;ProtocolVersion=3;SecureSocketLayer=true;Servers=dc01.example.org,dc02.example.org,dc03.example.org;Timeout=00:00:10

### 1.2 Docker

When running in Docker from Visual Studio you may try different LDAP-ports if you get exceptions:

- 389 (standard none SSL)
- 636 (standard SSL)
- 3268 (global catalog none SSL)
- 3269 (global catalog SSL)

Not sure about these:

- 1389
- 1636

Links:

- [Google: ldap 3268 3269](https://www.google.com/search?q=ldap+3268+3269)
- [Is LDAP on Port 3269 Secure?](https://social.technet.microsoft.com/Forums/ie/en-US/9386b3de-b58b-4877-b6dd-8ae0ce834e93/is-ldap-on-port-3269-secure?forum=winserverDS)

## 2 Notes

### 2.1 Kerberos, Negotiate and Ntlm

To get it to work with the following:

- LdabConnection.AuthType = AuthType.Kerberos
- LdabConnection.AuthType = AuthType.Netogiate
- LdabConnection.AuthType = AuthType.Ntlm

We need to construct the credential like this:

	var credential = new NetworkCredential("username", "passsword", "domain");

This does not work (only with AuthType.Basic):

	var credential = new NetworkCredential("username", "passsword");

or

	var credential = new NetworkCredential("DOMAIN\\username", "passsword");