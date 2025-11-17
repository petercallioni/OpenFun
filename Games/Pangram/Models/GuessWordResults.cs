namespace Pangram.Models
{
    public enum GuessWordResults
    {
        NONE,
        VALID,
        VALID_PANGRAM,
        INVALID,
        ALREADY_GUESSED,
        FORBIDDEN_CHARACTERS,
        DOES_NOT_CONTAIN_MAIN_LETTER
    }
}
