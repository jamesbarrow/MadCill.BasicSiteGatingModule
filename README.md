# Basic Site Gating Module
A simple HTTP Module for IIS to gate a site behind a single password. Designed to protect staging sites from public viewing.

## Instructions
**Add the dll to the bin directory:**
MadCill.BasicSiteGatingModule.dll

#Add the following to the very top of the <webServer><modules> section of the web config: 
<add type="MadCill.BasicSiteGatingModule.SimpleGatingModule, MadCill.BasicSiteGatingModule" name="SimpleSecurityControl" />

**(OPTIONAL) Add the following settings to the app config section - each param is optional, password at least, is advised**

    <!--Simple Gating Section-->
    <add key="SimpleSecurity.Password" value="!password" />
    <add key="SimpleSecurity.CookieName" value="SimpleGating" />
    <add key="SimpleSecurity.SessionLifetime" value="0" />
	<!--Add in domain names, separate them by a semi-colon ';'-->
    <add key="SimpleSecurity.DomainWhitelist" value="www.domain1.com;www.domain2.com" />
    <!--Add in ip addresses, no ports, separate them by a semi-colon ';'-->
    <add key="SimpleSecurity.IPWhitelist" value="0" />
    <!--Add in relative url addresses separate them by a semi-colon ';'. Wildcards can be used to ungate sub pages or relative paths
    If you want to ungate a specific page you may also need to ungate tracking and media, i.e. /yourpage;/layouts/system/~;/-/media/~ -->
    <add key="SimpleSecurity.UrlWhitelist" value="/unicorn.aspx" />
    <!--Types are "Hashed" or "SimpleEncryption" note "SimpleEncryption" requires an encryption key and iv-->
    <add key="SimpleSecurity.SecurityType" value="Hashed" />
    <!--If using SimpleEncryption provide an 8 character key-->
    <add key="SimpleSecurity.EncryptionKey" value="12345678" />
    <!--If using SimpleEncryption provide an 8 character iv-->
    <add key="SimpleSecurity.EncryptionIV" value="abcdefgh" />
    <!--If using CDN or something else that can't use cookies to to bypass the gating, then you can apply an http-header name and code. Provide a name with alpha-numeric characters only, no spaces-->
    <add key="SimpleSecurity.HttpHeaderParameter" value="bypass-gating" />
    <!--If using SimpleEncryption provide an 8 character iv-->
    <add key="SimpleSecurity.HttpHeaderCode" value="testing" />
    <!--If a custom login page is required - this can be provided here. Not included here will default to the basic gating page. -->
    <add key="SimpleSecurity.CustomLoginHtml" value="/App_Data/SiteGating/CustomGatingPage.html" />
    <!--OR Separating it in a {domain}|{path to login};{domain}|{path to login};{default login page} will allow a login page per domain
    <add key="SimpleSecurity.CustomLoginHtml" value="domain1.com|/LoginPage1.html;domain2.com|/LoginPage2.html;/LoginPage1.html" />-->
    <!--END Simple Gating Section-->
