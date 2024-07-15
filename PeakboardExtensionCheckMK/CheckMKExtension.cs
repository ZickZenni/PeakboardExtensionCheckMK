using Peakboard.ExtensionKit;

namespace PeakboardExtensionCheckMK
{
    public class CheckMKExtension : ExtensionBase
    {
        protected override ExtensionDefinition GetDefinitionOverride()
        {
            return new ExtensionDefinition{
                ID = "CheckMK",
                Name = "CheckMK Extension",
                Description = "This is an Extension for accessing CheckMK data",
                Version = "1.0",
                Author = "Peakboard Team",
                Company = "Peakboard GmbH",
                Copyright = "Copyright © 2024",
            };
        }
    }
}
