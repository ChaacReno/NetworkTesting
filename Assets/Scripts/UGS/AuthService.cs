using System;
using Unity.Services.Authentication;
using Unity.Services.Core;

namespace UGS
{
    public static class AuthService
    {
        public static async void SignInAnonymous(Action callback = null)
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
            
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            callback?.Invoke();
        }
    }
}