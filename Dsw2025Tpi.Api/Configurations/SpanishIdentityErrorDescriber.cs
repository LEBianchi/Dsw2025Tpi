using Microsoft.AspNetCore.Identity;

public class SpanishIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DefaultError()
    {
        return new IdentityError { Code = nameof(DefaultError), Description = "Ha ocurrido un error inesperado." };
    }

    public override IdentityError ConcurrencyFailure()
    {
        return new IdentityError { Code = nameof(ConcurrencyFailure), Description = "Fallo de concurrencia optimista, el objeto ha sido modificado." };
    }

    public override IdentityError PasswordMismatch()
    {
        return new IdentityError { Code = nameof(PasswordMismatch), Description = "Contraseña incorrecta." };
    }

    public override IdentityError InvalidToken()
    {
        return new IdentityError { Code = nameof(InvalidToken), Description = "Token inválido." };
    }

    public override IdentityError LoginAlreadyAssociated()
    {
        return new IdentityError { Code = nameof(LoginAlreadyAssociated), Description = "Un usuario con ese login ya existe." };
    }

    public override IdentityError InvalidUserName(string userName)
    {
        return new IdentityError { Code = nameof(InvalidUserName), Description = $"El usuario '{userName}' no es válido, solo puede contener letras o dígitos." };
    }

    public override IdentityError InvalidEmail(string email)
    {
        return new IdentityError { Code = nameof(InvalidEmail), Description = $"El email '{email}' no es válido." };
    }

    public override IdentityError DuplicateUserName(string userName)
    {
        return new IdentityError { Code = nameof(DuplicateUserName), Description = $"El usuario '{userName}' ya está en uso." };
    }

    public override IdentityError DuplicateEmail(string email)
    {
        return new IdentityError { Code = nameof(DuplicateEmail), Description = $"El email '{email}' ya está en uso." };
    }

    public override IdentityError InvalidRoleName(string role)
    {
        return new IdentityError { Code = nameof(InvalidRoleName), Description = $"El rol '{role}' no es válido." };
    }

    public override IdentityError DuplicateRoleName(string role)
    {
        return new IdentityError { Code = nameof(DuplicateRoleName), Description = $"El rol '{role}' ya existe." };
    }

    public override IdentityError UserAlreadyHasPassword()
    {
        return new IdentityError { Code = nameof(UserAlreadyHasPassword), Description = "El usuario ya tiene contraseña." };
    }

    public override IdentityError UserLockoutNotEnabled()
    {
        return new IdentityError { Code = nameof(UserLockoutNotEnabled), Description = "El bloqueo no está habilitado para este usuario." };
    }

    public override IdentityError UserAlreadyInRole(string role)
    {
        return new IdentityError { Code = nameof(UserAlreadyInRole), Description = $"El usuario ya tiene el rol '{role}'." };
    }

    public override IdentityError UserNotInRole(string role)
    {
        return new IdentityError { Code = nameof(UserNotInRole), Description = $"El usuario no tiene el rol '{role}'." };
    }

    public override IdentityError PasswordTooShort(int length)
    {
        return new IdentityError { Code = nameof(PasswordTooShort), Description = $"La contraseña debe tener al menos {length} caracteres." };
    }

    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        return new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "La contraseña debe tener al menos un carácter no alfanumérico." };
    }

    public override IdentityError PasswordRequiresDigit()
    {
        return new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "La contraseña debe tener al menos un dígito ('0'-'9')." };
    }

    public override IdentityError PasswordRequiresLower()
    {
        return new IdentityError { Code = nameof(PasswordRequiresLower), Description = "La contraseña debe tener al menos una minúscula ('a'-'z')." };
    }

    public override IdentityError PasswordRequiresUpper()
    {
        return new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "La contraseña debe tener al menos una mayúscula ('A'-'Z')." };
    }
}