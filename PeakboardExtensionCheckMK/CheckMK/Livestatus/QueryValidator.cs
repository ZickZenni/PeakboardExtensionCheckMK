using CheckMK.Livestatus.Validators;

namespace CheckMK.Livestatus
{
    public interface IValidator
    {
        void Validate(QueryValidator validator, string query);
    }

    public class QueryValidator
    {
        private readonly IValidator[] _validators;

        public bool AdvancedLQL { get; private set; }

        public QueryValidator(bool advanced) {
            AdvancedLQL = advanced;

            _validators = new IValidator[]
            {
                new KeywordValidator()
            };
        }

        public bool Validate(string query)
        {
            if (query.EndsWith(";"))
                query = query.Remove(query.Length - 1, 1);

            foreach (var validator in _validators)
            {
                validator.Validate(this, query);
            }
            return true;
        }
    }
}
