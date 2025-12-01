using System.Net.Mail;
using System.Text.RegularExpressions;
using PhoneNumbers;


/*
 * Basic and important validation for email and phone given by users.
 * This class creates the basic checks every user's info needs.
 * For example: Checks if an email is in the right form of "name@gmail.com, etc..."
 *              Checks if a number has enough digits, etc..
 *              Both use known libraries for mail and phone-nubers.
 */
namespace FindMyCoffee.Services.Security
{
    public class UserInfoChecker
    {
        //Checks the shape of the email given (Offline). If the shape is wrong so will not proceed to external (Online) service -> and save time.
        public static bool IsEmailValid(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                //TODO: User an external service to check if the mail is real and belongs to the user.
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        //Checks the shape of the phone-number given (Offline). If the shape is wrong so will not proceed to external (Online) service -> and save time.
        public static bool IsPhoneNumberValid(string phoneNumber)
        {
            PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();
            try
            {
                //Needs to enter region: a phone like 0501234567 needs to be written as +972501234567
                PhoneNumber parsedPhoneNumber = phoneNumberUtil.Parse(phoneNumber, null);
                bool phoneValid = phoneNumberUtil.IsValidNumber(parsedPhoneNumber);
                //TODO: User an external service to check if the phone number is real and belongs to the user.
                return phoneValid;
            }
            catch (NumberParseException)
            {
                return false;
            }
        }
    }
}
