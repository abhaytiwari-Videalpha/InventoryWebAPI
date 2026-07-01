namespace IdentityService.API.Exceptions;
public class UnauthorizedException : Exception
{
    public UnauthorizedException(String message)
        :base(message)
    {
        
    }
}