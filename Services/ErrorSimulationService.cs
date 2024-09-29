using FakeUserGenerator.Models;
using System;

namespace FakeUserGenerator.Services
{
    public class ErrorSimulationService : IFakeErrorSimulationService
    {
        public void SimulateError(User user, Random random, double errorCount)
        {
            int totalErrors = (int)Math.Floor(errorCount);
            bool hasFractionalError = random.NextDouble() < (errorCount - totalErrors);

            for (int i = 0; i < totalErrors; i++)
            {
                ApplyRandomError(user, random);
            }

            if (hasFractionalError)
            {
                ApplyRandomError(user, random);
            }
        }

        private void ApplyRandomError(User user, Random random)
        {
            int errorType = random.Next(3); 
            switch (errorType)
            {
                case 0: 
                    user.FullName = DeleteRandomCharacter(user.FullName, random);
                    break;
                case 1: 
                    user.FullName = AddRandomCharacter(user.FullName, random);
                    break;
                case 2: 
                    user.FullName = SwapAdjacentCharacters(user.FullName, random);
                    break;
            }
        }

        private string DeleteRandomCharacter(string original, Random random)
        {
            if (string.IsNullOrEmpty(original)) return original;
            int position = random.Next(original.Length);
            return original.Remove(position, 1);
        }

        private string AddRandomCharacter(string original, Random random)
        {
            if (string.IsNullOrEmpty(original)) return original;
            char randomChar = (char)random.Next('a', 'z'); 
            int position = random.Next(original.Length + 1);
            return original.Insert(position, randomChar.ToString());
        }

        private string SwapAdjacentCharacters(string original, Random random)
        {
            if (original.Length < 2) return original;
            int position = random.Next(original.Length - 1);
            char firstChar = original[position];
            char secondChar = original[position + 1];
            char[] chars = original.ToCharArray();
            chars[position] = secondChar;
            chars[position + 1] = firstChar;
            return new string(chars);
        }

    }
}
