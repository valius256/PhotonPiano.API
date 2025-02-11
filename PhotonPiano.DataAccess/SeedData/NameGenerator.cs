namespace PhotonPiano.DataAccess.SeedData;

public static class NameGenerator
{
    private static readonly string[] FirstNames =
    {
        "Anh", "Bảo", "Châu", "Dũng", "Đức", "Hà", "Hải", "Hạnh", "Hiền", "Hoàng",
        "Hùng", "Hương", "Khánh", "Lan", "Linh", "Mai", "Minh", "Nam", "Nga", "Nhung",
        "Phương", "Quang", "Quỳnh", "Tâm", "Thảo", "Thủy", "Tiến", "Trang", "Trinh", "Trung",
        "Tú", "Tuấn", "Vân", "Việt", "Xuân", "Yến"
    };

    private static readonly string[] LastNames =
    {
        "Nguyễn", "Trần", "Lê", "Phạm", "Hoàng", "Huỳnh", "Phan", "Vũ", "Võ", "Đặng",
        "Bùi", "Đỗ", "Hồ", "Ngô", "Dương", "Lý"
    };

    public static string GenerateFullName()
    {
        var random = new Random();
        var firstName = FirstNames[random.Next(FirstNames.Length)];
        var lastName = LastNames[random.Next(LastNames.Length)];
        return $"{lastName} {firstName}";
    }
}