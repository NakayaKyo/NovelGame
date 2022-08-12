using System;
using System.Globalization;

namespace NovelSystem.Common
{
    public class NovelSystemCommon
    {
        // 文字列をFloatに変換する
        public float StringToFloat(string changeString)
        {
            float value = 0.0f;

            try
            {
                value = float.Parse(changeString, CultureInfo.InvariantCulture.NumberFormat);
            }
            catch (Exception e)
            {
                //Debug.Log("Error:StringToFloat (" + changeString + ") = " + e);
                throw e;
            }

            return value;
        }

        // 偶数なのかチェックする
        public bool CheckEvenNumber(int num)
        {
            int result;
            try
            {
                Math.DivRem(num, 2, out result);
                if (result == 0) { return true; }
                else { return false; }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}
    
