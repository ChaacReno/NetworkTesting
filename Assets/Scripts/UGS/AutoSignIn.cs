using UnityEngine;
using UnityEngine.Events;
namespace UGS
{
    public class AutoSignIn : MonoBehaviour
    {
        public static readonly UnityEvent HandleSignIn = new();
        
        private void Start()
        {
            AuthService.SignInAnonymous(HandleSignIn.Invoke);
        }
    }
}