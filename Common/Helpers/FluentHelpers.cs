namespace Common.Helpers
{
    public class FluentHelpers
    {
        public static string PropertyName { get; }
        public static string PropertyValue { get; }
        public static string ComparisonValue { get; }
        public static string MinLength { get; }
        public static string MaxLength { get; }
        public static string TotalLength { get; }

        static FluentHelpers()
        {
            PropertyName = "{PropertyName}";
            PropertyValue = "{PropertyValue}";
            ComparisonValue = "{ComparisonValue}";
            MinLength = "{MinLength}";
            MaxLength = "{MaxLength}";
            TotalLength = "{TotalLength}";
        }
    }
}
