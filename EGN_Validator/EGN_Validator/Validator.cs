using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGN_Validator
{
    public static class Validator
    {
        public static readonly int[] EGN_WEIGHTS = new int[] { 2, 4, 8, 5, 10, 9, 7, 3, 6 };

        public static readonly Dictionary<int, string> EGN_REGIONS = (new List<KeyValuePair<int, string>>
        {
            new KeyValuePair<int, string>(43, "Blagoevgrad"),
            new KeyValuePair<int, string>(93,"Burgas"),
            new KeyValuePair<int, string>(139,"Varna"),
            new KeyValuePair<int, string>(169,"Veliko Tarnovo"),
            new KeyValuePair<int, string>(183,"Vidin"),
            new KeyValuePair<int, string>(217, "Vraca"),
            new KeyValuePair<int, string>(233,"Gabrovo"),
            new KeyValuePair<int, string>(281,"Kardjali"),
            new KeyValuePair<int, string>(301,"Kustendil"),
            new KeyValuePair<int, string>(319,"Lovech"),
            new KeyValuePair<int, string>(341,"Montana"),
            new KeyValuePair<int, string>(377,"Pazardjik"),
            new KeyValuePair<int, string>(395,"Pernik"),
            new KeyValuePair<int, string>(435,"Pleven"),
            new KeyValuePair<int, string>(501,"Plovdiv"),
            new KeyValuePair<int, string>(527,"Razgrad"),
            new KeyValuePair<int, string>(555,"Ruse"),
            new KeyValuePair<int, string>(575,"Silistra"),
            new KeyValuePair<int, string>(601,"Sliven"),
            new KeyValuePair<int, string>(623,"Smolian"),
            new KeyValuePair<int, string>(721,"Sofia - city"),
            new KeyValuePair<int, string>(751,"Sofia - zone"),
            new KeyValuePair<int, string>(789,"Stara Zagora"),
            new KeyValuePair<int, string>(821,"Dobrich)"),
            new KeyValuePair<int, string>(843,"Turgovishte"),
            new KeyValuePair<int, string>(871,"Haskovo"),
            new KeyValuePair<int, string>(903,"Shumen"),
            new KeyValuePair<int, string>(925,"Yambol"),
            new KeyValuePair<int, string>(999,"Other/Unknown"),

        }).ToDictionary(k => k.Key, v => v.Value);

        public static bool IsValid(string egn)
        {
            long stupidTest;
            if (string.IsNullOrWhiteSpace(egn) || egn.Length != 10 || (egn = egn.Trim()).Length != 10 || !long.TryParse(egn, out stupidTest))
            {
                return false;
            }

            int year;
            int month;
            int day;

            int.TryParse(egn.Substring(0, 2), out year);
            int.TryParse(egn.Substring(2, 2), out month);
            int.TryParse(egn.Substring(4, 2), out day);

            if(month > 40 && month <= 52 && day > DateTime.DaysInMonth(year + 2000, month - 40))
            {
                return false;
            } else if (month > 20 && month <= 32 && day > DateTime.DaysInMonth(year + 1800, month - 20))
            {
                return false;
            }
            else if (month <= 12 && day >= DateTime.DaysInMonth(year + 1900, month))
            {
                return false;
            }

            int checksum;
            int egnsum = 0;

            int.TryParse(egn.Substring(9, 1), out checksum);

            for(int i = 0; i < 9; i++)
            {
                int value;
                int.TryParse(egn.Substring(i, 1), out value);

                egnsum += value * EGN_WEIGHTS[i];
            }

            int valid_checksum = egnsum % 11;

            if(valid_checksum == 10)
            {
                valid_checksum = 0;
            }

            if(valid_checksum == checksum)
            {
                return true;
            }

            return false;

        }

        public static string GetInfo(string egn)
        {
            if (!IsValid(egn))
            {
                return "Error: Please provide a valid EGN";
            }

            int year;
            int month;
            int day;

            int.TryParse(egn.Substring(0, 2), out year);
            int.TryParse(egn.Substring(2, 2), out month);
            int.TryParse(egn.Substring(4, 2), out day);

            if (month > 40 && month <= 52)
            {
                year += 2000;
                month -= 40;
            }
            else if (month > 20 && month <= 32)
            {
                year += 1800;
                month -= 20;
            }
            else if (month <= 12)
            {
                year += 1900;
            }

            DateTime date = new DateTime(year, month, day);

            int sex_flag;
            int.TryParse(egn.Substring(8, 1), out sex_flag);

            string sex = sex_flag % 2 == 1 ? "female" : "male";

            int low_region_num = 0;
            string region = null;
            int regionNumber;

            int.TryParse(egn.Substring(6, 3), out regionNumber);

            foreach (KeyValuePair<int, string> high_region_number in EGN_REGIONS.OrderBy(k => k.Key))
            {
                if(regionNumber >= low_region_num && regionNumber <= high_region_number.Key)
                {
                    region = high_region_number.Value;
                    break;
                }

                low_region_num = high_region_number.Key + 1;
            }

            if(sex_flag % 2 == 1)
            {
                regionNumber--;
            }

            int childNumber = (regionNumber - low_region_num) / 2 + 1;

            return string.Format("Information for EGN: {0} represents a {1}, born on {2} in {3}. There were {4} more children born earlier in that region.",
                egn,
                sex,
                date.ToShortDateString(),
                region,
                childNumber - 1
                );
        }
    }
}

///* Return array with EGN info */
//function egn_parse($egn)
//{
//    global $EGN_REGIONS;
//    global $MONTHS_BG;
//    if (!egn_valid($egn))
//        return false;
//$ret = array();
//$ret["year"] = substr($egn, 0, 2);
//$ret["month"] = substr($egn, 2, 2);
//$ret["day"] = substr($egn, 4, 2);
//    if ($ret["month"] > 40) {
//    $ret["month"] -= 40;
//    $ret["year"] += 2000;
//    } else
//if ($ret["month"] > 20) {
//    $ret["month"] -= 20;
//    $ret["year"] += 1800;
//    } else {
//    $ret["year"] += 1900;
//    }
//$ret["birthday_text"] = (int)$ret["day"]." ".$MONTHS_BG[(int)$ret["month"]]." ".$ret["year"]." г.";
//$region = substr($egn, 6, 3);
//$ret["region_num"] = $region;
//$ret["sex"] = substr($egn, 8, 1) % 2;
//$ret["sex_text"] = "жена";
//    if (!$ret["sex"])
//    $ret["sex_text"] = "мъж";
//$first_region_num = 0;
//    foreach ($EGN_REGIONS as $region_name => $last_region_num) {
//        if ($region >= $first_region_num && $region <= $last_region_num) {
//        $ret["region_text"] = $region_name;
//            break;
//        }
//    $first_region_num = $last_region_num + 1;
//    }
//    if (substr($egn, 8, 1) % 2 != 0)
//    $region--;
//$ret["birthnumber"] = ($region - $first_region_num) / 2 + 1;
//    return $ret;
//}

///* Return text with EGN info */
//function egn_info($egn)
//{
//    if (!egn_valid($egn))
//    {
//        return "<b>".htmlspecialchars($egn)."</b> невалиден ЕГН";
//    }
//$data = egn_parse($egn);
//$ret = "<b>".htmlspecialchars($egn)."</b> е ЕГН на <b>{$data['sex_text']}</b>, ";
//$ret.= "роден".($data["sex"] ? "а" : "")." на <b>{$data['birthday_text']}</b> в ";
//$ret.= "регион <b>{$data['region_text']}</b> ";
//    if ($data["birthnumber"] - 1) {
//    $ret.= "като преди ".($data["sex"] ? "нея" : "него")." ";
//        if ($data["birthnumber"] - 1 > 1) {
//        $ret.= "в този ден и регион са се родили <b>".($data["birthnumber"] - 1)."</b>";
//        $ret.= $data["sex"] ? " момичета" : " момчета";
//        } else {
//        $ret.= "в този ден и регион се е родило <b>1</b>";
//        $ret.= $data["sex"] ? " момиче" : " момче";
//        }
//    } else {
//    $ret.= "като е ".($data["sex"] ? "била" : "бил")." ";
//    $ret.= "<b>първото ".($data["sex"] ? " момиче" : " момче")."</b> ";
//    $ret.= "родено в този ден и регион";
//    }
//    return $ret;
//}
