using System.Collections.Generic;

public static class NumericSplit
{
    /// <summary>
    /// 정수를 숫자 목록으로 분해한다.
    /// </summary>
    public static List<int> Split(int num)
    {
        List<int> numerics = new List<int>();
        if (num == 0)
            numerics.Add(0);

        int temp = num;
        while (temp != 0)
        {
            int rest_value = temp % 10;
            temp /= 10;
            numerics.Add(rest_value);
        }

        return numerics;
    }
}