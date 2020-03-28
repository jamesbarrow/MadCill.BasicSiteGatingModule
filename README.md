# Basic Site Gating Module
A simple HTTP Module for IIS to gate a site behind a single password. Designed to protect staging sites from public viewing.

## Instructions
**Add the dll to the bin directory:**
MadCill.BasicSiteGatingModule.dll

**Add the following to the very top of the <webServer><modules> section of the web config:**
```xml
 <add type="MadCill.BasicSiteGatingModule.SimpleGatingModule, JamesBarrow.BasicSiteGatingModule" name="SimpleSecurityControl" />
```

**(OPTIONAL) Add the following settings to the app config section - each param is optional, password at least, is advised**

```xml
<!--Simple Gating Section-->
<add key="SimpleSecurity.Password" value="!password" />
<add key="SimpleSecurity.CookieName" value="SimpleGating" />
<add key="SimpleSecurity.SessionLifetime" value="0" />
<!--Add in ip addresses, no ports, separate them by a semi-colon ';'-->
<add key="SimpleSecurity.IPWhitelist" value="0" />
<!--Types are "Hashed" or "SimpleEncryption" note "SimpleEncryption" requires an encryption key and iv-->
<add key="SimpleSecurity.SecurityType" value="Hashed" />
<!--If using SimpleEncryption provide an 8 character key-->
<add key="SimpleSecurity.EncryptionKey" value="12345678" />
<!--If using SimpleEncryption provide an 8 character iv-->
<add key="SimpleSecurity.EncryptionIV" value="abcdefgh" />
<!--END Simple Gating Section-->
```
