#if NETCOREAPP
using System.Text;
#endif

namespace LibIRD.DiscUtils.CoreCompat
{
    internal static class EncodingHelper
    {
        private static bool _registered;

        public static void RegisterEncodings()
        {
            if (_registered)
                return;

            _registered = true;

#if NETCOREAPP
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        }
    }
}