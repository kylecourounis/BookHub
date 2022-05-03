namespace BookHub.Server.Logic.Enums
{
    internal enum OrderState
    {
        Failed,     // For when the payment fails to process
        Processing, // Checking with payment processing service to see if the payment went through
        Processed,  // Payment processed
        Completed   // When the item exchange has actually taken place 
    }
}
