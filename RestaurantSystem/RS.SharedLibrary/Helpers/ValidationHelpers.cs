using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RS.SharedLibrary.Helpers
{
    /// <summary>
    /// Helpers for different kind of validations
    /// </summary>
    public static class ValidationHelpers
    {
        /// <summary>
        /// Validates an email address
        /// </summary>
        /// <param name="emailAddress">The email address to validate</param>
        public static bool ValidateEmailAddress(this string emailAddress)
        {
            try
            {
                // Start by doing some basic validations
                if (emailAddress.Count(c => (c == '@')) > 1)  return false;

                // Make sure it's only one dot (.) after the @ sign
                var lastPartOfEmailAddress = emailAddress.Split('@')[1];

                if (lastPartOfEmailAddress.Count(c => (c == '.')) != 1) return false;

                // Make sure there is something entered after the dot
                if (lastPartOfEmailAddress.Split('.')[1].Length < 2) return false;

                // Create a new mail address object
                var address = new MailAddress(emailAddress);

                // Verify that the parsed address is the same as the one sent in.
                // If true, the address is valid
                return address.Address == emailAddress;
            }
            catch
            {
                // If the object could not be created, we instantly return false
                return false;
            }
        }
    }
}
