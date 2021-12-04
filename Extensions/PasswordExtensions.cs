using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Extensions
{
    public static class PasswordExtensions
    {
        // Define default min and max password lengths.
        private const int DefaultMinPasswordLength = 8;
        private const int DefaultMaxPasswordLength = 25;

        // Define supported password characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        private const string PasswordCharsLowercase = "abcdefgijkmnopqrstwxyz";
        private const string PasswordCharsUppercase = "ABCDEFGHJKLMNPQRSTWXYZ";
        private const string PasswordCharsNumeric = "23456789";
        private const string PasswordCharsSpecial = "*$-+?_&=!%{}/";


        private static SHA256CryptoServiceProvider sha256CryptoServiceProvider;

        private static SHA256CryptoServiceProvider Sha256CryptoServiceProvider
        {
            get { return sha256CryptoServiceProvider ??= new SHA256CryptoServiceProvider(); }
        }

        public static string ToPasswordSha256Hash(this string password, byte[] salt = null)
        {
            byte[] result;
            var secretBytes = Encoding.UTF8.GetBytes(password);
            if (salt == null)
            {
                result = secretBytes;
            }
            else
            {
                result = new byte[secretBytes.Length + password.Length];
                secretBytes.CopyTo(result, 0);
                salt.CopyTo(result, secretBytes.Length);
            }

            return ToHex(Sha256CryptoServiceProvider.ComputeHash(result, 0, result.Length));
        }

        public static string ToPasswordHmacSha256Hash(this string password, string passwordKey = null)
        {
            passwordKey ??= "";

            using var hmac256 = new HMACSHA256(passwordKey.ToByteArray());
            var hash = hmac256.ComputeHash(password.ToByteArray());
            return Convert.ToBase64String(hash);
        }

        public static (byte[] hashedPassword, byte[] passwordKey) ToPasswordHmacSha512Hash(this string password)
        {
            using var hmac = new HMACSHA512();
            var passwordKey = hmac.Key;
            var passwordHash = hmac.ComputeHash(password.ToByteArray());
            return (passwordHash, passwordKey);
        }

        public static byte[] ToPasswordHmacSha512Hash(this string password, byte[] passwordKey)
        {
            using var hmac = new HMACSHA512(passwordKey);
            return hmac.ComputeHash(password.ToByteArray());
        }

        private static string ToHex(this byte[] bytes)
        {
            var c = new char[bytes.Length * 2];

            for (int bx = 0, cx = 0; bx < bytes.Length; ++bx, ++cx)
            {
                var b = ((byte)(bytes[bx] >> 4));
                c[cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);

                b = ((byte)(bytes[bx] & 0x0F));
                c[++cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);
            }

            return new string(c);
        }

        public static string GenerateRandomPassword()
        {
            return GenerateRandomPassword(DefaultMinPasswordLength, DefaultMaxPasswordLength);
        }

        public static string GenerateRandomPassword(int length)
        {
            return GenerateRandomPassword(length, length);
        }

        public static string GenerateRandomPassword(int minLength, int maxLength, bool hasSpecialCharacters = true)
        {
            // Make sure that input parameters are valid.
            if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                return null;

            var charGroups = GetCharGroups();

            if (hasSpecialCharacters)
            {
                charGroups.Add(PasswordCharsSpecial.ToCharArray());
            }

            // Use this array to track the number of unused characters in each
            // character group.
            var charsLeftInGroup = new int[charGroups.Count];

            // Initially, all characters in each group are not used.
            for (var i = 0; i < charsLeftInGroup.Length; i++)
                charsLeftInGroup[i] = charGroups[i].Length;

            // Use this array to track (iterate through) unused character groups.
            var leftGroupsOrder = new int[charGroups.Count];

            // Initially, all character groups are not used.
            for (var i = 0; i < leftGroupsOrder.Length; i++)
                leftGroupsOrder[i] = i;

            // Because we cannot use the default randomizer, which is based on the
            // current time (it will produce the same "random" number within a
            // second), we will use a random number generator to seed the
            // randomizer.

            // Use a 4-byte array to fill it with random bytes and convert it then
            // to an integer value.
            var randomBytes = new byte[4];

            // Generate 4 random bytes.
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            // Convert 4 bytes into a 32-bit integer value.
            var seed = (randomBytes[0] & 0x7f) << 24 |
                       randomBytes[1] << 16 |
                       randomBytes[2] << 8 |
                       randomBytes[3];

            // Now, this is real randomization.
            var random = new Random(seed);

            // This array will hold password characters.
            // Allocate appropriate memory for the password.
            var password = minLength < maxLength
                ? new char[random.Next(minLength, maxLength + 1)]
                : new char[minLength];

            // Index of the last non-processed group.
            var lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

            GeneratePasswordCharacters(password, lastLeftGroupsOrderIdx, random, leftGroupsOrder, charsLeftInGroup, charGroups);

            // Convert password characters into a string and return the result.
            return new string(password);
        }

        private static void GeneratePasswordCharacters(char[] password, int lastLeftGroupsOrderIdx, Random random, int[] leftGroupsOrder,
            int[] charsLeftInGroup, List<char[]> charGroups)
        {
            // Generate password characters one at a time.
            for (var i = 0; i < password.Length; i++)
            {
                // If only one character group remained unprocessed, process it
                // otherwise, pick a random character group from the unprocessed
                // group list. To allow a special character to appear in the
                // first position, increment the second parameter of the Next
                // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                var nextLeftGroupsOrderIdx = lastLeftGroupsOrderIdx == 0
                    ? 0
                    : random.Next(0, lastLeftGroupsOrderIdx);

                // Get the actual index of the character group, from which we will
                // pick the next character.
                var nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                // Get the index of the last unprocessed characters in this group.
                var lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                // If only one unprocessed character is left, pick it; otherwise,
                // get a random character from the unused character list.
                var nextCharIdx = lastCharIdx == 0
                    ? 0
                    : random.Next(0, lastCharIdx + 1);

                // Add this character to the password.
                password[i] = charGroups[nextGroupIdx][nextCharIdx];

                ProcessCharacters(charsLeftInGroup, charGroups, lastCharIdx, nextGroupIdx, nextCharIdx);

                // If we processed the last group, start all over.
                if (lastLeftGroupsOrderIdx == 0)
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                // There are more unprocessed groups left.
                else
                {
                    // Swap processed group with the last unprocessed group
                    // so that we don't pick it until we process all groups.
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        var temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] =
                            leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }

                    // Decrement the number of unprocessed groups.
                    lastLeftGroupsOrderIdx--;
                }
            }
        }

        private static void ProcessCharacters(int[] charsLeftInGroup, List<char[]> charGroups, int lastCharIdx, int nextGroupIdx, int nextCharIdx)
        {
            // If we processed the last character in this group, start over.
            if (lastCharIdx == 0)
                charsLeftInGroup[nextGroupIdx] = charGroups[nextGroupIdx].Length;
            // There are more unprocessed characters left.
            else
            {
                // Swap processed character with the last unprocessed character
                // so that we don't pick it until we process all characters in
                // this group.
                if (lastCharIdx != nextCharIdx)
                {
                    var temp = charGroups[nextGroupIdx][lastCharIdx];
                    charGroups[nextGroupIdx][lastCharIdx] =
                        charGroups[nextGroupIdx][nextCharIdx];
                    charGroups[nextGroupIdx][nextCharIdx] = temp;
                }

                // Decrement the number of unprocessed characters in
                // this group.
                charsLeftInGroup[nextGroupIdx]--;
            }
        }

        private static List<char[]> GetCharGroups()
        {
            // Create a local array containing supported password characters
            // grouped by types. You can remove character groups from this
            // array, but doing so will weaken the password strength.
            var charGroups = new List<char[]>
            {
                PasswordCharsLowercase.ToCharArray(),
                PasswordCharsUppercase.ToCharArray(),
                PasswordCharsNumeric.ToCharArray()
            };
            return charGroups;
        }
    }
}
