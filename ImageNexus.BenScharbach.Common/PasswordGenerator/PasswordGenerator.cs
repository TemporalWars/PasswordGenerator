//-----------------------------------------------------------------------------
// PasswordGenerator.cs
//
// Ben Scharbach - Simple Password Generator Tool - 10/01/2012
// Copyright (C) Image-Nexus, LLC. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace ImageNexus.BenScharbach.Common.PasswordGenerator
{
    /// <summary>
    /// The <see cref="PasswordGenerator"/> is used to generate random passwords with lengths of 0-99.
    /// </summary>
    public class PasswordGenerator : IPasswordGenerator
    {
        // Enum of character sets
        internal enum CharacterSetsEnum
        {
            AlphaLowerCase,
            AlphaUpperCase,
            Numeric,
            Symbols,
        }

        // Character sets
        private readonly char[] _alphaLowCharacterSet = new[]{ 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        private readonly char[] _alphaUpCharacterSet = new[]{ 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        private readonly char[] _numbericCharacterSet = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '9' };
        private readonly char[] _symbolCharacterSet = new[] { '~', '!', '@', '#', '$', '%', '^', '&', '*' };

        // Dictionary holding character sets; key = enum, value = char[]
        internal readonly Dictionary<CharacterSetsEnum, char[]> CharacterSets = new Dictionary<CharacterSetsEnum,char[]>(); 

        // Password length
        internal readonly int PasswordLength;
        internal const int MaxPasswordLength = 99;
        // Character sets count.
        internal int CharacterSetsCount;
        internal CharacterSetsEnum[] CharacterSetsAvailable;
        internal List<CharacterSetsEnum> CharacterSetsForRandom;
        private bool _useAlphaLowercaseSet;
        private bool _useAlphaUppercaseSet;
        private bool _useNumericSet;
        private bool _useSymbolSet;
        private static readonly Random RandomGenerator = new Random();
        

        #region Properties

        /// <summary>
        /// Gets or sets to use the lowercase Alpha set.
        /// </summary>
        public bool UseAlphaLowercaseSet
        {
            get { return _useAlphaLowercaseSet; }
            set
            {
                if (value)
                {
                    if (!CharacterSets.ContainsKey(CharacterSetsEnum.AlphaLowerCase))
                        CharacterSets.Add(CharacterSetsEnum.AlphaLowerCase, _alphaLowCharacterSet);
                }
                else
                {
                    CharacterSets.Remove(CharacterSetsEnum.AlphaLowerCase);
                }
                UpdateAvailableCharacterSets();

                _useAlphaLowercaseSet = value;
            }
        }

        /// <summary>
        /// Gets or sets to use the uppercase Alpha set.
        /// </summary>
        public bool UseAlphaUppercaseSet
        {
            get { return _useAlphaUppercaseSet; }
            set
            {
                if (value)
                {
                    if (!CharacterSets.ContainsKey(CharacterSetsEnum.AlphaUpperCase))
                        CharacterSets.Add(CharacterSetsEnum.AlphaUpperCase, _alphaUpCharacterSet);
                }
                else
                {
                    CharacterSets.Remove(CharacterSetsEnum.AlphaUpperCase);
                }
                UpdateAvailableCharacterSets();

                _useAlphaUppercaseSet = value;
            }
        }

        /// <summary>
        /// Gets or sets to use the numeric set.
        /// </summary>
        public bool UseNumericSet
        {
            get { return _useNumericSet; }
            set
            {
                if (value)
                {
                    if (!CharacterSets.ContainsKey(CharacterSetsEnum.Numeric))
                        CharacterSets.Add(CharacterSetsEnum.Numeric, _numbericCharacterSet);
                }
                else
                {
                    CharacterSets.Remove(CharacterSetsEnum.Numeric);
                }
                UpdateAvailableCharacterSets();

                _useNumericSet = value;
            }
        }

        /// <summary>
        /// Gets or sets to use the symbol set.
        /// </summary>
        public bool UseSymbolSet
        {
            get { return _useSymbolSet; }
            set
            {
                if (value)
                {
                    if (!CharacterSets.ContainsKey(CharacterSetsEnum.Symbols))
                        CharacterSets.Add(CharacterSetsEnum.Symbols, _symbolCharacterSet);
                }
                else
                {
                    CharacterSets.Remove(CharacterSetsEnum.Symbols);
                }
                UpdateAvailableCharacterSets();

                _useSymbolSet = value;
            }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="passwordLength">Password length to generate. (1-99)</param>
        public PasswordGenerator(int passwordLength)
        {
            if (passwordLength < 1 || passwordLength > MaxPasswordLength)
                throw new ArgumentOutOfRangeException("passwordLength", string.Format("Password length given MUST fall in the range of 0-{0}.",MaxPasswordLength));

            PasswordLength = passwordLength;

            // Set default sets to use
            UseAlphaLowercaseSet = true;
            UseAlphaUppercaseSet = true;
            UseNumericSet = true;

            UpdateAvailableCharacterSets();
        }

        /// <summary>
        /// Generates a password.
        /// </summary>
        /// <returns>Generated password as string.</returns>
        public string Generate()
        {
            var passwordStringBuilder = new StringBuilder(PasswordLength, MaxPasswordLength);
            for (var i = 0; i < PasswordLength; i++)
            {
                passwordStringBuilder.Append(RetrieveRandomCharacter(CharacterSets, CharacterSetsForRandom, CharacterSetsCount));
            }

            return passwordStringBuilder.ToString();
        }

        /// <summary>
        /// Generates a batch of passwords using the .Net Parallel.ForEach contruct.
        /// </summary>
        /// <returns>Collection of generated passwords as string.</returns>
        public List<string> GenerateParallel(int batchSize)
        {
            // Call my ParallelFor
            List<StringWrapper> passwordStrings =
                DoParallelForGeneratePassword.DoParallelForAction(this,
                                                                  PasswordLength,
                                                                  batchSize);
            var passwords = new List<string>(batchSize);
            for (var i = 0; i < batchSize; i++)
            {
                passwords.Add(passwordStrings[i].Password);
            }

            return passwords;
        }

        /// <summary>
        /// Retrieves a single character, randomly picked, from one of the <see cref="CharacterSetsEnum"/>.
        /// </summary>
        /// <param name="characterSets"> </param>
        /// <param name="characterSetsForRandom"> </param>
        /// <param name="characterSetCount"> </param>
        /// <returns>Random picked character from some available character set.</returns>
        internal static char RetrieveRandomCharacter(Dictionary<CharacterSetsEnum, char[]> characterSets, List<CharacterSetsEnum> characterSetsForRandom, int characterSetCount)
        {
            // 1st- choose a location within the available sets to work with
            var characterSetToUse = RandomGenerator.Next(1, characterSetsForRandom.Count) - 1;
            // 2nd - choose a set enum
            var characterSetToUseEnum = characterSetsForRandom[characterSetToUse];

            // 3rd - Get random character from proper set.
            return RetrieveRandomCharacterFromSet(characterSets, characterSetToUseEnum);
        }

        /// <summary>
        /// Retrieves a single character, randomly picked, for the requested <paramref name="setToUse"/>.
        /// </summary>
        /// <param name="characterSets"> </param>
        /// <param name="setToUse"><see cref="CharacterSetsEnum"/> to use.</param>
        /// <returns>Random picked char.</returns>
        private static char RetrieveRandomCharacterFromSet(Dictionary<CharacterSetsEnum, char[]> characterSets, CharacterSetsEnum setToUse)
        {
            char[] charSet;
            if (characterSets.TryGetValue(setToUse, out charSet))
            {
                var charPosition = RandomGenerator.Next(1, charSet.Length) - 1;
                return charSet[charPosition];
            }

            var errorMessage =
                string.Format(
                    "Internal error: RetrieveRandomCharacterFromSet unable to locate the given enumeration {0}",
                    setToUse);
            throw new InvalidOperationException(errorMessage);
        }

        /// <summary>
        /// Updates the available character set enumeration array, by checking
        /// the current dictionary keys.
        /// </summary>
        private void UpdateAvailableCharacterSets()
        {
            CharacterSetsAvailable = new CharacterSetsEnum[CharacterSets.Count];
            CharacterSets.Keys.CopyTo(CharacterSetsAvailable, 0);
            CharacterSetsCount = CharacterSets.Count;

            // Note: To add more randoms, going to add the _characterSetsAvailable array 
            //       to a new array * 3.
            var count = CharacterSetsCount * 3;
            CharacterSetsForRandom = new List<CharacterSetsEnum>(count);
            for (var i = 0; i < 3; i++)
            {
                foreach (var characterSetsEnum in CharacterSetsAvailable)
                {
                    CharacterSetsForRandom.Add(characterSetsEnum);
                }
            }
        }
    }
}
