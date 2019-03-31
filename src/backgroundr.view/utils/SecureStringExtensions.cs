using System;
using System.Runtime.InteropServices;
using System.Security;

namespace backgroundr.view.utils
{
    internal static class SecureStringExtensions
    {
        /// <summary>
        ///     Converts the source string into a secure string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <returns>A secure version of the source string.</returns>
        public static SecureString ToSecureString(this string source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var result = new SecureString();
            foreach (var c in source.ToCharArray()) {
                result.AppendChar(c);
            }
            result.MakeReadOnly();
            return result;
        }

        /// <summary>
        ///     Converts the source secure string into a standard insecure string.
        /// </summary>
        /// <param name="source">The source secure string.</param>
        /// <returns>The standard insecure string.</returns>
        public static string ToInsecureString(this SecureString source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var unmanagedString = IntPtr.Zero;
            try {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(source);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}