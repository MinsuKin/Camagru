using System.ComponentModel.DataAnnotations;

public class RegisterModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    // [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    // [DataType(DataType.Password)]
    // public string ConfirmPassword { get; set; } = "";
}