namespace doof.Helpers;

public class SupportedAppLanguages
{
    public required Dictionary<string, Language> Dict { get; init; }
}

public class Language
{
    public required string Icc { get; set; }

    public required string Culture { get; set; }
}