namespace Demo.models;

public class ResultJsonInfo<T> : ResultJsonBase
{
    public ResultJsonInfo()
    {

    }

    #region field

    private T data;

    #endregion

    #region property

    /// <summary>
    /// 返回数据
    /// </summary>
    public T Data
    {
        get { return data; }
        set { data = value; }
    }

    #endregion
}


public class ResultJsonBase
{

    public ResultJsonBase()
    {

    }

    #region field

    private int status;
    private string info;

    #endregion

    #region property

    /// <summary>
    /// 状态
    /// </summary>
    public int Status
    {
        get { return status; }
        set { status = value; }
    }

    /// <summary>
    /// 提示信息
    /// </summary>
    public string Info
    {
        get { return info; }
        set { info = value; }
    }

    #endregion
}

