﻿using Gistlyn.Common.Interfaces;
using ServiceStack.Caching;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Web;
using System.Threading.Tasks;
using Gistlyn.Common.Objects;
using System;
using Gistlyn.ServiceInterface;

namespace Gistlyn.ServiceInterfaces.Auth
{
    public class UserSession
    {
        ICacheClient cache;

        public UserSession(ICacheClient cache)
        {
            this.cache = cache;
        }

        public string GetSessionId()
        {
            var keyParts =  SessionFeature.GetSessionKey().Split(':');
            return keyParts[keyParts.Length - 1];
        }

        //public void SetGistHash(string hash)
        //{
        //    var session = SessionFeature.GetOrCreateSession<CustomUserSession>(cache);
        //    string key = SessionFeature.GetSessionKey();
        //    session.GistHash = hash;
        //    cache.Set<CustomUserSession>(key, session);
        //}

        public void SetScriptRunnerInfo(string gistHash, AppDomain scriptAppDomain, DomainWrapper wrapper)
        {
            var session = SessionFeature.GetOrCreateSession<CustomUserSession>(cache);
            string key = SessionFeature.GetSessionKey();
            ScriptRunnerInfo info = new ScriptRunnerInfo() { GistHash = gistHash, ScriptDomain = scriptAppDomain, DomainWrapper = wrapper };

            lock (session.lockObj)
            {
                if (session.Scripts.ContainsKey(gistHash))
                    session.Scripts[gistHash] = info;
                else
                    session.Scripts.Add(gistHash, info);
                    
            }
            cache.Set<CustomUserSession>(key, session);
        }

        public ScriptRunnerInfo GetScriptRunnerInfo(string gistHash)
        {
            var session = SessionFeature.GetOrCreateSession<CustomUserSession>(cache);
            string key = SessionFeature.GetSessionKey();

            lock (session.lockObj)
            {
                return session.Scripts.ContainsKey(gistHash) ? session.Scripts[gistHash] : null;
            }
        }

        public void LogoutUser()
        {
            string key = SessionFeature.GetSessionKey();
            cache.Remove(key);
        }

        public CustomUserSession GetCustomSession()
        {
            return SessionFeature.GetOrCreateSession<CustomUserSession>(cache); ;

            //if we need to check that the user is not authenticated yet (session can be null in this case);
            //string key = SessionFeature.GetSessionKey();
            //return cache.Get<CustomUserSession>(key);
        }
    }
}
