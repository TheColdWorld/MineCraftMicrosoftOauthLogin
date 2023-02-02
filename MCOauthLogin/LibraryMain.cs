/*
    Copyright (©) 2022-2023  TheColdWorld

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
    USA
*/
namespace MCOauthLogin
{
	public abstract class LoginUris
    {
		protected LoginUris() { }
		public const string FirstLoginUri = "https://login.live.com/oauth20_authorize.srf?client_id=00000000402b5328&response_type=code&scope=service%3A%3Auser.auth.xboxlive.com%3A%3AMBI_SSL&redirect_uri=https%3A%2F%2Flogin.live.com%2Foauth20_desktop.srf";//第一步登录(网页浏览器)
		public const string SecondLoginUri = "https://login.live.com/oauth20_token.srf";//微软令牌(POST)[ List<KeyValuePair<string, string>> ]
        public const string ThirdLoginUri = "https://user.auth.xboxlive.com/user/authenticate";//Xbox Live令牌(POST)[ Json ] { System.Net.Http.Headers.MediaTypeHeaderValue("application/json") }
        public const string ForthLoginUri = "https://xsts.auth.xboxlive.com/xsts/authorize";//Xsts令牌(POST) [ Json ] { System.Net.Http.Headers.MediaTypeHeaderValue("application/json") }
        public const string FifthLoginUri = "https://api.minecraftservices.com/authentication/login_with_xbox";//Minecraft令牌(POST) [ Json ] { System.Net.Http.Headers.MediaTypeHeaderValue("application/json") }
        public const string CheckHaveMCAbleUri = "https://api.minecraftservices.com/entitlements/mcstore";//检查是否有MC(GET) AuthenticationHeaderValue("Bearer", McToken)
		public const string GetMCProfileUri = "https://api.minecraftservices.com/minecraft/profile";//MC档案(用户名和UUID)
    }
	public class LoginCore
	{
        protected LoginCore() { }
		public LoginCore(string Token)
		{
            System.Collections.Generic.List<KeyValuePair<string, string>> pair = new System.Collections.Generic.List<KeyValuePair<string, string>>
            {
                new("client_id", "00000000402b5328"),
                new("code", Token),
                new("grant_type", "authorization_code"),
                new("redirect_uri", "https://login.live.com/oauth20_desktop.srf"),
                new("scope", "service::user.auth.xboxlive.com::MBI_SSL")
            };
            System.Net.Http.HttpContent Context = new System.Net.Http.FormUrlEncodedContent(pair);
            System.Net.Http.HttpResponseMessage Msg = httpClient.PostAsync(LoginUris.SecondLoginUri, Context).GetAwaiter().GetResult();
            Msg.EnsureSuccessStatusCode();
            CsharpJson.JsonDocument tmp1 = CsharpJson.JsonDocument.FromString(Msg.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            _microsoftToken = tmp1.Object["access_token"].ToString();
            _microsoftReFreshToken = tmp1.Object["refresh_token"].ToString();
            _XBLToken = string.Empty;
            _XstsToken = string.Empty;
            _UserHash = string.Empty;
            _MinecraftToken = string.Empty;
            _UUID = string.Empty;
            _UserName = string.Empty;
        }
        public LoginCore(System.Uri uri)
        {
            string Token = uri.ToString().Substring(uri.ToString().IndexOf("=") + 1).Split(new string[] { "&" }, StringSplitOptions.None)[0];
           System.Collections.Generic.List<KeyValuePair<string, string>> pair = new System.Collections.Generic.List<KeyValuePair<string, string>>
            {
                new("client_id", "00000000402b5328"),
                new("code", Token),
                new("grant_type", "authorization_code"),
                new("redirect_uri", "https://login.live.com/oauth20_desktop.srf"),
                new("scope", "service::user.auth.xboxlive.com::MBI_SSL")
            };
            System.Net.Http.HttpContent Context = new System.Net.Http.FormUrlEncodedContent(pair);
            System.Net.Http.HttpResponseMessage Msg = httpClient.PostAsync(LoginUris.SecondLoginUri, Context).GetAwaiter().GetResult();
            Msg.EnsureSuccessStatusCode();
            CsharpJson.JsonDocument tmp1 = CsharpJson.JsonDocument.FromString(Msg.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            _microsoftToken = tmp1.Object["access_token"].ToString();
            _microsoftReFreshToken = tmp1.Object["refresh_token"].ToString();
            _XBLToken = string.Empty;
            _XstsToken = string.Empty;
            _MinecraftToken = string.Empty;
            _UserHash = string.Empty;
            _UUID = string.Empty;
            _UserName = string.Empty;
        }
        public void GetXblToken()
        {
            CsharpJson.JsonObject tmp = new();
            CsharpJson.JsonObject tmp1 = new();
            tmp["AuthMethod"] = "RPS";
            tmp["SiteName"] = "user.auth.xboxlive.com";
            tmp["RpsTicket"] = _microsoftToken;
            tmp1["Properties"] = tmp;
            tmp1["RelyingParty"] = "http://auth.xboxlive.com";
            tmp1["TokenType"] = "JWT";
            HttpContent Context= new StringContent(new CsharpJson.JsonDocument() { Object = tmp1 }.ToJson());
            Context.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            System.Net.Http.HttpResponseMessage Msg = httpClient.PostAsync(LoginUris.ThirdLoginUri, Context).GetAwaiter().GetResult();
            Msg.EnsureSuccessStatusCode();
            CsharpJson.JsonDocument tmp2 = CsharpJson.JsonDocument.FromString(Msg.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            _XBLToken= tmp2.Object["Token"].ToString();
            _UserHash = tmp2.Object["DisplayClaims"].ToObject()["xui"].ToArray()[0].ToObject()["uhs"].ToString();
        }
        public void Refresh_microsoftToken()
        {
            System.Collections.Generic.List<KeyValuePair<string, string>> pair = new System.Collections.Generic.List<KeyValuePair<string, string>>
            {
                new("client_id", "00000000402b5328"),
                new("refresh_token", _microsoftReFreshToken),
                new("grant_type", "refresh_token"),
                new("redirect_uri", "https://login.live.com/oauth20_desktop.srf"),
                new("scope", "service::user.auth.xboxlive.com::MBI_SSL")
            };
            System.Net.Http.HttpContent Context = new System.Net.Http.FormUrlEncodedContent(pair);
            System.Net.Http.HttpResponseMessage Msg = httpClient.PostAsync(LoginUris.SecondLoginUri, Context).GetAwaiter().GetResult();
            Msg.EnsureSuccessStatusCode();
            CsharpJson.JsonDocument tmp1 = CsharpJson.JsonDocument.FromString(Msg.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            _microsoftToken = tmp1.Object["access_token"].ToString();
            _microsoftReFreshToken = tmp1.Object["refresh_token"].ToString();
        }
        public void GetXstsToken()
        {
            if (_XBLToken == string.Empty) throw new XboxLoginException("未获取Xbox Live Token");
            CsharpJson.JsonObject tmp1 = new();
            tmp1["SandboxId"] = "RETAIL";
            CsharpJson.JsonArray tmp2 = new();
            tmp2[0] = new CsharpJson.JsonValue(_XBLToken);
            tmp1["UserTokens"] = tmp2;
            CsharpJson.JsonObject tmp3 = new();
            tmp3["Properties"] = tmp1;
            tmp3["RelyingParty"] = "rp://api.minecraftservices.com/";
            tmp3["TokenType"] = "JWT";
            System.Net.Http.HttpContent Context = new StringContent(new CsharpJson.JsonDocument() { Object = tmp3 }.ToJson());
            Context.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            System.Net.Http.HttpResponseMessage Msg = httpClient.PostAsync(LoginUris.ForthLoginUri, Context).GetAwaiter().GetResult();
            Msg.EnsureSuccessStatusCode();
            tmp1= CsharpJson.JsonDocument.FromString(Msg.Content.ReadAsStringAsync().GetAwaiter().GetResult()).Object;
            _XstsToken = tmp1["Token"].ToString();
            if (_UserHash != tmp1["DisplayClaims"].ToObject()["xui"].ToArray()[0].ToObject()["uhs"].ToString()) throw new XboxLoginException("User Hash 不同");
        }
        public void GetMcToken()
        {
            if (_XBLToken == string.Empty) throw new XboxLoginException("未获取Xbox Live Token");
            if (_XstsToken == string.Empty) throw new XboxLoginException("未获取Xsts Token");
            CsharpJson.JsonObject tmp = new();
            tmp["identityToken"] = "XBL3.0 x=" + _UserHash + ";" + _XstsToken;
            System.Net.Http.HttpContent Context = new StringContent(new CsharpJson.JsonDocument() { Object = tmp }.ToJson());
            Context.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            System.Net.Http.HttpResponseMessage Msg = httpClient.PostAsync(LoginUris.FifthLoginUri, Context).GetAwaiter().GetResult();
            Msg.EnsureSuccessStatusCode();
            _MinecraftToken= CsharpJson.JsonDocument.FromString(Msg.Content.ReadAsStringAsync().GetAwaiter().GetResult()).Object["access_token"].ToString();
        }
        public bool GetHaveMcAble()
        {
            if (_XBLToken == string.Empty) throw new XboxLoginException("未获取Xbox Live Token");
            if (_XstsToken == string.Empty) throw new XboxLoginException("未获取Xsts Token");
            if (_MinecraftToken == string.Empty) throw new XboxLoginException("未获取Minecraft Token");
            System.Net.Http.HttpClient httpClient1 = new();
            httpClient1.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _MinecraftToken);
            System.Net.Http.HttpResponseMessage Msg = httpClient1.GetAsync(LoginUris.CheckHaveMCAbleUri).GetAwaiter().GetResult();
            Msg.EnsureSuccessStatusCode();
            if (!string.IsNullOrEmpty(Msg.Content.ReadAsStringAsync().GetAwaiter().GetResult()))
            {
                _HaveMcAble = true;
            return true;
            }
           else
            {
                throw new XboxLoginException("你没有Minecraft");
            }
        }
        public McProfile GetMcProFile()
        {
            if (_XBLToken == string.Empty) throw new XboxLoginException("未获取Xbox Live Token");
            if (_XstsToken == string.Empty) throw new XboxLoginException("未获取Xsts Token");
            if (_MinecraftToken == string.Empty) throw new XboxLoginException("未获取Minecraft Token");
            if (!GetHaveMcAble()) throw new XboxLoginException("你没有Minecraft");
            System.Net.Http.HttpClient httpClient1 = new();
            httpClient1.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _MinecraftToken);
            System.Net.Http.HttpResponseMessage Msg = httpClient1.GetAsync(LoginUris.GetMCProfileUri).GetAwaiter().GetResult();
            Msg.EnsureSuccessStatusCode();
            CsharpJson.JsonObject tmp = CsharpJson.JsonDocument.FromString(Msg.Content.ReadAsStringAsync().GetAwaiter().GetResult()).Object;
            _UUID = tmp["id"].ToString();
            _UserName = tmp["name"].ToString();
            return new McProfile(tmp["name"].ToString(), tmp["id"].ToString(), _MinecraftToken);
        }
        private System.Net.Http.HttpClient httpClient = new();
        public string MicrosoftToken { get { return _microsoftToken; } }
        public string MicrosoftReFreshToken { get { return _microsoftReFreshToken; } }
        public string XBLToken { get { return _XBLToken; } }
        public string UserHash { get { return _UserHash; } }
        public string XstsToken { get { return  _XstsToken;  } }
        public string MinecraftToken { get { return _MinecraftToken;  } }
        public bool HaveMcAble { get { return _HaveMcAble;  } }
        public string UUID { get { return _UUID; } }
        public string UserName { get { return _UserName; } }
        private string _microsoftToken;
        private string _microsoftReFreshToken;
        private string _XBLToken;
        private string _UserHash;
        private string _XstsToken;
        private string _MinecraftToken;
        private bool _HaveMcAble;
        private string _UUID;
        private string _UserName;
    }
    public class McProfile
    {
        protected McProfile() { }
        public McProfile(string Token)
        {
            LoginCore loginCore = new(Token);
            loginCore.GetXblToken();
            loginCore.GetXstsToken();
            loginCore.GetMcToken();
            loginCore.GetMcProFile();
            UserName = loginCore.UserName;
            UserUUID = loginCore.UUID;
            MinecraftToken = loginCore.MinecraftToken;
        }
        public McProfile(System.Uri Uri)
        {
            LoginCore loginCore = new(Uri);
            loginCore.GetXblToken();
            loginCore.GetXstsToken();
            loginCore.GetMcToken();
            loginCore.GetMcProFile();
            UserName = loginCore.UserName;
            UserUUID = loginCore.UUID;
            MinecraftToken = loginCore.MinecraftToken;
        }
        public McProfile(string username, string useruuid, string Mctoken)
        {
            UserName = username;UserUUID = useruuid; MinecraftToken= Mctoken;
        }
        public string UserName;
        public string UserUUID;
        public string MinecraftToken;
    }

    public class XboxLoginException : System.ApplicationException
    {
        public XboxLoginException(string Message):base(Message) {}
    }
}
