/*
 * Copyright (C) 2007, 2008 Chris Meadowcroft <crmeadowcroft@gmail.com>
 *
 * This file is part of CMPlayer, a free video player.
 * See http://sourceforge.net/projects/crmplayer for updates.
 *
 * CMPlayer is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * CMPlayer is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System.Collections.Frozen;

namespace DvdNavigatorCrm;

public class DvdLanguageCodes
{
    static readonly FrozenDictionary<string, string> languageCode;
    static readonly FrozenDictionary<string, string> iso639Code;

    static DvdLanguageCodes()
    {
        var iso639 = new Dictionary<string, string>();
        iso639["en"] = "eng";
        iso639["fr"] = "fre";
        iso639["de"] = "ger";
        iso639["ja"] = "jpn";
        iso639["zh"] = "chi";
        iso639["it"] = "ita";
        iso639["mn"] = "mon";
        iso639["nl"] = "dut";
        iso639["fa"] = "per";
        iso639["pl"] = "pol";
        iso639["he"] = "heb";
        iso639["ru"] = "rus";
        iso639["sv"] = "swe";
        iso639["hi"] = "hin";

        var lang = new Dictionary<string, string>();
        lang["ab"] = "Abkhazian";
        lang["aa"] = "Afar";
        lang["af"] = "Afrikaans";
        lang["sq"] = "Albanian";
        lang["am"] = "Amharic, Ameharic";
        lang["ar"] = "Arabic";
        lang["hy"] = "Armenian";
        lang["as"] = "Assamese";
        lang["ay"] = "Aymara";
        lang["az"] = "Azerbaijani";
        lang["ba"] = "Bashkir";
        lang["eu"] = "Basque";
        lang["bn"] = "Bengali, Bangla";
        lang["dz"] = "Bhutani";
        lang["bh"] = "Bihari";
        lang["bi"] = "Bislama";
        lang["br"] = "Breton";
        lang["bg"] = "Bulgarian";
        lang["my"] = "Burmese";
        lang["be"] = "Byelorussian";
        lang["km"] = "Cambodian";
        lang["ca"] = "Catalan";
        lang["zh"] = "Chinese";
        lang["co"] = "Corsican";
        lang["hr"] = "Hrvatski (Croatian)";
        lang["cs"] = "Czech (Ceske)";
        lang["da"] = "Dansk (Danish)";
        lang["nl"] = "Dutch (Nederlands)";
        lang["en"] = "English";
        lang["eo"] = "Esperanto";
        lang["et"] = "Estonian";
        lang["fo"] = "Faroese";
        lang["fj"] = "Fiji";
        lang["fi"] = "Finnish";
        lang["fr"] = "French";
        lang["fy"] = "Frisian";
        lang["gl"] = "Galician";
        lang["ka"] = "Georgian";
        lang["de"] = "Deutsch (German)";
        lang["el"] = "Greek";
        lang["kl"] = "Greenlandic";
        lang["gn"] = "Guarani";
        lang["gu"] = "Gujarati";
        lang["ha"] = "Hausa";
        lang["iw"] = "Hebrew";
        lang["hi"] = "Hindi";
        lang["hu"] = "Hungarian";
        lang["is"] = "Islenka (Icelandic)";
        lang["in"] = "Indonesian";
        lang["ia"] = "Interlingua";
        lang["ie"] = "Interlingue";
        lang["ik"] = "Inupiak";
        lang["ga"] = "Irish";
        lang["it"] = "Italian";
        lang["ja"] = "Japanese";
        lang["jw"] = "Javanese";
        lang["kn"] = "Kannada";
        lang["ks"] = "Kashmiri";
        lang["kk"] = "Kazakh";
        lang["rw"] = "Kinyarwanda";
        lang["ky"] = "Kirghiz";
        lang["rn"] = "Kirundi";
        lang["ko"] = "Korean";
        lang["ku"] = "Kurdish";
        lang["lo"] = "Laothian";
        lang["la"] = "Latin";
        lang["lv"] = "Latvian, Lettish";
        lang["ln"] = "Lingala";
        lang["lt"] = "Lithuanian";
        lang["mk"] = "Macedonian";
        lang["mg"] = "Malagasy";
        lang["ms"] = "Malay";
        lang["ml"] = "Malayalam";
        lang["mt"] = "Maltese";
        lang["mi"] = "Maori";
        lang["mr"] = "Marathi";
        lang["mo"] = "Moldavian";
        lang["mn"] = "Mongolian";
        lang["na"] = "Nauru";
        lang["ne"] = "Nepali";
        lang["no"] = "Norwegian (Norsk)";
        lang["oc"] = "Occitan";
        lang["or"] = "Oriya";
        lang["om"] = "Afan (Oromo)";
        lang["pa"] = "Panjabi";
        lang["ps"] = "Pashto, Pushto";
        lang["fa"] = "Persian";
        lang["pl"] = "Polish";
        lang["pt"] = "Portuguese";
        lang["qu"] = "Quechua";
        lang["rm"] = "Rhaeto-Romance";
        lang["ro"] = "Romanian";
        lang["ru"] = "Russian";
        lang["sm"] = "Samoan";
        lang["sg"] = "Sangho";
        lang["sa"] = "Sanskrit";
        lang["gd"] = "Scots Gaelic";
        lang["sh"] = "Serbo-Crotain";
        lang["st"] = "Sesotho";
        lang["sr"] = "Serbian";
        lang["tn"] = "Setswana";
        lang["sn"] = "Shona";
        lang["sd"] = "Sindhi";
        lang["si"] = "Singhalese";
        lang["ss"] = "Siswati";
        lang["sk"] = "Slovak";
        lang["sl"] = "Slovenian";
        lang["so"] = "Somali";
        lang["es"] = "Spanish (Espanol)";
        lang["su"] = "Sundanese";
        lang["sw"] = "Swahili";
        lang["sv"] = "Svenska (Swedish)";
        lang["tl"] = "Tagalog";
        lang["tg"] = "Tajik";
        lang["tt"] = "Tatar";
        lang["ta"] = "Tamil";
        lang["te"] = "Telugu";
        lang["th"] = "Thai";
        lang["bo"] = "Tibetian";
        lang["ti"] = "Tigrinya";
        lang["to"] = "Tonga";
        lang["ts"] = "Tsonga";
        lang["tr"] = "Turkish";
        lang["tk"] = "Turkmen";
        lang["tw"] = "Twi";
        lang["uk"] = "Ukranian";
        lang["ur"] = "Urdu";
        lang["uz"] = "Uzbek";
        lang["vi"] = "Vietnamese";
        lang["vo"] = "Volapuk";
        lang["cy"] = "Welsh";
        lang["wo"] = "Wolof";
        lang["ji"] = "Yiddish";
        lang["yo"] = "Yoruba";
        lang["xh"] = "Xhosa";
        lang["zu"] = "Zulu";

        languageCode = lang.ToFrozenDictionary();
        iso639Code = iso639.ToFrozenDictionary();
    }

    public static string GetLanguageText(string code)
    {
        if (string.IsNullOrEmpty(code) || (code[0] == '\0'))
        {
            return string.Empty;
        }
        string language;
        if (!languageCode.TryGetValue(code.ToLower(), out language))
        {
            language = code;
        }
        return language;
    }

    public static string GetLanguage639Code(string code)
    {
        if (code == null)
        {
            return string.Empty;
        }
        string language;
        if (!iso639Code.TryGetValue(code.ToLower(), out language))
        {
            language = code + " ";
        }
        return language;
    }
}

