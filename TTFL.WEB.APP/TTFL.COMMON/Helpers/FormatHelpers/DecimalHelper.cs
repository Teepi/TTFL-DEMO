namespace TTFL.COMMON.Helpers.FormatHelpers
{
    public class DecimalHelper
    {
        /// <summary>
        /// Convert decimal to decimal with selected number of digit
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digitNumber"></param>
        /// <returns></returns>
        public static decimal ConvertToDecimalwithDigits(decimal value, int digitNumber)
        {
            return decimal.Round(value, digitNumber, MidpointRounding.AwayFromZero);
        }
    }
}
