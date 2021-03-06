using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Rapid.Tools.SPDeploy.AddIn.UI.Forms
{
    public class Validator
    {
        public static bool IsValidGuid(string stringToCheck)
        {
            Regex guidRegex = new Regex(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$", RegexOptions.Compiled);
            return guidRegex.IsMatch(stringToCheck);
        }

        public static bool isValidUrl(string stringToCheck)
        {
            //  regex doesn't work???
            //Regex urlRegex = new Regex(@"^((http[s]?|ftp):\/)?\/?([^:\/\s]+)((\/\w+)*\/)([\w\-\.]+[^#?\s]+)(.*)?(#[\w\-]+)?$", RegexOptions.Compiled);
            //return urlRegex.IsMatch(stringToCheck);

            Uri uri = null;
            return Uri.TryCreate(stringToCheck, UriKind.Absolute, out uri);
        }     
    }
}
