using System;

//Minmax is a data structure created to represent two ended value selection in between a max and a min limit,
//gives the ability of selecting a slice of numeric choices
[Serializable]
public class MinMax
{
    public float minLimit = 0;
    public float maxLimit = 10;

    public float pivotMin;
    public float pivotMax;

    public float diff = 1;

    public MinMax() { }

    public MinMax(float min, float max)
    {
        this.minLimit = min;
        this.maxLimit = max;
    }

    public MinMax(float min, float max, float diff)
    {
        this.minLimit = min;
        this.maxLimit = max;
        this.diff = diff;
    }

    public MinMax(float min, float max, float pivotMin, float pivotMax, float diff)
    {
        this.minLimit = min;
        this.maxLimit = max;
        this.pivotMin = pivotMin;
        this.pivotMax = pivotMax;
        this.diff = diff;
    }
}
