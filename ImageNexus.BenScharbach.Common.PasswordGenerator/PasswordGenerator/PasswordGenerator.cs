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
    public class PasswordGenerator
    {
        // Enum of character sets
        private enum CharacterSetsEnum
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
        private readonly Dictionary<CharacterSetsEnum, char[]> _characterSets = new Dictionary<CharacterSetsEnum,char[]>(); 

        // Random Generator
        private readonly Random _randomGenerator = new Random();

        // Password length
        private readonly int _passwordLength;
        private const int MaxPasswordLength = 99;
        // Character sets count.
        private int _characterSetsCount;
        private CharacterSetsEnum[] _characterSetsAvailable;
        private bool _useAlphaLowercaseSet;
        private bool _useAlphaUppercaseSet;
        private bool _useNumericSet;
        private bool _useSymbolSet;

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
                    if (!_characterSets.ContainsKey(CharacterSetsEnum.AlphaLowerCase))
                        _characterSets.Add(CharacterSetsEnum.AlphaLowerCase, _alphaLowCharacterSet);
                }
                else
                {
                    _characterSets.Remove(CharacterSetsEnum.AlphaLowerCase);
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
                    if (!_characterSets.ContainsKey(CharacterSetsEnum.AlphaUpperCase))
                        _characterSets.Add(CharacterSetsEnum.AlphaUpperCase, _alphaUpCharacterSet);
                }
                else
                {
                    _characterSets.Remove(CharacterSetsEnum.AlphaUpperCase);
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
                    if (!_characterSets.ContainsKey(CharacterSetsEnum.Numeric))
                        _characterSets.Add(CharacterSetsEnum.Numeric, _numbericCharacterSet);
                }
                else
                {
                    _characterSets.Remove(CharacterSetsEnum.Numeric);
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
                    if (!_characterSets.ContainsKey(CharacterSetsEnum.Symbols))
                        _characterSets.Add(CharacterSetsEnum.Symbols, _symbolCharacterSet);
                }
                else
                {
                    _characterSets.Remove(CharacterSetsEnum.Symbols);
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

            _passwordLength = passwordLength;

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
            var passwordStringBuilder = new StringBuilder(_passwordLength, MaxPasswordLength);
            for (var i = 0; i < _passwordLength; i++)
            {
                passwordStringBuilder.Append(RetrieveRandomCharacter());
            }

            // Parallel.ForEach

            return passwordStringBuilder.ToString();
        }

        /// <summary>
        /// Retrieves a single character, randomly picked, from one of the <see cref="CharacterSetsEnum"/>.
        /// </summary>
        /// <returns>Random picked character from some available character set.</returns>
        private char RetrieveRandomCharacter()
        {
            // Note: To add more randoms, going to add the _characterSetsAvailable array 
            //       to a new array * 3.
            var count = _characterSetsCount*3;
            var characterSetsForRandom = new List<CharacterSetsEnum>(count);
            for (var i = 0; i < 3; i++)
            {
                foreach (var characterSetsEnum in _characterSetsAvailable)
                {
                    characterSetsForRandom.Add(characterSetsEnum);
                }
            }

            // 1st- choose a location within the available sets to work with
            var characterSetToUse = _randomGenerator.Next(1, count) - 1;
            // 2nd - choose a set enum
            var characterSetToUseEnum = characterSetsForRandom[characterSetToUse];

            // 3rd - Get random character from proper set.
            return RetrieveRandomCharacterFromSet(characterSetToUseEnum);
        }

        /// <summary>
        /// Retrieves a single character, randomly picked, for the requested <paramref name="setToUse"/>.
        /// </summary>
        /// <param name="setToUse"><see cref="CharacterSetsEnum"/> to use.</param>
        /// <returns>Random picked char.</returns>
        private char RetrieveRandomCharacterFromSet(CharacterSetsEnum setToUse)
        {
            char[] charSet;
            if (_characterSets.TryGetValue(setToUse, out charSet))
            {
                var charPosition = _randomGenerator.Next(1, charSet.Length) - 1;
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
            _characterSetsAvailable = new CharacterSetsEnum[_characterSets.Count];
            _characterSets.Keys.CopyTo(_characterSetsAvailable, 0);
            _characterSetsCount = _characterSets.Count;
        }
    }
}
