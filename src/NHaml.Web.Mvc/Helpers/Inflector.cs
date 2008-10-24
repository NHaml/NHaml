using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Web;

namespace NHaml.Web.Mvc
{
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public static class Inflector
  {
    #region Default Rules

    [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
    static Inflector()
    {
      AddPlural("$", "s");
      AddPlural("s$", "s");
      AddPlural("(ax|test)is$", "$1es");
      AddPlural("(octop|vir)us$", "$1i");
      AddPlural("(alias|status)$", "$1es");
      AddPlural("(bu)s$", "$1ses");
      AddPlural("(buffal|tomat)o$", "$1oes");
      AddPlural("([ti])um$", "$1a");
      AddPlural("sis$", "ses");
      AddPlural("(?:([^f])fe|([lr])f)$", "$1$2ves");
      AddPlural("(hive)$", "$1s");
      AddPlural("([^aeiouy]|qu)y$", "$1ies");
      AddPlural("(x|ch|ss|sh)$", "$1es");
      AddPlural("(matr|vert|ind)ix|ex$", "$1ices");
      AddPlural("([m|l])ouse$", "$1ice");
      AddPlural("^(ox)$", "$1en");
      AddPlural("(quiz)$", "$1zes");

      AddSingular("s$", "");
      AddSingular("(n)ews$", "$1ews");
      AddSingular("([ti])a$", "$1um");
      AddSingular("((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$", "$1$2sis");
      AddSingular("(^analy)ses$", "$1sis");
      AddSingular("([^f])ves$", "$1fe");
      AddSingular("(hive)s$", "$1");
      AddSingular("(tive)s$", "$1");
      AddSingular("([lr])ves$", "$1f");
      AddSingular("([^aeiouy]|qu)ies$", "$1y");
      AddSingular("(s)eries$", "$1eries");
      AddSingular("(m)ovies$", "$1ovie");
      AddSingular("(x|ch|ss|sh)es$", "$1");
      AddSingular("([m|l])ice$", "$1ouse");
      AddSingular("(bus)es$", "$1");
      AddSingular("(o)es$", "$1");
      AddSingular("(shoe)s$", "$1");
      AddSingular("(cris|ax|test)es$", "$1is");
      AddSingular("(octop|vir)i$", "$1us");
      AddSingular("(alias|status)es$", "$1");
      AddSingular("^(ox)en", "$1");
      AddSingular("(vert|ind)ices$", "$1ex");
      AddSingular("(matr)ices$", "$1ix");
      AddSingular("(quiz)zes$", "$1");

      AddIrregular("person", "people");
      AddIrregular("man", "men");
      AddIrregular("child", "children");
      AddIrregular("sex", "sexes");
      AddIrregular("move", "moves");

      AddUncountable("equipment");
      AddUncountable("information");
      AddUncountable("rice");
      AddUncountable("money");
      AddUncountable("species");
      AddUncountable("series");
      AddUncountable("fish");
      AddUncountable("sheep");
    }

    #endregion

    private class Rule
    {
      private readonly Regex _regex;
      private readonly string _replacement;

      public Rule(string pattern, string replacement)
      {
        _regex = new Regex(pattern, RegexOptions.IgnoreCase);
        _replacement = replacement;
      }

      public string Apply(string word)
      {
        if (!_regex.IsMatch(word))
        {
          return null;
        }

        return _regex.Replace(word, _replacement);
      }
    }

    private static void AddIrregular(string singular, string plural)
    {
      AddPlural("(" + singular[0] + ")" + singular.Substring(1) + "$", "$1" + plural.Substring(1));
      AddSingular("(" + plural[0] + ")" + plural.Substring(1) + "$", "$1" + singular.Substring(1));
    }

    private static void AddUncountable(string word)
    {
      _uncountables.Add(word.ToLowerInvariant());
    }

    private static void AddPlural(string rule, string replacement)
    {
      _plurals.Add(new Rule(rule, replacement));
    }

    private static void AddSingular(string rule, string replacement)
    {
      _singulars.Add(new Rule(rule, replacement));
    }

    private static readonly List<Rule> _plurals = new List<Rule>();
    private static readonly List<Rule> _singulars = new List<Rule>();
    private static readonly List<string> _uncountables = new List<string>();

    public static string Pluralize(string word)
    {
      return ApplyRules(_plurals, word);
    }

    public static string Singularize(string word)
    {
      return ApplyRules(_singulars, word);
    }

    private static string ApplyRules(IList<Rule> rules, string word)
    {
      var result = word;

      if (!_uncountables.Contains(word.ToLowerInvariant()))
      {
        for (var i = rules.Count - 1; i >= 0; i--)
        {
          if ((result = rules[i].Apply(word)) != null)
          {
            break;
          }
        }
      }

      return result;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Titleize")]
    public static string Titleize(string word)
    {
      return Regex.Replace(Humanize(Underscore(word)), @"\b([a-z])",
        match => match.Captures[0].Value.ToUpperInvariant());
    }

    public static string Humanize(string lowercaseAndUnderscoredWord)
    {
      return Capitalize(Regex.Replace(lowercaseAndUnderscoredWord, @"_", " "));
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pascalize")]
    public static string Pascalize(string lowercaseAndUnderscoredWord)
    {
      return Regex.Replace(lowercaseAndUnderscoredWord, "(?:^|_)(.)",
        match => match.Groups[1].Value.ToUpperInvariant());
    }

    public static string Underscore(string pascalCasedWord)
    {
      return Regex.Replace(
        Regex.Replace(
          Regex.Replace(pascalCasedWord, @"([A-Z]+)([A-Z][a-z])", "$1_$2"), @"([a-z\d])([A-Z])",
          "$1_$2"), @"[-\s]", "_").ToLowerInvariant();
    }

    public static string Capitalize(string word)
    {
      return word.Substring(0, 1).ToUpperInvariant() + word.Substring(1).ToLowerInvariant();
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Uncapitalize")]
    public static string Uncapitalize(string word)
    {
      return word.Substring(0, 1).ToLowerInvariant() + word.Substring(1);
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ordinalize")]
    public static string Ordinalize(string number)
    {
      var n = int.Parse(number, CultureInfo.InvariantCulture);
      var nMod100 = n%100;

      if (nMod100 >= 11 && nMod100 <= 13)
      {
        return number + "th";
      }

      switch (n%10)
      {
        case 1:
          return number + "st";
        case 2:
          return number + "nd";
        case 3:
          return number + "rd";
        default:
          return number + "th";
      }
    }
  }
}