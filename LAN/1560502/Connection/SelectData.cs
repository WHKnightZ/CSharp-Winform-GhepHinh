using System;

// Lưu các thông tin được gửi đi khi có thay đổi mảnh đang chọn

namespace GhepHinh
{
    [Serializable]
    public class SelectData
    {
        public int index;

        public SelectData(int index)
        {
            this.index = index;
        }
    }
}
