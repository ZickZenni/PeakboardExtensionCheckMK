namespace CheckMK.Livestatus.Validators
{
    internal class SimpleQueryValidator : IValidator
    {
        public void Validate(QueryValidator validator, string query)
        {
            if (query.Length == 0)
                throw new LivestatusInvalidQueryException(0, 0, "Query cannot be the length 0.");
        }
    }
}
