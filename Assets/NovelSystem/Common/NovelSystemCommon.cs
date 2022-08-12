using System;
using System.Globalization;

namespace NovelSystem.Common
{
    public class NovelSystemCommon
    {
        // �������Float�ɕϊ�����
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

        // �����Ȃ̂��`�F�b�N����
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
    
