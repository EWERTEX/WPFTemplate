using System.Text.RegularExpressions;

namespace WPFTemplate.Helpers
{
    public static partial class ValidationHelper
    {
        public static bool IsValidEmail(string email)
        {
	        return !string.IsNullOrWhiteSpace(email) && EmailRegex().IsMatch(email);
        }
        
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;

            var hasMinimum8Chars = password.Length >= 8;
            var hasUpperChar = password.Any(char.IsUpper);
            var hasLowerChar = password.Any(char.IsLower);
            var hasDigit = password.Any(char.IsDigit);
            var hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasMinimum8Chars && hasUpperChar && hasLowerChar && hasDigit && hasSpecialChar;
        }
        
        public static bool IsValidSnils(string snils)
        {
            if (string.IsNullOrWhiteSpace(snils)) return false;
            
            var cleanSnils = new string(snils.Where(char.IsDigit).ToArray());
            if (cleanSnils.Length != 11) return false;
            
            var sum = 0;
            for (var i = 0; i < 9; i++)
            {
                sum += int.Parse(cleanSnils[i].ToString()) * (9 - i);
            }

            int controlNumber;
            
            switch (sum)
            {
	            case < 100:
		            controlNumber = sum;
		            break;
	            case 100:
	            case 101:
		            controlNumber = 0;
		            break;
	            case > 101:
	            {
		            var remainder = sum % 101;
		            controlNumber = (remainder == 100) ? 0 : remainder;
		            break;
	            }
            }
            
            var actualControl = int.Parse(cleanSnils.Substring(9, 2));

            return controlNumber == actualControl;
        }

        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        private static partial Regex EmailRegex();
    }
}