using System;
using System.Reflection;
using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;

namespace Talifun.Commander.Command.DropBoxUploader.Configuration
{
    public static class DropBoxExtensions
    {
        private static readonly Assembly Assembly = typeof (DropBoxRequestToken).Assembly;

        private static object GetOAuthToken(string dropBoxRequestKey, string dropBoxRequestSecret)
        {
            var oAuthToken = Assembly.CreateInstance("AppLimit.CloudComputing.SharpBox.Common.Net.oAuth.Token.OAuthToken",
                false,
                BindingFlags.Default | BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new Object[] { dropBoxRequestKey, dropBoxRequestSecret },
                null,
                null);

            return oAuthToken;
        }

        public static DropBoxRequestToken GetDropBoxRequestToken(string dropBoxRequestKey, string dropBoxRequestSecret)
        {
            var oAuthToken = GetOAuthToken(dropBoxRequestKey, dropBoxRequestSecret);

            var requestToken = (DropBoxRequestToken)Assembly.CreateInstance("AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.DropBoxRequestToken",
                false,
                BindingFlags.Default | BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new Object[] { oAuthToken },
                null,
                null);

            return requestToken;
        }

        private static FieldInfo _getRealTokenFieldInfo;
        private static object GetRealToken(DropBoxRequestToken dropBoxRequestToken)
        {
            if (_getRealTokenFieldInfo == null)
            {
                _getRealTokenFieldInfo = dropBoxRequestToken
                    .GetType()
                    .GetField("RealToken", BindingFlags.Instance | BindingFlags.NonPublic);
            }

            var realToken = _getRealTokenFieldInfo.GetValue(dropBoxRequestToken);

            return realToken;
        }

        private static PropertyInfo _getRequestTokenKeyPropertyInfo;
        public static string GetTokenKey(this DropBoxRequestToken dropBoxRequestToken)
        {
            var realToken = GetRealToken(dropBoxRequestToken);

            if (_getRequestTokenKeyPropertyInfo == null)
            {
                _getRequestTokenKeyPropertyInfo = realToken
                    .GetType()
                    .GetProperty("TokenKey", BindingFlags.Instance | BindingFlags.Public);
            }

            var tokenKey = (string)_getRequestTokenKeyPropertyInfo.GetValue(realToken, null);
            return tokenKey;
        }

        private static PropertyInfo _getRequestTokenSecretPropertyInfo;
        public static string GetTokenSecret(this DropBoxRequestToken dropBoxRequestToken)
        {
            var realToken = GetRealToken(dropBoxRequestToken);

            if (_getRequestTokenSecretPropertyInfo == null)
            {
                _getRequestTokenSecretPropertyInfo = realToken
                    .GetType()
                    .GetProperty("TokenSecret", BindingFlags.Instance | BindingFlags.Public);
            }

            var tokenSecret = (string)_getRequestTokenSecretPropertyInfo.GetValue(realToken, null);
            return tokenSecret;
        }

        public static ICloudStorageAccessToken GetDropBoxAccessToken(string dropBoxRequestKey, string dropBoxRequestSecret, string appkey, string appsecret)
        {
            var oAuthToken = GetOAuthToken(dropBoxRequestKey, dropBoxRequestSecret);
            var dropBoxBaseTokenInformation = new DropBoxBaseTokenInformation
                {
                    ConsumerKey = appkey,
                    ConsumerSecret = appsecret
                };

            var requestToken = (DropBoxRequestToken)Assembly.CreateInstance("AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.DropBoxToken",
                false,
                BindingFlags.Default | BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public,
                null,
                new Object[] { oAuthToken, dropBoxBaseTokenInformation },
                null,
                null);

            return requestToken;
        }

        private static PropertyInfo _getAccessTokenKeyPropertyInfo;
        public static string GetTokenKey(this ICloudStorageAccessToken dropBoxAccessToken)
        {
            if (_getAccessTokenKeyPropertyInfo == null)
            {
                _getAccessTokenKeyPropertyInfo = dropBoxAccessToken
                    .GetType()
                    .Assembly
                    .GetType("AppLimit.CloudComputing.SharpBox.Common.Net.oAuth.Token.OAuthToken")
                    .GetProperty("TokenKey", BindingFlags.Instance | BindingFlags.Public);
            }

            var tokenKey = (string)_getAccessTokenKeyPropertyInfo.GetValue(dropBoxAccessToken, null);
            return tokenKey;
        }

        private static PropertyInfo _getAccessTokenSecretPropertyInfo;
        public static string GetTokenSecret(this ICloudStorageAccessToken dropBoxAccessToken)
        {
            if (_getAccessTokenSecretPropertyInfo == null)
            {
                _getAccessTokenSecretPropertyInfo = dropBoxAccessToken
                    .GetType()
                    .Assembly
                    .GetType("AppLimit.CloudComputing.SharpBox.Common.Net.oAuth.Token.OAuthToken")
                    .GetProperty("TokenSecret", BindingFlags.Instance | BindingFlags.Public);
            }

            var tokenSecret = (string)_getAccessTokenSecretPropertyInfo.GetValue(dropBoxAccessToken, null);
            return tokenSecret;
        }
    }
}
