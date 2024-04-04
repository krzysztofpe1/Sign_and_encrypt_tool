namespace SignAndEncyptTool
{
    internal class SAEException : Exception
    {
        public SAEException(string? message) : base(message) { }
        public SAEException(string? message, Exception innerException) : base(message, innerException) { }
    }
}
