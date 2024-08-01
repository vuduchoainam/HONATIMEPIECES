using SharedClassLibrary.DTOs;
using static SharedClassLibrary.DTOs.ServiceResponse;

namespace SharedClassLibrary.Contracts
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAccount(UserDTO userDTO);
        Task<LoginResponse> LoginAccount(LoginDTO loginDTO);
    }
}
