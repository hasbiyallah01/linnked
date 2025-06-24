namespace Linnked.Core.Domain.Enum
{
    public enum Gender
    {
        Male = 1,
        Female
    }

    public enum PaymentStatus
    {
        Pending,
        Success,
        Failed
    }

    public enum PaymentType
    {
        Subscription,
        Session
    }
}
