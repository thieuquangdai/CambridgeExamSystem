namespace CambridgeExamSystem.Application.Interfaces;

public interface IPasswordService
{
    string GenerateTemporaryPassword(int length = 12);
}
