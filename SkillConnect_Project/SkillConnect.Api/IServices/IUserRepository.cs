public interface IUserRepository
{
    Task<string> CreateUserAsync (CreateUserRequestDto request);

    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task<CreateUserProfileResponseDto> CreateUserProfileAsync(CreateUserProfileRequestDto request);
}