namespace CambridgeExamSystem.Domain.Common;

public static class RoleNames
{
    public const string Student = "Student";
    public const string Teacher = "Teacher";
    public const string Admin = "Admin";
    public const string AdminOrTeacher = Admin + "," + Teacher;

    public static readonly string[] All = [Student, Teacher, Admin];
}
