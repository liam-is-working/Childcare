namespace Childcare.Authorization{
    public class Constants
    {
        public static readonly string CreateOperationName = "Create";
        public static readonly string ReadOperationName = "Read";
        public static readonly string UpdateOperationName = "Update";
        public static readonly string DeleteOperationName = "Delete";
        public static readonly string ApproveOperationName = "Approve";
        public static readonly string RejectOperationName = "Reject";

        public static readonly string BlogAdministratorsRole = "BlogAdministrators";
        public static readonly string BlogManagersRole = "BlogManagers";

        public static readonly string PatientAdministratorsRole = "PatientAdministrators";
        public static readonly string PatientManagersRole = "PatientManagers";
    }
}