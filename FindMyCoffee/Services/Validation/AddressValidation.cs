namespace FindMyCoffee.Services.Validation
{

    /*
     * Applies a basic validation for address given.
     * Gives the first layer of validation before the API (Geopify at this moment) takes action.
     */
    public class AddressValidation
    {
        public static string? ValidateAddressInput(string street, string city, string country, string? state)
        {
            if (string.IsNullOrWhiteSpace(street)) return "Street is required.";
            if (string.IsNullOrWhiteSpace(city)) return "City is required.";
            if (string.IsNullOrWhiteSpace(country)) return "Country is required.";

            street = street.Trim();
            city = city.Trim();
            country = country.Trim();
            state = state?.Trim();

            if (street.Length < 3) return "Street must be at least 3 characters.";
            if (city.Length < 2) return "City must be at least 2 characters.";
            if (country.Length < 2) return "Country must be at least 2 characters.";

            if (!street.Any(char.IsLetter)) return "Street must contain letters.";
            if (!city.Any(char.IsLetter)) return "City must contain letters.";
            if (!country.Any(char.IsLetter)) return "Country must contain letters.";

            if (!string.IsNullOrWhiteSpace(state))
            {
                if (state.Length < 2) return "State must be at least 2 characters.";
                if (!state.Any(char.IsLetter)) return "State must contain letters.";
            }

            return null;
        }

    }
}
