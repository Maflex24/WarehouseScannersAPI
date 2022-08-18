namespace WarehouseScannersAPI.Dtos
{
    public class EmployeeChangePasswordDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
