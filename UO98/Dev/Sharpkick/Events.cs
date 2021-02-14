using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Sharpkick
{
    /// <summary>Delegate for EventSink.OnLogin event</summary>
    /// <param name="e">Arguments to the event, requires Username And Password</param>
    delegate void OnLoginEventHandler(LoginEventArgs e);

    /// <summary>Handles server events</summary>
    static class EventSink
    {
        /// <summary>Event fires when a login packet is received (0x80)</summary>
        public static event OnLoginEventHandler OnLogin;
        /// <summary>Invoke the OnLogin event.</summary>
        /// <param name="e">Arguments to the event, requires Username And Password</param>
        public static void InvokeOnLogin(LoginEventArgs e) { if (OnLogin != null) OnLogin(e); }
    }

    class LoginEventArgs : EventArgs
    {
        /// <summary>IN: The supplied Username</summary>
        public string Username { get; private set; }
        
        /// <summary>IN: The supplied Password</summary>
        public string Password { get; private set; }

        /// <summary>IN: The client IP Address</summary>
        public IPAddress IPAddress { get; private set; }

        /// <summary>OUT: Set if the the event was handled. A return value of false will result in the supplied values being sent to the server.</summary>
        public bool Handled { get; set; }
        
        /// <summary>OUT: Set if the login is accepted</summary>
        public bool Accepted { get; set; }

        /// <summary>OUT: If accepted, this is the account id for the user</summary>
        public int AccountID { get; set; }

        /// <summary>OUT: If rejected, this is the reason</summary>
        public ALRReason RejectedReason { get; set; }

        /// <summary>
        /// Event arguments for Login Packet (0x80) received event.
        /// </summary>
        /// <param name="username">The supplied username, not case sensitive (converted)</param>
        /// <param name="password">The supplied password, case sensitive</param>
        /// <param name="ipaddress">IPAddress of creating client</param>
        public LoginEventArgs(string username, string password, IPAddress ipaddress)
        {
            Username = username.ToLowerInvariant();
            Password = password;
            IPAddress = ipaddress;
        }
    }

}
