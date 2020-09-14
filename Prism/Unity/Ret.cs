using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wanghzh.Prism
{
    public class Ret
    {
        /// <summary>
        /// 提示信息
        /// </summary>
        public string AlertMess
        {
            get;
            set;
        }
        /// <summary>
        /// 完成结果
        /// </summary>
        public bool GetResult
        {
            get;
            set;
        }

        /// <summary>
        /// 附加属性
        /// </summary>
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception
        {
            get;
            set;
        }
    }
}
